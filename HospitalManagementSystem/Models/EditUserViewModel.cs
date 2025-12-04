using System;
using System.ComponentModel.DataAnnotations;

namespace HospitalManagementSystem.Models.ViewModels
{
    public class EditUserViewModel
    {
        public int UserId { get; set; }

        [Required, StringLength(100)]
        public string FullName { get; set; }

        [Required, EmailAddress, StringLength(200)]
        public string Email { get; set; }

        [Required, StringLength(20)]
        public string Phone { get; set; }

        [Required, StringLength(300)]
        public string Address { get; set; }

        [Required, DataType(DataType.Date)]
        public DateTime DOB { get; set; }
    }
}
