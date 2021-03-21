﻿using System.Collections.Generic;
using BlogAPI.Attributes;
using BlogAPI.Interfaces;

namespace BlogAPI.Models
{
    public class UpdateUser : IUpdater
    {
        public int Id { get; set; }

        public Role? Role { get; set; }

        public List<Topic> Interests { get; set; }
    }
}
