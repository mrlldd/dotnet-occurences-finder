using System;
using System.Collections;
using System.Collections.Generic;

namespace OccurrencesFinder.Repository.Entities
{
    public class Document
    {
        public int Id { get; set; }
        public string AbsolutePath { get; set; }
        public DateTime DoneAt { get; set; }
        public DocumentType Type { get; set; }
        public ICollection<WordsGroup> WordsGroups { get; set; }
    }
}