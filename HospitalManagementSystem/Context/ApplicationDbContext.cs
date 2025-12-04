
using HospitalManagementSystem.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Security.Cryptography;
using System.Text;

namespace HospitalManagementSystem.Context
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> opts) : base(opts) { }

       
        public DbSet<Patient> Patients { get; set; } = null!;
        public DbSet<Doctor> Doctors { get; set; } = null!;
        public DbSet<Appointment> Appointments { get; set; } = null!;
        public DbSet<LabOrder> LabOrders { get; set; } = null!;

     
        public DbSet<ApplicationUser> Users { get; set; } = null!;
        public DbSet<Role> Roles { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

         
            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.Patient)
                .WithMany()
                .HasForeignKey(a => a.PatientId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.Doctor)
                .WithMany()
                .HasForeignKey(a => a.DoctorId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Patient>()
       .HasOne(p => p.Doctor)
       .WithMany(d => d.Patients)
       .HasForeignKey(p => p.DoctorId)
       .OnDelete(DeleteBehavior.Cascade); 


            
            modelBuilder.Entity<Role>().HasData(
                new Role { RoleId = 1, RoleName = "Admin" },
                new Role { RoleId = 2, RoleName = "Doctor" },
                new Role { RoleId = 3, RoleName = "Patient" },
                new Role { RoleId = 4, RoleName = "LabOrder" },
                new Role { RoleId = 5, RoleName = "Ward" }
            );

           
            var adminPassword = "Admin@123";
            using var sha = SHA256.Create();
            var hash = Convert.ToHexString(sha.ComputeHash(Encoding.UTF8.GetBytes(adminPassword)));

            modelBuilder.Entity<ApplicationUser>().HasData(
                new ApplicationUser
                {
                    UserId = 1,
                    FullName = "Super Admin",
                    Email = "admin@example.com",
                    PasswordHash = hash,
                    Phone = "9988776655",
                    Address = "Head Office",
                    DOB = new DateTime(2001, 1, 1),
                    RoleId = 1
                }
            );
        }
    }
}
