using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalAppointment_domain.Entities
{
    public class Appointment
    {
        public int Id { get; set; }
        public int PatientId { get; set; }
        public int DoctorId { get; set; }
        public DateTime AppointmentDateTime { get; set; }
        [Timestamp]
        public byte[] RowVerion { get; set; }
        public Doctor Doctor { get; set; }
        public Patient Patient { get; set; }
    }
}
