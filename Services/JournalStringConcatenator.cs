using oa.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace oa.Services
{
    public class JournalStringConcatenator
    {
        public class JournalString
        {
            public int? JournalId { get; set; }
            public string? concatenatedString { get; set; }
        }
        public List<JournalString> CombineJournalStrings(List<JournalString> journalStrings)
        {
            var combinedJournalStrings = journalStrings
                .GroupBy(js => js.JournalId)
                .Select(group => new JournalString
                {
                    JournalId = group.Key,
                    concatenatedString = string.Join(" | ", group.Select(js => js.concatenatedString))
                })
                .ToList();

            return combinedJournalStrings;
        }
    }
}