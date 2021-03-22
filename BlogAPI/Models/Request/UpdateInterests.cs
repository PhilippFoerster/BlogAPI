using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlogAPI.Models
{
    public class UpdateInterests
    {
        public bool Add { get; set; }

        public List<string> Interests { get; set; }
    }
}
