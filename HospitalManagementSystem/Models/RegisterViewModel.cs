using System;
using System.ComponentModel.DataAnnotations;

namespace HospitalManagementSystem.Models
{
    public class RegisterViewModel
    {
        [Required, Display(Name = "Full Name"), StringLength(100)]
        public string FullName { get; set; }

        [Required, EmailAddress, Display(Name = "Email")]
        public string Email { get; set; }

        [Required, Display(Name = "Phone")]
        [RegularExpression(@"^(\+\d{1,3}[- ]?)?\d{10}$", ErrorMessage = "Invalid phone number.")]
        public string Phone { get; set; }

        [Required, Display(Name = "Address")]
        public string Address { get; set; }

        [Required, DataType(DataType.Date), Display(Name = "Date of Birth")]
        public DateTime DOB { get; set; }

        [Required, DataType(DataType.Password)]
        public string Password { get; set; }

        [Required, Display(Name = "Role")]
        public int RoleId { get; set; }
    }
}
