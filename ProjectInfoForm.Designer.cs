﻿namespace PacketViewerLogViewer
{
    partial class ProjectInfoForm
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ProjectInfoForm));
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.tProjectFolder = new System.Windows.Forms.TextBox();
            this.tOpenedLog = new System.Windows.Forms.TextBox();
            this.tSourceVideo = new System.Windows.Forms.TextBox();
            this.tYoutubeURL = new System.Windows.Forms.TextBox();
            this.tPackedLogsURL = new System.Windows.Forms.TextBox();
            this.saveProjectDlg = new System.Windows.Forms.SaveFileDialog();
            this.btnDownloadSource = new System.Windows.Forms.Button();
            this.IL = new System.Windows.Forms.ImageList(this.components);
            this.btnDownloadYoutube = new System.Windows.Forms.Button();
            this.btnUploadToYoutube = new System.Windows.Forms.Button();
            this.gbLocalFiles = new System.Windows.Forms.GroupBox();
            this.btnMake7zip = new System.Windows.Forms.Button();
            this.lVideoSourceOK = new System.Windows.Forms.Label();
            this.lOpenedLogOK = new System.Windows.Forms.Label();
            this.lProjectFolderOK = new System.Windows.Forms.Label();
            this.gbOnlineFile = new System.Windows.Forms.GroupBox();
            this.gbProjectInfo = new System.Windows.Forms.GroupBox();
            this.tTagBox = new System.Windows.Forms.TextBox();
            this.btnAddTag = new System.Windows.Forms.Button();
            this.tagContainer = new System.Windows.Forms.FlowLayoutPanel();
            this.lTagLabel = new System.Windows.Forms.Label();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnCopySummary = new System.Windows.Forms.Button();
            this.btnExtractZip = new System.Windows.Forms.Button();
            this.gbLocalFiles.SuspendLayout();
            this.gbOnlineFile.SuspendLayout();
            this.gbProjectInfo.SuspendLayout();
            this.tagContainer.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 55);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(82, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Opened Log file";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 16);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(72, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Project Folder";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 94);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(71, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "Source Video";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 16);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(72, 13);
            this.label4.TabIndex = 3;
            this.label4.Text = "Youtube URL";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 55);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(80, 13);
            this.label5.TabIndex = 4;
            this.label5.Text = "Download URL";
            // 
            // tProjectFolder
            // 
            this.tProjectFolder.Location = new System.Drawing.Point(20, 32);
            this.tProjectFolder.Name = "tProjectFolder";
            this.tProjectFolder.Size = new System.Drawing.Size(345, 20);
            this.tProjectFolder.TabIndex = 2;
            this.tProjectFolder.TextChanged += new System.EventHandler(this.ProjectInfo_TextChanged);
            // 
            // tOpenedLog
            // 
            this.tOpenedLog.Location = new System.Drawing.Point(20, 71);
            this.tOpenedLog.Name = "tOpenedLog";
            this.tOpenedLog.Size = new System.Drawing.Size(345, 20);
            this.tOpenedLog.TabIndex = 3;
            this.tOpenedLog.TextChanged += new System.EventHandler(this.ProjectInfo_TextChanged);
            // 
            // tSourceVideo
            // 
            this.tSourceVideo.Location = new System.Drawing.Point(20, 110);
            this.tSourceVideo.Name = "tSourceVideo";
            this.tSourceVideo.Size = new System.Drawing.Size(345, 20);
            this.tSourceVideo.TabIndex = 4;
            this.tSourceVideo.TextChanged += new System.EventHandler(this.ProjectInfo_TextChanged);
            // 
            // tYoutubeURL
            // 
            this.tYoutubeURL.Location = new System.Drawing.Point(20, 32);
            this.tYoutubeURL.Name = "tYoutubeURL";
            this.tYoutubeURL.Size = new System.Drawing.Size(345, 20);
            this.tYoutubeURL.TabIndex = 5;
            this.tYoutubeURL.TextChanged += new System.EventHandler(this.ProjectInfo_TextChanged);
            // 
            // tPackedLogsURL
            // 
            this.tPackedLogsURL.Location = new System.Drawing.Point(20, 71);
            this.tPackedLogsURL.Name = "tPackedLogsURL";
            this.tPackedLogsURL.Size = new System.Drawing.Size(345, 20);
            this.tPackedLogsURL.TabIndex = 6;
            this.tPackedLogsURL.TextChanged += new System.EventHandler(this.ProjectInfo_TextChanged);
            // 
            // saveProjectDlg
            // 
            this.saveProjectDlg.DefaultExt = "pvlv";
            this.saveProjectDlg.Filter = "PVLV Project Files (*.pvlv)|*.pvlv|All files|*.*";
            // 
            // btnDownloadSource
            // 
            this.btnDownloadSource.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnDownloadSource.ImageIndex = 2;
            this.btnDownloadSource.ImageList = this.IL;
            this.btnDownloadSource.Location = new System.Drawing.Point(399, 71);
            this.btnDownloadSource.Name = "btnDownloadSource";
            this.btnDownloadSource.Size = new System.Drawing.Size(160, 23);
            this.btnDownloadSource.TabIndex = 11;
            this.btnDownloadSource.Text = "Open in browser";
            this.btnDownloadSource.UseVisualStyleBackColor = true;
            this.btnDownloadSource.Click += new System.EventHandler(this.BtnDownloadSource_Click);
            // 
            // IL
            // 
            this.IL.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("IL.ImageStream")));
            this.IL.TransparentColor = System.Drawing.Color.Transparent;
            this.IL.Images.SetKeyName(0, "Fairytale_ark.png");
            this.IL.Images.SetKeyName(1, "Fairytale_upload.png");
            this.IL.Images.SetKeyName(2, "Fairytale_browser.png");
            this.IL.Images.SetKeyName(3, "Fairytale_editcopy.png");
            this.IL.Images.SetKeyName(4, "Fairytale_apply.png");
            this.IL.Images.SetKeyName(5, "Fairytale_fileopen.png");
            // 
            // btnDownloadYoutube
            // 
            this.btnDownloadYoutube.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnDownloadYoutube.ImageIndex = 2;
            this.btnDownloadYoutube.ImageList = this.IL;
            this.btnDownloadYoutube.Location = new System.Drawing.Point(399, 32);
            this.btnDownloadYoutube.Name = "btnDownloadYoutube";
            this.btnDownloadYoutube.Size = new System.Drawing.Size(160, 23);
            this.btnDownloadYoutube.TabIndex = 12;
            this.btnDownloadYoutube.Text = "Open in browser";
            this.btnDownloadYoutube.UseVisualStyleBackColor = true;
            this.btnDownloadYoutube.Click += new System.EventHandler(this.BtnDownloadYoutube_Click);
            // 
            // btnUploadToYoutube
            // 
            this.btnUploadToYoutube.Enabled = false;
            this.btnUploadToYoutube.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnUploadToYoutube.ImageIndex = 1;
            this.btnUploadToYoutube.ImageList = this.IL;
            this.btnUploadToYoutube.Location = new System.Drawing.Point(399, 110);
            this.btnUploadToYoutube.Name = "btnUploadToYoutube";
            this.btnUploadToYoutube.Size = new System.Drawing.Size(160, 20);
            this.btnUploadToYoutube.TabIndex = 14;
            this.btnUploadToYoutube.Text = "Upload to Youtube";
            this.btnUploadToYoutube.UseVisualStyleBackColor = true;
            // 
            // gbLocalFiles
            // 
            this.gbLocalFiles.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbLocalFiles.Controls.Add(this.btnExtractZip);
            this.gbLocalFiles.Controls.Add(this.btnMake7zip);
            this.gbLocalFiles.Controls.Add(this.lVideoSourceOK);
            this.gbLocalFiles.Controls.Add(this.lOpenedLogOK);
            this.gbLocalFiles.Controls.Add(this.lProjectFolderOK);
            this.gbLocalFiles.Controls.Add(this.label2);
            this.gbLocalFiles.Controls.Add(this.btnUploadToYoutube);
            this.gbLocalFiles.Controls.Add(this.label1);
            this.gbLocalFiles.Controls.Add(this.label3);
            this.gbLocalFiles.Controls.Add(this.tProjectFolder);
            this.gbLocalFiles.Controls.Add(this.tOpenedLog);
            this.gbLocalFiles.Controls.Add(this.tSourceVideo);
            this.gbLocalFiles.Location = new System.Drawing.Point(12, 129);
            this.gbLocalFiles.Name = "gbLocalFiles";
            this.gbLocalFiles.Size = new System.Drawing.Size(565, 141);
            this.gbLocalFiles.TabIndex = 15;
            this.gbLocalFiles.TabStop = false;
            this.gbLocalFiles.Text = "Local Files";
            // 
            // btnMake7zip
            // 
            this.btnMake7zip.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnMake7zip.ImageIndex = 0;
            this.btnMake7zip.ImageList = this.IL;
            this.btnMake7zip.Location = new System.Drawing.Point(399, 32);
            this.btnMake7zip.Name = "btnMake7zip";
            this.btnMake7zip.Size = new System.Drawing.Size(160, 20);
            this.btnMake7zip.TabIndex = 18;
            this.btnMake7zip.Text = "Make .7z";
            this.btnMake7zip.UseVisualStyleBackColor = true;
            this.btnMake7zip.Click += new System.EventHandler(this.BtnMake7zip_Click);
            // 
            // lVideoSourceOK
            // 
            this.lVideoSourceOK.AutoSize = true;
            this.lVideoSourceOK.Font = new System.Drawing.Font("Wingdings 2", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(2)));
            this.lVideoSourceOK.Location = new System.Drawing.Point(371, 110);
            this.lVideoSourceOK.Name = "lVideoSourceOK";
            this.lVideoSourceOK.Size = new System.Drawing.Size(22, 17);
            this.lVideoSourceOK.TabIndex = 17;
            this.lVideoSourceOK.Text = "W";
            // 
            // lOpenedLogOK
            // 
            this.lOpenedLogOK.AutoSize = true;
            this.lOpenedLogOK.Font = new System.Drawing.Font("Wingdings 2", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(2)));
            this.lOpenedLogOK.Location = new System.Drawing.Point(371, 71);
            this.lOpenedLogOK.Name = "lOpenedLogOK";
            this.lOpenedLogOK.Size = new System.Drawing.Size(22, 17);
            this.lOpenedLogOK.TabIndex = 16;
            this.lOpenedLogOK.Text = "W";
            // 
            // lProjectFolderOK
            // 
            this.lProjectFolderOK.AutoSize = true;
            this.lProjectFolderOK.Font = new System.Drawing.Font("Wingdings 2", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(2)));
            this.lProjectFolderOK.Location = new System.Drawing.Point(371, 32);
            this.lProjectFolderOK.Name = "lProjectFolderOK";
            this.lProjectFolderOK.Size = new System.Drawing.Size(22, 17);
            this.lProjectFolderOK.TabIndex = 15;
            this.lProjectFolderOK.Text = "W";
            // 
            // gbOnlineFile
            // 
            this.gbOnlineFile.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbOnlineFile.Controls.Add(this.label4);
            this.gbOnlineFile.Controls.Add(this.label5);
            this.gbOnlineFile.Controls.Add(this.tYoutubeURL);
            this.gbOnlineFile.Controls.Add(this.btnDownloadSource);
            this.gbOnlineFile.Controls.Add(this.btnDownloadYoutube);
            this.gbOnlineFile.Controls.Add(this.tPackedLogsURL);
            this.gbOnlineFile.Location = new System.Drawing.Point(12, 276);
            this.gbOnlineFile.Name = "gbOnlineFile";
            this.gbOnlineFile.Size = new System.Drawing.Size(565, 110);
            this.gbOnlineFile.TabIndex = 16;
            this.gbOnlineFile.TabStop = false;
            this.gbOnlineFile.Text = "Online Files";
            // 
            // gbProjectInfo
            // 
            this.gbProjectInfo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbProjectInfo.BackColor = System.Drawing.SystemColors.Control;
            this.gbProjectInfo.Controls.Add(this.tTagBox);
            this.gbProjectInfo.Controls.Add(this.btnAddTag);
            this.gbProjectInfo.Controls.Add(this.tagContainer);
            this.gbProjectInfo.Location = new System.Drawing.Point(12, 12);
            this.gbProjectInfo.Name = "gbProjectInfo";
            this.gbProjectInfo.Size = new System.Drawing.Size(565, 111);
            this.gbProjectInfo.TabIndex = 17;
            this.gbProjectInfo.TabStop = false;
            this.gbProjectInfo.Text = "Project Info";
            // 
            // tTagBox
            // 
            this.tTagBox.AutoCompleteCustomSource.AddRange(new string[] {
            "San d\'Oria",
            "Bastok",
            "Windurst",
            "Jeuno",
            "Mhaura",
            "Selbina"});
            this.tTagBox.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.tTagBox.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.CustomSource;
            this.tTagBox.Location = new System.Drawing.Point(20, 19);
            this.tTagBox.Name = "tTagBox";
            this.tTagBox.Size = new System.Drawing.Size(127, 20);
            this.tTagBox.TabIndex = 0;
            this.tTagBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TTagBox_KeyDown);
            // 
            // btnAddTag
            // 
            this.btnAddTag.Location = new System.Drawing.Point(153, 19);
            this.btnAddTag.Name = "btnAddTag";
            this.btnAddTag.Size = new System.Drawing.Size(71, 23);
            this.btnAddTag.TabIndex = 1;
            this.btnAddTag.Text = "Add Tag";
            this.btnAddTag.UseVisualStyleBackColor = true;
            this.btnAddTag.Click += new System.EventHandler(this.BtnAddTag_Click);
            // 
            // tagContainer
            // 
            this.tagContainer.AutoSize = true;
            this.tagContainer.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tagContainer.Controls.Add(this.lTagLabel);
            this.tagContainer.Location = new System.Drawing.Point(9, 45);
            this.tagContainer.Name = "tagContainer";
            this.tagContainer.Size = new System.Drawing.Size(40, 13);
            this.tagContainer.TabIndex = 2;
            // 
            // lTagLabel
            // 
            this.lTagLabel.AutoSize = true;
            this.lTagLabel.Location = new System.Drawing.Point(3, 0);
            this.lTagLabel.Name = "lTagLabel";
            this.lTagLabel.Size = new System.Drawing.Size(34, 13);
            this.lTagLabel.TabIndex = 0;
            this.lTagLabel.Text = "Tags:";
            this.lTagLabel.Click += new System.EventHandler(this.LTagLabel_Click);
            // 
            // btnSave
            // 
            this.btnSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnSave.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnSave.ImageIndex = 4;
            this.btnSave.ImageList = this.IL;
            this.btnSave.Location = new System.Drawing.Point(12, 396);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(88, 23);
            this.btnSave.TabIndex = 7;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.BtnSave_Click);
            // 
            // btnCopySummary
            // 
            this.btnCopySummary.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCopySummary.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnCopySummary.ImageIndex = 3;
            this.btnCopySummary.ImageList = this.IL;
            this.btnCopySummary.Location = new System.Drawing.Point(411, 396);
            this.btnCopySummary.Name = "btnCopySummary";
            this.btnCopySummary.Size = new System.Drawing.Size(160, 23);
            this.btnCopySummary.TabIndex = 18;
            this.btnCopySummary.Text = "Copy Summary";
            this.btnCopySummary.UseVisualStyleBackColor = true;
            this.btnCopySummary.Click += new System.EventHandler(this.BtnCopySummary_Click);
            // 
            // btnExtractZip
            // 
            this.btnExtractZip.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnExtractZip.ImageIndex = 5;
            this.btnExtractZip.ImageList = this.IL;
            this.btnExtractZip.Location = new System.Drawing.Point(399, 70);
            this.btnExtractZip.Name = "btnExtractZip";
            this.btnExtractZip.Size = new System.Drawing.Size(160, 20);
            this.btnExtractZip.TabIndex = 19;
            this.btnExtractZip.Text = "Extract Archive";
            this.btnExtractZip.UseVisualStyleBackColor = true;
            this.btnExtractZip.Click += new System.EventHandler(this.BtnExtractZip_Click);
            // 
            // ProjectInfoForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(590, 431);
            this.Controls.Add(this.btnCopySummary);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.gbProjectInfo);
            this.Controls.Add(this.gbOnlineFile);
            this.Controls.Add(this.gbLocalFiles);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "ProjectInfoForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Project Information";
            this.Load += new System.EventHandler(this.ProjectInfoForm_Load);
            this.Shown += new System.EventHandler(this.ProjectInfoForm_Shown);
            this.gbLocalFiles.ResumeLayout(false);
            this.gbLocalFiles.PerformLayout();
            this.gbOnlineFile.ResumeLayout(false);
            this.gbOnlineFile.PerformLayout();
            this.gbProjectInfo.ResumeLayout(false);
            this.gbProjectInfo.PerformLayout();
            this.tagContainer.ResumeLayout(false);
            this.tagContainer.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox tProjectFolder;
        private System.Windows.Forms.TextBox tOpenedLog;
        private System.Windows.Forms.TextBox tSourceVideo;
        private System.Windows.Forms.TextBox tYoutubeURL;
        private System.Windows.Forms.TextBox tPackedLogsURL;
        private System.Windows.Forms.SaveFileDialog saveProjectDlg;
        private System.Windows.Forms.Button btnDownloadSource;
        private System.Windows.Forms.Button btnDownloadYoutube;
        private System.Windows.Forms.Button btnUploadToYoutube;
        private System.Windows.Forms.GroupBox gbLocalFiles;
        private System.Windows.Forms.GroupBox gbOnlineFile;
        private System.Windows.Forms.GroupBox gbProjectInfo;
        private System.Windows.Forms.TextBox tTagBox;
        private System.Windows.Forms.FlowLayoutPanel tagContainer;
        private System.Windows.Forms.Button btnAddTag;
        private System.Windows.Forms.Label lTagLabel;
        public System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Label lVideoSourceOK;
        private System.Windows.Forms.Label lOpenedLogOK;
        private System.Windows.Forms.Label lProjectFolderOK;
        public System.Windows.Forms.Button btnCopySummary;
        private System.Windows.Forms.Button btnMake7zip;
        private System.Windows.Forms.ImageList IL;
        private System.Windows.Forms.Button btnExtractZip;
    }
}