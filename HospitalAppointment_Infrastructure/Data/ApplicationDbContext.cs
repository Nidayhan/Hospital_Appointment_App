using HospitalAppointment_domain.Entities;
using HospitalAppointment_Infrastructure.Configuration;
using Microsoft.EntityFrameworkCore;

namespace HospitalAppointment_Infrastructure.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new AppointmentConfiguration());
            modelBuilder.ApplyConfiguration(new DepartmentConfiguration());
            modelBuilder.ApplyConfiguration(new UserConfiguration());
            // add user refresh token configuration if you create one in future
        }

        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<Department> Department { get; set; }
        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<Patient> Patients { get; set; }
        public DbSet<User> Users { get; set; }

        // Refresh tokens are stored in a simple table
        public DbSet<RefreshToken> RefreshTokens { get; set; }
    }
}
