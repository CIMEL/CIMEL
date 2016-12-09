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
        private const int IMG_QUESTION = 0;
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

            // initial regions combo box
            StringBuilder sb = new StringBuilder();
            string error;

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
        }

        private void Init(string region)
        {
            // validate and initial data and instrument Id
            this.InitFDATA(region);

            // validate the parameters files
            this.ValidateOption(region, ConfigOptions.Singleton.INS_PARA_Dir, lblFIPT, lblVal_FIPT);
            // validate the medata files
            this.ValidateOption(region, ConfigOptions.Singleton.METADATA_Dir, lblMETADATA, lblVal_METADATA);
            // validate the chart set dir
            this.ValidateOption(region, ConfigOptions.Singleton.CHARTSET_Dir, lblCHARTSET, lblVal_CHARTSET);
            // validate the output folder
            this.ValidateOption(region, ConfigOptions.Singleton.OUTPUT_Dir, lblFOUT, lblVal_FOUT);
            // validate modifies files
            this.ValidateOption(region, ConfigOptions.Singleton.MODIS_BRDF_Dir, lblFBRDF, lblVal_FBRDF);
            // validate data files
            this.ValidateOption(region, ConfigOptions.Singleton.DATA_Dir, lblDAT, lblVal_FDAT);

            // Show all errors
            if (sb.Length > 0)
            {
                MessageBox.Show(sb.ToString(), @"参数校验", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
                this.btnAction.Enabled = true;
        }

        private void InitFDATA(string region)
        {
            // initial and validate the data file name
            string[] dats =
                Directory.EnumerateFiles(ConfigOptions.Singleton.DATA_Dir, "*.*", SearchOption.TopDirectoryOnly)
                    .Where(s => s.EndsWith(".ALL") || s.EndsWith(".ALR")).ToArray();
            if (dats.Length == 0)
            {
                error = "- 没有找到主数据文件 (*.ALL, *.ALR)";
                sb.AppendLine(error);
                SetToError(this.lblVal_FDATA, error);
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
                        SetToError(this.lblVal_STNS_FN, error);
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
        }

        private void ValidateOption(string region, string optionDir, Label optionLabel, Label valLabel)
        {
            string path = optionDir;

            if (string.IsNullOrEmpty(path))
            {
                optionLabel.Text = Properties.Settings.Default.LBL_INITIAL;
                this.SetToError(valLabel, LABEL_EMPTY);
                return;
            }

            try
            {
                path = Path.Combine(optionDir, region);
            }
            catch (Exception)
            {
                if (!path.EndsWith(Path.DirectorySeparatorChar.ToString()))
                    path += Path.DirectorySeparatorChar;
                path += region;
                this.SetToError(valLabel, "目录无效");
                return;
            }
            finally
            {
                optionLabel.Text = path;
            }

            try
            {
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
            }
            catch (Exception)
            {
                this.SetToError(valLabel, "目录无法创建");
                return;
            }

            this.SetToGood(valLabel);
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

        private void SetToQuestion(Label control)
        {
            control.ImageIndex = IMG_QUESTION; // 0: question
            this.toolTip1.SetToolTip(control,string.Empty);
        }

        private void SetToDefault()
        {
            //set the label of dirs to initializing
            //set the marks to question
            //this.SetToQuestion(this.);
            // this.SetToError(this.lblVal_STNS_FN, LABEL_EMPTY);
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
            {
                // disable action
                this.btnAction.Enabled = false;
                this.SetToDefault();
            }
            else
            {
                this.SetToGood(this.lblVal_STNS_FN);
                this.Init(cmbRegions.Text);
            }
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