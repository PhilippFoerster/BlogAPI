using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlogAPI.Models.Respond
{
    public class UsersResponse
    {
        public int TotalCount { get; set; }

        public List<UserWithRolesResponse> Users { get; set; }
    }
}
