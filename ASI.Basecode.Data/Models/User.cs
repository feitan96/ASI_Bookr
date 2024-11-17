using System;
using System.Collections.Generic;

namespace ASI.Basecode.Data.Models
{
    public partial class User
    {
        public User()
        {
            Admins = new HashSet<Admin>();
            Bookings = new HashSet<Booking>();
            Superadmins = new HashSet<Superadmin>();
        }

        public int Id { get; set; }
        public string UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public string Password { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public bool? IsDeleted { get; set; }
        public bool? AllowNotifications { get; set; }
        public bool IsDarkMode { get; set; }
        public int DefaultBookDuration { get; set; }
        public string PasswordResetToken { get; set; }
        public DateTime? ResetTokenExpiry { get; set; }

        public virtual ICollection<Admin> Admins { get; set; }
        public virtual ICollection<Booking> Bookings { get; set; }
        public virtual ICollection<Superadmin> Superadmins { get; set; }
    }
}
