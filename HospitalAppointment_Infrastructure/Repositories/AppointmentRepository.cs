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

        public async Task<Appointment?> GetByIdAsync(int id)
        {
            return await _context.Appointments
                .Include(a => a.Doctor)
                .Include(a => a.Patient)
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<bool> IsDoctorAvailable(int doctorId, DateTime appointmentDateTime)
        {
            // Normalize incoming comparison to UTC for consistent checks
            var appointmentUtc = appointmentDateTime.Kind == DateTimeKind.Utc
                ? appointmentDateTime
                : appointmentDateTime.ToUniversalTime();

            // consider a booked slot if any existing appointment is within the same minute
            return await _context.Appointments
                .AnyAsync(a => a.DoctorId == doctorId
                               && EF.Functions.DateDiffMinute(a.AppointmentDateTime, appointmentUtc) == 0);
        }

        public async Task SaveAppointment(Appointment appointment)
        {
            await _context.Appointments.AddAsync(appointment);
        }

        public void Update(Appointment appointment)
        {
            _context.Appointments.Update(appointment);
        }

        // Exact implementation of the interface member
        public async Task<IEnumerable<Appointment>> GetByPatientIdAsync(int patientId)
        {
            return await _context.Appointments
                .Include(a => a.Doctor)
                .Include(a => a.Patient)
                .Where(a => a.PatientId == patientId)
                .OrderBy(a => a.AppointmentDateTime)
                .ToListAsync();
        }

        // New: return appointments for a doctor on the given UTC date (from 00:00 UTC to next day 00:00 UTC)
        public async Task<IEnumerable<Appointment>> GetAppointmentsForDoctorDateAsync(int doctorId, DateTime dateUtc)
        {
            var dayStart = DateTime.SpecifyKind(dateUtc.Date, DateTimeKind.Utc);
            var dayEnd = dayStart.AddDays(1);

            return await _context.Appointments
                .Where(a => a.DoctorId == doctorId
                            && a.AppointmentDateTime >= dayStart
                            && a.AppointmentDateTime < dayEnd)
                .ToListAsync();
        }
    }
}
