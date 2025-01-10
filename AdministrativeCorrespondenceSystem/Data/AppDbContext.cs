using AdministrativeCorrespondenceSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace AdministrativeCorrespondenceSystem.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Person> Persons { get; set; }
        public DbSet<Letter> Letters { get; set; }
        public DbSet<LetterType> LetterTypes { get; set; }
        public DbSet<Note> Notes { get; set; }
    }
}
