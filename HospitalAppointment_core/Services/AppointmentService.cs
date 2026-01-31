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

        // Working hours and slot interval constants (can be moved to config later)
        private static readonly TimeSpan WorkStart = TimeSpan.FromHours(9);   // 09:00
        private static readonly TimeSpan WorkEnd = TimeSpan.FromHours(17);    // 17:00
        private const int SlotMinutes = 30;

        public AppointmentService(IAppointmentRepository appointmentRepository, IUnitOfWork unitOfWork)
        {
            _appointmentRepository = appointmentRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task CreateAppointment(int patientId, int doctorId, DateTime appointmentDateTime)
        {
            // Business rules for creating an appointment
            // 1) cannot create in the past
            new CannotCreatePastAppointmentRule(appointmentDateTime).Check();

            // 2) must be within working hours (inclusive)
            new AppointmentWithinWorkingHoursRule(appointmentDateTime.TimeOfDay, WorkStart, WorkEnd).Check();

            // 3) must be on the allowed slot interval (00 or 30 minutes)
            new AppointmentSlotIntervalRule(appointmentDateTime, SlotMinutes).Check();

            var hasAppointment = await _appointmentRepository.IsDoctorAvailable(doctorId, appointmentDateTime);

            if (hasAppointment)
            {
                throw new Exception("Doctor is not available this date time.");
            }

            var appointment = new Appointment
            {
                PatientId = patientId,
                DoctorId = doctorId,
                AppointmentDateTime = appointmentDateTime,
                Status = AppointmentStatus.Pending // explicitly set initial status
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
            catch (DbUpdateException)
            {
                throw new Exception("A database error occurred while saving the appointment.");
            }
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

        // Explicit interface implementation to ensure exact match
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
