using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _24_02_20_Homework_BlogLesson_49_WPF
{
    class MessageEventArgs: EventArgs 
    {
        public string StringMessage { get; set; }
        public float NumberMessage { get; set; }
        public Exception ExceptionMessage { get; set; }

        
        public MessageEventArgs(string message)
        {
            StringMessage = message;
        }

        public MessageEventArgs(float numberMessage)
        {
            NumberMessage = numberMessage;
        }
        public MessageEventArgs() { }
    }
}
