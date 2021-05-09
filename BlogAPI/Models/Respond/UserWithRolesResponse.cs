using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlogAPI.Models.Respond
{
    public class UserWithRolesResponse
    {
        public UserResponse User { get; set; }

        public RolesResponse Roles { get; set; }
    }
}
