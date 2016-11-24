using Aeronet.Chart.AeronetData;
using Peach.Log;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Aeronet.Chart
{
    /// <summary>
    /// The Aeronet data process UI form
    /// </summary>
    public partial class fmAeronetData : Form
    {
        // the instance of DataWork
        // we will manages it including start job and stop job
        private DataWorker _dataWork;

        public fmAeronetData()
        {
            InitializeComponent();
            // initial the instance of DataWork
            this._dataWork = new DataWorker();
            // register the events, info and Error
            this._dataWork.Informed += this.LogInfo;
            this._dataWork.Failed += this.LogError;
            this._dataWork.Started += this.WorkerStarted;
            this._dataWork.Completed += this.WorkerCompleted;
            // initial form events
            this.Load += fmAeronetData_Load;
        }

        private void fmAeronetData_Load(object sender, EventArgs e)
        {
            //Initial folder view
            var folders = ConfigOptions.Singleton.GetFolders();
            // clean up the directory view
            this.tvDirs.Nodes.Clear();
            foreach (var folderDescription in folders)
            {
                // each of the working folder will be a root folder
                TreeNode rootNode = new TreeNode(folderDescription.Name, 0, 0);
                // attaches the instance of folder description to the root node, which will be used for the file view
                rootNode.Tag = folderDescription;
                // todo: show description and path below the tree view
                // scans the subfolders and add them to the root node
                this.AppendSubfolders(rootNode, folderDescription);
                // add the root node to the tree view
                this.tvDirs.Nodes.Add(rootNode);
            }
        }

        private void AppendSubfolders(TreeNode rootNode, FolderDescription rootFolder)
        {
            DirectoryInfo root = new DirectoryInfo(rootFolder.Path);
            DirectoryInfo[] subfolders = root.GetDirectories();
            foreach (DirectoryInfo subFolder in subfolders)
            {
                //Initial a FolderDescription
                // All subfolders show the same description as the root folder
                string description = rootFolder.Description;
                FolderDescription folderDescription = new FolderDescription(subFolder.Name, subFolder.FullName, description);
                // Initial tree node
                TreeNode subNode = new TreeNode(folderDescription.Name, 0, 0);
                // attach the instance of folder description
                subNode.Tag = folderDescription;
                // adds its subfolders
                this.AppendSubfolders(subNode, folderDescription);
                // append the subnode to root node
                rootNode.Nodes.Add(subNode);
            }
        }

        /// <summary>
        /// Invoke
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btAction_Click(object sender, EventArgs e)
        {
            Button button = sender as Button;
            bool isStart = button.Text.CompareTo("Start") == 0;
            if (isStart)
            {
                // 2 initial worker and run it
                var worker = this._dataWork;
                // todo: it should run within a thread which can be terminated
                worker.Start();
            }
            else
            {
                // stop the worker
                // 1 stop the running worker
                var worker = this._dataWork;
                worker.Stop();
            }
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
            this.btAction.Text = @"Start";
        }

        /// <summary>
        /// Apply start state to the UI controls when the worker starts the job
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="message"></param>
        private void WorkerStarted(object sender, EventMessage message)
        {
            // todo:it should be performed within UI thread
            this.btAction.Text = @"Stop";
        }
    }
}