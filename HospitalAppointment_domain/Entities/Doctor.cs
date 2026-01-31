using System.Collections.Generic;

namespace HospitalAppointment_domain.Entities
{
    public class Doctor
    {
        public int Id { get; set; }
        // link to user record
        public int? UserId { get; set; }

        public string Name { get; set; } = string.Empty;
        public string Specialty { get; set; } = string.Empty;
        public int DepartmentId { get; set; }
        public Department Department { get; set; }
        public ICollection<Appointment> Appointments { get; set; }
        public bool IsActive { get; set; }
    }
}
