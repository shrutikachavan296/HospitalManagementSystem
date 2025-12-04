using System;
using System.ComponentModel.DataAnnotations;

namespace HospitalManagementSystem.Models
{
    public class Patient
    {
        [Key]
        public int PatientId { get; set; }

        [Required]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Name must contain only letters and spaces.")]
        [StringLength(100)]
        public string PatientName { get; set; } = string.Empty;
        [Required]
        public string Address { get; set; }

        [Required(ErrorMessage = "Phone Number is required")]
        [RegularExpression(@"^(\+\d{1,3}[- ]?)?\d{10}$", ErrorMessage = "Invalid phone number format.")]
        public string Phone { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Date of Birth")]
        public DateTime DOB { get; set; }

        public int? OwnerUserId { get; set; }
        [Required(ErrorMessage = "Please select a doctor.")]
        public int DoctorId { get; set; }

   
        public Doctor Doctor { get; set; }
    }
}
