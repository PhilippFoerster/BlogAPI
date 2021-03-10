using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlogAPI.Models
{
    public class Response
    {
        public bool Error { get; set; }
        public string Message { get; set; }

        public Response(bool error, string message)
        {
            Error = error;
            Message = message;
        }
    }
}
