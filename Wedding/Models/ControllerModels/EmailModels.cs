using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Wedding.Models
{
    public class RegisterEmail
    {
        public RegisterEmail(string callbackUrl)
        {
            CallbackUrl = callbackUrl;
        }

        public string CallbackUrl { get; set; }
    }
}