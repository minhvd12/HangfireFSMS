using System;
using System.Collections.Generic;

namespace FSMS.Entity.Models
{
    public partial class ChatHistory
    {
        public int Id { get; set; }
        public int Sender { get; set; }
        public int Receiver { get; set; }
        public string Message { get; set; }
        public DateTime SendTimeOnUtc { get; set; }
    }
}
