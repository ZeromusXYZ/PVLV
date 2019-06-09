﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using PacketViewerLogViewer.Packets;
using System.Diagnostics;
using System.IO;
using PacketViewerLogViewer.ClipboardHelper;
using SevenZip;
using System.Reflection;

namespace PacketViewerLogViewer
{
    public partial class ProjectInfoForm : Form
    {
        PacketTabPage tp ;
        int lastTagID = 0;
        private string CurrentArchieve = string.Empty;

        public ProjectInfoForm()
        {
            InitializeComponent();
        }

        private void AddTag(string name)
        {
            if (name.Trim(' ') == string.Empty)
                return;
            lastTagID++;
            var L = new Label();
            L.Tag = lastTagID;
            L.BorderStyle = BorderStyle.Fixed3D;
            L.BackColor = System.Drawing.SystemColors.Highlight;
            L.ForeColor = System.Drawing.SystemColors.HighlightText;
            tagContainer.Controls.Add(L);
            L.Text = name;
            L.AutoSize = true;
            L.Cursor = Cursors.No;
            L.Click += new EventHandler(LTagLabel_Click);
        }

        private void ClearTags()
        {
            foreach (Control c in tagContainer.Controls)
            {
                if ((c is Label) && (c.Tag != null) && ((int)c.Tag > 0))
                {
                    tagContainer.Controls.Remove(c as Label);
                }
            }
        }

        private void CreateVisualTags(string tagString)
        {
            ClearTags();
            var tags = tagString.Split(',').ToList();
            foreach(string t in tags)
            {
                var s = t.Trim(' ');
                AddTag(s);
            }
        }

        private string VisualTagsToString()
        {
            string res = string.Empty;
            foreach(Control c in tagContainer.Controls)
            {
                if ( (c is Label) && (c.Tag != null) && ((int)c.Tag > 0) )
                {
                    if (res != string.Empty)
                        res += ",";
                    res += (c as Label).Text;
                }
            }
            return res;
        }

        public void LoadFromPacketTapPage(PacketTabPage sourceTP)
        {
            tp = sourceTP;

            if (tp != null)
            {
                CreateVisualTags(tp.ProjectTags);
                tTagBox.Text = "";
                tProjectFolder.Text = tp.ProjectFolder;
                tOpenedLog.Text = tp.LoadedLogFile;
                tSourceVideo.Text = tp.LinkVideoFileName;
                tYoutubeURL.Text = tp.LinkYoutubeURL;
                tPackedLogsURL.Text = tp.LinkPacketsDownloadURL;

                gbProjectInfo.Text = "Project Information: " + Path.GetFileName(tp.ProjectFile);
            }
        }

        public void ApplyPacketTapPage()
        {
            if (tp != null)
            {
                tp.ProjectTags = VisualTagsToString();
                tp.ProjectFolder = tProjectFolder.Text;
                tp.LoadedLogFile = tOpenedLog.Text;
                tp.LinkVideoFileName = tSourceVideo.Text;
                tp.LinkYoutubeURL = tYoutubeURL.Text;
                tp.LinkPacketsDownloadURL = tPackedLogsURL.Text;
            }
        }

        private void ProjectInfoForm_Load(object sender, EventArgs e)
        {
            // Populate Autocomplete for tags
            tTagBox.AutoCompleteCustomSource.Clear();
            tTagBox.AutoCompleteCustomSource.AddRange(DataLookups.AllValues.ToArray());
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }

        private void BtnAddTag_Click(object sender, EventArgs e)
        {
            AddTag(tTagBox.Text);
            tTagBox.Text = "";
            tTagBox.Focus();
        }

        private void TTagBox_KeyDown(object sender, KeyEventArgs e)
        {
            if ((e.KeyCode == Keys.Enter) || (e.KeyCode == Keys.Return))
            {
                BtnAddTag_Click(null,null);
            }
        }

        private void LTagLabel_Click(object sender, EventArgs e)
        {
            if ((sender is Label) && ((int)(sender as Label).Tag > 0))
            {
                tagContainer.Controls.Remove(sender as Label);
            }
        }

        private void ProjectInfoForm_Shown(object sender, EventArgs e)
        {
            //tTagBox.Focus();
            btnSave.Focus();
        }

        private void BtnDownloadYoutube_Click(object sender, EventArgs e)
        {
            if (tYoutubeURL.Text != string.Empty)
                Process.Start(tYoutubeURL.Text);
        }

        private void BtnDownloadSource_Click(object sender, EventArgs e)
        {
            if (tPackedLogsURL.Text != string.Empty)
                Process.Start(tPackedLogsURL.Text);
        }

        private void ProjectInfo_TextChanged(object sender, EventArgs e)
        {
            bool res = true;
            // Project Folder
            var pDir = tProjectFolder.Text.TrimEnd(Path.DirectorySeparatorChar);
            var pAltName = Path.Combine(Path.GetDirectoryName(tp.ProjectFile), Path.GetFileNameWithoutExtension(tp.ProjectFile));
            if (Directory.Exists(pDir))
            {
                lProjectFolderOK.Text = "\x81";
                lProjectFolderOK.ForeColor = Color.LimeGreen;
                btnMake7zip.Enabled = true;

                if (File.Exists(pDir + ".7z"))
                {
                    CurrentArchieve = pDir + ".7z";
                    btnExtractZip.Enabled = true;
                    btnMake7zip.Enabled = false;
                }
                else
                if (File.Exists(pDir + ".zip"))
                {
                    CurrentArchieve = pDir + ".zip" ;
                    btnExtractZip.Enabled = true;
                    btnMake7zip.Enabled = false;
                }
                else
                if (File.Exists(pDir + ".rar"))
                {
                    CurrentArchieve = pDir + ".rar" ;
                    btnExtractZip.Enabled = true;
                    btnMake7zip.Enabled = false;
                }
                else
                if (File.Exists(pAltName + ".7z"))
                {
                    CurrentArchieve = pAltName + ".7z";
                    btnExtractZip.Enabled = true;
                    btnMake7zip.Enabled = false;
                }
                else
                if (File.Exists(pAltName + ".zip"))
                {
                    CurrentArchieve = pAltName + ".zip";
                    btnExtractZip.Enabled = true;
                    btnMake7zip.Enabled = false;
                }
                else
                if (File.Exists(pAltName + ".rar"))
                {
                    CurrentArchieve = pAltName + ".rar";
                    btnExtractZip.Enabled = true;
                    btnMake7zip.Enabled = false;
                }
                else
                {
                    CurrentArchieve = string.Empty;
                    btnExtractZip.Enabled = false;
                }

            }
            else
            {
                lProjectFolderOK.Text = "\xCE";
                lProjectFolderOK.ForeColor = Color.Red;
                btnMake7zip.Enabled = false;
                btnExtractZip.Enabled = false;
                CurrentArchieve = string.Empty;
                res = false;
            }

            // Attached Log file
            if (File.Exists(tOpenedLog.Text))
            {
                lOpenedLogOK.Text = "\x81";
                lOpenedLogOK.ForeColor = Color.LimeGreen;
            }
            else
            {
                lOpenedLogOK.Text = "\xCE";
                lOpenedLogOK.ForeColor = Color.Red;
                res = false;
            }

            // Linked Local Video
            if ( (tSourceVideo.Text == string.Empty) || (File.Exists(tSourceVideo.Text)) )
            {
                lVideoSourceOK.Text = "\x81";
                lVideoSourceOK.ForeColor = Color.LimeGreen;
            }
            else
            {
                lVideoSourceOK.Text = "\xCE";
                lVideoSourceOK.ForeColor = Color.Red;
            }

            btnDownloadSource.Enabled = (tPackedLogsURL.Text != string.Empty) && ( (tPackedLogsURL.Text.ToLower().StartsWith("http://")) || (tPackedLogsURL.Text.ToLower().StartsWith("https://")) );
            btnDownloadYoutube.Enabled = (tYoutubeURL.Text != string.Empty) && ((tYoutubeURL.Text.ToLower().StartsWith("http://")) || (tYoutubeURL.Text.ToLower().StartsWith("https://"))); ;

            btnSave.Enabled = res;
        }

        private void BtnCopySummary_Click(object sender, EventArgs e)
        {
            string cliptext = "";
            cliptext += "Name: " + Path.GetFileNameWithoutExtension(tp.ProjectFile) + "\n" ;
            if (tPackedLogsURL.Text != string.Empty)
                cliptext += "Logs: <" + tPackedLogsURL.Text + ">\n";
            if (tYoutubeURL.Text != string.Empty)
                cliptext += "Video: " + tYoutubeURL.Text + "\n";
            var t = VisualTagsToString();
            if (t != string.Empty)
                cliptext += "Tags: " + t + "\n";
            try
            {
                // Because nothing is ever as simple as the next line >.>
                // Clipboard.SetText(s);
                // Helper will (try to) prevent errors when copying to clipboard because of threading issues
                var cliphelp = new SetClipboardHelper(DataFormats.Text, cliptext);
                cliphelp.DontRetryWorkOnFailed = false;
                cliphelp.Go();
            }
            catch
            {
            }

        }

        public bool ExploreFile(string filePath)
        {
            if (!System.IO.File.Exists(filePath))
            {
                return false;
            }
            //Clean up file path so it can be navigated OK
            filePath = System.IO.Path.GetFullPath(filePath);
            System.Diagnostics.Process.Start("explorer.exe", string.Format("/select,\"{0}\"", filePath));
            return true;
        }

        private void BtnMake7zip_Click(object sender, EventArgs e)
        {
            using(var zipform = new CompressForm())
            {
                zipform.task = CompressForm.ZipTaskType.doZip;
                zipform.ArchiveFileName = Path.ChangeExtension(tp.ProjectFolder.TrimEnd(Path.DirectorySeparatorChar), ".7z");
                
                if (zipform.BuildArchieveFilesList(tProjectFolder.Text) <= 0)
                {
                    MessageBox.Show("Nothing to add", "Make .7z", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (zipform.ShowDialog() == DialogResult.OK)
                {
                    ProjectInfo_TextChanged(null, null);
                    ExploreFile(zipform.ArchiveFileName);
                    //MessageBox.Show("Done creating " + zipform.ArchiveFileName,"Make .7z",MessageBoxButtons.OK,MessageBoxIcon.Information);
                }
                else
                {
                    try
                    {
                        if (File.Exists(zipform.ArchiveFileName))
                            File.Delete(zipform.ArchiveFileName);
                    }
                    catch { }
                    MessageBox.Show("Error creating " + zipform.ArchiveFileName, "Make .7z", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void BtnExtractZip_Click(object sender, EventArgs e)
        {
            using (var zipform = new CompressForm())
            {
                zipform.task = CompressForm.ZipTaskType.doUnZip;
                zipform.ArchiveFileName = CurrentArchieve;
                zipform.ProjectName = Path.GetFileNameWithoutExtension(tp.ProjectFile);

                if (zipform.ShowDialog() == DialogResult.OK)
                {
                    // reload the project file after extraction
                    var oldDLUrl = tp.LinkPacketsDownloadURL;
                    var oldFile = tp.ProjectFile;
                    tp.LoadProjectFile(oldFile);
                    LoadFromPacketTapPage(tp);
                    tp.LinkPacketsDownloadURL = oldDLUrl;
                    tPackedLogsURL.Text = oldDLUrl;
                    ProjectInfo_TextChanged(null, null);
                    MessageBox.Show("Done extracting " + zipform.ArchiveFileName, "Extract Archive", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Error extracting " + zipform.ArchiveFileName, "Extract Archive", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}
