using HospitalAppointment_core.Interfaces.RepositoryInterfaces;
using HospitalAppointment_domain.Entities;
using HospitalAppointment_Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
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
        public async Task<bool> IsDoctorAvailable(int doctorId, DateTime appointmentDateTime)
        {
            return await _context.Appointments
                .AnyAsync(a => a.DoctorId == doctorId 
                && a.AppointmentDateTime == appointmentDateTime);
        }

        public async Task SaveAppointment(Appointment appointment)
        {
            await _context.Appointments.AddAsync(appointment);
        }
    }
}
