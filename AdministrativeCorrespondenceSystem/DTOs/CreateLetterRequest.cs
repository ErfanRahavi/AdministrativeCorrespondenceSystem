namespace AdministrativeCorrespondenceSystem.DTOs
{
    public class CreateLetterRequest
    {
        public string? Subject { get; set; }
        public string? Body { get; set; }
        public int SenderId { get; set; }
        public int ReceiverId { get; set; }
        public int LetterTypeId { get; set; }
    }
}

