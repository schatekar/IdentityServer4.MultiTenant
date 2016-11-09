using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace IdentityServer4.MultiTenant.ViewModels
{
    public class LoginViewModel
    {
        [Required]
        [Display(Name = "Username/Email")]
        public string Username { get; set; }

        [Required]
        [Display(Name = "Password")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public string Tenant { get; set; }

        [HiddenInput]
        public string ReturnUrl { get; set; }
        public bool RememberLogin { get; set; }
    }
}