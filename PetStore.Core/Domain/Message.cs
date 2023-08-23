using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetStore.Core.Domain
{
    public class Message
    {
        public string Text { get; set; }
        public MessageType Type { get; set; }

        public Message()
        {
                
        }
        public Message(string text, MessageType type)
        {
            Text = text;
            Type = type;
        }
    }

    public enum MessageType : byte
    {
       Success,
       Warning,
       Error
    }
}
