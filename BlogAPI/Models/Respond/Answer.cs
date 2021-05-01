using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace BlogAPI.Models.Request
{

    public class Answer
    {
        public Answer(List<string> messages, Type type = Request.Type.Error)
        {
            Messages = messages;
            Type = type.GetDescription();
        }
        public Answer(string message, Type type = Request.Type.Error)
        {
            Messages = new List<string> { message };
            Type = type.GetDescription();
        }

        public string Type { get; set; }

        public List<string> Messages { get; set; }
    }

    public enum Type
    {
        [Description("Error")]
        Error,
        [Description("Info")]
        Info,
        [Description("InvalidModel")]
        InvalidModel
    }
}
