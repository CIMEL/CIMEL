using Peach.Log;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Aeronet.Chart.Options;

namespace Aeronet.Chart.AeronetData
{
    public partial class fmDataProcessDialog : Form
    {
        // the instance of DataWork
        // we will manages it including start job and stop job
        private DataWorker _dataWork;
        private bool _isWorking;
        private TaskScheduler _fmTaskScheduler;
        private string LABEL_GOOD = @"GOOD";
        private string LABEL_EMPTY = @"NOT CONFIG";
        private string LOG_H_ERROR = @"[ERROR]";
        private string LOG_H_INFO = @"[INFO]";
        private string LOG_EXT = @"[EXT]";
        private string LOG_INT = @"[INT]";
        private string LOG_H_SUCCESS = @"[COMPLETED]";
        private string LOG_H_ABORTED = @"[ABORTED]";

        public fmDataProcessDialog()
        {
            InitializeComponent();
            // initial the instance of DataWork
            this._dataWork = new DataWorker();
            // register the events, info and Error
            this._dataWork.Informed += this.LogInfo;
            this._dataWork.Failed += this.LogError;
            this._dataWork.Started += this.WorkerStarted;
            this._dataWork.Completed += this.WorkerCompleted;
            // initial form event
            this.Load += fmDataProcessDialog_Load;
            this.Closing+=fmDataProcessDialog_Closing;
        }

        private void fmDataProcessDialog_Closing(object sender, CancelEventArgs e)
        {
            if (this._isWorking)
            {
                e.Cancel = true;
                MessageBox.Show(@"Cannot be closed, the data process is still in progress", @"Data Process Warning");
            }
        }

        private void fmDataProcessDialog_Load(object sender, EventArgs e)
        {
            // initial current task scheduler
            this._fmTaskScheduler = TaskScheduler.FromCurrentSynchronizationContext();
            // disable action
            this.btnAction.Enabled = false;

            // initial regions combo box
            StringBuilder sb = new StringBuilder();
            string error = string.Empty;

            this.cmbRegions.Items.Clear();
            this.cmbRegions.DisplayMember = ComboBoxItem.DisplayName;
            this.cmbRegions.ValueMember = ComboBoxItem.ValueName;
            try
            {
                var regions = RegionStore.Singleton.GetRegions();
                foreach (var region in regions)
                {
                    this.cmbRegions.Items.Add(
                        new
                        {
                            Text = string.Format("{0} ({1} , {2})", region.Name, region.Lat, region.Lon),
                            Value = region.Name
                        });
                }
            }
            catch (Exception ex)
            {
                error = string.Format("- Failed to initial regions <- {0}", ex.Message);
                sb.AppendLine(error);
                this.SetToError(lblVal_STNS_FN, error);
            }
            this.cmbRegions.Items.Insert(0, ComboBoxItem.EmptyItem.ToItem());
            this.cmbRegions.SelectedIndex = 0;

//          initial and validate the data file name
            string[] dats =
                Directory.EnumerateFiles(ConfigOptions.Singleton.DATA_Dir, "*.*", SearchOption.TopDirectoryOnly)
                    .Where(s => s.EndsWith(".ALL") || s.EndsWith(".ALR")).ToArray();
            if (dats.Length == 0)
            {
                error = "- Not found data files (*.ALL, *.ALR)";
                sb.AppendLine(error);
                SetToError(this.lblVal_FDATA,error);
            }
            else
            {
                string dataFileName = Path.GetFileNameWithoutExtension(dats[0]);
                if (string.IsNullOrEmpty(dataFileName))
                {
                    error = string.Format("- Invalid file name <-{0}", dats[0]);
                    sb.AppendLine(error);
                    SetToError(this.lblVal_FDATA, error);
                }
                else
                {
                    this.lblFDATA.Text = dataFileName;
                    SetToGood(this.lblVal_FDATA);
//                  initial and validate the region name
                    Match m = Regex.Match(dataFileName, "\\w+");
                    if (!m.Success)
                    {
                        error = string.Format("- Cannot recognize the region name from the data file name <- {0}",
                            dataFileName);
                        sb.AppendLine(error);
                        SetToError(this.lblVal_STNS_FN,error);
                    }
                    else
                    {
                        var region = RegionStore.Singleton.FindRegion(m.Value);
                        if (region == null)
                        {
                            error = string.Format("- Invalid region name <-{0}", m.Value);
                            sb.AppendLine(error);
                            SetToError(this.lblVal_STNS_FN, error);
                        }
                        else
                        {
                            this.cmbRegions.Text = string.Format("{0} ({1} , {2})", region.Name, region.Lat, region.Lon);
                            this.SetToGood(this.lblVal_STNS_FN);
                        }
                    }
//                  initial and validate the instrument id
                    m = Regex.Match(dataFileName, "\\d+");
                    if (!m.Success)
                    {
                        error =
                            string.Format("- Cannot recognize the id of the instrument from the data file name <- {0}",
                                dataFileName);
                        sb.AppendLine(error);
                        this.SetToError(this.lblVal_STNS_ID, error);

                    }
                    else
                    {
                        this.txtSTNS_ID.Text = m.Value;
                        this.SetToGood(this.lblVal_STNS_ID);
                    }
                }
            }
//          validate the parameters files            
            if (!string.IsNullOrEmpty(ConfigOptions.Singleton.INS_PARA_Dir))
            {
                this.lblFIPT.Text = ConfigOptions.Singleton.INS_PARA_Dir;
                this.SetToGood(this.lblVal_FIPT);
            }
            else
                this.SetToError(this.lblVal_FIPT, LABEL_EMPTY);
//          validate the medata files
            if (!string.IsNullOrEmpty(ConfigOptions.Singleton.METADATA_Dir))
            {
                string metaData = ConfigOptions.Singleton.METADATA_Dir;
                this.lblTEMP.Text = Path.Combine(metaData, "input");
                this.SetToGood(this.lblVal_TEMP);
                // checks if the directory is available
                try
                {
                    if (!Directory.Exists(this.lblTEMP.Text))
                        Directory.CreateDirectory(this.lblTEMP.Text);
                }
                catch (Exception)
                {
                    error = string.Format("- Cannot create the path <- {0}", this.lblTEMP.Text);
                    sb.AppendLine(error);
                    this.SetToError(this.lblVal_TEMP, error);
                }
            }
            else
                this.SetToError(this.lblVal_TEMP, LABEL_EMPTY);
//          validate the output folder
            if (!string.IsNullOrEmpty(ConfigOptions.Singleton.OUTPUT_Dir))
            {
                this.lblFOUT.Text = ConfigOptions.Singleton.OUTPUT_Dir;
                this.SetToGood(this.lblVal_FOUT);
            }
            else
                this.SetToError(this.lblVal_FOUT, LABEL_EMPTY);
//          validate modifies files
            if (!string.IsNullOrEmpty(ConfigOptions.Singleton.MODIS_BRDF_Dir))
            {
                this.lblFBRDF.Text = ConfigOptions.Singleton.MODIS_BRDF_Dir;
                this.SetToGood(this.lblVal_FBRDF);
            }
            else
                this.SetToError(this.lblVal_FBRDF, LABEL_EMPTY);
//          validate data files
            if (!string.IsNullOrEmpty(ConfigOptions.Singleton.DATA_Dir))
            {
                this.lblDAT.Text = ConfigOptions.Singleton.DATA_Dir;
                this.SetToGood(this.lblVal_FDAT);
            }
            else
                this.SetToError(this.lblVal_FDAT, LABEL_EMPTY);

//          Show all errors
            if (sb.Length > 0)
            {
                MessageBox.Show(sb.ToString(), @"Parameters Validation", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
                this.btnAction.Enabled = true;
        }

        /// <summary>
        /// Adds the Info to the list box of logs
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="message"></param>
        private void LogInfo(object sender, EventMessage message)
        {
            Task.Factory.StartNew(() =>
            {
                string msg = string.Format("{2}{0} -> {1}", (message.IsExternal ? LOG_EXT : LOG_INT), message.Message, LOG_H_INFO);
                this.LogMessage(msg);
                Logger.Default.Info(msg);
            }, CancellationToken.None, TaskCreationOptions.None, this._fmTaskScheduler);

        }

        /// <summary>
        /// Adds the error to the list box of logs
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="message"></param>
        private void LogError(object sender, EventMessage message)
        {
            Task.Factory.StartNew(() =>
            {
                string msg = string.Format("{2}{0} -> {1}", (message.IsExternal ? LOG_EXT : LOG_INT),  message.Message, LOG_H_ERROR);
                this.LogMessage(msg);
                Logger.Default.Error(msg);
            }, CancellationToken.None, TaskCreationOptions.None, this._fmTaskScheduler);

        }

        /// <summary>
        /// Apply complete state to the UI controls when the worker completes the job
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="message"></param>
        private void WorkerCompleted(object sender, EventMessage message)
        {
            Task.Factory.StartNew(() =>
            {
                this.btnClose.Enabled = true;
                this._isWorking = false;
                string msg = string.Format("{0} -> {1}", (message.IsExternal ? LOG_EXT : LOG_INT), message.Message);
                this.LogMessage(msg);
                Logger.Default.Info(msg);
            }, CancellationToken.None, TaskCreationOptions.None, this._fmTaskScheduler);
        }

        /// <summary>
        /// Apply start state to the UI controls when the worker starts the job
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="message"></param>
        private void WorkerStarted(object sender, EventMessage message)
        {
            Task.Factory.StartNew(() =>
            {
                this.btnClose.Enabled = false;
                this.btnAction.Enabled = true;
                this._isWorking = true;
                string msg = string.Format("{0} -> {1}", (message.IsExternal ? LOG_EXT : LOG_INT), message.Message);
                this.LogMessage(msg);
                Logger.Default.Info(msg);
            }, CancellationToken.None, TaskCreationOptions.None, this._fmTaskScheduler);
        }

        // see IPS Manufacturing\Project Files\Pull-O-Tron\Recyclometer\StatusForm.cs
        private void lstLogs_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index == -1)
            {
                return;
            }

            string line = this.lstLogs.Items[e.Index].ToString();
            Brush brush = Brushes.Black;
            if (line.Contains(LOG_EXT))
            {
                line = line.Replace(LOG_EXT, string.Empty);
                if (line.Contains(LOG_H_INFO))
                {
                    line = line.Replace(LOG_H_INFO, string.Empty);
                    brush = Brushes.Black;
                }
                else if (line.Contains(LOG_H_ERROR))
                {
                    line = line.Replace(LOG_H_ERROR, string.Empty);
                    brush = Brushes.Red;
                }
                else if (line.Contains(LOG_H_SUCCESS))
                {
                    brush = Brushes.Green;
                }
                else if (line.Contains(LOG_H_ABORTED))
                {
                    brush = Brushes.Red;
                }
            }
            else if (line.Contains(LOG_INT))
            {
                line = line.Replace(LOG_INT, string.Empty);
                if (line.Contains(LOG_H_INFO))
                {
                    line = line.Replace(LOG_H_INFO, string.Empty);
                    brush = Brushes.Gray;
                }
                else if (line.Contains(LOG_H_ERROR))
                {
                    line = line.Replace(LOG_H_ERROR, string.Empty);
                    brush = Brushes.PaleVioletRed;
                }
                else if (line.Contains(LOG_H_SUCCESS))
                {
                    brush = Brushes.LightGreen;
                }
                else if (line.Contains(LOG_H_ABORTED))
                {
                    brush = Brushes.PaleVioletRed;
                }
            }

            e.DrawBackground();
            if ((e.State & DrawItemState.Selected) == DrawItemState.Selected)
            {
                brush = new SolidBrush(SystemColors.HighlightText);
            }

            e.Graphics.DrawString(line, e.Font, brush, e.Bounds, StringFormat.GenericDefault);
            e.DrawFocusRectangle();
        }

        private void LogMessage(string message)
        {
            string[] strings = message.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string str in strings)
            {
                this.lstLogs.Items.Add(str);
            }

            int visibleLines = this.lstLogs.Height / this.lstLogs.ItemHeight;
            if (this.lstLogs.Items.Count > visibleLines)
            {
                this.lstLogs.TopIndex = this.lstLogs.Items.Count - visibleLines;
            }

            if (this.lstLogs.Items.Count > 2000)
            {
                for (int i = 0; i < 500; i++)
                {
                    this.lstLogs.Items.RemoveAt(0);
                }
            }
        }

        private void btnAction_Click(object sender, EventArgs e)
        {
            // prevent action from duplicated click
            this.btnAction.Enabled = false;
            // check action state
            string action = btnAction.Text;
            var worker = this._dataWork;
            switch (action)
            {
                case "Start":
                    // check validation state
                    if (AreAllGood())
                    {
                        //double confirms
                        string question =
                            "Are you sure to start to process data?\r\n[Yes]: Start immediately\r\n[No]: Check again";
                        if (DialogResult.No == MessageBox.Show(question,
                            @"Data Process Validation", MessageBoxButtons.YesNo, MessageBoxIcon.Warning,
                            MessageBoxDefaultButton.Button1))
                            return;
                    }
                    else
                    {
                        MessageBox.Show(@"Cannot process data until all errors above are clean.",
                            @"Data Process Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    // initial worker
                    string regionName = (string)((dynamic)(this.cmbRegions.SelectedItem)).Value;
                    string dataFileName = this.lblFDATA.Text;
                    string instrumentId = this.txtSTNS_ID.Text;
                    // start the worker
                    worker.Start(regionName,instrumentId,dataFileName);
                    break;
                case "Stop":
                    // stop the worker
                    worker.Stop();
                    break;
                default:
                    break;
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void SetToGood(Label control)
        {
            control.Text = LABEL_GOOD;
            control.ForeColor=Color.Green;
        }

        private void SetToError(Label control, string error)
        {
            control.Text = error;
            control.ForeColor = Color.Red;
        }

        private bool AreAllGood()
        {
            return lblVal_FDATA.Text == LABEL_GOOD
                   && lblVal_STNS_FN.Text == LABEL_GOOD
                   && lblVal_STNS_ID.Text == LABEL_GOOD
                   && lblVal_FIPT.Text == LABEL_GOOD
                   && lblVal_FBRDF.Text == LABEL_GOOD
                   && lblVal_TEMP.Text == LABEL_GOOD
                   && lblVal_FOUT.Text == LABEL_GOOD
                   && lblVal_FDAT.Text == LABEL_GOOD;
        }

        private void cmbRegions_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbRegions.Text == ComboBoxItem.EmptyItem.Text)
                this.SetToError(this.lblVal_STNS_FN, LABEL_EMPTY);
            else
                this.SetToGood(this.lblVal_STNS_FN);
        }

        private void txtSTNS_ID_TextChanged(object sender, EventArgs e)
        {
            if (txtSTNS_ID.Text == ComboBoxItem.EmptyItem.Text)
                this.SetToError(this.lblVal_STNS_ID, LABEL_EMPTY);
            else
                this.SetToGood(this.lblVal_STNS_ID);
        }
    }
}