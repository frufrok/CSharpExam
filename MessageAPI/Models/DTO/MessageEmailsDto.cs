namespace MessageAPI.Models.DTO
{
    public class MessageEmailsDto
    {
        public Guid Guid { get; set; }
        public string EmailFrom { get; set; }
        public string EmailTo { get; set; }
        public string Text { get; set; }
        public DateTime DateTime { get; set; }
    }
}
