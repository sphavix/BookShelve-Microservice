using System.ComponentModel.DataAnnotations;

namespace BookShelve.Api.Models.User
{
    public class RegisterUserDto : LoginUserDto
    {

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        public string Role { get; set; }
    }
}
