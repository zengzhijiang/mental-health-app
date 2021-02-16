using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MentalHealthApp.ViewModels
{
    public class UserViewModel
    {
        public int Id { get; set; }
        [Required]
        [StringLength(50)]
        [RegularExpression("^[-0-9A-Za-z_]{5,30}$",ErrorMessage = "Username can only be letters, numbers, and hyphens. Length must be 5-30 characters.")]
        [DisplayName("User Name")]
        public string UserName { get; set; }
        [Required]
        [StringLength(50)]               
        public string Password{ get; set; }
        [Required]
        [StringLength(50)]
        [DisplayName("Repeat Password")]
        public string RepeatPassword { get; set; }
        [Required]
        [StringLength(50)]
        [DisplayName("First Name")]
        public string FirstName { get; set; }
        [Required]
        [StringLength(50)]
        [DisplayName("Last Name")]
        public string LastName { get; set; }
        public int AccountType { get; set; }
        public string Avatar { get; set; }
        public string Email{ get; set; }
    }
}
