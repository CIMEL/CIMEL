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
using Aeronet.Chart.Properties;

namespace Aeronet.Chart.AeronetData
{
    public partial class fmDataProcessDialog : Form
    {
        // the instance of DataWork
        // we will manages it including start job and stop job
        private DataWorker _dataWork;
        private bool _isWorking;
        private TaskScheduler _fmTaskScheduler;
        private string LABEL_GOOD = @"正常";
        private string LABEL_EMPTY = @"缺少配置";
        private const int IMG_GOOD = 1;
        private const int IMG_ERROR = 2;
        private string LOG_H_ERROR = @"[ERROR]";
        private string LOG_H_INFO = @"[INFO]";
        private string LOG_EXT = @"[EXT]";
        private string LOG_INT = @"[INT]";
        private string LOG_H_SUCCESS = @"[COMPLETED]";
        private string LOG_H_ABORTED = @"[ABORTED]";
        private static object _workLocker=new object();

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
            lock (_workLocker)
            {
                if (this._isWorking)
                {
                    e.Cancel = true;
                    MessageBox.Show(@"数据处理中请不要关闭窗口", @"数据处理");
                }
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
            this.cmbRegions.Items.Insert(0, ComboBoxItem.EmptyItem.ToItem());
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
                error = string.Format("- 初始化站点失败 <- {0}", ex.Message);
                sb.AppendLine(error);
                this.SetToError(lblVal_STNS_FN, error);
            }
            this.cmbRegions.SelectedIndex = 0;

//          initial and validate the data file name
            string[] dats =
                Directory.EnumerateFiles(ConfigOptions.Singleton.DATA_Dir, "*.*", SearchOption.TopDirectoryOnly)
                    .Where(s => s.EndsWith(".ALL") || s.EndsWith(".ALR")).ToArray();
            if (dats.Length == 0)
            {
                error = "- 没有找到主数据文件 (*.ALL, *.ALR)";
                sb.AppendLine(error);
                SetToError(this.lblVal_FDATA,error);
            }
            else
            {
                string dataFileName = Path.GetFileNameWithoutExtension(dats[0]);
                if (string.IsNullOrEmpty(dataFileName))
                {
                    error = string.Format("- 文件名命名错误 <-{0}", dats[0]);
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
                        error = string.Format("- 不能从文件名识别出站点名称 <- {0}",
                            dataFileName);
                        sb.AppendLine(error);
                        SetToError(this.lblVal_STNS_FN,error);
                    }
                    else
                    {
                        var region = RegionStore.Singleton.FindRegion(m.Value);
                        if (region == null)
                        {
                            error = string.Format("- 站点不存在 <-{0}", m.Value);
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
                            string.Format("- 不能从文件名识别出站点仪器编号 <- {0}",
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
                this.lblMETADATA.Text = ConfigOptions.Singleton.METADATA_Dir;
                this.SetToGood(this.lblVal_METADATA);
            }
            else
                this.SetToError(this.lblVal_METADATA, LABEL_EMPTY);
            // validate the chart set dir
            if (!string.IsNullOrEmpty(ConfigOptions.Singleton.CHARTSET_Dir))
            {
                this.lblCHARTSET.Text = ConfigOptions.Singleton.CHARTSET_Dir;
                this.SetToGood(this.lblVal_CHARTSET);
            }
            else
                this.SetToError(this.lblVal_CHARTSET, LABEL_EMPTY);

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
                MessageBox.Show(sb.ToString(), @"参数校验", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
                lock (_workLocker)
                {
                    this._isWorking = false;
                }
                this.btnAction.Text = Settings.Default.BTN_START_TEXT;
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
                lock (_workLocker)
                {
                    this._isWorking = true;
                }
                this.btnClose.Enabled = false;

                this.btnAction.Text = Settings.Default.BTN_STOP_TEXT;
                this.btnAction.Enabled = true;
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
            var worker = this._dataWork;

            lock (_workLocker)
            {
                if (_isWorking)
                {
                    // stop the worker
                    worker.Stop();
                    this.btnAction.Enabled = true;
                }
                else
                {
                    // check validation state
                    if (AreAllGood())
                    {
                        //double confirms
                        string question =
                            "准备好了吗?\r\n[Yes]: 马上开始\r\n[No]: 检查参数";
                        if (DialogResult.No == MessageBox.Show(question,
                            @"参数校验", MessageBoxButtons.YesNo, MessageBoxIcon.Warning,
                            MessageBoxDefaultButton.Button1))
                        {
                            this.btnAction.Enabled = true;
                            return;
                        }
                    }
                    else
                    {
                        MessageBox.Show(@"抱歉，参数配置仍有错误",
                            @"参数校验", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        {
                            this.btnAction.Enabled = true;
                            return;
                        }
                    }

                    // initial worker
                    string regionName = (string)((dynamic)(this.cmbRegions.SelectedItem)).Value;
                    string dataFileName = this.lblFDATA.Text;
                    string instrumentId = this.txtSTNS_ID.Text;
                    // start the worker
                    worker.Start(regionName, instrumentId, dataFileName);
                }
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void SetToGood(Label control)
        {
            control.ImageIndex = IMG_GOOD;// 1: good
            this.toolTip1.SetToolTip(control,string.Empty);
        }

        private void SetToError(Label control, string error)
        {
            control.ImageIndex = IMG_ERROR;// 2: error
            this.toolTip1.SetToolTip(control, error);
        }

        private bool AreAllGood()
        {
            return lblVal_FDATA.ImageIndex == IMG_GOOD
                   && lblVal_STNS_FN.ImageIndex == IMG_GOOD
                   && lblVal_STNS_ID.ImageIndex == IMG_GOOD
                   && lblVal_FIPT.ImageIndex == IMG_GOOD
                   && lblVal_FBRDF.ImageIndex == IMG_GOOD
                   && lblVal_CHARTSET.ImageIndex == IMG_GOOD
                   && lblVal_FOUT.ImageIndex == IMG_GOOD
                   && lblVal_FDAT.ImageIndex == IMG_GOOD;
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
            if (string.IsNullOrEmpty(txtSTNS_ID.Text)|| txtSTNS_ID.Text == ComboBoxItem.EmptyItem.Text)
                this.SetToError(this.lblVal_STNS_ID, LABEL_EMPTY);
            else
                this.SetToGood(this.lblVal_STNS_ID);
        }
    }
}