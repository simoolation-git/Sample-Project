using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Domain.ViewModels
{
    public class ContactUsViewModel
    {
        [Display(Name = "Name")]
        public string Name { get; set; }

        [EmailAddress(ErrorMessage = "Please enter a valid email address")]
        [Display(Name = "Email Address")]
        public string Email { get; set; }

        [Display(Name = "Feedback Type")]
        public string FeedbackType { get; set; }

        [Required(ErrorMessage = "Your non-empty feedback is appreciated")]
        [MinLength(1, ErrorMessage = "Your non-empty feedback is appreciated")]
        [MaxLength(1000, ErrorMessage = "The feedback has to be less than 1000 characters long.")]
        //[RegularExpression(@"^[0-9a-zA-Z,;:.@#$%^&-_=+''-'\s]+$", ErrorMessage = "Don't even think about Hacking us! as tempting as it might be!")]
        [Display(Name = "Feedback")]
        public string FeedbackMessage { get; set; }
    }
}
