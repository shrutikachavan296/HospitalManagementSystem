using System;
using System.ComponentModel.DataAnnotations;

namespace HospitalManagementSystem.Models
{
    public enum LabStatus
    {
        Pending,
        Processing,
        Completed,
        Cancelled
    }

    public class LabOrder
    {
        [Key]
        public int LabOrderId { get; set; }

        [Required(ErrorMessage = "Please select a patient.")]
        public int PatientId { get; set; }

        [Required(ErrorMessage = "Test Name is required.")]
        public string TestName { get; set; }

        public DateTime OrderedAt { get; set; } = DateTime.Now;

        [Required(ErrorMessage = "Status is required.")]
        public LabStatus Status { get; set; }

        public Patient Patient { get; set; }
    }
}