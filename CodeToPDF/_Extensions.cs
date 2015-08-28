using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Windows.Forms;
using Microsoft.TeamFoundation.VersionControl.Client;

namespace CodeToPDF
{
    public static class StringExtensions
    {
        public static bool IsThisOrAnyNullOrWhiteSpace(this string origin, params string[] values)
        {
            if (values == null || values.Length <= 0)
                throw new ArgumentNullException("Function call with null parameters");

            return string.IsNullOrWhiteSpace(origin) || values.Any(v => string.IsNullOrWhiteSpace(v));
        }
    }

    public static class CursorExtensions
    {
        public static void Wait(this Form form)
        {
            form.Enabled = false;
            Cursor.Current = Cursors.WaitCursor;
        }

        public static void Default(this Form form)
        {
            form.Enabled = true;
            Cursor.Current = Cursors.Default;
        }
    }

    public static class TreeViewExtensions
    {
        public static List<TreeNode> GetCheckedNodes(this TreeView source)
        {
            if (source == null)
                throw new ArgumentNullException("source");

            if (!source.CheckBoxes)
                throw new Exception("This treeview don't support checkboxes");

            if (source.Nodes == null || source.Nodes.Count <= 0)
                throw new Exception("This treeview don't have nodes");

            return RecurseCheckedNodes(source.Nodes);
        }

        private static List<TreeNode> RecurseCheckedNodes(TreeNodeCollection theNodes)
        {
            List<TreeNode> aResult = new List<TreeNode>();

            if (theNodes != null)
            {
                foreach (TreeNode aNode in theNodes)
                {
                    if (aNode.Checked)
                        aResult.Add(aNode);

                    aResult.AddRange(RecurseCheckedNodes(aNode.Nodes));
                }
            }

            return aResult;
        }

        public static void CheckAllChildNodes(this TreeNode treeNode, bool state)
        {
            foreach (TreeNode node in treeNode.Nodes)
            {
                node.Checked = state;

                // If the current node has child nodes, call the CheckAllChildsNodes method recursively.
                if (node.Nodes.Count > 0)
                    node.CheckAllChildNodes(state);
            }
        }
        
        public static void GetSourceControlFoldersAsNodes(this TreeView trvSource, VersionControlServer server, string rootPath)
        {
            var textMask = rootPath + "/";

            ItemSet dataSource = server.GetItems(rootPath, VersionSpec.Latest, RecursionType.OneLevel, DeletedState.NonDeleted, ItemType.Folder, false);
            if (!dataSource.Items.Any()) return;

            var root = dataSource.Items.Skip(1).ToList();
            foreach (var folder in root)
            {
                var node = new TreeNode(folder.ServerItem.Replace(textMask, ""));
                node.Tag = folder;
                node.Name = folder.ItemId.ToString();

                GetChildNodes(server, node);
                trvSource.Nodes.Add(node);
            }
        }

        private static void GetChildNodes(VersionControlServer server, TreeNode sourceNode)
        {
            sourceNode.Nodes.Clear();

            var sourceItem = sourceNode.Tag as Item;
            var nodeTextMask = sourceItem.ServerItem + "/";

            var itemSet = server.GetItems(sourceItem.ServerItem, VersionSpec.Latest, RecursionType.OneLevel, DeletedState.NonDeleted, ItemType.Folder, false);
            if (itemSet.Items.Any())
            {
                var source = itemSet.Items.Skip(1).ToList();
                foreach (var folder in source)
                {
                    var node = new TreeNode(folder.ServerItem.Replace(nodeTextMask, ""));
                    node.Tag = folder;
                    node.Name = folder.ItemId.ToString();

                    GetChildNodes(server, node);
                    sourceNode.Nodes.Add(node);
                }
            }
        }
    }

    public static class ControlExtensions
    {
        public static void SetResourceText(this Control control, ResourceManager resourceManager)
        {
            if (control == null)
                throw new ArgumentNullException("control");

            if (resourceManager == null)
                throw new ArgumentNullException("resourceManager");

            var text = resourceManager.GetString(control.Name);
            if (string.IsNullOrWhiteSpace(text))
                return;

            control.Text = text;
        }
    }
}
