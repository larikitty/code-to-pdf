
using Microsoft.TeamFoundation.VersionControl.Client;

namespace CodeToPDF
{
    internal class BranchItem
    {
        public string ProjectName;
        internal const string Mask = "$/{0}/";

        public BranchItem(ItemIdentifier objectItem)
        {
            Object = objectItem;
        }
        
        public BranchItem(ItemIdentifier objectItem, string projectName)
        {
            ProjectName = projectName;
            Object = objectItem;
        }

        public string Text { get { return Object.Item.Replace(string.Format(Mask, ProjectName), ""); } }
        public ItemIdentifier Object { get; private set; }
    }
}
