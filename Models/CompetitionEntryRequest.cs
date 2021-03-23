namespace DataFlowExample.Models
{
    /// <summary>
    /// A Request that is received from the API Call
    /// </summary>
    public class CompetitionEntryRequest
    {
        public string Email { get; set; }
        public string Answer { get; set; }
    }
}
