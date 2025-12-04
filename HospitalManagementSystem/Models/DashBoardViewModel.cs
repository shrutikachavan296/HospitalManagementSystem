using System.Collections.Generic;

namespace HospitalManagementSystem.Models
{
    public class DashboardViewModel
    {
        public int TotalPatients { get; set; }
        public int TotalDoctors { get; set; }
        public int TotalAppointments { get; set; }
        public int PendingLabOrders { get; set; }

        public List<Appointment> RecentAppointments { get; set; }
    }

}
