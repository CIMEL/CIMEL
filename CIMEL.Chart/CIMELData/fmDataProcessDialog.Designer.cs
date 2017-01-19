namespace CIMEL.Chart.CIMELData
{
    partial class fmDataProcessDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(fmDataProcessDialog));
            this.lstLogs = new System.Windows.Forms.ListBox();
            this.btnClose = new System.Windows.Forms.Button();
            this.btnAction = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.cmbFdatas = new System.Windows.Forms.ComboBox();
            this.lblVal_METADATA = new System.Windows.Forms.Label();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.lblMETADATA = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.lblVal_CHARTSET = new System.Windows.Forms.Label();
            this.lblVal_FOUT = new System.Windows.Forms.Label();
            this.lblVal_FDAT = new System.Windows.Forms.Label();
            this.lblVal_FBRDF = new System.Windows.Forms.Label();
            this.lblVal_FIPT = new System.Windows.Forms.Label();
            this.lblVal_STNS_ID = new System.Windows.Forms.Label();
            this.lblVal_STNS_FN = new System.Windows.Forms.Label();
            this.lblVal_FDATA = new System.Windows.Forms.Label();
            this.txtSTNS_ID = new System.Windows.Forms.TextBox();
            this.lblCHARTSET = new System.Windows.Forms.Label();
            this.lblFOUT = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.lblFDAT = new System.Windows.Forms.Label();
            this.lblFBRDF = new System.Windows.Forms.Label();
            this.lblFIPT = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.cmbRegions = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.btnDraw = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // lstLogs
            // 
            this.lstLogs.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lstLogs.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
            this.lstLogs.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lstLogs.FormattingEnabled = true;
            this.lstLogs.Location = new System.Drawing.Point(14, 288);
            this.lstLogs.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.lstLogs.Name = "lstLogs";
            this.lstLogs.Size = new System.Drawing.Size(571, 412);
            this.lstLogs.TabIndex = 4;
            this.lstLogs.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.lstLogs_DrawItem);
            // 
            // btnClose
            // 
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClose.Location = new System.Drawing.Point(498, 707);
            this.btnClose.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(87, 30);
            this.btnClose.TabIndex = 6;
            this.btnClose.Text = global::CIMEL.Chart.Properties.Settings.Default.BTN_CLOSE_TEXT;
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // btnAction
            // 
            this.btnAction.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAction.Location = new System.Drawing.Point(404, 707);
            this.btnAction.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnAction.Name = "btnAction";
            this.btnAction.Size = new System.Drawing.Size(87, 30);
            this.btnAction.TabIndex = 3;
            this.btnAction.Text = global::CIMEL.Chart.Properties.Settings.Default.BTN_START_TEXT;
            this.btnAction.UseVisualStyleBackColor = true;
            this.btnAction.Click += new System.EventHandler(this.btnAction_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.cmbFdatas);
            this.groupBox1.Controls.Add(this.lblVal_METADATA);
            this.groupBox1.Controls.Add(this.lblMETADATA);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.lblVal_CHARTSET);
            this.groupBox1.Controls.Add(this.lblVal_FOUT);
            this.groupBox1.Controls.Add(this.lblVal_FDAT);
            this.groupBox1.Controls.Add(this.lblVal_FBRDF);
            this.groupBox1.Controls.Add(this.lblVal_FIPT);
            this.groupBox1.Controls.Add(this.lblVal_STNS_ID);
            this.groupBox1.Controls.Add(this.lblVal_STNS_FN);
            this.groupBox1.Controls.Add(this.lblVal_FDATA);
            this.groupBox1.Controls.Add(this.txtSTNS_ID);
            this.groupBox1.Controls.Add(this.lblCHARTSET);
            this.groupBox1.Controls.Add(this.lblFOUT);
            this.groupBox1.Controls.Add(this.label13);
            this.groupBox1.Controls.Add(this.label12);
            this.groupBox1.Controls.Add(this.lblFDAT);
            this.groupBox1.Controls.Add(this.lblFBRDF);
            this.groupBox1.Controls.Add(this.lblFIPT);
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.cmbRegions);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(14, 16);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.groupBox1.Size = new System.Drawing.Size(572, 267);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "参数校验";
            // 
            // cmbFdatas
            // 
            this.cmbFdatas.FormattingEnabled = true;
            this.cmbFdatas.Location = new System.Drawing.Point(151, 49);
            this.cmbFdatas.Name = "cmbFdatas";
            this.cmbFdatas.Size = new System.Drawing.Size(250, 25);
            this.cmbFdatas.TabIndex = 1;
            this.cmbFdatas.SelectedIndexChanged += new System.EventHandler(this.cmbFdatas_SelectedIndexChanged);
            // 
            // lblVal_METADATA
            // 
            this.lblVal_METADATA.ImageIndex = 0;
            this.lblVal_METADATA.ImageList = this.imageList1;
            this.lblVal_METADATA.Location = new System.Drawing.Point(126, 239);
            this.lblVal_METADATA.Name = "lblVal_METADATA";
            this.lblVal_METADATA.Size = new System.Drawing.Size(15, 17);
            this.lblVal_METADATA.TabIndex = 21;
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "question");
            this.imageList1.Images.SetKeyName(1, "good");
            this.imageList1.Images.SetKeyName(2, "error");
            // 
            // lblMETADATA
            // 
            this.lblMETADATA.AutoSize = true;
            this.lblMETADATA.Location = new System.Drawing.Point(148, 239);
            this.lblMETADATA.Name = "lblMETADATA";
            this.lblMETADATA.Size = new System.Drawing.Size(52, 17);
            this.lblMETADATA.TabIndex = 12;
            this.lblMETADATA.Text = global::CIMEL.Chart.Properties.Settings.Default.LBL_INITIAL;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 239);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(83, 17);
            this.label2.TabIndex = 30;
            this.label2.Text = "其他参数目录:";
            // 
            // lblVal_CHARTSET
            // 
            this.lblVal_CHARTSET.ImageIndex = 0;
            this.lblVal_CHARTSET.ImageList = this.imageList1;
            this.lblVal_CHARTSET.Location = new System.Drawing.Point(126, 213);
            this.lblVal_CHARTSET.Name = "lblVal_CHARTSET";
            this.lblVal_CHARTSET.Size = new System.Drawing.Size(15, 17);
            this.lblVal_CHARTSET.TabIndex = 20;
            // 
            // lblVal_FOUT
            // 
            this.lblVal_FOUT.ImageIndex = 0;
            this.lblVal_FOUT.ImageList = this.imageList1;
            this.lblVal_FOUT.Location = new System.Drawing.Point(126, 187);
            this.lblVal_FOUT.Name = "lblVal_FOUT";
            this.lblVal_FOUT.Size = new System.Drawing.Size(15, 17);
            this.lblVal_FOUT.TabIndex = 19;
            // 
            // lblVal_FDAT
            // 
            this.lblVal_FDAT.ImageIndex = 0;
            this.lblVal_FDAT.ImageList = this.imageList1;
            this.lblVal_FDAT.Location = new System.Drawing.Point(126, 161);
            this.lblVal_FDAT.Name = "lblVal_FDAT";
            this.lblVal_FDAT.Size = new System.Drawing.Size(15, 17);
            this.lblVal_FDAT.TabIndex = 18;
            // 
            // lblVal_FBRDF
            // 
            this.lblVal_FBRDF.ImageIndex = 0;
            this.lblVal_FBRDF.ImageList = this.imageList1;
            this.lblVal_FBRDF.Location = new System.Drawing.Point(126, 135);
            this.lblVal_FBRDF.Name = "lblVal_FBRDF";
            this.lblVal_FBRDF.Size = new System.Drawing.Size(15, 17);
            this.lblVal_FBRDF.TabIndex = 17;
            // 
            // lblVal_FIPT
            // 
            this.lblVal_FIPT.ImageIndex = 0;
            this.lblVal_FIPT.ImageList = this.imageList1;
            this.lblVal_FIPT.Location = new System.Drawing.Point(126, 109);
            this.lblVal_FIPT.Name = "lblVal_FIPT";
            this.lblVal_FIPT.Size = new System.Drawing.Size(15, 17);
            this.lblVal_FIPT.TabIndex = 16;
            // 
            // lblVal_STNS_ID
            // 
            this.lblVal_STNS_ID.BackColor = System.Drawing.SystemColors.Control;
            this.lblVal_STNS_ID.ImageIndex = 0;
            this.lblVal_STNS_ID.ImageList = this.imageList1;
            this.lblVal_STNS_ID.Location = new System.Drawing.Point(126, 80);
            this.lblVal_STNS_ID.Name = "lblVal_STNS_ID";
            this.lblVal_STNS_ID.Size = new System.Drawing.Size(15, 17);
            this.lblVal_STNS_ID.TabIndex = 15;
            // 
            // lblVal_STNS_FN
            // 
            this.lblVal_STNS_FN.ImageIndex = 0;
            this.lblVal_STNS_FN.ImageList = this.imageList1;
            this.lblVal_STNS_FN.Location = new System.Drawing.Point(126, 20);
            this.lblVal_STNS_FN.Name = "lblVal_STNS_FN";
            this.lblVal_STNS_FN.Size = new System.Drawing.Size(15, 17);
            this.lblVal_STNS_FN.TabIndex = 13;
            // 
            // lblVal_FDATA
            // 
            this.lblVal_FDATA.BackColor = System.Drawing.SystemColors.Control;
            this.lblVal_FDATA.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblVal_FDATA.ImageIndex = 0;
            this.lblVal_FDATA.ImageList = this.imageList1;
            this.lblVal_FDATA.Location = new System.Drawing.Point(126, 51);
            this.lblVal_FDATA.Name = "lblVal_FDATA";
            this.lblVal_FDATA.Size = new System.Drawing.Size(15, 17);
            this.lblVal_FDATA.TabIndex = 14;
            // 
            // txtSTNS_ID
            // 
            this.txtSTNS_ID.Location = new System.Drawing.Point(151, 81);
            this.txtSTNS_ID.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.txtSTNS_ID.Name = "txtSTNS_ID";
            this.txtSTNS_ID.Size = new System.Drawing.Size(140, 23);
            this.txtSTNS_ID.TabIndex = 2;
            this.txtSTNS_ID.TextChanged += new System.EventHandler(this.txtSTNS_ID_TextChanged);
            // 
            // lblCHARTSET
            // 
            this.lblCHARTSET.AutoSize = true;
            this.lblCHARTSET.Location = new System.Drawing.Point(147, 213);
            this.lblCHARTSET.Name = "lblCHARTSET";
            this.lblCHARTSET.Size = new System.Drawing.Size(52, 17);
            this.lblCHARTSET.TabIndex = 11;
            this.lblCHARTSET.Text = global::CIMEL.Chart.Properties.Settings.Default.LBL_INITIAL;
            // 
            // lblFOUT
            // 
            this.lblFOUT.AutoSize = true;
            this.lblFOUT.Location = new System.Drawing.Point(147, 187);
            this.lblFOUT.Name = "lblFOUT";
            this.lblFOUT.Size = new System.Drawing.Size(52, 17);
            this.lblFOUT.TabIndex = 10;
            this.lblFOUT.Text = global::CIMEL.Chart.Properties.Settings.Default.LBL_INITIAL;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(6, 213);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(71, 17);
            this.label13.TabIndex = 29;
            this.label13.Text = "图像集目录:";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(6, 187);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(71, 17);
            this.label12.TabIndex = 28;
            this.label12.Text = "主输出目录:";
            // 
            // lblFDAT
            // 
            this.lblFDAT.AutoSize = true;
            this.lblFDAT.Location = new System.Drawing.Point(147, 161);
            this.lblFDAT.Name = "lblFDAT";
            this.lblFDAT.Size = new System.Drawing.Size(52, 17);
            this.lblFDAT.TabIndex = 9;
            this.lblFDAT.Text = global::CIMEL.Chart.Properties.Settings.Default.LBL_INITIAL;
            // 
            // lblFBRDF
            // 
            this.lblFBRDF.AutoSize = true;
            this.lblFBRDF.Location = new System.Drawing.Point(147, 135);
            this.lblFBRDF.Name = "lblFBRDF";
            this.lblFBRDF.Size = new System.Drawing.Size(52, 17);
            this.lblFBRDF.TabIndex = 8;
            this.lblFBRDF.Text = global::CIMEL.Chart.Properties.Settings.Default.LBL_INITIAL;
            // 
            // lblFIPT
            // 
            this.lblFIPT.AutoSize = true;
            this.lblFIPT.Location = new System.Drawing.Point(147, 109);
            this.lblFIPT.Name = "lblFIPT";
            this.lblFIPT.Size = new System.Drawing.Size(52, 17);
            this.lblFIPT.TabIndex = 7;
            this.lblFIPT.Text = global::CIMEL.Chart.Properties.Settings.Default.LBL_INITIAL;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(6, 161);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(71, 17);
            this.label8.TabIndex = 27;
            this.label8.Text = "主数据目录:";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(6, 135);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(114, 17);
            this.label7.TabIndex = 26;
            this.label7.Text = "BRDF参数文件目录:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 109);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(83, 17);
            this.label4.TabIndex = 25;
            this.label4.Text = "参数文件目录:";
            // 
            // cmbRegions
            // 
            this.cmbRegions.FormattingEnabled = true;
            this.cmbRegions.Location = new System.Drawing.Point(151, 17);
            this.cmbRegions.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.cmbRegions.Name = "cmbRegions";
            this.cmbRegions.Size = new System.Drawing.Size(250, 25);
            this.cmbRegions.TabIndex = 0;
            this.cmbRegions.SelectedIndexChanged += new System.EventHandler(this.cmbRegions_SelectedIndexChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 84);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(59, 17);
            this.label5.TabIndex = 24;
            this.label5.Text = "站点仪器:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 20);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(35, 17);
            this.label3.TabIndex = 22;
            this.label3.Text = "站点:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 52);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(71, 17);
            this.label1.TabIndex = 23;
            this.label1.Text = "主数据文件:";
            // 
            // toolTip1
            // 
            this.toolTip1.IsBalloon = true;
            this.toolTip1.ShowAlways = true;
            this.toolTip1.ToolTipIcon = System.Windows.Forms.ToolTipIcon.Info;
            // 
            // btnDraw
            // 
            this.btnDraw.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDraw.Location = new System.Drawing.Point(312, 707);
            this.btnDraw.Name = "btnDraw";
            this.btnDraw.Size = new System.Drawing.Size(87, 30);
            this.btnDraw.TabIndex = 5;
            this.btnDraw.Text = "生成矩阵(&M)";
            this.btnDraw.UseVisualStyleBackColor = true;
            this.btnDraw.Click += new System.EventHandler(this.btnDraw_Click);
            // 
            // fmDataProcessDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(600, 753);
            this.Controls.Add(this.btnDraw);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btnAction);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.lstLogs);
            this.Font = global::CIMEL.Chart.Properties.Settings.Default.DEFAULT_FONT;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "fmDataProcessDialog";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = global::CIMEL.Chart.Properties.Settings.Default.FM_DATA_PRO_TEXT;
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListBox lstLogs;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Button btnAction;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox cmbRegions;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label lblFDAT;
        private System.Windows.Forms.Label lblFBRDF;
        private System.Windows.Forms.Label lblFIPT;
        private System.Windows.Forms.Label lblCHARTSET;
        private System.Windows.Forms.Label lblFOUT;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label lblVal_STNS_ID;
        private System.Windows.Forms.Label lblVal_STNS_FN;
        private System.Windows.Forms.Label lblVal_FDATA;
        private System.Windows.Forms.Label lblVal_CHARTSET;
        private System.Windows.Forms.Label lblVal_FOUT;
        private System.Windows.Forms.Label lblVal_FDAT;
        private System.Windows.Forms.Label lblVal_FBRDF;
        private System.Windows.Forms.Label lblVal_FIPT;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.Label lblVal_METADATA;
        private System.Windows.Forms.Label lblMETADATA;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cmbFdatas;
        private System.Windows.Forms.TextBox txtSTNS_ID;
        private System.Windows.Forms.Button btnDraw;
    }
}