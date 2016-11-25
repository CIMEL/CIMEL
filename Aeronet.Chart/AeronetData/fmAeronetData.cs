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
        public fmAeronetData()
        {
            InitializeComponent();
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
                rootNode.ImageKey = @"folder";
                // attaches the instance of folder description to the root node, which will be used for the file view
                rootNode.Tag = folderDescription;
                // scans the subfolders and add them to the root node
                this.AppendSubfolders(rootNode, folderDescription);
                // add the root node to the tree view
                this.tvDirs.Nodes.Add(rootNode);
            }

            this.tvDirs.NodeMouseClick += tvDirs_NodeMouseClick;

            if (this.tvDirs.Nodes.Count > 0)
            {
                // initial file view and description label
                FolderDescription folderDescription = this.tvDirs.Nodes[0].Tag as FolderDescription;
                // show the file view of the first folder node
                this.OnSelectedNodeChanged(folderDescription);
            }
        }

        private void tvDirs_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            var folderDescription = e.Node.Tag as FolderDescription;
            this.OnSelectedNodeChanged(folderDescription);
        }

        private void OnSelectedNodeChanged(FolderDescription folderDesc)
        {
            if (folderDesc != null)
            {
                // show the file view of the first folder node
                this.fileBrowser1.LoadFiles(folderDesc.Path);
                // show description on description label
                string description = string.Format("{0}: {1}\r\n{2}", folderDesc.Name, folderDesc.Description,
                    folderDesc.Path);
                this.lblDescription.Text = description;
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
                subNode.ImageKey = @"folder";
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
            // launch process dialog
            fmDataProcessDialog fmDataProcessDlg = new fmDataProcessDialog();
            fmDataProcessDlg.StartPosition = FormStartPosition.CenterParent;
            fmDataProcessDlg.ShowDialog(this);
        }

        /// <summary>
        /// Imports the selected files to current directory
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnImport_Click(object sender, EventArgs e)
        {
            var selectedFolder = this.GetSelectedFolder();
            if (selectedFolder == null) return;

            // show file open dialog
            var fileOpenDlg = new OpenFileDialog();
            fileOpenDlg.Multiselect = true;
            fileOpenDlg.CheckFileExists = true;
            fileOpenDlg.InitialDirectory = string.IsNullOrEmpty(selectedFolder.Path)
                ? Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
                : selectedFolder.Path;
            var result = fileOpenDlg.ShowDialog(this);
            if (result == DialogResult.OK)
            {
                string[] files = fileOpenDlg.FileNames;
                // copy to the current directory
                this.fileBrowser1.Copy(files, selectedFolder.Path);
                // refresh view
                this.fileBrowser1.LoadFiles(selectedFolder.Path);
            }
        }

        /// <summary>
        /// Refresh the view from the current directory
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRefresh_Click(object sender, EventArgs e)
        {
            var selectedFolder = this.GetSelectedFolder();
            if (selectedFolder == null) return;
            // loads the view from the current directory
            this.fileBrowser1.LoadFiles(selectedFolder.Path);
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private FolderDescription GetSelectedFolder()
        {
            if (this.tvDirs.Nodes.Count == 0) return null;
            var folderDesc = this.tvDirs.Nodes[0].Tag as FolderDescription;
            return folderDesc;
        }
    }
}