﻿using System.ComponentModel.DataAnnotations;

namespace DemoCookieAuth.Models
{
    public class SignInModel
    {
        [Required]
        public string Username { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public bool RememberMe { get; set; }
    }
}
