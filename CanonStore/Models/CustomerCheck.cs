using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CanonStore.Models
{
    public class CustomerCheck
    {
        [Required]
        public string UserName { get; set; }
        [Required]
        [RegularExpression(@"^(?=.*[a-zA-Z])(?=.*\d).{6,}", ErrorMessage = "Must contain at least 1 number and 1 letter and must be 6 characters")]

        public string Password { get; set; }
        public string LoginErrorMessage { get; internal set; }
    }
}