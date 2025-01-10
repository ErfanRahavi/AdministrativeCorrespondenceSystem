namespace AdministrativeCorrespondenceSystem.DTOs
{
    public class LetterDto
    {
        public int Id { get; set; }
        public string? Subject { get; set; }
        public string? Body { get; set; } 
        public string? SenderName { get; set; }
        public string? ReceiverName { get; set; }
        public DateTime? SentDate { get; set; }
        public string? LetterTypeName { get; set; }
        public int? ParentLetterId { get; set; }

        public string? AttachmentPath { get; set; }
    }
}
