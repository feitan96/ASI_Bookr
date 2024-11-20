using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Basecode.Services.ServiceModels
{
    public class RecurringBookingViewModel
    {
        public RecurringBookingViewModel() { }

        public RecurringBookingViewModel(int bookingId, string day) {
            this.BookingId = bookingId;
            this.Day = day;
        }

        public int RecurringId { get; set; }

        [Required(ErrorMessage = "Booking ID is required.")]
        public int BookingId { get; set; }

        [Required(ErrorMessage = "Day is required.")]
        public string Day { get; set; }
    }
}
