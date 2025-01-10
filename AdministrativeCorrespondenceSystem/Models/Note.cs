namespace AdministrativeCorrespondenceSystem.Models
{
    public class Note
    {
        public int Id { get; set; }
        public string? Content { get; set; }
        public int LetterId { get; set; }

        public Letter Letter { get; set; }
    }
}
