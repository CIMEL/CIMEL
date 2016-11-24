namespace Aeronet.Chart
{
    partial class fmAeronetData
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(fmAeronetData));
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.tvDirs = new System.Windows.Forms.TreeView();
            this.fileBrowser1 = new Aeronet.Chart.AeronetData.FileBrowser();
            this.btAction = new System.Windows.Forms.Button();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // statusStrip1
            // 
            this.statusStrip1.Location = new System.Drawing.Point(0, 441);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(573, 22);
            this.statusStrip1.TabIndex = 1;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // splitContainer1
            // 
            this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer1.Location = new System.Drawing.Point(4, 4);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.tvDirs);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.fileBrowser1);
            this.splitContainer1.Size = new System.Drawing.Size(564, 408);
            this.splitContainer1.SplitterDistance = 228;
            this.splitContainer1.TabIndex = 2;
            // 
            // tvDirs
            // 
            this.tvDirs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tvDirs.Location = new System.Drawing.Point(0, 0);
            this.tvDirs.Name = "tvDirs";
            this.tvDirs.Size = new System.Drawing.Size(228, 408);
            this.tvDirs.TabIndex = 0;
            // 
            // fileBrowser1
            // 
            this.fileBrowser1.CurrentDirectory = "";
            this.fileBrowser1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.fileBrowser1.Location = new System.Drawing.Point(0, 0);
            this.fileBrowser1.Name = "fileBrowser1";
            this.fileBrowser1.Size = new System.Drawing.Size(332, 408);
            this.fileBrowser1.TabIndex = 0;
            // 
            // btAction
            // 
            this.btAction.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btAction.Location = new System.Drawing.Point(494, 415);
            this.btAction.Name = "btAction";
            this.btAction.Size = new System.Drawing.Size(75, 23);
            this.btAction.TabIndex = 2;
            this.btAction.Text = "Process";
            this.btAction.UseVisualStyleBackColor = true;
            this.btAction.Click += new System.EventHandler(this.btAction_Click);
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "folder");
            // 
            // fmAeronetData
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(573, 463);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.btAction);
            this.Controls.Add(this.statusStrip1);
            this.Name = "fmAeronetData";
            this.Text = "fmAeronetData";
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Button btAction;
        private System.Windows.Forms.TreeView tvDirs;
        private AeronetData.FileBrowser fileBrowser1;
        private System.Windows.Forms.ImageList imageList1;
    }
}