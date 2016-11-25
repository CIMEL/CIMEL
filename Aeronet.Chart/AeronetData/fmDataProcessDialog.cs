using Peach.Log;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Aeronet.Chart.AeronetData
{
    public partial class fmDataProcessDialog : Form
    {
        // the instance of DataWork
        // we will manages it including start job and stop job
        private DataWorker _dataWork;

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
        }

        private void fmDataProcessDialog_Load(object sender, EventArgs e)
        {
            // 2 initial worker and run it
            var worker = this._dataWork;
            // todo: it should run within a thread which can be terminated
            // worker.Start();
        }

        /// <summary>
        /// Adds the Info to the list box of logs
        /// todo: reference the code implementation in the PZM
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="message"></param>
        private void LogInfo(object sender, EventMessage message)
        {
            string info = "";
            bool external = true;
            string msg = string.Format("{0} -> {1}", (external ? "EXT" : "INT"), info);
            Logger.Default.Info(msg);
            Console.WriteLine(msg);
        }

        /// <summary>
        /// Adds the error to the list box of logs
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="message"></param>
        private void LogError(object sender, EventMessage message)
        {
            string error = "";
            bool external = true;
            string msg = string.Format("{0} -> {1}", (external ? "EXT" : "INT"), error);
            Logger.Default.Error(msg);
            Console.WriteLine(msg);
        }

        /// <summary>
        /// Apply complete state to the UI controls when the worker completes the job
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="message"></param>
        private void WorkerCompleted(object sender, EventMessage message)
        {
            // todo:it should be performed within UI thread
            this.btnClose.Enabled = true;
        }

        /// <summary>
        /// Apply start state to the UI controls when the worker starts the job
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="message"></param>
        private void WorkerStarted(object sender, EventMessage message)
        {
            // todo:it should be performed within UI thread
            this.btnClose.Enabled = false;
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            // stop the worker
            // 1 stop the running worker
            var worker = this._dataWork;

            worker.Stop();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}