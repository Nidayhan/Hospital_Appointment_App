using System;

namespace HospitalAppointment_domain.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string PasswordHash { get; set; } = string.Empty;
        public string Role { get; set; } = "Patient";
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;

        public string TcKimlikNo { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
    }
}
