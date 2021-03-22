using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlogAPI.Models
{
    public class UpdateRoles
    {
        public bool Add { get; set; }

        public List<string> Roles { get; set; }
    }
}
