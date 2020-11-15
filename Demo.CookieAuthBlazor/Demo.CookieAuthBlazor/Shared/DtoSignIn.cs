using System.ComponentModel.DataAnnotations;

namespace Demo.CookieAuthBlazor.Shared
{
     public class DtoSignIn
    {
        [Required]
        [MaxLength(255)]
        public string Username { get; set; }

        [Required]
        [MaxLength(255)]
        public string Password { get; set; }

        public bool RememberMe { get; set; }
    }
}
