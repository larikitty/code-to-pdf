namespace CodeToPDF
{
    partial class frmPrincipal
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmPrincipal));
            this.fbdPath = new System.Windows.Forms.FolderBrowserDialog();
            this.bgwProcess = new System.ComponentModel.BackgroundWorker();
            this.lblFileName = new System.Windows.Forms.Label();
            this.btnSearch = new System.Windows.Forms.Button();
            this.prbTotal = new System.Windows.Forms.ProgressBar();
            this.lblPath = new System.Windows.Forms.Label();
            this.txtPath = new System.Windows.Forms.TextBox();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnStart = new System.Windows.Forms.Button();
            this.lblFolders = new System.Windows.Forms.Label();
            this.lblBranches = new System.Windows.Forms.Label();
            this.lblProjects = new System.Windows.Forms.Label();
            this.lblCollections = new System.Windows.Forms.Label();
            this.btnConnect = new System.Windows.Forms.Button();
            this.lblServerName = new System.Windows.Forms.Label();
            this.txtServer = new System.Windows.Forms.TextBox();
            this.trvFolders = new System.Windows.Forms.TreeView();
            this.lstCollections = new System.Windows.Forms.ListBox();
            this.lstProjects = new System.Windows.Forms.ListBox();
            this.lstBranches = new System.Windows.Forms.ListBox();
            this.SuspendLayout();
            // 
            // bgwProcess
            // 
            this.bgwProcess.WorkerReportsProgress = true;
            this.bgwProcess.WorkerSupportsCancellation = true;
            // 
            // lblFileName
            // 
            resources.ApplyResources(this.lblFileName, "lblFileName");
            this.lblFileName.Name = "lblFileName";
            // 
            // btnSearch
            // 
            resources.ApplyResources(this.btnSearch, "btnSearch");
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.UseVisualStyleBackColor = true;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // prbTotal
            // 
            resources.ApplyResources(this.prbTotal, "prbTotal");
            this.prbTotal.Name = "prbTotal";
            this.prbTotal.Step = 1;
            this.prbTotal.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            // 
            // lblPath
            // 
            resources.ApplyResources(this.lblPath, "lblPath");
            this.lblPath.Name = "lblPath";
            // 
            // txtPath
            // 
            resources.ApplyResources(this.txtPath, "txtPath");
            this.txtPath.CharacterCasing = System.Windows.Forms.CharacterCasing.Lower;
            this.txtPath.Name = "txtPath";
            // 
            // btnCancel
            // 
            resources.ApplyResources(this.btnCancel, "btnCancel");
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnStart
            // 
            resources.ApplyResources(this.btnStart, "btnStart");
            this.btnStart.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnStart.Name = "btnStart";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // lblFolders
            // 
            resources.ApplyResources(this.lblFolders, "lblFolders");
            this.lblFolders.Name = "lblFolders";
            // 
            // lblBranches
            // 
            resources.ApplyResources(this.lblBranches, "lblBranches");
            this.lblBranches.Name = "lblBranches";
            // 
            // lblProjects
            // 
            resources.ApplyResources(this.lblProjects, "lblProjects");
            this.lblProjects.Name = "lblProjects";
            // 
            // lblCollections
            // 
            resources.ApplyResources(this.lblCollections, "lblCollections");
            this.lblCollections.Name = "lblCollections";
            // 
            // btnConnect
            // 
            resources.ApplyResources(this.btnConnect, "btnConnect");
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.UseVisualStyleBackColor = true;
            this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
            // 
            // lblServerName
            // 
            resources.ApplyResources(this.lblServerName, "lblServerName");
            this.lblServerName.Name = "lblServerName";
            // 
            // txtServer
            // 
            resources.ApplyResources(this.txtServer, "txtServer");
            this.txtServer.CharacterCasing = System.Windows.Forms.CharacterCasing.Lower;
            this.txtServer.Name = "txtServer";
            // 
            // trvFolders
            // 
            resources.ApplyResources(this.trvFolders, "trvFolders");
            this.trvFolders.CheckBoxes = true;
            this.trvFolders.FullRowSelect = true;
            this.trvFolders.Name = "trvFolders";
            this.trvFolders.PathSeparator = "/";
            this.trvFolders.AfterCheck += new System.Windows.Forms.TreeViewEventHandler(this.trvFolders_AfterCheck);
            // 
            // lstCollections
            // 
            this.lstCollections.DisplayMember = "DisplayName";
            this.lstCollections.FormattingEnabled = true;
            resources.ApplyResources(this.lstCollections, "lstCollections");
            this.lstCollections.Name = "lstCollections";
            this.lstCollections.ValueMember = "Identifier";
            this.lstCollections.SelectedIndexChanged += new System.EventHandler(this.lstCollections_SelectedIndexChanged);
            // 
            // lstProjects
            // 
            this.lstProjects.DisplayMember = "DisplayName";
            this.lstProjects.FormattingEnabled = true;
            resources.ApplyResources(this.lstProjects, "lstProjects");
            this.lstProjects.Name = "lstProjects";
            this.lstProjects.ValueMember = "Identifier";
            this.lstProjects.SelectedIndexChanged += new System.EventHandler(this.lstProjects_SelectedIndexChanged);
            // 
            // lstBranches
            // 
            this.lstBranches.DisplayMember = "Text";
            this.lstBranches.FormattingEnabled = true;
            resources.ApplyResources(this.lstBranches, "lstBranches");
            this.lstBranches.Name = "lstBranches";
            this.lstBranches.ValueMember = "Object";
            this.lstBranches.SelectedIndexChanged += new System.EventHandler(this.lstBranches_SelectedIndexChanged);
            // 
            // frmPrincipal
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.lstBranches);
            this.Controls.Add(this.lstProjects);
            this.Controls.Add(this.lstCollections);
            this.Controls.Add(this.trvFolders);
            this.Controls.Add(this.lblFileName);
            this.Controls.Add(this.btnSearch);
            this.Controls.Add(this.prbTotal);
            this.Controls.Add(this.lblPath);
            this.Controls.Add(this.txtPath);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnStart);
            this.Controls.Add(this.lblFolders);
            this.Controls.Add(this.lblBranches);
            this.Controls.Add(this.lblProjects);
            this.Controls.Add(this.lblCollections);
            this.Controls.Add(this.btnConnect);
            this.Controls.Add(this.lblServerName);
            this.Controls.Add(this.txtServer);
            this.MaximizeBox = false;
            this.Name = "frmPrincipal";
            this.Load += new System.EventHandler(this.frmPrincipal_Load);
            this.CursorChanged += new System.EventHandler(this.frmPrincipal_CursorChanged);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.FolderBrowserDialog fbdPath;
        private System.ComponentModel.BackgroundWorker bgwProcess;
        private System.Windows.Forms.Label lblFileName;
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.ProgressBar prbTotal;
        private System.Windows.Forms.Label lblPath;
        private System.Windows.Forms.TextBox txtPath;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.Label lblFolders;
        private System.Windows.Forms.Label lblBranches;
        private System.Windows.Forms.Label lblProjects;
        private System.Windows.Forms.Label lblCollections;
        private System.Windows.Forms.Button btnConnect;
        private System.Windows.Forms.Label lblServerName;
        private System.Windows.Forms.TextBox txtServer;
        private System.Windows.Forms.TreeView trvFolders;
        private System.Windows.Forms.ListBox lstCollections;
        private System.Windows.Forms.ListBox lstProjects;
        private System.Windows.Forms.ListBox lstBranches;
    }
}

