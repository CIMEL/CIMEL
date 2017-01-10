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
using CIMEL.Chart.Options;
using CIMEL.Chart.Properties;
using CIMEL.Dog;
using CIMEL.Chart.Properties;
using CIMEL.Core;
using CIMEL.Dog;
using Newtonsoft.Json.Linq;

namespace CIMEL.Chart.CIMELData
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
        private string LOG_H_DOG = @"[DOG]";
        private static object _workLocker = new object();

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
                    // checks if the super dog is still working
                    var isActived = CIMELDog.Default.IsAlive();
                    if(!isActived)
                    {
                        Utility.ShowDogAlert(this,isActived.Message);
                        this._dataWork.Stop();
                        Utility.ExitApp();
                    }

                    e.Cancel = true;
                    this.ShowAlert(@"数据处理中请不要关闭窗口", @"数据处理");
                }
                else
                {
                    // checks if the super dog is still working
                    CIMELDog.Default.IsAlive(true);
                }
            }
        }

        private void fmDataProcessDialog_Load(object sender, EventArgs e)
        {
            // checks if the super dog is still working
            if (!CIMELDog.Default.IsAlive(true)) return;

            // initial current task scheduler
            this._fmTaskScheduler = TaskScheduler.FromCurrentSynchronizationContext();

            // initial regions combo box
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
                var error = string.Format("- 初始化站点失败 <- {0}", ex.Message);
                this.SetToError(lblVal_STNS_FN, error);
            }
            this.cmbRegions.SelectedIndex = 0;
        }

        private void Init(string region)
        {
            // validate data files
            string dataDir= this.ValidateOption(region, ConfigOptions.Singleton.DATA_Dir, lblFDAT, lblVal_FDAT);

            // validate and initial data and instrument Id
            this.InitFDATA(dataDir);

            // validate the parameters files
            this.ValidateOption(string.Empty, ConfigOptions.Singleton.INS_PARA_Dir, lblFIPT, lblVal_FIPT);
            // validate the medata files
            this.ValidateOption(String.Empty, ConfigOptions.Singleton.METADATA_Dir, lblMETADATA, lblVal_METADATA);
            // validate the chart set dir
            this.ValidateOption(region, ConfigOptions.Singleton.CHARTSET_Dir, lblCHARTSET, lblVal_CHARTSET);
            // validate the output folder
            this.ValidateOption(region, ConfigOptions.Singleton.OUTPUT_Dir, lblFOUT, lblVal_FOUT);
            // validate modifies files
            this.ValidateOption(string.Empty, ConfigOptions.Singleton.MODIS_BRDF_Dir, lblFBRDF, lblVal_FBRDF);
        }

        private void InitFDATA(string dataDir)
        {
            // defaults
            this.cmbFdatas.Text = Settings.Default.LBL_INITIAL;
            this.SetToQuestion(lblVal_FDATA);

            // initial and validate the data file name
            if (string.IsNullOrEmpty(dataDir))
                return;

            this.cmbFdatas.Items.Clear();
            this.cmbFdatas.DisplayMember = ComboBoxItem.DisplayName;
            this.cmbFdatas.ValueMember = ComboBoxItem.ValueName;
            this.cmbFdatas.Items.Add(ComboBoxItem.EmptyItem.ToItem());
            string[] dats;
            try
            {
                dats =
                    Directory.EnumerateFiles(dataDir, "*.*", SearchOption.TopDirectoryOnly)
                        .Where(s => s.EndsWith(".ALL")).ToArray();
                if (dats.Length == 0)
                {
                    SetToError(this.lblVal_FDATA, "没有找到主输入数据文件 (*.ALL, *.ALR)");
                    return;
                }
            }
            catch (Exception)
            {
                SetToError(this.lblVal_FDATA, "没有找到主输入数据文件 (*.ALL, *.ALR)");
                return;
            }

            //string dataFileName = Path.GetFileNameWithoutExtension(dats[0]);
            //if (string.IsNullOrEmpty(dataFileName))
            //{
            //    SetToError(this.lblVal_FDATA, string.Format("- 文件名命名错误 <-{0}", dats[0]));
            //    return;
            //}

            this.cmbFdatas.Items.AddRange(dats.Select(d =>
            {
                string fileName = Path.GetFileNameWithoutExtension(d);
                return new {Text = fileName, Value = fileName};
            }).Cast<object>().ToArray());

            this.cmbFdatas.SelectedIndex = dats.Length > 0 ? 1 : 0;

            //this.lblFDATA.Text = dataFileName;
            //SetToGood(this.lblVal_FDATA);
        }

        private void InitSTNS_ID(string dataFileName)
        {
            if (string.IsNullOrEmpty(dataFileName))
            {
                this.txtSTNS_ID.Text = Properties.Settings.Default.LBL_INITIAL;
                this.SetToQuestion(lblVal_STNS_ID);
                return;
            }

            // initial and validate the instrument id
            Match m = Regex.Match(dataFileName, "\\d+");
            if (!m.Success)
            {
               string error =
                    string.Format("- 不能从文件名识别出站点仪器编号 <- {0}",
                        dataFileName);
                this.SetToError(this.lblVal_STNS_ID, error);
            }
            else
            {
                this.txtSTNS_ID.Text = m.Value;
                this.SetToGood(this.lblVal_STNS_ID);
            }
        }

        private string ValidateOption(string region, string optionDir, Label optionLabel, Label valLabel)
        {
            string path = optionDir;

            if (string.IsNullOrEmpty(path))
            {
                optionLabel.Text = Properties.Settings.Default.LBL_INITIAL;
                this.SetToError(valLabel, LABEL_EMPTY);
                return string.Empty;
            }

            if (!Directory.Exists(optionDir))
            {
                this.SetToError(valLabel, "对不起，目录不存在，请查看参数配置");
                return string.Empty;
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
                return string.Empty;
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
                return string.Empty;
            }

            this.SetToGood(valLabel);
            return path;
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
                string msg = string.Format("{2}{0} -> {1}", (message.IsExternal ? LOG_EXT : LOG_INT), message.Message, LOG_H_ERROR);
                Logger.Default.Error(msg);

                if (!message.ShowDlg)
                    // display alert to the log list box
                    this.LogMessage(msg);
                else
                    // display alert within dialog
                    this.ShowAlert(message.Message, Settings.Default.FM_CIMEL_DATA_TEXT);

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
                // checks if it's Dog exception
                string msg = message.Message;
                bool isDogException = msg.StartsWith(LOG_H_DOG);
                // if true, remove the [DOG] mark
                if (isDogException)
                    msg = msg.Replace(LOG_H_DOG, string.Empty);

                msg = string.Format("{0} -> {1}", (message.IsExternal ? LOG_EXT : LOG_INT), msg);
                this.LogMessage(msg);
                Logger.Default.Info(msg);

                if (isDogException)
                {
                    // remove the [ABORTED] mark
                    msg = msg.Replace(LOG_H_ABORTED, string.Empty);
                    Utility.ShowDogAlert(this, msg);
                    Utility.ExitApp();
                }

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
                    // checks if the super dog is still working
                    CIMELDog.Default.IsAlive(true);
                }
                else
                {
                    // checks if the super dog is still working
                    if (!CIMELDog.Default.IsAlive(true)) return;

                    // check validation state
                    if (AreAllGood())
                    {
                        //double confirms
                        string question =
                            "准备好了吗?\r\n[Yes]: 马上开始\r\n[No]: 检查参数";
                        if (DialogResult.No == MessageBox.Show(this, question,
                            @"参数校验", MessageBoxButtons.YesNo, MessageBoxIcon.Warning,
                            MessageBoxDefaultButton.Button1))
                        {
                            this.btnAction.Enabled = true;
                            return;
                        }
                    }
                    else
                    {
                        this.ShowAlert(@"抱歉，参数配置仍有错误",@"参数校验");
                        this.btnAction.Enabled = true;
                        return;
                    }

                    // initial worker
                    string regionName = (string)((dynamic)(this.cmbRegions.SelectedItem)).Value;
                    string dataFileName = this.cmbFdatas.Text;
                    string instrumentId = this.txtSTNS_ID.Text;
                    // start the worker
                    worker.Start(regionName, instrumentId, dataFileName);
                }
            }
        }
        private void btnClose_Click(object sender, EventArgs e)
        {
            // checks if the super dog is still working
            if (!CIMELDog.Default.IsAlive(true)) return;
            
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
            //set the marks to question
            this.SetToQuestion(this.lblVal_FDATA);
            this.SetToQuestion(this.lblVal_CHARTSET);
            this.SetToQuestion(this.lblVal_FBRDF);
            this.SetToQuestion(this.lblVal_FDAT);
            this.SetToQuestion(this.lblVal_FIPT);
            this.SetToQuestion(this.lblVal_FOUT);
            this.SetToQuestion(this.lblVal_METADATA);
            this.SetToQuestion(this.lblVal_STNS_ID);
            //set the label of dirs to initializing
            this.cmbFdatas.Text = Settings.Default.LBL_INITIAL;
            this.lblCHARTSET.Text = Settings.Default.LBL_INITIAL;
            this.lblFBRDF.Text = Settings.Default.LBL_INITIAL;
            this.lblFDAT.Text = Settings.Default.LBL_INITIAL;
            this.lblFIPT.Text = Settings.Default.LBL_INITIAL;
            this.lblFOUT.Text = Settings.Default.LBL_INITIAL;
            this.lblMETADATA.Text = Settings.Default.LBL_INITIAL;
            this.txtSTNS_ID.Text = Settings.Default.LBL_INITIAL;
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
                   && lblVal_FDAT.ImageIndex == IMG_GOOD
                   && lblVal_METADATA.ImageIndex == IMG_GOOD;
        }

        private void cmbRegions_SelectedIndexChanged(object sender, EventArgs e)
        {
            // checks if the super dog is still working
            if (!CIMELDog.Default.IsAlive(true)) return;
            
            if (cmbRegions.Text == ComboBoxItem.EmptyItem.Text)
            {
                // disable action
                this.btnAction.Enabled = false;
                this.SetToQuestion(this.lblVal_STNS_FN);
                this.SetToDefault();
            }
            else
            {
                this.SetToGood(this.lblVal_STNS_FN);
                string region = ((dynamic) cmbRegions.SelectedItem).Value;
                this.Init(region);
                this.btnAction.Enabled = AreAllGood();
            }
        }

        private void txtSTNS_ID_TextChanged(object sender, EventArgs e)
        {
            // checks if the super dog is still working
            if (!CIMELDog.Default.IsAlive(true)) return;
            
            if (string.IsNullOrEmpty(txtSTNS_ID.Text))
                this.SetToError(this.lblVal_STNS_ID, LABEL_EMPTY);
            else if(txtSTNS_ID.Text == Settings.Default.LBL_INITIAL)
                this.SetToQuestion(this.lblVal_STNS_ID);
            else
                this.SetToGood(this.lblVal_STNS_ID);
        }

        private void cmbFdatas_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.cmbFdatas.Text == ComboBoxItem.EmptyItem.Text)
            {
                SetToQuestion(lblVal_FDATA);
                this.InitSTNS_ID(string.Empty);
            }
            else if (string.IsNullOrEmpty(this.cmbFdatas.Text))
            {
                SetToError(lblVal_FDATA, "- 文件名命名错误");
                this.InitSTNS_ID(string.Empty);
            }
            else
            {
                SetToGood(this.lblVal_FDATA);
                // initial STNS_ID
                string dataFile = this.cmbFdatas.Text;
                this.InitSTNS_ID(dataFile);
            }
        }
    }
}