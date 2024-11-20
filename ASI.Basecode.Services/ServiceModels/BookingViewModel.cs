using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;


namespace ASI.Basecode.Services.ServiceModels
{
    public class BookingViewModel
    {
        public BookingViewModel()
        {
            // Initialize the Room, User, Admin, and Superadmin properties to ensure they are not null.
            this.Room = new RoomViewModel();
            this.User = new UserViewModel();
        }

        public int BookingId { get; set; }

        [Required(ErrorMessage = "Please select a room.")]
        public int RoomId { get; set; }

        [Required(ErrorMessage = "Please select a user.")]
        public int UserId { get; set; }

        [Required(ErrorMessage = "Please select a start date.")]
        public DateTime BookingStartDate { get; set; }

        //[DataType(DataType.Date)]
        //[DateGreaterThan("BookingStartDate", ErrorMessage = "Booking end date must be later than the start date.")]
        [Required(ErrorMessage = "Please select an end date.")]
        public DateTime? BookingEndDate { get; set; }

        [Required(ErrorMessage = "Please select a check in time.")]
        public string CheckInTimeString { get; set; }

        [Required(ErrorMessage = "Please select a check out time.")]

        public string CheckOutTimeString { get; set; }

        // Helper method to parse the time
        public static TimeSpan ParseTimeSpan(string timeString)
        {
            if (string.IsNullOrEmpty(timeString))
                return default;

            try
            {
                return TimeSpan.Parse(timeString);
            }
            catch
            {
                try
                {
                    return DateTime.ParseExact(timeString, "h:mm tt", CultureInfo.InvariantCulture).TimeOfDay;
                }
                catch
                {
                    return default;
                }
            }
        }


        public string Status { get; set; }

        public DateTime? CreatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public int? ApproveDisapproveBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool IsRecurring { get; set; }

        [Required(ErrorMessage = "Meeting Title is required.")]
        [StringLength(200, ErrorMessage = "Meeting Title cannot exceed 200 characters.")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Please select atleast one day")]
        public string SelectedDays { get; set; }

        // Navigational properties
        public RoomViewModel Room { get; set; }
        public UserViewModel User { get; set; }
        public List<RecurringBookingViewModel> RecurringBookings { get; set; }

        // Additional display properties for UI convenience
        public string RoomName => Room?.Name ?? "Unknown Room";
        public string UserFullName => $"{User?.FirstName} {User?.LastName}" ?? "Unknown User";

        // To display the formatted booking date, e.g., in a UI-friendly format.
        public string FormattedBookingStartDate => BookingStartDate.ToString("MMMM dd, yyyy") ?? "Not Booked Yet";
        public string FormattedBookingEndDate
        {
            get
            {
                // If BookingEndDate is null, use BookingStartDate for the formatted output
                var dateToUse = BookingEndDate ?? BookingStartDate;
                return dateToUse.ToString("MMMM dd, yyyy");
            }
        }
        // Property to get the list of selected days from the comma-separated string
        public List<string> SelectedDaysList
        {
            get
            {
                // Return an empty list if SelectedDays is null or empty
                if (string.IsNullOrEmpty(SelectedDays))
                {
                    return new List<string>();
                }

                // Split the comma-separated string and return as a list of strings
                return SelectedDays.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                                   .Select(day => day.Trim())
                                   .ToList();
            }
        }

        // Custom validation logic
        //public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        //{
        //    if (IsRecurring && !BookingEndDate.HasValue)
        //    {
        //        yield return new ValidationResult(
        //            "Booking End Date is required for recurring bookings.",
        //            new[] { nameof(BookingEndDate) }
        //        );
        //    }
        //}
    }

    //public class DateGreaterThanAttribute : ValidationAttribute, IClientValidatable
    //{
    //    private readonly string _comparisonDateProperty;

    //    public DateGreaterThanAttribute(string comparisonDateProperty)
    //    {
    //        _comparisonDateProperty = comparisonDateProperty;
    //        ErrorMessage = "{0} must be later than {1}.";
    //    }

    //    // Server-side validation
    //    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    //    {
    //        var comparisonProperty = validationContext.ObjectType.GetProperty(_comparisonDateProperty);
    //        if (comparisonProperty == null)
    //        {
    //            return new ValidationResult($"Unknown property: {_comparisonDateProperty}");
    //        }

    //        var comparisonDateValue = comparisonProperty.GetValue(validationContext.ObjectInstance, null) as DateTime?;

    //        if (comparisonDateValue == null)
    //        {
    //            return ValidationResult.Success; // If comparison date is null, validation passes
    //        }

    //        if (value is DateTime currentDate && currentDate <= comparisonDateValue)
    //        {
    //            return new ValidationResult(string.Format(ErrorMessage, validationContext.DisplayName, comparisonProperty.Name));
    //        }

    //        return ValidationResult.Success;
    //    }

    //    // Client-side validation
    //    public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
    //    {
    //        var rule = new ModelClientValidationRule
    //        {
    //            ValidationType = "dategreaterthan",
    //            ErrorMessage = this.ErrorMessage
    //        };

    //        rule.ValidationParameters.Add("comparisondateproperty", _comparisonDateProperty);

    //        yield return rule;
    //    }
    //}

}
