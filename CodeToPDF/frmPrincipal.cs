using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.Framework.Client;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.VersionControl.Client;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Resources;
using System.Windows.Forms;
using System.Xml.Linq;

namespace CodeToPDF
{
    public partial class frmPrincipal : Form
    {
        private ResourceManager rm = new ResourceManager("CodeToPDF.CodeToPDF", typeof(CodeToPDF).Assembly);

        private TfsConfigurationServer server;
        private ReadOnlyCollection<CatalogNode> collections;
        private ReadOnlyCollection<CatalogNode> projects;

        private TfsTeamProjectCollection projectCollection { get { return new TfsTeamProjectCollection(new Uri(string.Format("{0}/{1}", txtServer.Text, lstCollections.Text))); } }
        private VersionControlServer vcServer { get { return projectCollection.GetService<VersionControlServer>(); } }
        private BranchObject[] branchCollection { get { return vcServer.QueryRootBranchObjects(RecursionType.Full); } }

        private List<string> ExcludeFolders 
        { 
            get 
            {
                var xmlFile = new XDocument();
                using (Stream file = new FileStream("XML/ExcludeFolders.xml", FileMode.Open))
                    xmlFile = XDocument.Load(file);

                return (from folder in xmlFile.Root.Elements("Folder")
                        select folder.Attribute("Name").Value.ToString().ToLower()).ToList(); 
            } 
        }
        private List<string> IncludeExtensions 
        { 
            get 
            {
                var xmlFile = new XDocument();
                using (Stream file = new FileStream("XML/FileExtensions.xml", FileMode.Open))
                    xmlFile = XDocument.Load(file);

                return (from extension in xmlFile.Root.Elements("Extension")
                        select "." + extension.Attribute("Type").Value.ToString().ToLower()).ToList();
            } 
        }

        #region Inicialization and Load
        public frmPrincipal()
        {
            InitializeComponent();

            foreach (Control c in this.Controls)
                c.SetResourceText(rm);
        }

        private void InitializeControls()
        {
            ClearControls();
            ToggleControls(true);

            txtServer.Text = "http://tfs.bematech.com:8080/tfs";
            prbTotal.Maximum = 100;
            fbdPath.Reset();

            InitializeBackground();

            txtServer.Focus();
        }

        private void InitializeBackground()
        {
            bgwProcess.WorkerSupportsCancellation = true;
            bgwProcess.WorkerReportsProgress = true;
            bgwProcess.DoWork += new DoWorkEventHandler(StartConversion);
            bgwProcess.RunWorkerCompleted += new RunWorkerCompletedEventHandler(ProcessCompleted);
            bgwProcess.ProgressChanged += new ProgressChangedEventHandler(ComputeProgress);
        }

        private void ClearControls()
        {
            txtServer.Clear();
            lstCollections.DataSource = null;
            lstProjects.DataSource = null;
            lstBranches.DataSource = null;
            trvFolders.Nodes.Clear();
            txtPath.Clear();
            prbTotal.Value = 0;
            lblFileName.Text = "...";
        }

        private void ToggleControls(bool enable)
        {
            txtServer.Enabled = enable;
            btnConnect.Enabled = enable;

            lstCollections.Enabled = enable;
            lstProjects.Enabled = enable;
            lstBranches.Enabled = enable;

            trvFolders.Enabled = enable;

            txtPath.Enabled = enable;
            btnSearch.Enabled = enable;

            btnStart.Enabled = enable;
            btnCancel.Enabled = !enable;
        }

        private void frmPrincipal_Load(object sender, EventArgs e)
        {
            InitializeControls();
        }

        private void frmPrincipal_CursorChanged(object sender, EventArgs e)
        {
            if (Cursor.Current == Cursors.WaitCursor)
                Application.DoEvents();
        }
        #endregion

        #region Connect
        private void btnConnect_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtServer.Text))
            {
                MessageBox.Show(rm.GetString("msgNoServer"));
                InitializeControls();
                return;
            }

            Connect(txtServer.Text);
        }

        private void Connect(string serverName)
        {
            Uri uriServer;

            if (!Uri.TryCreate(serverName, UriKind.RelativeOrAbsolute, out uriServer))
            {
                MessageBox.Show(rm.GetString("msgServerUriError"));
                InitializeControls();
                return;
            }

            this.Wait();

            server = TfsConfigurationServerFactory.GetConfigurationServer(uriServer);

            this.Default();

            GetCollections();
        } 
        #endregion

        #region Collections
        private void GetCollections()
        {
            this.Wait();

            var resourceCollectionType = new[] { CatalogResourceTypes.ProjectCollection };

            collections = server.CatalogNode.QueryChildren(resourceCollectionType, false, CatalogQueryOptions.None);

            lstCollections.DataSource = collections.Select(s => s.Resource).ToList();

            this.Default();
        }

        private void lstCollections_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstCollections.SelectedValue == null) 
            {
                lstProjects.DataSource = null;
                lstBranches.DataSource = null;
                return;
            }

            GetProjects((lstCollections.SelectedValue is Guid) ? (Guid)lstCollections.SelectedValue : (Guid)((CatalogResource)lstCollections.SelectedValue).Identifier);
        } 
        #endregion

        #region Projects
        private void GetProjects(Guid collectionId)
        {
            var collection = collections.FirstOrDefault(f => f.Resource.Identifier == collectionId);
            if (collection == null)
            {
                MessageBox.Show(rm.GetString("msgNoProjects"));
                lstBranches.ClearSelected();
                lstProjects.ClearSelected();

                return;
            }

            this.Wait();

            var resourceProjectType = new[] { CatalogResourceTypes.TeamProject };

            projects = collection.QueryChildren(resourceProjectType, false, CatalogQueryOptions.None);

            lstProjects.DataSource = projects.Select(s => s.Resource).ToList();

            this.Default();
        }

        private void lstProjects_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstProjects.SelectedValue == null)
            {
                lstBranches.DataSource = null;
                return;
            }

            GetBranches((lstProjects.SelectedValue is Guid) ? (Guid)lstProjects.SelectedValue : (Guid)((CatalogResource)lstProjects.SelectedValue).Identifier);
        } 
        #endregion

        #region Branches
        private void GetBranches(Guid projectId)
        {
            var project = projects.FirstOrDefault(f => f.Resource.Identifier == projectId);
            if (project == null || branchCollection.Count() <= 0)
            {
                MessageBox.Show(rm.GetString("msgNoBranches"));
                lstBranches.ClearSelected();

                return;
            }

            var filteredBranches = branchCollection.Where(w => w.Properties.RootItem.Item.StartsWith(string.Format(BranchItem.Mask, project.Resource.DisplayName)));
            if (filteredBranches.Count() <= 0)
            {
                MessageBox.Show(rm.GetString("msgNoBranches"));
                lstBranches.ClearSelected();

                return;
            }

            lstBranches.DataSource = filteredBranches.Select(b => new BranchItem(b.Properties.RootItem, project.Resource.DisplayName)).ToList();
        }

        private void lstBranches_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstBranches.SelectedValue == null) return;

            SetSourceControlTree((BranchItem)lstBranches.SelectedItem);
        } 
        #endregion

        #region Folders
        private void SetSourceControlTree(BranchItem branch)
        {
            if (branch == null)
            {
                MessageBox.Show(rm.GetString("msgBranchNotFound"));
                lstBranches.ClearSelected();
                trvFolders.SelectedNode = null;

                return;
            }

            this.Wait();

            trvFolders.Nodes.Clear();
            trvFolders.GetSourceControlFoldersAsNodes(vcServer, branch.Object.Item);

            if (trvFolders.Nodes.Count <= 0)
            {
                MessageBox.Show(rm.GetString("msgBranchNotFound"));
                lstBranches.ClearSelected();
                trvFolders.SelectedNode = null;
                this.Default();

                return;
            }

            this.Default();
        }
        
        private void trvFolders_AfterCheck(object sender, TreeViewEventArgs e)
        {
            if (e.Action == TreeViewAction.Unknown) return;
            if (e.Node.Nodes.Count <= 0) return;

            this.Wait();

            e.Node.CheckAllChildNodes(e.Node.Checked);

            this.Default();
        }

        private void ResetSourceControlTree()
        {
            trvFolders.Nodes.Clear();

            prbTotal.Value = 0;
            prbTotal.Maximum = 100;

            fbdPath.Reset();

            lblFileName.Text = "...";
        } 
        #endregion

        #region Path to save files
        private void btnSearch_Click(object sender, EventArgs e)
        {
            fbdPath.ShowNewFolderButton = true;

            if (fbdPath.ShowDialog(this) != System.Windows.Forms.DialogResult.OK)
                return;

            txtPath.Text = fbdPath.SelectedPath;
        } 
        #endregion

        #region Processing Control Buttons
        private void btnStart_Click(object sender, EventArgs e)
        {
            ToggleControls(false);
            ConvertFiles();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            bgwProcess.CancelAsync();

            ResetSourceControlTree();
            ToggleControls(true);
            InitializeBackground();
        }         
        #endregion

        #region Converting Files
        private void ShowMessage(string messageResourceName)
        {
            MessageBox.Show(rm.GetString(messageResourceName));
            this.Default();
            ToggleControls(true);
        }

        private void ConvertFiles()
        {
            //Preparing form to start process
            this.Wait();

            if (txtPath.Text.IsThisOrAnyNullOrWhiteSpace(txtServer.Text, lstCollections.Text, lstProjects.Text, lstBranches.Text))
            {
                ShowMessage("msgMissingParameters");
                return;
            }

            //Get Selected Folders
            var selectedFolders = trvFolders.GetCheckedNodes().Where(w => !string.IsNullOrWhiteSpace(w.Name)).Select(s => s.Tag as Item).ToList();
            if (selectedFolders == null || selectedFolders.Count <= 0)
            {
                ShowMessage("msgNoFoldersSelected");
                return;
            }

            //Get Source Control Files
            var files = new List<Item>();

            //files on root branch
            var rootPath = string.Format("$/{0}/{1}", lstProjects.Text, lstBranches.Text.Replace("\\", "/"));
            var tempRootSet = vcServer.GetItems(rootPath, VersionSpec.Latest, RecursionType.OneLevel, DeletedState.NonDeleted, ItemType.File, true);
            if (tempRootSet.Items.Count() > 0)
                files.AddRange(tempRootSet.Items);

            //files on folders from branch
            selectedFolders.ForEach(f =>
            {
                var tempSet = vcServer.GetItems(f.ServerItem, VersionSpec.Latest, RecursionType.OneLevel, DeletedState.NonDeleted, ItemType.File, true);
                if (tempSet.Items.Count() > 0)
                    files.AddRange(tempSet.Items);
            });


            //To generate extensions file
            var debugFileTypes = files.Select(f => f.ServerItem.Split("\\".ToCharArray()).Last().Split(".".ToCharArray()).Last()).Distinct().ToList();
            
            if (files.Count() <= 0)
            {
                ShowMessage("msgNoItems");
                return;
            }

            //Preparing Exclusions and Inclusions
            var itemsToWork = new List<Item>();
            
            //Filtering File Extensions by XML
            var toInclude = IncludeExtensions;
            if (toInclude != null && toInclude.Any())
                toInclude.ForEach(e => { itemsToWork.AddRange(files.Where(f => f.ServerItem.ToLower().EndsWith(e))); });

            if (itemsToWork.Count() <= 0)
            {
                ShowMessage("msgNoItems");
                return;
            }
            
            //Exclude folders by XML
            var toExclude = ExcludeFolders;
            if (toExclude != null && toExclude.Any())
            {
                var tempFiles = new List<Item>();
                toExclude.ForEach(e => tempFiles.AddRange(itemsToWork.Where(f => f.ServerItem.ToLower().Contains(string.Format("/{0}/", e))).ToList()));

                if (tempFiles != null && tempFiles.Any())
                    tempFiles.ForEach(e => itemsToWork.Remove(e));
            }

            if (itemsToWork.Count() <= 0)
            {
                ShowMessage("msgNoItems");
                return;
            }
            

            //Creating model of items to process
            var branch = lstBranches.SelectedItem as BranchItem;
            var process = new Process();
            process.Info = new ProcessInfo();
            process.Info.PathToSave = txtPath.Text + "\\";
            process.Info.RootFolder = branch.Object.Item;
            process.Items = itemsToWork.Select(s => new ProcessItem { Parent = process.Info, Item = s });

            this.Default();

            //Starting process
            bgwProcess.RunWorkerAsync(process);
        } 
        #endregion
        
        #region Background Worker Process
        private void StartConversion(object sender, DoWorkEventArgs e)
        {
            e.Result = ProcessConversion((BackgroundWorker)sender, e);
        }

        private string ProcessConversion(BackgroundWorker worker, DoWorkEventArgs e)
        {
            if (worker.CancellationPending)
            {
                e.Cancel = true;
                return rm.GetString("msgProcessCancelled");
            }

            var process = (Process)e.Argument;
            var listOfItems = process.Items;
            var toDivide = listOfItems.Count();
            var counter = 0;

            prbTotal.Value = 0;
            prbTotal.Maximum = 100;
            prbTotal.UseWaitCursor = true;

            foreach (var item in listOfItems)
            {
                var fileSaved = SaveFile(item);

                counter++;
                worker.ReportProgress(((counter * 100) / toDivide), fileSaved);
            }

            return rm.GetString("msgProcessCompleted");
        }

        private string SaveFile(ProcessItem item)
        {
            var fileToWrite = string.Empty;
            using (var stream = new StreamReader(item.Item.DownloadFile()))
                fileToWrite = stream.ReadToEnd();

            var fullPath = item.Item.ServerItem;
            var fileName = Path.GetFileName(fullPath);
            var folder = Path.GetDirectoryName(fullPath.Replace(item.Parent.RootFolder, "")) + "\\";

            var newFile = string.Format("{0}{1}{2}.pdf", item.Parent.PathToSave, folder, fileName);

           var fInfo = new FileInfo(newFile);
            fInfo.Directory.Create();

            var doc = new Document();
            PdfWriter.GetInstance(doc, new FileStream(newFile, FileMode.Create));

            doc.Open();

            if (string.IsNullOrWhiteSpace(fileToWrite))
                doc.Add(new Chunk());
            else
                doc.Add(new Paragraph(fileToWrite));

            doc.Close();

            return folder + fileName;
        }

        private void ComputeProgress(object sender, ProgressChangedEventArgs e)
        {
            lblFileName.Text = (string)e.UserState;
            prbTotal.Value = e.ProgressPercentage;
        }

        private void ProcessCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
                MessageBox.Show(e.Error.Message);
            else if (e.Cancelled)
            {
                lblFileName.Text = "...";
                MessageBox.Show(rm.GetString("msgProcessCancelled"));
            }
            else
            {
                lblFileName.Text = e.Result.ToString();
                MessageBox.Show(rm.GetString("msgProcessCompleted"));
            }

            ToggleControls(false);
        } 
        #endregion
    }
}
