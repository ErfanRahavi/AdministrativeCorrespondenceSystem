namespace AdministrativeCorrespondenceSystem.Models
{
    public class Letter
    {
        public int Id { get; set; }
        public string? Subject { get; set; }
        public string? Body { get; set; }
        public DateTime SentDate { get; set; }
        public int SenderId { get; set; }
        public int ReceiverId { get; set; }
        public int LetterTypeId { get; set; }
        public int? ParentLetterId { get; set; } 
        public string? AttachmentPath { get; set; } 
        
        public LetterStatus Status { get; set; }

        public Person Sender { get; set; }
        public Person Receiver { get; set; }
        public LetterType LetterType { get; set; }
        public Letter ParentLetter { get; set; }
        public ICollection<Note> Notes { get; set; }
    }
}
