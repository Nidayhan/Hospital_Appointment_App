using HospitalAppointment_core.Interfaces;
using HospitalAppointment_domain.Entities;
using HospitalAppointment_Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalAppointment_Infrastructure.Repositories
{
    public class AppointmentRepository : IAppointmentRepository
    {
        private readonly ApplicationDbContext _context;

        public AppointmentRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public bool IsDoctorAvailable(int doctorId, DateTime dateTime)
        {
            return !_context.Appointments
                .Any(a => a.DoctorId == doctorId 
                && a.AppointmentDateTime == dateTime);
        }

        public void SaveAppointment(Appointment appointment)
        {
            _context.Appointments.Add(appointment);
            _context.SaveChanges();
        }
    }
}
