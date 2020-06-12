﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using PacketViewerLogViewer.Packets;
using System.IO;
using PacketViewerLogViewer.ClipboardHelper;
using PacketViewerLogViewer.PVLVHelper;
using PacketViewerLogViewer.FileExtHelper;
using PacketViewerLogViewer.FFXIUtils;
using PacketViewerLogViewer.helpers;

namespace PacketViewerLogViewer
{

    public partial class MainForm : Form
    {
        public static MainForm thisMainForm;
        List<string> AllUsedTempFiles = new List<string>();

        string defaultTitle = "";
        static readonly string urlGitHub = "https://github.com/ZeromusXYZ/PVLV";
        static readonly string urlDiscord = "https://discord.gg/GhVfDtK";
        static readonly string urlVideoLAN = "https://www.videolan.org/";
        static readonly string url7Zip = "https://www.7-zip.org/";
        static readonly string url7ZipRequiredVer = "https://sourceforge.net/p/sevenzip/discussion/45797/thread/adc65bfa/";

        public PacketParser CurrentPP;
        SearchParameters searchParameters;

        const string InfoGridHeader = "     |  0  1  2  3   4  5  6  7   8  9  A  B   C  D  E  F    | 0123456789ABCDEF\n" +
                "-----+----------------------------------------------------  -+------------------\n";

        public MainForm()
        {
            InitializeComponent();
            thisMainForm = this;
            searchParameters = new SearchParameters();
            searchParameters.Clear();
        }

        private void mmFileExit_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void mmAboutGithub_Click(object sender, EventArgs e)
        {
            Process.Start(urlGitHub);
        }

        private void mmAboutVideoLAN_Click(object sender, EventArgs e)
        {
            Process.Start(urlVideoLAN);
        }

        private void MmAboutDiscord_Click(object sender, EventArgs e)
        {
            Process.Start(urlDiscord);
        }

        private void mmAboutAbout_Click(object sender, EventArgs e)
        {
            using (AboutBoxForm ab = new AboutBoxForm())
            {
                ab.ShowDialog();
            }
        }

        private void RegisterFileExt()
        {
            try
            {
                // Might also need to check
                // HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Explorer\FileExts\
                FileAssociations.EnsureAssociationsSet();
                //FileAssociations.EnsureURIAssociationsSet();
            }
            catch
            {
                // Set File or URI Association failed ?
            }
        }

        private void LoadDataFromGameclient()
        {
            if ((Properties.Settings.Default.UseGameClientData == false) || (!Directory.Exists(FFXIHelper.FFXI_InstallationPath)))
                return;

            // Items
            FFXIHelper.FFXI_LoadItemsFromDats(ref DataLookups.ItemsList.items);
            DataLookups.ItemsList.UpdateData();

            // Enabled dynamic loading for dialog text
            DataLookups.DialogsList.EnableCache = true;

            // NPC Names
            var mobList = new Dictionary<uint, FFXI_MobListEntry>();
            mobList.Add(0, new FFXI_MobListEntry()); // Id 0 = "none"
            for (ushort z = 0; z < 0x1FF; z++)
                FFXIHelper.FFXI_LoadMobListForZone(ref mobList, z);
            DataLookups.NLUOrCreate("@actors").AddValuesFromMobList(ref mobList);
            DataLookups.NLUOrCreate("npcname").AddValuesFromMobList(ref mobList); // Not sure if we're ever gonna use this, but meh
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            defaultTitle = Text;
            RegisterFileExt();
            PacketColors.UpdateColorsFromSettings();
            Application.UseWaitCursor = true;
            try
            {
                Directory.SetCurrentDirectory(Application.StartupPath);
                if (DataLookups.LoadLookups() == false)
                {
                    MessageBox.Show("Errors while loading lookup data: " + DataLookups.AllLoadErrors, "Error Loading Lookup Data", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                }

                if (FFXIHelper.FindPaths())
                    LoadDataFromGameclient();
            }
            catch (Exception x)
            {
                MessageBox.Show("Exception: " + x.Message, "Loading Lookup Data", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                Close();
                return;
            }
            tcPackets.TabPages.Clear();
            Application.UseWaitCursor = false;
        }



        private void mmFileOpen_Click(object sender, EventArgs e)
        {
            openLogFileDialog.Title = "Open log file";
            if (openLogFileDialog.ShowDialog() != DialogResult.OK)
                return;
            TryOpenFile(openLogFileDialog.FileName);
        }

        private void TryOpenFile(string aFileName)
        {
            if (Path.GetExtension(aFileName).ToLower() == ".pvlv")
            {
                // Open Project File
                TryOpenProjectFile(aFileName);
            }
            else
            {
                TryOpenLogFile(aFileName, true);
            }
        }

        private void TryOpenProjectFile(string ProjectFile)
        {
            PacketTabPage tp = CreateNewPacketsTabPage();
            tp.LoadProjectFile(ProjectFile);
            tp.Text = Helper.MakeTabName(ProjectFile);

            using (var projectDlg = new ProjectInfoForm())
            {
                projectDlg.LoadFromPacketTapPage(tp);
                projectDlg.btnSave.Text = "Open";
                projectDlg.cbOpenedLog.Enabled = true;
                if (projectDlg.ShowDialog() == DialogResult.OK)
                {
                    projectDlg.ApplyPacketTapPage();
                    TryOpenLogFile(tp.LoadedLogFile, false);
                    tp.SaveProjectFile();
                }
                else
                {
                    tcPackets.TabPages.Remove(tp);
                }
            }

        }

        private void TryOpenLogFile(string logFile, bool alsoLoadProject)
        {

            PacketTabPage tp;
            if (alsoLoadProject)
            {
                tp = CreateNewPacketsTabPage();
                tp.LoadProjectFileFromLogFile(logFile);
            }
            else
            {
                tp = GetCurrentPacketTabPage();
            }

            //tp.ProjectFolder = Helper.MakeProjectDirectoryFromLogFileName(logFile);
            tp.Text = Helper.MakeTabName(logFile);

            tp.PLLoaded.Clear();
            tp.PLLoaded.Filter.Clear();
            if (!tp.PLLoaded.LoadFromFile(logFile))
            {
                MessageBox.Show("Error loading file: " + logFile, "File Open Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                tp.PLLoaded.Clear();
                tcPackets.TabPages.Remove(tp);
                return;
            }
            if (tp.PLLoaded.Count() <= 0)
            {
                MessageBox.Show("File contains no useful data.\n" + logFile, "File Open Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                tcPackets.TabPages.Remove(tp);
                return;
            }
            Text = defaultTitle + " - " + logFile;
            tp.LoadedLogFile = logFile;
            tp.PL.CopyFrom(tp.PLLoaded);
            tp.FillListBox();
            UpdateStatusBarAndTitle(tp);
            if (Properties.Settings.Default.AutoOpenVideoForm && ((tp.LinkVideoFileName != string.Empty) || (tp.LinkYoutubeURL != string.Empty)))
            {
                MmVideoOpenLink_Click(null, null);
            }
        }

        public void lbPackets_SelectedIndexChanged(object sender, EventArgs e)
        {
            ListBox lb = (sender as ListBox);
            if (!(lb.Parent is PacketTabPage))
                return;
            PacketTabPage tp = (lb.Parent as PacketTabPage);
            if ((lb.SelectedIndex < 0) || (lb.SelectedIndex >= tp.PL.Count()))
            {
                rtInfo.SelectionColor = rtInfo.ForeColor;
                rtInfo.SelectionBackColor = rtInfo.BackColor;
                rtInfo.Text = "Please select a valid item from the list";
                return;
            }
            PacketData pd = tp.PL.GetPacket(lb.SelectedIndex);
            cbShowBlock.Enabled = false;
            UpdatePacketDetails(tp, pd, "-");
            cbShowBlock.Enabled = true;
            lb.Invalidate();
            if ((tp.videoLink != null) && (tp.videoLink.cbFollowPacketList.Checked))
            {
                tp.videoLink.MoveToDateTime(pd.VirtualTimeStamp);
            }
        }


        private void cbOriginalData_CheckedChanged(object sender, EventArgs e)
        {
            PacketTabPage tp = GetCurrentPacketTabPage();
            if (tp == null)
            {
                rtInfo.SelectionColor = rtInfo.ForeColor;
                rtInfo.SelectionBackColor = rtInfo.BackColor;
                rtInfo.Text = "Please select open a list first";
                return;
            }

            PacketData pd = tp.GetSelectedPacket();
            if (pd == null)
            {
                rtInfo.SelectionColor = rtInfo.ForeColor;
                rtInfo.SelectionBackColor = rtInfo.BackColor;
                rtInfo.Text = "Please select a valid item from the list";
                return;
            }

            UpdatePacketDetails(tp, pd, "-");
        }

        private void mmFileClose_Click(object sender, EventArgs e)
        {
            if ((tcPackets.SelectedIndex >= 0) && (tcPackets.SelectedIndex < tcPackets.TabCount))
            {
                tcPackets.TabPages.RemoveAt(tcPackets.SelectedIndex);
            }
            /*
            PLLoaded.Clear();
            PLLoaded.ClearFilters();
            PL.Clear();
            PL.ClearFilters();
            FillListBox(lbPackets,PL);
            */
        }

        private void mmFileAppend_Click(object sender, EventArgs e)
        {
            openLogFileDialog.Title = "Append log file";
            if (openLogFileDialog.ShowDialog() != DialogResult.OK)
                return;

            PacketTabPage tp = GetCurrentOrNewPacketTabPage();
            tp.Text = "Multi";
            tp.LoadedLogFile = "?Multiple Sources";
            tp.ProjectFolder = string.Empty;

            if (!tp.PLLoaded.LoadFromFile(openLogFileDialog.FileName))
            {
                MessageBox.Show("Error loading file: " + openLogFileDialog.FileName, "File Append Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                tp.PLLoaded.Clear();
                return;
            }
            Text = defaultTitle + " - " + tp.LoadedLogFile;
            tp.PL.CopyFrom(tp.PLLoaded);
            tp.FillListBox();
            UpdateStatusBarAndTitle(tp);
        }

        private void RawDataToRichText(PacketParser pp, RichTextBox rt)
        {
            RichTextBox rtInfo = rt;
            string rtf = string.Empty;
            List<Color> colorTable = new List<Color>();
            int LastForeCol = -1;
            int LastBackCol = -1;

            int GetRTFColor(Color col)
            {
                var p = colorTable.IndexOf(col);
                if (p < 0)
                {
                    p = colorTable.Count;
                    colorTable.Add(col);
                }
                return p + 1;
            }

            void SetRTFColor(Color Fore, Color Back)
            {
                var f = GetRTFColor(Fore);
                var b = GetRTFColor(Back);
                //rtf += "\\cf" + f.ToString() + "\\highlight" + b.ToString();
                if ((f == LastForeCol) && (b == LastBackCol))
                    return;
                if (f != LastForeCol)
                    rtf += "\\cf" + f.ToString();
                if (b != LastBackCol)
                    rtf += "\\highlight" + b.ToString();
                rtf += " ";
                LastForeCol = f;
                LastBackCol = b;
            }


            string BuildHeaderWithColorTable()
            {
                string rtfHead = string.Empty;
                rtfHead += "{\\rtf1\\ansi\\ansicpg1252\\deff0\\nouicompat\\deflang2057{\\fonttbl{\\f0\\fnil\\fcharset0 Consolas;}}";
                rtfHead += "{\\colortbl;";
                foreach (var col in colorTable)
                {
                    rtfHead += "\\red" + col.R.ToString() + "\\green" + col.G.ToString() + "\\blue" + col.B.ToString() + ";";
                }
                rtfHead += "}";
                rtfHead += "\\viewkind4\\uc1\\pard\\cf1\\highlight2\\f0\\fs18 ";
                // {\colortbl ;\red169\green169\blue169;\red255\green255\blue255;\red25\green25\blue112;\red0\green0\blue0;\red210\green105\blue30;\red100\green149\blue237;\red60\green179\blue113;\red233\green150\blue122;\red165\green42\blue42;}
                return rtfHead;
            }

            void SetColorBasic(byte n)
            {
                SetRTFColor(rtInfo.ForeColor, rtInfo.BackColor);
                //rtInfo.SelectionFont = rtInfo.Font;
                //rtInfo.SelectionColor = rtInfo.ForeColor;
                //rtInfo.SelectionBackColor = rtInfo.BackColor;
            }

            void SetColorGrid()
            {
                SetRTFColor(Color.DarkGray, rtInfo.BackColor);
                //rtInfo.SelectionFont = rtInfo.Font;
                //rtInfo.SelectionColor = Color.DarkGray;
                //rtInfo.SelectionBackColor = rtInfo.BackColor;
            }

            void SetColorSelect(byte n, bool forchars)
            {
                //if (!forchars)
                //{
                //    rtInfo.SelectionFont = new Font(rtInfo.Font, FontStyle.Italic);
                //}
                //else
                //{
                //    rtInfo.SelectionFont = rtInfo.Font;
                //}
                SetRTFColor(Color.Yellow, Color.DarkBlue);
                //rtInfo.SelectionColor = Color.Yellow;
                //rtInfo.SelectionBackColor = Color.DarkBlue;
            }

            void SetColorNotSelect(byte n, bool forchars)
            {
                //rtInfo.SelectionFont = rtInfo.Font;
                if ((pp.SelectedFields.Count > 0) || forchars)
                {
                    SetRTFColor(pp.GetDataColor(n), rtInfo.BackColor);
                    //rtInfo.SelectionColor = pp.GetDataColor(n);
                    //rtInfo.SelectionBackColor = rtInfo.BackColor;
                }
                else
                {
                    SetRTFColor(rtInfo.BackColor, pp.GetDataColor(n));
                    //rtInfo.SelectionColor = rtInfo.BackColor;
                    //rtInfo.SelectionBackColor = pp.GetDataColor(n);
                }
            }


            void AddChars(int startIndex)
            {
                SetColorGrid();
                rtf += "  | ";
                //rtInfo.AppendText("  | ");
                for (int c = 0; (c < 0x10) && ((startIndex + c) < pp.ParsedBytes.Count); c++)
                {
                    var n = pp.ParsedBytes[startIndex + c];
                    if (pp.SelectedFields.IndexOf(n) >= 0)
                    {
                        SetColorSelect(n, true);
                    }
                    else
                    {
                        SetColorNotSelect(n, true);
                    }
                    char ch = (char)pp.PD.GetByteAtPos(startIndex + c);
                    if (ch == 92)
                        rtf += "\\\\";
                    else
                    if (ch == 64)
                        rtf += "\\@";
                    else
                    if (ch == 123)
                        rtf += "\\{";
                    else
                    if (ch == 125)
                        rtf += "\\}";
                    else
                    if ((ch < 32) || (ch >= 128))
                        rtf += '.';
                    else
                        rtf += ch.ToString();
                    //rtInfo.AppendText(ch.ToString());
                }
            }

            rtInfo.SuspendLayout();
            rtInfo.ForeColor = SystemColors.WindowText;
            rtInfo.BackColor = SystemColors.Window;
            // rtInfo.Clear();

            SetColorGrid();

            rtf += InfoGridHeader.Replace("\n", "\\par\n");
            //rtInfo.AppendText(InfoGridHeader);
            int addCharCount = 0;
            byte lastFieldIndex = 0;

            for (int i = 0; i < pp.PD.RawBytes.Count; i += 0x10)
            {
                SetColorGrid();
                rtf += i.ToString("X").PadLeft(4, ' ') + " | ";
                //rtInfo.AppendText(i.ToString("X").PadLeft(4, ' ') + " | ");
                for (int i2 = 0; i2 < 0x10; i2++)
                {
                    if ((i + i2) < pp.ParsedBytes.Count)
                    {
                        var n = pp.ParsedBytes[i + i2];
                        lastFieldIndex = n;
                        if (pp.SelectedFields.Count > 0)
                        {
                            if (pp.SelectedFields.IndexOf(n) >= 0)
                            {
                                // Is selected field
                                SetColorSelect(n, false);
                            }
                            else
                            {
                                // we have non-selected field
                                SetColorNotSelect(n, false);
                            }
                        }
                        else
                        {
                            // No fields selected
                            SetColorNotSelect(n, false);
                        }
                        rtf += pp.PD.GetByteAtPos(i + i2).ToString("X2");
                        //rtInfo.AppendText(pp.PD.GetByteAtPos(i + i2).ToString("X2"));
                        addCharCount++;
                    }
                    else
                    {
                        SetColorGrid();
                        rtf += "  ";
                        //rtInfo.AppendText("  ");
                    }

                    if ((i + i2 + 1) < pp.ParsedBytes.Count)
                    {
                        var n = pp.ParsedBytes[i + i2 + 1];
                        if (n != lastFieldIndex)
                        {
                            SetColorBasic(n);
                        }
                    }
                    else
                    {
                        SetColorGrid();
                    }

                    rtf += " ";
                    // rtInfo.AppendText(" ");
                    if ((i2 % 0x4) == 0x3)
                    {
                        rtf += " ";
                        //rtInfo.AppendText(" ");
                    }
                }
                if (addCharCount > 0)
                {
                    AddChars(i);
                    addCharCount = 0;
                }
                rtf += "\\par\n";
                // rtInfo.AppendText("\r\n");
            }
            rtf += "}\n";
            rtInfo.WordWrap = false;
            rtInfo.Rtf = BuildHeaderWithColorTable() + rtf;
            rtInfo.Refresh();
            rtInfo.ResumeLayout();
        }

        public void UpdatePacketDetails(PacketTabPage tp, PacketData pd, string SwitchBlockName, bool dontReloadParser = false)
        {
            if ((tp == null) || (pd == null))
                return;
            tp.CurrentSync = pd.PacketSync;
            lInfo.Text = pd.OriginalHeaderText;
            rtInfo.Clear();

            if ((dontReloadParser == false) || (pd.PP == null))
            {
                pd.PP = new PacketParser(pd.PacketID, pd.PacketLogType);
                pd.PP.AssignPacket(pd);
            }

            if (pd.PP == null)
                return;

            if ((tp.PL.IsPreParsed == false) || (pd.PP.PreParsedSwitchBlock != SwitchBlockName))
                pd.PP.ParseData(SwitchBlockName);

            CurrentPP = pd.PP;
            CurrentPP.ToGridView(dGV);
            cbShowBlock.Enabled = false;
            if (CurrentPP.SwitchBlocks.Count > 0)
            {
                cbShowBlock.Items.Clear();
                cbShowBlock.Items.Add("-");
                cbShowBlock.Items.AddRange(CurrentPP.SwitchBlocks.ToArray());
                cbShowBlock.Show();
            }
            else
            {
                cbShowBlock.Items.Clear();
                cbShowBlock.Hide();
            }
            for (int i = 0; i < cbShowBlock.Items.Count; i++)
            {
                if ((SwitchBlockName == "-") && (cbShowBlock.Items[i].ToString() == CurrentPP.LastSwitchedBlock))
                {
                    if (cbShowBlock.SelectedIndex != i)
                        cbShowBlock.SelectedIndex = i;
                    //break;
                }
                else
                if (cbShowBlock.Items[i].ToString() == SwitchBlockName)
                {
                    if (cbShowBlock.SelectedIndex != i)
                        cbShowBlock.SelectedIndex = i;
                    //break;
                }
            }
            cbShowBlock.Enabled = true;

            if (cbOriginalData.Checked)
            {
                rtInfo.SuspendLayout();
                rtInfo.SelectionColor = rtInfo.ForeColor;
                rtInfo.SelectionBackColor = rtInfo.BackColor;
                rtInfo.Text = "Source:\r\n" + string.Join("\r\n", pd.RawText.ToArray());
                rtInfo.Refresh();
                rtInfo.ResumeLayout();
            }
            else
            {
                RawDataToRichText(CurrentPP, rtInfo);
            }

        }

        private void mmFileSettings_Click(object sender, EventArgs e)
        {
            using (SettingsForm settingsDialog = new SettingsForm())
            {
                if (settingsDialog.ShowDialog() == DialogResult.OK)
                {
                    Properties.Settings.Default.Save();
                    PacketColors.UpdateColorsFromSettings();
                    LoadDataFromGameclient();
                    //MessageBox.Show("Settings saved");
                }
                settingsDialog.Dispose();
            }
        }

        private void CbShowBlock_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!cbShowBlock.Enabled)
                return;

            if (!(tcPackets.SelectedTab is PacketTabPage))
                return;
            PacketTabPage tp = (tcPackets.SelectedTab as PacketTabPage);

            cbShowBlock.Enabled = false;
            if ((tp.lbPackets.SelectedIndex < 0) || (tp.lbPackets.SelectedIndex >= tp.PL.Count()))
            {
                rtInfo.SelectionColor = rtInfo.ForeColor;
                rtInfo.SelectionBackColor = rtInfo.BackColor;
                rtInfo.Text = "Please select a valid item from the list";
                return;
            }
            PacketData pd = tp.PL.GetPacket(tp.lbPackets.SelectedIndex);
            var sw = cbShowBlock.SelectedIndex;
            if (sw >= 0)
            {
                UpdatePacketDetails(tp, pd, cbShowBlock.Items[sw].ToString(), true);
            }
            else
            {
                UpdatePacketDetails(tp, pd, "-", true);
            }
            cbShowBlock.Enabled = true;
            tp.lbPackets.Invalidate();
        }

        private void dGV_SelectionChanged(object sender, EventArgs e)
        {
            if ((CurrentPP == null) || (CurrentPP.PD == null))
                return;
            if (dGV.Tag != null)
                return;
            CurrentPP.SelectedFields.Clear();
            for (int i = 0; i < dGV.RowCount; i++)
            {
                if ((dGV.Rows[i].Selected) && (i < CurrentPP.ParsedView.Count))
                {
                    var f = CurrentPP.ParsedView[i].FieldIndex;
                    //if (f != 0xFF)
                    CurrentPP.SelectedFields.Add(f);
                }
            }
            CurrentPP.ToGridView(dGV);
            RawDataToRichText(CurrentPP, rtInfo);
        }


        public void UpdateStatusBarAndTitle(PacketTabPage tp)
        {
            if (tp == null)
            {
                sbProjectInfo.Text = "Not a project";
                sbExtraInfo.Text = "";
                return;
            }

            var t = tp.LoadedLogFile;
            if (t.StartsWith("?"))
                t = t.TrimStart('?');
            Text = defaultTitle + " - " + t;
            if (tp.ProjectFolder != string.Empty)
                sbProjectInfo.Text = "Project Folder: " + tp.ProjectFolder;
            else
                sbProjectInfo.Text = "Not a project";

            if (File.Exists(tp.LinkVideoFileName))
                sbExtraInfo.Text = "Local Video Linked";
            else
            if (tp.LinkYoutubeURL != string.Empty)
                sbExtraInfo.Text = "Youtube Linked";
            else
                sbExtraInfo.Text = "";
        }

        private void TcPackets_SelectedIndexChanged(object sender, EventArgs e)
        {
            TabControl tc = (sender as TabControl);
            if (!(tc.SelectedTab is PacketTabPage))
            {
                UpdateStatusBarAndTitle(null);
                return;
            }
            PacketTabPage tp = (tc.SelectedTab as PacketTabPage);
            UpdateStatusBarAndTitle(tp);
            PacketData pd = tp.PL.GetPacket(tp.lbPackets.SelectedIndex);
            cbShowBlock.Enabled = false;
            UpdatePacketDetails(tp, pd, "-");
            cbShowBlock.Enabled = true;
        }

        private void MmAddFromClipboard_Click(object sender, EventArgs e)
        {
            if ((!Clipboard.ContainsText()) || (Clipboard.GetText() == string.Empty))
            {
                MessageBox.Show("Nothing to paste", "Paste from Clipboard", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            try
            {
                PacketTabPage tp = GetCurrentOrNewPacketTabPage();
                var cText = Clipboard.GetText().Replace("\r", "");
                List<string> clipText = new List<string>();
                clipText.AddRange(cText.Split((char)10).ToList());

                tp.Text = "Clipboard   ";
                tp.LoadedLogFile = "?Paste from Clipboard";
                tp.ProjectFolder = string.Empty;

                if (!tp.PLLoaded.LoadFromStringList(clipText, PacketLogFileFormats.Unknown, PacketLogTypes.Unknown))
                {
                    MessageBox.Show("Error loading data from clipboard", "Clipboard Paste Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    tp.PLLoaded.Clear();
                    tcPackets.TabPages.Remove(tp);
                    return;
                }
                if (tp.PLLoaded.Count() <= 0)
                {
                    MessageBox.Show("Clipboard contained no useful data.", "Clipboard Paste", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    tcPackets.TabPages.Remove(tp);
                    return;
                }
                Text = defaultTitle + " - " + tp.LoadedLogFile;
                tp.PL.CopyFrom(tp.PLLoaded);
                tp.FillListBox();
                UpdateStatusBarAndTitle(tp);
            }
            catch (Exception x)
            {
                MessageBox.Show("Paste Failed, Exception: " + x.Message, "Paste from Clipboard", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private PacketTabPage CreateNewPacketsTabPage()
        {
            PacketTabPage tp = new PacketTabPage(this);
            tp.lbPackets.SelectedIndexChanged += lbPackets_SelectedIndexChanged;
            tcPackets.TabPages.Add(tp);
            tcPackets.SelectedTab = tp;
            tp.lbPackets.Focus();
            return tp;
        }

        private PacketTabPage GetCurrentOrNewPacketTabPage()
        {
            PacketTabPage tp = GetCurrentPacketTabPage();
            if (tp == null)
            {
                tp = CreateNewPacketsTabPage();
            }
            return tp;
        }

        public PacketTabPage GetCurrentPacketTabPage()
        {
            if (!(tcPackets.SelectedTab is PacketTabPage))
            {
                return null;
            }
            else
            {
                return (tcPackets.SelectedTab as PacketTabPage);
            }
        }

        private void MmFilterEdit_Click(object sender, EventArgs e)
        {
            var tp = GetCurrentPacketTabPage();
            using (var filterDlg = new FilterForm())
            {
                filterDlg.btnOK.Enabled = (tp != null);
                if (tp != null)
                {
                    filterDlg.Filter.CopyFrom(tp.PL.Filter);
                    filterDlg.LoadLocalFromFilter();
                }
                if (filterDlg.ShowDialog(this) == DialogResult.OK)
                {
                    filterDlg.SaveLocalToFilter();
                    UInt16 lastSync = tp.CurrentSync;
                    tp.PL.Filter.CopyFrom(filterDlg.Filter);
                    tp.PL.FilterFrom(tp.PLLoaded);
                    tp.FillListBox(lastSync);
                    tp.CenterListBox();

                }
            }
        }

        private void MmFilterReset_Click(object sender, EventArgs e)
        {
            var tp = GetCurrentPacketTabPage();
            if (tp != null)
            {
                UInt16 lastSync = tp.CurrentSync;
                tp.PL.Filter.Clear();
                tp.PL.CopyFrom(tp.PLLoaded);
                tp.FillListBox(lastSync);
                tp.CenterListBox();
            }

        }

        private void MmFilterApply_Click(object sender, EventArgs e)
        {
        }

        private void MMFilterApplyItem_Click(object sender, EventArgs e)
        {
            var tp = GetCurrentPacketTabPage();
            if (tp == null)
                return;

            if (sender is ToolStripMenuItem)
            {
                var mITem = (sender as ToolStripMenuItem);
                // apply filter
                UInt16 lastSync = tp.CurrentSync;
                tp.PL.Filter.LoadFromFile(Path.Combine(Application.StartupPath, "data", "filter", mITem.Text + ".pfl"));
                tp.PL.FilterFrom(tp.PLLoaded);
                tp.FillListBox(lastSync);
                tp.CenterListBox();
            }
        }

        private void MmFilterApply_DropDownOpening(object sender, EventArgs e)
        {
            // generate menu
            // GetFiles
            try
            {
                mmFilterApply.DropDownItems.Clear();
                var di = new DirectoryInfo(Path.Combine(Application.StartupPath, "data", "filter"));
                var files = di.GetFiles("*.pfl");
                foreach (var fi in files)
                {
                    ToolStripMenuItem mi = new ToolStripMenuItem(Path.GetFileNameWithoutExtension(fi.Name));
                    mi.Click += MMFilterApplyItem_Click;
                    mmFilterApply.DropDownItems.Add(mi);
                }
                if (files.Length <= 0)
                {
                    ToolStripMenuItem mi = new ToolStripMenuItem("no filters found");
                    mi.Enabled = false;
                    mmFilterApply.DropDownItems.Add(mi);
                }
            }
            catch
            {
                // Do nothing
            }
        }

        private void MmSearchSearch_Click(object sender, EventArgs e)
        {
            var tp = GetCurrentPacketTabPage();
            if (tp == null)
                return;
            using (SearchForm SearchDlg = new SearchForm())
            {
                if (tp.PL.IsPreParsed == false)
                {
                    searchParameters.SearchByParsedData = false;
                    SearchDlg.gbSearchByField.Enabled = false;
                }
                SearchDlg.searchParameters.CopyFrom(this.searchParameters);
                var res = SearchDlg.ShowDialog();
                if ((res == DialogResult.OK) || (res == DialogResult.Retry))
                {
                    searchParameters.CopyFrom(SearchDlg.searchParameters);
                    if (res == DialogResult.OK)
                        FindNext();
                    else
                    if (res == DialogResult.Retry)
                        FindAsNewTab();
                }
            }
        }

        private void MmSearchNext_Click(object sender, EventArgs e)
        {
            var tp = GetCurrentPacketTabPage();
            if (tp == null)
                return;
            if ((searchParameters.SearchIncoming == false) && (searchParameters.SearchOutgoing == false))
            {
                MmSearchSearch_Click(null, null);
                return;
            }
            else
                FindNext();
        }

        private void FindNext()
        {
            var tp = GetCurrentPacketTabPage();

            if ((tp == null) || (tp.lbPackets.Items.Count <= 0))
            {
                MessageBox.Show("Nothing to search in !", "Search", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var startIndex = tp.lbPackets.SelectedIndex;
            if ((startIndex < 0) && (startIndex >= tp.lbPackets.Items.Count))
                startIndex = -1;
            int i = startIndex + 1;
            for (int c = 0; c < tp.lbPackets.Items.Count - 1; c++)
            {
                if (i >= tp.lbPackets.Items.Count)
                    i = 0;
                var pd = tp.PL.GetPacket(i);
                if (pd.MatchesSearch(searchParameters))
                {
                    // Select index
                    tp.lbPackets.SelectedIndex = i;
                    // Move to center
                    var iHeight = tp.lbPackets.ItemHeight;
                    if (iHeight <= 0)
                        iHeight = 8;
                    var iCount = tp.lbPackets.Size.Height / iHeight;
                    var tPos = i - (iCount / 2);
                    if (tPos < 0)
                        tPos = 0;
                    tp.lbPackets.TopIndex = tPos;
                    tp.lbPackets.Focus();
                    // We're done
                    return;
                }
                i++;
            }
            MessageBox.Show("No matches found !", "Search", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void FindAsNewTab()
        {
            var tp = GetCurrentPacketTabPage();

            if ((tp == null) || (tp.lbPackets.Items.Count <= 0))
            {
                MessageBox.Show("Nothing to search in !", "Search as New Tab", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            PacketTabPage newtp = CreateNewPacketsTabPage();
            newtp.Text = "*" + tp.Text;
            newtp.LoadedLogFile = "Search Result";

            var count = newtp.PLLoaded.SearchFrom(tp.PL, searchParameters);

            if (count <= 0)
            {
                MessageBox.Show("No matches found !", "Search as New Tab", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                newtp.PL.CopyFrom(newtp.PLLoaded);
                newtp.FillListBox();
            }
            UpdateStatusBarAndTitle(newtp);
        }

        private void MmFilePasteNew_Click(object sender, EventArgs e)
        {

            if ((!Clipboard.ContainsText()) || (Clipboard.GetText() == string.Empty))
            {
                MessageBox.Show("Nothing to paste", "Paste from Clipboard", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            try
            {
                PacketTabPage tp = CreateNewPacketsTabPage();
                tp.Text = "Clipboard   ";
                tp.LoadedLogFile = "?Paste from Clipboard";
                tp.ProjectFolder = string.Empty;
                tcPackets.SelectedTab = tp;

                var cText = Clipboard.GetText().Replace("\r", "");
                List<string> clipText = new List<string>();
                clipText.AddRange(cText.Split((char)10).ToList());

                if (!tp.PLLoaded.LoadFromStringList(clipText, PacketLogFileFormats.Unknown, PacketLogTypes.Unknown))
                {
                    MessageBox.Show("Error loading data from clipboard", "Clipboard Paste Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    tp.PLLoaded.Clear();
                    tcPackets.TabPages.Remove(tp);
                    return;
                }
                if (tp.PLLoaded.Count() <= 0)
                {
                    MessageBox.Show("Clipboard contained no useful data.", "Clipboard Paste", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    tcPackets.TabPages.Remove(tp);
                    return;
                }
                Text = defaultTitle + " - " + tp.LoadedLogFile;
                tp.PL.CopyFrom(tp.PLLoaded);
                tp.FillListBox();
                UpdateStatusBarAndTitle(tp);
            }
            catch (Exception x)
            {
                MessageBox.Show("Paste Failed, Exception: " + x.Message, "Paste from Clipboard", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void BtnCopyRawSource_Click(object sender, EventArgs e)
        {
            PacketTabPage tp = GetCurrentPacketTabPage();
            if (tp == null)
            {
                MessageBox.Show("No Packet List selected", "Copy", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            PacketData pd = tp.GetSelectedPacket();
            if (pd == null)
            {
                MessageBox.Show("No Packet selected", "Copy", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string cliptext = "";
            foreach (string s in pd.RawText)
            {
                // re-add the linefeeds
                if (cliptext != string.Empty)
                    cliptext += "\n";
                cliptext += s;
            }
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

        private void TcPackets_DrawItem(object sender, DrawItemEventArgs e)
        {
            // Source: https://social.technet.microsoft.com/wiki/contents/articles/50957.c-winform-tabcontrol-with-add-and-close-button.aspx
            // Adapted to using resources and without the add button
            try
            {
                TabControl tabControl = (sender as TabControl);
                var tabPage = tabControl.TabPages[e.Index];
                var tabRect = tabControl.GetTabRect(e.Index);
                tabRect.Inflate(-2, -2);
                var closeImage = Properties.Resources.close_icon;
                if ((tabControl.Alignment == TabAlignment.Top) || (tabControl.Alignment == TabAlignment.Bottom))
                {
                    // for tabs at the top/bottom
                    e.Graphics.DrawImage(closeImage,
                        (tabRect.Right - closeImage.Width),
                        tabRect.Top + (tabRect.Height - closeImage.Height) / 2);
                    TextRenderer.DrawText(e.Graphics, tabPage.Text, tabPage.Font,
                        tabRect, tabPage.ForeColor, TextFormatFlags.Left);
                }
                else
                if (tabControl.Alignment == TabAlignment.Left)
                {
                    // for tabs to the left
                    e.Graphics.DrawImage(closeImage,
                        tabRect.Left + (tabRect.Width - closeImage.Width) / 2,
                        tabRect.Top);
                    var tSize = e.Graphics.MeasureString(tabPage.Text, tabPage.Font);
                    e.Graphics.TranslateTransform(tabRect.Left + tabRect.Width, tabRect.Bottom);
                    e.Graphics.RotateTransform(-90);
                    var textBrush = new SolidBrush(tabPage.ForeColor);
                    e.Graphics.DrawString(tabPage.Text, tabPage.Font, textBrush, 0, -tabRect.Width - (tSize.Height / -4), StringFormat.GenericDefault);
                }
                else
                {
                    // If you want it on the right as well, you code it >.>
                }
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }

        private void TcPackets_MouseDown(object sender, MouseEventArgs e)
        {
            // Process MouseDown event only till (tabControl.TabPages.Count - 1) excluding the last TabPage
            TabControl tabControl = (sender as TabControl);
            for (var i = 0; i < tabControl.TabPages.Count; i++)
            {
                var tabRect = tabControl.GetTabRect(i);
                tabRect.Inflate(-2, -2);
                var closeImage = Properties.Resources.close_icon;
                Rectangle imageRect;
                if ((tabControl.Alignment == TabAlignment.Top) || (tabControl.Alignment == TabAlignment.Bottom))
                {
                    imageRect = new Rectangle(
                        (tabRect.Right - closeImage.Width),
                        tabRect.Top + (tabRect.Height - closeImage.Height) / 2,
                        closeImage.Width,
                        closeImage.Height);
                }
                else
                {
                    imageRect = new Rectangle(
                        tabRect.Left + (tabRect.Width - closeImage.Width) / 2,
                        tabRect.Top,
                        closeImage.Width,
                        closeImage.Height);
                }
                if (imageRect.Contains(e.Location))
                {
                    tabControl.TabPages.RemoveAt(i);
                    break;
                }
            }
        }

        public void OpenParseEditor(string parseFileName)
        {
            string editFile = Application.StartupPath + Path.DirectorySeparatorChar + parseFileName;
            if (!File.Exists(editFile))
            {
                if (MessageBox.Show("Parser \"" + parseFileName + "\" doesn't exists, create one ?", "Edit Parse File", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                    return;

                var s = "file;" + Path.GetFileNameWithoutExtension(parseFileName) + ";unnamed package";
                s += "\r\n\r\n";
                s += "rem;insert your parser fields here";
                try
                {
                    File.WriteAllText(editFile, s);
                }
                catch
                {
                    MessageBox.Show("Failed to create new parser file", "Edit Parse File", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
            if (Properties.Settings.Default.ExternalParseEditor)
            {
                Process.Start(editFile);
            }
            else
            {
                // Open in-app editor
                var editDlg = new ParseEditorForm();
                editDlg.LoadFromFile(editFile);
                editDlg.Show();
                //MessageBox.Show("Internal editor not implemented yet");
            }
        }

        /*
        private int InfoByteByPos(int pos,PacketParser pp)
        {
            string s = InfoGridHeader;
            int addCharCount = 0;
            for (int i = 0; i < pp.PD.RawBytes.Count; i += 0x10)
            {
                s += i.ToString("X").PadLeft(4, ' ') + " | " ;

                for (int i2 = 0; i2 < 0x10; i2++)
                {
                    var thisByteStartPos = s.Length;

                    if ((i + i2) < pp.ParsedBytes.Count)
                    {
                        s += "XX" ;
                        addCharCount++;
                    }
                    else
                    {
                        s += "  " ;
                    }

                    s += " " ;
                    if ((i2 % 0x4) == 0x3)
                        s += " " ;

                    var thisByteEndPos = s.Length;
                    if ((pos >= thisByteStartPos) && (pos <= thisByteEndPos))
                        return (i + i2);
                }

                if (addCharCount > 0)
                {
                    s += "  | " ;
                    for (int c = 0; (c < 0x10) && ((i + c) < pp.ParsedBytes.Count); c++)
                    {
                        s += "Y" ;
                    }
                    addCharCount = 0;
                }
                s += "\n" ;
            }

            return -1 ;
        }
        */

        private void RtInfo_SelectionChanged(object sender, EventArgs e)
        {
            /*
            if ((rtInfo.Enabled == false) || (dGV.Enabled == false))
                return;
            // Doesn't work on original data as we can't predict it's layout
            if (cbOriginalData.Checked == true)
                return;
            if (PP == null)
                return;
            // Get selection
            var firstPos = rtInfo.SelectionStart;
            var lastPos = rtInfo.SelectionStart + rtInfo.SelectionLength;
            if ((firstPos < 0) || (lastPos < firstPos))
                return;

            rtInfo.Enabled = false;
            dGV.Enabled = false;
            try
            {
                var firstSelected = InfoByteByPos(firstPos, PP);
                var lastSelected = InfoByteByPos(lastPos, PP);
                if ((firstSelected >= 0) && (lastSelected >= 0))
                {
                    List<int> selected = new List<int>();
                    selected.Clear();
                    for (int i = 0; i < PP.ParsedBytes.Count; i++)
                    {
                        if ((i >= firstSelected) && (i <= lastSelected))
                            selected.Add(PP.ParsedBytes[i]);
                    }

                    for (int i = 0; i < PP.ParsedView.Count; i++)
                    {
                        var b = (selected.IndexOf(PP.ParsedView[i].FieldIndex) >= 0);
                        dGV.Rows[i].Selected = b;
                    }
                }
            }
            catch (Exception x)
            {
                MessageBox.Show(x.Message);
            }
            finally
            {
                dGV.Enabled = true;
                rtInfo.Enabled = true;
            }
            */
        }

        private void MmVideoOpenLink_Click(object sender, EventArgs e)
        {
            if (VideoLinkForm.GetVLCLibPath() == string.Empty)
            {
                MessageBox.Show("VideoLAN VLC needs to be installed on your PC to use the video linking feature", "libvlc not found", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            Application.UseWaitCursor = true;
            Cursor = Cursors.WaitCursor;
            VideoLinkForm videoLink = null;
            try
            {
                PacketTabPage thisTP = GetCurrentPacketTabPage();
                if ((thisTP != null) && (thisTP.videoLink != null))
                {
                    // MessageBox.Show("You already have a video link open for this packet", "Video Link Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    thisTP.videoLink.BringToFront();
                    Cursor = Cursors.Default;
                    Application.UseWaitCursor = false;
                    return;
                }
                // Create our virtualtime stamps now
                if (thisTP != null)
                    thisTP.PL.BuildVirtualTimeStamps();
                videoLink = new VideoLinkForm();
                videoLink.sourceTP = thisTP;
                videoLink.Show();
                videoLink.BringToFront();
                UpdateStatusBarAndTitle(thisTP);
            }
            catch (Exception x)
            {
                if (videoLink != null)
                    videoLink.Dispose();
                MessageBox.Show("Could not create video link, likely libvlc not correcty installed !\r\n" + x.Message, "Video Link Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Cursor = Cursors.Default;
                Application.UseWaitCursor = false;
                return;
            }
            Cursor = Cursors.Default;
            Application.UseWaitCursor = false;
        }

        private void MmVideoViewProject_Click(object sender, EventArgs e)
        {
        }

        private void TcPackets_ControlRemoved(object sender, ControlEventArgs e)
        {
            if (e.Control is PacketTabPage)
            {
                PacketTabPage tp = (e.Control as PacketTabPage);
                if (tp.PLLoaded.Count() > 0)
                    tp.SaveProjectFile();

                var gsp = tp.GetSelectedPacket();
                if ((CurrentPP != null) && (gsp != null) && (gsp.PP != null) && (gsp.PP == CurrentPP))
                {
                    CurrentPP = null;
                    dGV.Rows.Clear();
                    rtInfo.Clear();
                    lInfo.Text = "";
                    cbShowBlock.Visible = false;
                }
                try
                {
                    if (tp.videoLink != null)
                        tp.videoLink.Close();
                }
                catch { }

                if (tcPackets.TabCount <= 1)
                    Text = defaultTitle;
            }
        }

        private void MainForm_Shown(object sender, EventArgs e)
        {
            // Properly draw everything before we start
            this.Refresh();
            // Handle arguments
            var args = Environment.GetCommandLineArgs().ToList();
            args.RemoveAt(0);
            foreach (string arg in args)
            {
                if (File.Exists(arg))
                {
                    // open log
                    TryOpenFile(arg);
                }
            }
        }

        private void MainForm_DragEnter(object sender, DragEventArgs e)
        {
            // trying to add some file dropping
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Copy;
            else
                e.Effect = DragDropEffects.None;
        }

        private void MainForm_DragDrop(object sender, DragEventArgs e)
        {
            string[] s = (string[])e.Data.GetData(DataFormats.FileDrop, false);
            if (s == null)
                return;
            for (int i = 0; i < s.Length; i++)
            {
                if (File.Exists(s[i]))
                    TryOpenFile(s[i]);
            }
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            // try deleting all created temp-files when closing
            foreach (var fn in AllUsedTempFiles)
                try
                {
                    File.Delete(fn);
                }
                catch { }
        }

        private void MMExtraGameView_Click(object sender, EventArgs e)
        {
            if (GameViewForm.GV == null)
            {
                _ = new GameViewForm();
            }
            GameViewForm.GV.Show();
            GameViewForm.GV.BringToFront();
        }

        private void MmFile_Click(object sender, EventArgs e)
        {

        }

        private void MMFileProjectDetails_Click(object sender, EventArgs e)
        {
            var tp = GetCurrentPacketTabPage();
            if (tp == null)
            {
                MessageBox.Show("You need to open a log file first before you can view it's project settings", "View Project", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            using (var projectDlg = new ProjectInfoForm())
            {
                projectDlg.LoadFromPacketTapPage(tp);
                if (projectDlg.ShowDialog() == DialogResult.OK)
                {
                    projectDlg.ApplyPacketTapPage();
                    if (!tp.SaveProjectFile())
                    {
                        MessageBox.Show("Project file was NOT saved !\r\nEither you don't have write permission,\r\nor are not able to save the file because of restrictions placed by this program", "Project NOT saved", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
            }
        }

        private void MMExtraUpdateParser_Click(object sender, EventArgs e)
        {
            if (Properties.Settings.Default.ParserDataUpdateZipURL == string.Empty)
            {
                MessageBox.Show("No update URL has been set, please go to program settings to set one up", "No update URL", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (MessageBox.Show("Do you want to download packet data ?\r\n" +
                "\r\n" +
                "This will update your lookup and parse data from the PVLV-Data repository on GitHub at \r\n" +
                Properties.Settings.Default.ParserDataUpdateZipURL + "\r\n" +
                "\r\n" +
                "Any changes you have made will be overwritten if you do.\r\n" +
                "This does NOT check for version updates of the program itself !\r\n" +
                "Also note that it is possible that this data is OLDER than your current one.",
                "Update data ?",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning) != DialogResult.Yes)
                return;

            using (var loadform = new LoadingForm(this))
            {
                try
                {
                    if ((CompressForm.SevenZipDLLPath == null) || (CompressForm.SevenZipDLLPath == string.Empty))
                        CompressForm.SevenZipDLLPath = CompressForm.TryGet7ZipLibrary();

                    loadform.Text = "Updating data ...";
                    loadform.pb.Hide();
                    loadform.lTextInfo.Text = "Downloading ...";
                    loadform.lTextInfo.Show();
                    loadform.Show();
                    loadform.lTextInfo.Refresh();

                    System.Threading.Thread.Sleep(250);
                    // Delete the old download data if there
                    var localDataDir = Path.Combine(Application.StartupPath, "data");
                    var tempFile = Path.GetTempFileName();

                    PVLVHelper.FileDownloader.DownloadFileFromURLToPath(Properties.Settings.Default.ParserDataUpdateZipURL, tempFile);

                    loadform.lTextInfo.Text = "Unpacking ...";
                    loadform.lTextInfo.Refresh();
                    System.Threading.Thread.Sleep(500);

                    var unzipper = new SevenZip.SevenZipExtractor(tempFile, SevenZip.InArchiveFormat.SevenZip);
                    var filelist = unzipper.ArchiveFileData;

                    loadform.pb.Minimum = 0;
                    loadform.pb.Maximum = filelist.Count;
                    loadform.pb.Step = 1;
                    loadform.pb.Show();

                    foreach (var fd in filelist)
                    {
                        // Skip directories
                        if ((fd.Attributes & 0x10) != 0)
                            continue;

                        try
                        {
                            var zippedName = fd.FileName;
                            var targetName = Path.Combine(localDataDir, zippedName);
                            var targetFileDir = Path.GetDirectoryName(targetName);
                            if (!Directory.Exists(targetFileDir))
                                Directory.CreateDirectory(targetFileDir);
                            var fs = File.Create(targetName);
                            unzipper.ExtractFile(fd.FileName, fs);
                            fs.Close();
                            loadform.pb.PerformStep();
                            System.Threading.Thread.Sleep(25);
                        }
                        catch (Exception x)
                        {
                            if (MessageBox.Show("Exception extracting file:\r\n" + x + "\r\n" + fd + "\r\nDo you want to continue ?", "Exception", MessageBoxButtons.YesNo, MessageBoxIcon.Error) == DialogResult.No)
                            {
                                break;
                            }
                        }
                    }
                    unzipper.Dispose();
                    loadform.pb.Hide();

                    loadform.lTextInfo.Text = "Done ...";
                    loadform.lTextInfo.Refresh();
                    System.Threading.Thread.Sleep(1000);
                    File.Delete(tempFile);

                    MessageBox.Show("Done downloading and unpacking data from \r\n" +
                        Properties.Settings.Default.ParserDataUpdateZipURL + "\r\n\r\n" +
                        "Some changes will only be visible after you restart the program.", "Update data", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                }
                catch (Exception x)
                {
                    MessageBox.Show("Exception updating:\r\n" + x.Message, "Update Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void MMAbout7ZipMain_Click(object sender, EventArgs e)
        {
            Process.Start(url7Zip);
        }

        private void MMAbout7ZipDownload_Click(object sender, EventArgs e)
        {
            Process.Start(url7ZipRequiredVer);
        }

        private void MMExtraImportFromGame_Click(object sender, EventArgs e)
        {
            /*
            if ((Properties.Settings.Default.POLUtilsDataFolder == string.Empty) || (!Directory.Exists(Properties.Settings.Default.POLUtilsDataFolder)))
            {
                MessageBox.Show("No POLUtils update folder has been set, or it doesn't exist, please go to program settings to set one up", "No update folder", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            */
            if (!Directory.Exists(FFXIHelper.FFXI_InstallationPath))
            {
                MessageBox.Show("No FFXI Installation found to extract data from !", "No game client", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (MessageBox.Show("You are about to import data from \r\n" +
                FFXIHelper.FFXI_InstallationPath + "\r\n\r\n" +
                "The following lookups will be overwritten:\r\n" +
                "- items.txt",
                "Import game data", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return;

            using (var loadform = new LoadingForm(this))
            {
                loadform.Text = "Importing ...";
                loadform.pb.Hide();
                loadform.lTextInfo.Show();
                loadform.Show();

                FFXIHelper.FFXI_LoadItemsFromDats(ref DataLookups.ItemsList.items);
                DataLookups.ItemsList.UpdateData();
                /*
                var itemFiles = Directory.GetFiles(Properties.Settings.Default.POLUtilsDataFolder, "items-*.xml");
                for(var c = 0; c < itemFiles.Length; c++)
                {
                    loadform.lTextInfo.Text = "Items " + (c + 1).ToString() + "/" + itemFiles.Length.ToString();
                    loadform.Refresh();
                    items.AddRange(SEHelper.ReadItemListFromXML(itemFiles[c]));
                    System.Threading.Thread.Sleep(250);
                }
                */
                loadform.lTextInfo.Text = "Saving "+ DataLookups.ItemsList.items.Count.ToString() +" items ...";
                loadform.Refresh();
                // var sorteditems = DataLookups.ItemsList.items.OrderBy(d => d.Value);
                var itemsString = new List<string>();
                itemsString.Add("id;name");
                foreach(var item in DataLookups.ItemsList.items)
                {
                    if ((item.Value.Id > 0) && (item.Value.Name != string.Empty) && (item.Value.Name != "."))
                    {
                        itemsString.Add(item.Value.Id.ToString() + ";" + item.Value.Name);
                    }
                }
                File.WriteAllLines(Path.Combine(DataLookups.DefaultLookupPath(),"items.txt"), itemsString);
                System.Threading.Thread.Sleep(500);

                loadform.lTextInfo.Text = "Reloading lookups ...";
                loadform.Refresh();
                DataLookups.LoadLookups(false);
            }

        }

        private void mmExtraExportPacketsAsCSV_Click(object sender, EventArgs e)
        {
            PacketTabPage thisTP = GetCurrentPacketTabPage();
            if (thisTP == null)
                return;

            if (!thisTP.PL.IsPreParsed)
            {
                MessageBox.Show("This function requires the pre-parse setting to be enabled","Export to CSV",MessageBoxButtons.OK,MessageBoxIcon.Warning);
                return ;
            }

            saveCSVFileDialog.FileName = Path.GetFileNameWithoutExtension(thisTP.ProjectFile) + ".csv";

            if (saveCSVFileDialog.ShowDialog() == DialogResult.OK)
            {
                if (ExportCSVHelper.ExportPacketToCSV(thisTP.PL, saveCSVFileDialog.FileName))
                    MessageBox.Show("Exported as:\r\n" + saveCSVFileDialog.FileName, "Export CSV",MessageBoxButtons.OK,MessageBoxIcon.Information);
                else
                    MessageBox.Show("Export failed !", "Export CSV", MessageBoxButtons.OK,MessageBoxIcon.Error);
            }
        }
    }
}
