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
using CIMEL.Dog;

namespace CIMEL.Chart.CIMELData
{
    public partial class FileViewer : UserControl
    {
        public string CurrentDirectory { get; set; }

        public FileViewer()
        {
            InitializeComponent();
            // initial current directory
            this.CurrentDirectory = string.Empty;
            // clean up the items in the view
            this.lvFiles.Items.Clear();
            // apply 'Details' view
            this.lvFiles.View = View.Details;
        }

        /// <summary>
        /// Loads file view from the given directory
        /// </summary>
        public void LoadFiles(string directory)
        {
            if (string.IsNullOrEmpty(directory) || !Directory.Exists(directory)) return;// do nothing

            // update currentDirectory
            if (String.CompareOrdinal(directory, this.CurrentDirectory) != 0)
            {
                this.CurrentDirectory = directory;
            }

            // clean up current view
            this.lvFiles.Items.Clear();

            var diCurrent = new DirectoryInfo(this.CurrentDirectory);

            ListViewItem.ListViewSubItem[] subItems;
            ListViewItem item = null;
            // attaches sub folders
            foreach (DirectoryInfo dir in diCurrent.GetDirectories())
            {
                // 0: imageIndex -> folder
                // dir.Name: -> the Name column
                item = new ListViewItem(dir.Name, 0);
                subItems = new ListViewItem.ListViewSubItem[]
                {
                    new ListViewItem.ListViewSubItem(item, ""), // -> the Type column, the folder won't show Type
                    new ListViewItem.ListViewSubItem(item, dir.LastAccessTime.ToString("yyyy-MM-dd")) // -> the lastModified column
                };
                // Add Type and LastModified columns to the entry
                item.SubItems.AddRange(subItems);
                // Add the entry to the view
                this.lvFiles.Items.Add(item);
            }
            // attaches files
            foreach (FileInfo file in diCurrent.GetFiles())
            {
                // 1: imageIndex -> file
                // fileName: -> the Name column
                item = new ListViewItem(file.Name, 1);
                // Initial Type and LastModified columns
                subItems = new ListViewItem.ListViewSubItem[]
                {
                    new ListViewItem.ListViewSubItem(item, file.Extension), // -> the Type column
                    new ListViewItem.ListViewSubItem(item, file.LastAccessTime.ToString("yyyy-MM-dd")) // -> the lastModified column
                };
                // Add Type and LastModified columns to the entry
                item.SubItems.AddRange(subItems);
                // Add the entry to the view
                this.lvFiles.Items.Add(item);
            }
            // Apply width of column
            this.lvFiles.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
        }

        private void lvFiles_DragEnter(object sender, DragEventArgs e)
        {
            // checks if the super dog is still working
            if (!CIMELDog.Default.IsAlive(true)) return;

            // just allow files drag-drop
            e.Effect = e.Data.GetDataPresent(DataFormats.FileDrop) ? DragDropEffects.Copy : DragDropEffects.None;
        }

        private void lvFiles_DragDrop(object sender, DragEventArgs e)
        {
            // checks if the super dog is still working
            if (!CIMELDog.Default.IsAlive(true)) return;
            
            // just process files drag-drop
            if (!e.Data.GetDataPresent(DataFormats.FileDrop)) return;

            // 1. copy the files
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            this.Copy(files, this.CurrentDirectory);
            // 2. refresh the view
            this.LoadFiles(this.CurrentDirectory);
        }

        /// <summary>
        /// Copies the given files to the given directory
        /// </summary>
        /// <param name="files">the files be copying</param>
        /// <param name="destDir">the destination directory</param>
        public void Copy(IEnumerable<string> files, string destDir)
        {
            Parallel.ForEach(files, (file) =>
            {
                string fileName = Path.GetFileName(file);
                string newFile = Path.Combine(destDir, fileName);
                File.Copy(file, newFile, true);
            });
        }
    }
}