using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using OccurrencesFinder.Utilities.Extensions;
using OccurrencesFinder.Utilities.ReconciliationService.WebApi.Utilities;

namespace OccurrencesFinder.Utilities
{
    public class WordFileOccurrencesFinder
    {
        public async Task<Dictionary<string, Dictionary<string, Dictionary<string, int>>>> FindAsync(
            IDictionary<string, string[]> groups, params string[] paths)
        {
            if (!paths.Any())
            {
                throw new ArgumentException("There should be at least one path to find occurrences.", nameof(paths));
            }

            var validatedGroups = ValidateGroups(groups);
            var findResult = await paths
                .Select(path => new Func<Task<KeyValuePair<string, Dictionary<string, Dictionary<string, int>>>>>(() =>
                {
                    using var file = File.OpenRead(path);
                    using var doc = WordprocessingDocument.Open(file, false);
                    var result = groups
                        .ToDictionary(x => x.Key,
                            x => x.Value.ToDictionary(y => y, _ => 0)
                        );
                    doc.MainDocumentPart.Document.Descendants<Text>()
                        .Select(x => x.Text)
                        .ForEach(text =>
                        {
                            foreach (var (key, strings) in validatedGroups)
                            {
                                var found = strings
                                    .FirstOrDefault(x => text.Contains(x, StringComparison.OrdinalIgnoreCase));
                                if (!string.IsNullOrEmpty(found))
                                {
                                    result[key][found]++;
                                }
                            }
                        });
                    return Task.FromResult(
                        new KeyValuePair<string, Dictionary<string, Dictionary<string, int>>>(path, result));
                }))
                .ExecuteMultipleWithResultsAsync(4);


            return findResult
                .ToDictionary(x => x.Key,
                    x => x.Value);
        }

        private static IDictionary<string, string[]> ValidateGroups(IDictionary<string, string[]> groups)
            => groups
                .Select(x => x.Value.Any()
                    ? x
                    : new KeyValuePair<string, string[]>(x.Key, new[]
                    {
                        x.Key
                    })
                )
                .ToDictionary(x => x.Key, x => x.Value);
    }
}