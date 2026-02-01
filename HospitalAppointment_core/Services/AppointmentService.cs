using HospitalAppointment_core.BusinessRules;
using HospitalAppointment_core.Interfaces;
using HospitalAppointment_core.Interfaces.RepositoryInterfaces;
using HospitalAppointment_core.DTOs;
using HospitalAppointment_domain.Entities;
using HospitalAppointment_domain.Enums;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalAppointment_core.Services
{
    public class AppointmentService : IAppointmentService
    {
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPatientRepository _patientRepository;
        private readonly IUserRepository _userRepository;

        // Working hours and slot interval constants (keep near the top of the class)
        private static readonly TimeSpan WorkStart = TimeSpan.FromHours(9);   // 09:00
        private static readonly TimeSpan WorkEnd = TimeSpan.FromHours(16.5);    // 16:30
        private const int SlotMinutes = 30;

        public AppointmentService(
            IAppointmentRepository appointmentRepository,
            IUnitOfWork unitOfWork,
            IPatientRepository patientRepository,
            IUserRepository userRepository)
        {
            _appointmentRepository = appointmentRepository;
            _unitOfWork = unitOfWork;
            _patientRepository = patientRepository;
            _userRepository = userRepository;
        }

        public async Task CreateAppointment(int patientId, int doctorId, DateTime appointmentDateTime)
        {
            // Normalize to UTC to avoid timezone issues (adjust if your app uses local times)
            var requestedUtc = appointmentDateTime.Kind == DateTimeKind.Utc
                ? appointmentDateTime
                : appointmentDateTime.ToUniversalTime();

            // Resolve patient: first try by patient.Id (as caller may pass a patient table id).
            var patient = await _patientRepository.GetByIdAsync(patientId);

            // If not found, patientId might have been a User.Id (token user id). Try resolving:
            if (patient == null)
            {
                var possibleUser = await _userRepository.GetByIdAsync(patientId);
                if (possibleUser != null)
                {
                    // find patient by TcKimlikNo that was created when the user registered
                    patient = await _patientRepository.GetByTcAsync(possibleUser.TcKimlikNo);
                    if (patient != null)
                    {
                        // use the resolved patient table id going forward
                        patientId = patient.Id;
                    }
                }
            }

            if (patient == null)
                throw new InvalidOperationException("Patient not found.");

            // Use BusinessRule classes (defense-in-depth)
            new CannotCreatePastAppointmentRule(requestedUtc).Check();
            new AppointmentSlotIntervalRule(requestedUtc, SlotMinutes).Check();
            new AppointmentWithinWorkingHoursRule(requestedUtc.TimeOfDay, WorkStart, WorkEnd).Check();

            // Availability check in DB
            var hasAppointment = await _appointmentRepository.IsDoctorAvailable(doctorId, requestedUtc);
            if (hasAppointment)
            {
                throw new InvalidOperationException("Doctor is not available this date time.");
            }

            var appointment = new Appointment
            {
                PatientId = patientId,
                DoctorId = doctorId,
                AppointmentDateTime = requestedUtc,
                Status = AppointmentStatus.Pending
            };

            await _appointmentRepository.SaveAppointment(appointment);
            try
            {
                await _unitOfWork.CommitAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new Exception("The appointment could not be saved because it was modified by another user.");
            }
            catch (DbUpdateException ex)
            {
                var dbMsg = ex.GetBaseException()?.Message ?? ex.Message;
                throw new Exception("A database error occurred while saving the appointment. DB message: " + dbMsg);
            }
        }

        public async Task<IEnumerable<DateTime>> GetAvailableSlotsAsync(int doctorId, DateTime dateUtc)
        {
            // Ensure dateUtc is a UTC date (midnight)
            var dayUtc = DateTime.SpecifyKind(dateUtc.Date, DateTimeKind.Utc);

            var allSlots = GenerateSlotsForDate(dayUtc); // slots are DateTimes with Kind = Utc

            var appointments = await _appointmentRepository.GetAppointmentsForDoctorDateAsync(doctorId, dayUtc);

            var booked = appointments
                .Select(a => a.AppointmentDateTime.Kind == DateTimeKind.Utc ? a.AppointmentDateTime : a.AppointmentDateTime.ToUniversalTime())
                .ToHashSet();

            var available = allSlots.Except(booked).OrderBy(d => d);

            return available;
        }

        private static HashSet<DateTime> GenerateSlotsForDate(DateTime dateUtc)
        {
            var slots = new HashSet<DateTime>();

            // Force the date to be a UTC date (strip time, set Kind = Utc)
            var dayUtc = DateTime.SpecifyKind(dateUtc.Date, DateTimeKind.Utc);

            var current = dayUtc.Add(WorkStart);
            var end = dayUtc.Add(WorkEnd);

            while (current <= end)
            {
                // current is already UTC kind
                slots.Add(current);
                current = current.AddMinutes(SlotMinutes);
            }

            return slots;
        }

        public async Task CancelAppointment(int appointmentId, int currentUserId)
        {
            var appointment = await _appointmentRepository.GetByIdAsync(appointmentId);

            if (appointment == null)
            {
                throw new Exception("Appointment not found");
            }
            new CannotCancelPastAppointmentRule(appointment.AppointmentDateTime).Check();
            new CannotModifyCompletedAppointmentRule(appointment.Status).Check();
            new PatientCanOnlyCancelOwnAppointmentRule(appointment.PatientId, currentUserId).Check();

            appointment.Status = AppointmentStatus.Cancelled;
            _appointmentRepository.Update(appointment);

            try
            {
                await _unitOfWork.CommitAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new Exception("Appointment was modified by another user.");
            }
        }

        async Task<IEnumerable<AppointmentResponseDTO>> IAppointmentService.GetAppointmentsByPatientAsync(int patientId)
        {
            var appointments = await _appointmentRepository.GetByPatientIdAsync(patientId);

            return appointments.Select(a => new AppointmentResponseDTO
            {
                Id = a.Id,
                PatientId = a.PatientId,
                DoctorId = a.DoctorId,
                AppointmentDateTime = a.AppointmentDateTime,
                Status = a.Status,
                DoctorName = a.Doctor != null ? a.Doctor.Name : null
            }).ToList();
        }
    }
}
