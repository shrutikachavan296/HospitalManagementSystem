using System;
using System.ComponentModel.DataAnnotations;

namespace HospitalManagementSystem.Models
{
    public class Appointment
    {
        public int AppointmentId { get; set; }

        [Required(ErrorMessage = "Please select a patient.")]
        public int PatientId { get; set; }

        [Required(ErrorMessage = "Please select a doctor.")]
        public int DoctorId { get; set; }

        [Required(ErrorMessage = "Appointment Date is required.")]
        [DataType(DataType.Date)]
        public DateTime AppointmentDate { get; set; }

        public string Notes { get; set; }

        public int? OwnerUserId { get; set; }

  
        public Patient Patient { get; set; }
        public Doctor Doctor { get; set; }
    }
}
