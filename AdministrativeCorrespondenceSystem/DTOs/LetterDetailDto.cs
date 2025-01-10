namespace AdministrativeCorrespondenceSystem.DTOs
{
    public class LetterDetailDto
    {
        public int Id { get; set; }
        public string Subject { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
        public string? Sender { get; set; }
        public string? Receiver { get; set; }
        public string? AttachmentPath { get; set; }
        public string? LetterTypeName { get; set; }
        public DateTime SentDate { get; set; }
    }
}
