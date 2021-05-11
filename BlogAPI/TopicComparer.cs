using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlogAPI.Models;

namespace BlogAPI
{
    /// <summary>
    /// Used to compare topics by name
    /// </summary>
    public class TopicComparer : IEqualityComparer<Topic>
    {
        public bool Equals(Topic x, Topic y) => x is not null && y is not null && x.Name == y.Name;
        
        public int GetHashCode(Topic obj) => obj.Name.GetHashCode();
        
    }
}
