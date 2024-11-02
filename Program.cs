using Microsoft.EntityFrameworkCore;
using Model;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

using var context = new LanguageDbContext();

context.Database.EnsureDeleted();
context.Database.EnsureCreated();

context.Sentences.Add(new Sentence { Text = "Hello, World!", IsPhrase = false });

context.SaveChanges();

var sentences = context.Sentences.ToList();

foreach (var sentence in sentences)
{
    Console.WriteLine(sentence.Text);
}


namespace Model
{
    public class Sentence
    {
        public int Id { get; set; }

        public required string Text { get; set; }

        public bool IsPhrase { get; set; }
    }

    public class LanguageDbContext : DbContext
    {
        public DbSet<Sentence> Sentences { get; set; } = default!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            _ = optionsBuilder.UseSqlite("DataSource=temp.db");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Sentence>().Property(e => e.IsPhrase).HasConversion(Storage.BooleanToCharConverter.Default);
        }
    }
}

namespace Storage
{
    public sealed class BooleanToCharConverter : ValueConverter<bool, char>
    {
        public static readonly BooleanToCharConverter Default = new();

        public BooleanToCharConverter()
            : base(v => ConvertToChar(v), v => ConvertToBoolean(v))
        {
        }

        private static char ConvertToChar(bool value)
        {
            return value ? 'Y' : 'N';
        }

        private static bool ConvertToBoolean(char value)
        {
            return value == 'Y';
        }
    }
}