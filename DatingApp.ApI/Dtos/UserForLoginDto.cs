using System.ComponentModel.DataAnnotations;

namespace DatingApp.ApI.Dtos
{
    public class UserForLoginDto
    {
        [Required]
        public string Username { get; set; }
        [Required]
        [StringLength(8, MinimumLength = 4, ErrorMessage = "you should specify password between 4 and 8 character")]

        public string Password { get; set; }
    }
}