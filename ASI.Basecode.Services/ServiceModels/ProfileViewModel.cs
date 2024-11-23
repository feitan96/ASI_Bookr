using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Basecode.Services.ServiceModels
{
    public class ProfileViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "First name is required.")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last name is required.")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Phone number is required.")]
        [RegularExpression(@"^\+63 \d{10}$", ErrorMessage = "Phone number must start with +63 and contain 10 digits after it.")]
        public string PhoneNumber { get; set; }

        public bool AllowNotifications { get; set; }
        public bool IsDarkMode { get; set; }

        [Required(ErrorMessage = "Book duration is required.")]
        public int DefaultBookDuration{ get; set; }
    }
}
