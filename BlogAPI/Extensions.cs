using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogAPI
{
    public static class Extensions
    {
        public static string GetUser(this HttpRequest request) => Encoding.UTF8.GetString(Convert.FromBase64String(request.Headers["Authorization"].ToString()[6..])).Split(":")[0];
    }
}
