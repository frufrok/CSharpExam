namespace MessageAPI.Models
{
    public class Message
    {
        public int Id { get; set; }
        public Guid Guid { get; set; }
        public Guid UserFromGuid { get; set; }
        public Guid UserToGuid { get; set; }
        public string Text { get; set; }
        public DateTime DateTime { get; set; }
        public bool IsReaded { get; set; }
    }
}
