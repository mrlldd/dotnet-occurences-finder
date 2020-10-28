using Microsoft.EntityFrameworkCore;
using OccurrencesFinder.Repository.Entities;

namespace OccurrencesFinder.Repository
{
    public class OFContext : DbContext
    {
        public DbSet<Document> Documents { get; set; }
        public DbSet<WordsGroup> WordsGroups { get; set; }
        public DbSet<Word> Words { get; set; }
        public OFContext(DbContextOptions options) : base(options)
        {
        }
        
        

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<Document>(document =>
                {
                    document
                        .HasKey(x => x.Id);
                    document
                        .Property(x => x.Id)
                        .ValueGeneratedOnAdd();
                });

            modelBuilder
                .Entity<WordsGroup>(group =>
                {
                    group
                        .HasKey(x => x.Id);
                    group
                        .Property(x => x.Id)
                        .ValueGeneratedOnAdd();
                    group
                        .HasOne(x => x.Document)
                        .WithMany(x => x.WordsGroups)
                        .HasForeignKey(x => x.DocumentId)
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder
                .Entity<Word>(word =>
                {
                    word
                        .HasKey(x => x.Id);
                    word
                        .Property(x => x.Id)
                        .ValueGeneratedOnAdd();
                    word
                        .HasOne(x => x.Group)
                        .WithMany(x => x.Words)
                        .HasForeignKey(x => x.GroupId)
                        .OnDelete(DeleteBehavior.Cascade);
                });
        }
    }
}