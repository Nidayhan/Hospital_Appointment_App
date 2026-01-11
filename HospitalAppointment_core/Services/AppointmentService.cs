using HospitalAppointment_core.Interfaces;
using HospitalAppointment_domain.Entities;
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

        public AppointmentService(IAppointmentRepository appointmentRepository)
        {
            _appointmentRepository = appointmentRepository;
        }
        public void CreateAppointment(int patientId, int doctorId, DateTime dateTime)
        {
            var isAvailable = _appointmentRepository.IsDoctorAvailable(doctorId, dateTime);

            if (!isAvailable)
            {
                throw new Exception("Doctor is not available this date time.");
            }
            var appointment = new Appointment
            {
                PatientId = patientId,
                DoctorId = doctorId,
                AppointmentDateTime = dateTime
            };

            _appointmentRepository.SaveAppointment(appointment);
        }
    }
}
