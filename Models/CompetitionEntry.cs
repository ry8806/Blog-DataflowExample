using System;
using System.Net;

namespace DataFlowExample.Models
{
    public class CompetitionEntry
    {
        public string Email { get; set; }
        public string Answer { get; set; }
        public IPAddress IPAddress { get; set; }
        public DateTime Created { get; set; }
    }
}
