using HospitalAppointment_core.Interfaces;
using HospitalAppointment_core.Interfaces.RepositoryInterfaces;
using HospitalAppointment_domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace HospitalAppointment_core.Services
{
    public class AppointmentService : IAppointmentService
    {
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly IUnitOfWork _unitOfWork;


        public AppointmentService(IAppointmentRepository appointmentRepository, IUnitOfWork unitOfWork)
        {
            _appointmentRepository = appointmentRepository;
            _unitOfWork = unitOfWork;
        }
        public async Task CreateAppointment(int patientId, int doctorId, DateTime appointmentDateTime)
        {

            var hasAppointment = await _appointmentRepository.IsDoctorAvailable(doctorId, appointmentDateTime);

            if (hasAppointment)
            {
                throw new Exception("Doctor is not available this date time.");
            }
            var appointment = new Appointment
            {
                PatientId = patientId,
                DoctorId = doctorId,
                AppointmentDateTime = appointmentDateTime
            };

            await _appointmentRepository.SaveAppointment(appointment);
            try
            {
                await _unitOfWork.CommitAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                // RowVersion çakışması
                throw new Exception(
                    "The appointment could not be saved because it was modified by another user.");
            }
            catch (DbUpdateException)
            {
                // DB constraint, FK vs.
                throw new Exception(
                    "A database error occurred while saving the appointment.");
            }

        }
    }
}
