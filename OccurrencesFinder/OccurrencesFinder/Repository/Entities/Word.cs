using System;

namespace OccurrencesFinder.Repository.Entities
{
    public class Word
    {
        public int Id { get; set; }
        public string Value { get; set; }
        public int Occurences { get; set; }
        public DateTime DoneAt { get; set; }
        public int GroupId { get; set; }
        public WordsGroup Group { get; set; }
    }
}