using BlogAPI.Attributes;

namespace BlogAPI.Models
{
    public class ModifyUser
    {
        public int Id { get; set; }

        public Role Role { get; set; }
    }
}
