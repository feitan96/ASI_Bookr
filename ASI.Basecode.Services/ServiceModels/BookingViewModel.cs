using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Basecode.Services.ServiceModels
{
    public class BookingViewModel
    {
        public BookingViewModel()
        {
            // Initialize the Room, User, Admin, and Superadmin properties to ensure they are not null.
            this.Room = new RoomViewModel();
            this.User = new UserViewModel();
            //this.Admin = new AdminViewModel();
            //this.Superadmin = new SuperadminViewModel();
        }

        public int BookingId { get; set; }

        [Required]
        public int RoomId { get; set; }

        [Required]
        public int UserId { get; set; }

        public int? AdminId { get; set; }
        public int? SuperadminId { get; set; }

        [Required]
        public DateTime BookingCheckInDateTime { get; set; }

        [Required]
        public DateTime BookingCheckOutDateTime { get; set; }

        public string Status { get; set; }

        public DateTime? CreatedDate { get; set; }
        public int? CreatedBy { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }

        // Navigational properties
        public RoomViewModel Room { get; set; }
        public UserViewModel User { get; set; }
        //public AdminViewModel Admin { get; set; }
        //public SuperadminViewModel Superadmin { get; set; }

        // Additional display properties for UI convenience
        public string RoomName => Room?.Name ?? "Unknown Room";
        public string UserFullName => $"{User?.FirstName} {User?.LastName}" ?? "Unknown User";
        //public string AdminFullName => $"{Admin?.User.FirstName} {Admin?.User.LastName}" ?? "Unknown Admin";
        //public string SuperadminFullName => $"{Superadmin?.User,FirstName} {Superadmin?.USer.LastName}" ?? "Unknown Superadmin";

        // To display the formatted booking date, e.g., in a UI-friendly format.
        public string FormattedCheckInDate => BookingCheckInDateTime.ToString("MMMM dd, yyyy") ?? "Not Booked Yet";
        public string FormattedCheckOutDate => BookingCheckOutDateTime.ToString("MMMM dd, yyyy") ?? "Not Booked Yet";

    }
}
