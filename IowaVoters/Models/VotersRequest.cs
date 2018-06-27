namespace IowaVoters.Models
{
    public class VotersRequest
    {
        private string _county;
        public string County {
            get
            {
                return _county;
            }
            set
            {
                _county = value.ToLower();
            }
        }
        public int Month { get; set; }
        public PartyEnum Party { get; set; }
        public StatusEnum Status { get; set; }
        public int Start { get; set; } = 0;
        public int Limit { get; set; } = 2;
        public enum PartyEnum
        {
            Any,
            Republican,
            Democrat,
            Other,
            No_party
        }
        public enum StatusEnum
        {
            Any,
            Active,
            Inactive,
        }
    }
}