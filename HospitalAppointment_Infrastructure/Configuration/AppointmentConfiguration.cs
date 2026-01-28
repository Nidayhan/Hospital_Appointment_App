using HospitalAppointment_domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalAppointment_Infrastructure.Configuration
{
    public class AppointmentConfiguration : IEntityTypeConfiguration<Appointment>
    {
        public void Configure(EntityTypeBuilder<Appointment> builder)
        {
            builder.HasKey(x => x.Id); //tablonun primary key i

            builder.Property(x => x.AppointmentDateTime).IsRequired();

            builder.Property(x => x.PatientId).IsRequired();

            builder.Property(x => x.DoctorId).IsRequired();

            builder.HasIndex(x => new { x.DoctorId, x.AppointmentDateTime })
                .IsUnique();
        }
    }
}
