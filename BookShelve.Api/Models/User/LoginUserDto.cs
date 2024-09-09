using System.ComponentModel.DataAnnotations;

namespace BookShelve.Api.Models.User
{
    public class LoginUserDto
    {
        [Required]
        [EmailAddress]
        public string UserEmail { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
