using System.Collections;
using System.Collections.Generic;

namespace OccurrencesFinder.Repository.Entities
{
    public class WordsGroup
    {
        public int Id { get; set; }
        public string Name { get; set; }
        
        
        public int DocumentId { get; set; }
        public Document Document { get; set; }
        
        public ICollection<Word> Words { get; set; }
    }
}