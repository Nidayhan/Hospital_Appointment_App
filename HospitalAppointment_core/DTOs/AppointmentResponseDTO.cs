using HospitalAppointment_domain.Enums;
using System;

namespace HospitalAppointment_core.DTOs
{
    public class AppointmentResponseDTO
    {
        public int Id { get; set; }
        public int PatientId { get; set; }
        public int DoctorId { get; set; }
        public DateTime AppointmentDateTime { get; set; }
        public AppointmentStatus Status { get; set; }
        public string? DoctorName { get; set; }
    }
}
