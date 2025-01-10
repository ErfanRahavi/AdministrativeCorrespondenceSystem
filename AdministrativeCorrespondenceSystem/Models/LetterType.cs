namespace AdministrativeCorrespondenceSystem.Models
{
    public class LetterType
    {
        public int Id { get; set; }
        public string? Name { get; set; }

        public ICollection<Letter> Letters { get; set; }
    }
}
