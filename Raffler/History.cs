using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Raffler
{
    public class History
    {
        public List<HistoryEntry> NameHistory { get; }

        public class HistoryEntry
        {
            public HistoryEntry(string name, DateTime? changedToAt)
            {
                Name = name;
                ChangedToAt = changedToAt;
            }

            public string Name { get; }
            public DateTime? ChangedToAt { get; }
        }
    }
}
