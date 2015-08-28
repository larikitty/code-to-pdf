using System.Collections.Generic;
using Microsoft.TeamFoundation.VersionControl.Client;

namespace CodeToPDF
{
    internal class Process
    {
        public ProcessInfo Info { get; set; }
        public IEnumerable<ProcessItem> Items { get; set; }
    }

    internal class ProcessInfo
    {
        public string RootFolder { get; set; }
        public string PathToSave { get; set; }
    }

    internal class ProcessItem
    {
        public ProcessInfo Parent { get; set; }
        public Item Item { get; set; }
    }
}
