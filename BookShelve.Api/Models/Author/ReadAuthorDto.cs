using System.ComponentModel.DataAnnotations;

namespace BookShelve.Api.Models.Author
{
    public class ReadAuthorDto : BaseDto
    {
        [Required]
        [MaxLength(50)]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(50)]
        public string LastName { get; set; }

        [StringLength(300)]
        public string? Bio { get; set; }
    }
}
