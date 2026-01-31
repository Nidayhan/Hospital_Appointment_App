using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HospitalAppointment_domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HospitalAppointment_Infrastructure.Configuration
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("Users");

            builder.HasKey(u => u.Id);

            builder.Property(u => u.FirstName).IsRequired().HasMaxLength(100);
            builder.Property(u => u.LastName).IsRequired().HasMaxLength(100);
            builder.Property(u => u.PasswordHash).IsRequired();
            builder.Property(u => u.TcKimlikNo).IsRequired().HasMaxLength(11);
            builder.Property(u => u.PhoneNumber).HasMaxLength(20);
            builder.Property(u => u.Role).IsRequired().HasMaxLength(50);

            // Unique index on TC
            builder.HasIndex(u => u.TcKimlikNo).IsUnique();
        }
    }
}
