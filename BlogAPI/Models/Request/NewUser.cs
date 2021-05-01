using System.ComponentModel.DataAnnotations;

namespace BlogAPI.Models
{
    public class NewUser
    {
        [Required]
        public string Mail { get; set; }

        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }

    }
}
