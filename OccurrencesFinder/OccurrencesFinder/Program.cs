using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using OccurrencesFinder.Repository;
using OccurrencesFinder.Repository.Entities;
using OccurrencesFinder.Utilities;
using OccurrencesFinder.Utilities.Extensions;

namespace OccurrencesFinder
{
    public class Program
    {
        private const string localhostString =
            "Data Source=localhost\\SQLEXPRESS;Initial Catalog=test;Integrated Security=SSPI;";

        public static async Task Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            var paths = new[]
            {
                @"D:\template.docx"
            };
            var groups = new Dictionary<string, string[]>
            {
                {
                    "anything", new[]
                    {
                        "nothing"
                    }
                }
            };
            var found = await new WordFileOccurrencesFinder().FindAsync(groups, paths);

            await using var dbContext = new DbContextOptionsBuilder<OFContext>()
                .UseSqlServer(localhostString)
                .Options
                .Map(x => new OFContext(x));
            await dbContext.Database.EnsureCreatedAsync();
            var docs = await dbContext.Documents
                .Where(x => paths.Contains(x.AbsolutePath))
                .Include(x => x.WordsGroups)
                .ThenInclude(x => x.Words)
                .ToArrayAsync();
            var now = DateTime.Now;
            foreach (var (key, value) in found)
            {
                var doc = docs
                              .FirstOrDefault(x =>
                                  x.AbsolutePath.Equals(key, StringComparison.OrdinalIgnoreCase))
                          ?? dbContext.Add(new Document
                          {
                              AbsolutePath = key,
                              Type = DocumentType.Docx,
                              WordsGroups = new List<WordsGroup>()
                          }).Entity;
                doc.DoneAt = now;
                foreach (var (groupName, groupWords) in value)
                {
                    var group = doc
                                    .WordsGroups
                                    .FirstOrDefault(x => x.Name.Equals(groupName, StringComparison.OrdinalIgnoreCase))
                                ?? new WordsGroup
                                {
                                    Name = groupName,
                                    Words = new List<Word>()
                                }.SideEffect(doc.WordsGroups.Add);
                    foreach (var (wordKey, occurrences) in groupWords)
                    {
                        var word = group
                                       .Words
                                       .FirstOrDefault(x => x.Value.Equals(wordKey, StringComparison.OrdinalIgnoreCase))
                                   ?? new Word
                                       {
                                           Value = wordKey
                                       }
                                       .SideEffect(group.Words.Add);
                        word.Occurences = occurrences;
                        word.DoneAt = now;
                    }
                }
            }

            await dbContext.SaveChangesAsync();
            await dbContext.Database.CloseConnectionAsync();
            Console.Read();
        }
    }
}