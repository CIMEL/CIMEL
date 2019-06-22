using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CIMEL.Core;
using CIMEL.Figure;
using System.IO;
using System.Threading.Tasks;
using CIMEL.Core.Event;
using System.Threading;

namespace CIMEL.Chart.Chart
{
    public partial class ThreeDChart : UserControl
    {
        private TaskScheduler _uiTaskScheduler;

        public string DataFolder { get { return this._dataFolder; } set { this._dataFolder = value; } }
        public DataConfigFile DataConfigFile { get { return _objDataConfigFile; } set { _objDataConfigFile = value; } }

        private string _dataFolder;
        private DataConfigFile _objDataConfigFile;
        private MultipleLinePlot _3dLines;
        private Surface _3dsurface;
        private FigureType _figureType;
        private Label _lblmessage;

        public ThreeDChart()
        {
            InitializeComponent();
            this._lblmessage = new Label();
            this._lblmessage.AutoSize = false;
            this._lblmessage.TextAlign = ContentAlignment.MiddleCenter;
            this._lblmessage.Dock = DockStyle.Fill;
            this._lblmessage.Visible = false;
            this.Controls.Add(this._lblmessage);
            this.Load += ThreeDChart_Load;
            // this._figureType = figureType;
            this._3dLines = new MultipleLinePlot();
            this._3dLines.Info += OnInfo;
            this._3dsurface = new Surface();
            this._3dsurface.Info += OnInfo;
            this.Init(true);
        }

        private void OnInfo(object sender, MessageArgs args)
        {
            string msg = args.Message;
            Task.Factory.StartNew(() =>
            {
                this._lblmessage.Text = msg;
            }, CancellationToken.None, TaskCreationOptions.None, this._uiTaskScheduler);
        }

        private void ThreeDChart_Load(object sender, EventArgs e)
        {
            this._uiTaskScheduler = TaskScheduler.FromCurrentSynchronizationContext();
        }

        private void Init(bool state)
        {
            this.pictureBox1.Image = null;
        }

        public void Draw(int year, int month, int[] days)
        {
            if (this.DataConfigFile == null)
                throw new NotImplementedException("属性 DataConfigFile 没有初始化");

            if (this.DataFolder == null)
                throw new NotImplementedException("属性 DataFolder 没有初始化");

            this.UseWaitCursor = true;
            this.pictureBox1.Visible = false;

            string figureFile = string.Empty;
            string cachePath = this.GetCachePath();

            if (this._figureType == FigureType.ThreeDLines)
            {
                this.DrawMultipleLines(year, month,days,figureFile,cachePath);
            } else if (this._figureType == FigureType.ThreeDSurface) {

                this.DrawSurface(year, month, days, figureFile, cachePath);
            }
        }

        private void ShowImage(string image)
        {
            if (!string.IsNullOrEmpty(image))
            {
                Image pic = Bitmap.FromFile(image);
                this.pictureBox1.Image = pic;
            }
            this.pictureBox1.Visible = true;
            this.UseWaitCursor = false;
        }

        private void DrawMultipleLines(int year,int month,int[] days,string figureFile,string cachePath)
        {
            // load from cache
            string figureName = MultipleLinePlot.FigureName(this.DataConfigFile.Name, year, month, days);
            string cacheFile = Path.Combine(cachePath, figureName);
            if (File.Exists(cacheFile))
            {
                figureFile = cacheFile;
                this.ShowImage(figureFile);
            }
            else
            {
                this._lblmessage.Visible = true;
                Task draw = Task.Factory.StartNew<string>(() =>
                {
                    string result = this._3dLines.Draw(year, month, days, this.DataConfigFile, this.DataFolder);
                    // move to cache
                    File.Move(result, cacheFile);
                    // make sure it's moved
                    if (File.Exists(cacheFile))
                        result = cacheFile;
                    else
                    {
                        // as the moving action is failed, do nothing
                    }
                    return result;
                })
                .ContinueWith(t =>
                {
                    string file = string.Empty;
                    if (t.Exception != null)
                    {
                        string error = t.Exception.InnerException != null ? t.Exception.InnerException.Message :
                        t.Exception.Message;
                        this.ShowAlert(error, @"3D图错误");
                    }
                    else
                        file = t.Result;

                    this._lblmessage.Visible = false;
                    this.ShowImage(file);
                }, CancellationToken.None,
                TaskContinuationOptions.None,
                this._uiTaskScheduler);
            }
        }

        private void DrawSurface(int year, int month, int[] days, string figureFile, string cachePath)
        {
            // load from cache
            string figureName = Surface.FigureName(this.DataConfigFile.Name, year, month, days);
            string cacheFile = Path.Combine(cachePath, figureName);
            if (File.Exists(cacheFile))
            {
                figureFile = cacheFile;
                this.ShowImage(figureFile);
            }
            else
            {
                this._lblmessage.Visible = true;
                Task draw = Task.Factory.StartNew<string>(() =>
                {
                    string result = this._3dsurface.Draw(year, month, days, this.DataConfigFile, this.DataFolder);
                    // move to cache
                    File.Move(result, cacheFile);
                    // make sure it's moved
                    if (File.Exists(cacheFile))
                        result = cacheFile;
                    else
                    {
                        // as the moving action is failed, do nothing
                    }
                    return result;
                })
                .ContinueWith(t =>
                {
                    string file = string.Empty;
                    if (t.Exception != null)
                    {
                        string error = t.Exception.InnerException != null ? t.Exception.InnerException.Message :
                        t.Exception.Message;
                        this.ShowAlert(error, @"3D图错误");
                    }
                    else
                        file = t.Result;

                    this._lblmessage.Visible = false;
                    this.ShowImage(file);
                }, CancellationToken.None,
                TaskContinuationOptions.None,
                this._uiTaskScheduler);
            }
        }

        public void Init(FigureType figureType)
        {
            //disiable
            this.Disable();
            this._figureType = figureType;
        }

        public void Disable()
        {
            this.pictureBox1.Enabled = false;
            this.pictureBox1.Image = null;
            this.pictureBox1.Refresh();
        }

        public void Enable()
        {
            this.pictureBox1.Enabled = true;
            this.pictureBox1.Refresh();
        }

        private string GetCachePath() {
            if (this.DataFolder == null)
                throw new NotImplementedException("属性 DataFolder 没有初始化");

            if (this._figureType == FigureType.ThreeDLines)
            {
                string cachePath = Path.Combine(this.DataFolder, "3dml");
                if (!Directory.Exists(cachePath))
                    Directory.CreateDirectory(cachePath);

                return cachePath;
            }
            else if (this._figureType == FigureType.ThreeDSurface) {
                string cachePath = Path.Combine(this.DataFolder, "3dsf");
                if (!Directory.Exists(cachePath))
                    Directory.CreateDirectory(cachePath);

                return cachePath;
            }

            return this.DataFolder;
        }
    }
}
