namespace Aeronet.Chart.AeronetData
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
            this.lstLogs = new System.Windows.Forms.ListBox();
            this.btnClose = new System.Windows.Forms.Button();
            this.btnAction = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.lblVal_TEMP = new System.Windows.Forms.Label();
            this.lblVal_FOUT = new System.Windows.Forms.Label();
            this.lblVal_FDAT = new System.Windows.Forms.Label();
            this.lblVal_FBRDF = new System.Windows.Forms.Label();
            this.lblVal_FIPT = new System.Windows.Forms.Label();
            this.lblVal_STNS_ID = new System.Windows.Forms.Label();
            this.lblVal_STNS_FN = new System.Windows.Forms.Label();
            this.lblVal_FDATA = new System.Windows.Forms.Label();
            this.txtSTNS_ID = new System.Windows.Forms.TextBox();
            this.lblTEMP = new System.Windows.Forms.Label();
            this.lblFOUT = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.lblDAT = new System.Windows.Forms.Label();
            this.lblFBRDF = new System.Windows.Forms.Label();
            this.lblFIPT = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.cmbRegions = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.lblFDATA = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // lstLogs
            // 
            this.lstLogs.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lstLogs.FormattingEnabled = true;
            this.lstLogs.Location = new System.Drawing.Point(12, 220);
            this.lstLogs.Name = "lstLogs";
            this.lstLogs.Size = new System.Drawing.Size(446, 316);
            this.lstLogs.TabIndex = 0;
            // 
            // btnClose
            // 
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClose.Location = new System.Drawing.Point(383, 541);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(75, 23);
            this.btnClose.TabIndex = 1;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // btnAction
            // 
            this.btnAction.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAction.Location = new System.Drawing.Point(302, 541);
            this.btnAction.Name = "btnAction";
            this.btnAction.Size = new System.Drawing.Size(75, 23);
            this.btnAction.TabIndex = 2;
            this.btnAction.Text = "Start";
            this.btnAction.UseVisualStyleBackColor = true;
            this.btnAction.Click += new System.EventHandler(this.btnAction_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.lblVal_TEMP);
            this.groupBox1.Controls.Add(this.lblVal_FOUT);
            this.groupBox1.Controls.Add(this.lblVal_FDAT);
            this.groupBox1.Controls.Add(this.lblVal_FBRDF);
            this.groupBox1.Controls.Add(this.lblVal_FIPT);
            this.groupBox1.Controls.Add(this.lblVal_STNS_ID);
            this.groupBox1.Controls.Add(this.lblVal_STNS_FN);
            this.groupBox1.Controls.Add(this.lblVal_FDATA);
            this.groupBox1.Controls.Add(this.txtSTNS_ID);
            this.groupBox1.Controls.Add(this.lblTEMP);
            this.groupBox1.Controls.Add(this.lblFOUT);
            this.groupBox1.Controls.Add(this.label13);
            this.groupBox1.Controls.Add(this.label12);
            this.groupBox1.Controls.Add(this.lblDAT);
            this.groupBox1.Controls.Add(this.lblFBRDF);
            this.groupBox1.Controls.Add(this.lblFIPT);
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.cmbRegions);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.lblFDATA);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(446, 204);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Parameters";
            // 
            // lblVal_TEMP
            // 
            this.lblVal_TEMP.AutoSize = true;
            this.lblVal_TEMP.Location = new System.Drawing.Point(237, 182);
            this.lblVal_TEMP.Name = "lblVal_TEMP";
            this.lblVal_TEMP.Size = new System.Drawing.Size(56, 13);
            this.lblVal_TEMP.TabIndex = 25;
            this.lblVal_TEMP.Text = "PENDING";
            // 
            // lblVal_FOUT
            // 
            this.lblVal_FOUT.AutoSize = true;
            this.lblVal_FOUT.Location = new System.Drawing.Point(237, 161);
            this.lblVal_FOUT.Name = "lblVal_FOUT";
            this.lblVal_FOUT.Size = new System.Drawing.Size(56, 13);
            this.lblVal_FOUT.TabIndex = 24;
            this.lblVal_FOUT.Text = "PENDING";
            // 
            // lblVal_FDAT
            // 
            this.lblVal_FDAT.AutoSize = true;
            this.lblVal_FDAT.Location = new System.Drawing.Point(237, 140);
            this.lblVal_FDAT.Name = "lblVal_FDAT";
            this.lblVal_FDAT.Size = new System.Drawing.Size(56, 13);
            this.lblVal_FDAT.TabIndex = 23;
            this.lblVal_FDAT.Text = "PENDING";
            // 
            // lblVal_FBRDF
            // 
            this.lblVal_FBRDF.AutoSize = true;
            this.lblVal_FBRDF.Location = new System.Drawing.Point(237, 119);
            this.lblVal_FBRDF.Name = "lblVal_FBRDF";
            this.lblVal_FBRDF.Size = new System.Drawing.Size(56, 13);
            this.lblVal_FBRDF.TabIndex = 22;
            this.lblVal_FBRDF.Text = "PENDING";
            // 
            // lblVal_FIPT
            // 
            this.lblVal_FIPT.AutoSize = true;
            this.lblVal_FIPT.Location = new System.Drawing.Point(237, 98);
            this.lblVal_FIPT.Name = "lblVal_FIPT";
            this.lblVal_FIPT.Size = new System.Drawing.Size(56, 13);
            this.lblVal_FIPT.TabIndex = 21;
            this.lblVal_FIPT.Text = "PENDING";
            // 
            // lblVal_STNS_ID
            // 
            this.lblVal_STNS_ID.AutoSize = true;
            this.lblVal_STNS_ID.Location = new System.Drawing.Point(237, 73);
            this.lblVal_STNS_ID.Name = "lblVal_STNS_ID";
            this.lblVal_STNS_ID.Size = new System.Drawing.Size(56, 13);
            this.lblVal_STNS_ID.TabIndex = 20;
            this.lblVal_STNS_ID.Text = "PENDING";
            // 
            // lblVal_STNS_FN
            // 
            this.lblVal_STNS_FN.AutoSize = true;
            this.lblVal_STNS_FN.Location = new System.Drawing.Point(237, 44);
            this.lblVal_STNS_FN.Name = "lblVal_STNS_FN";
            this.lblVal_STNS_FN.Size = new System.Drawing.Size(56, 13);
            this.lblVal_STNS_FN.TabIndex = 19;
            this.lblVal_STNS_FN.Text = "PENDING";
            // 
            // lblVal_FDATA
            // 
            this.lblVal_FDATA.AutoSize = true;
            this.lblVal_FDATA.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblVal_FDATA.Location = new System.Drawing.Point(237, 20);
            this.lblVal_FDATA.Name = "lblVal_FDATA";
            this.lblVal_FDATA.Size = new System.Drawing.Size(56, 13);
            this.lblVal_FDATA.TabIndex = 18;
            this.lblVal_FDATA.Text = "PENDING";
            // 
            // txtSTNS_ID
            // 
            this.txtSTNS_ID.Location = new System.Drawing.Point(79, 70);
            this.txtSTNS_ID.Name = "txtSTNS_ID";
            this.txtSTNS_ID.Size = new System.Drawing.Size(121, 20);
            this.txtSTNS_ID.TabIndex = 17;
            this.txtSTNS_ID.TextChanged += new System.EventHandler(this.txtSTNS_ID_TextChanged);
            // 
            // lblTEMP
            // 
            this.lblTEMP.AutoSize = true;
            this.lblTEMP.Location = new System.Drawing.Point(76, 182);
            this.lblTEMP.Name = "lblTEMP";
            this.lblTEMP.Size = new System.Drawing.Size(58, 13);
            this.lblTEMP.TabIndex = 16;
            this.lblTEMP.Text = "[Initializing]";
            // 
            // lblFOUT
            // 
            this.lblFOUT.AutoSize = true;
            this.lblFOUT.Location = new System.Drawing.Point(76, 161);
            this.lblFOUT.Name = "lblFOUT";
            this.lblFOUT.Size = new System.Drawing.Size(58, 13);
            this.lblFOUT.TabIndex = 15;
            this.lblFOUT.Text = "[Initializing]";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(6, 182);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(40, 13);
            this.label13.TabIndex = 14;
            this.label13.Text = "TEMP:";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(6, 161);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(39, 13);
            this.label12.TabIndex = 13;
            this.label12.Text = "FOUT:";
            // 
            // lblDAT
            // 
            this.lblDAT.AutoSize = true;
            this.lblDAT.Location = new System.Drawing.Point(76, 140);
            this.lblDAT.Name = "lblDAT";
            this.lblDAT.Size = new System.Drawing.Size(58, 13);
            this.lblDAT.TabIndex = 12;
            this.lblDAT.Text = "[Initializing]";
            // 
            // lblFBRDF
            // 
            this.lblFBRDF.AutoSize = true;
            this.lblFBRDF.Location = new System.Drawing.Point(76, 119);
            this.lblFBRDF.Name = "lblFBRDF";
            this.lblFBRDF.Size = new System.Drawing.Size(58, 13);
            this.lblFBRDF.TabIndex = 11;
            this.lblFBRDF.Text = "[Initializing]";
            // 
            // lblFIPT
            // 
            this.lblFIPT.AutoSize = true;
            this.lblFIPT.Location = new System.Drawing.Point(76, 98);
            this.lblFIPT.Name = "lblFIPT";
            this.lblFIPT.Size = new System.Drawing.Size(58, 13);
            this.lblFIPT.TabIndex = 10;
            this.lblFIPT.Text = "[Initializing]";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(6, 140);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(38, 13);
            this.label8.TabIndex = 9;
            this.label8.Text = "FDAT:";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(6, 119);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(45, 13);
            this.label7.TabIndex = 8;
            this.label7.Text = "FBRDF:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 98);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(33, 13);
            this.label4.TabIndex = 7;
            this.label4.Text = "FIPT:";
            // 
            // cmbRegions
            // 
            this.cmbRegions.FormattingEnabled = true;
            this.cmbRegions.Location = new System.Drawing.Point(79, 41);
            this.cmbRegions.Name = "cmbRegions";
            this.cmbRegions.Size = new System.Drawing.Size(121, 21);
            this.cmbRegions.TabIndex = 6;
            this.cmbRegions.SelectedIndexChanged += new System.EventHandler(this.cmbRegions_SelectedIndexChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 73);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(56, 13);
            this.label5.TabIndex = 4;
            this.label5.Text = "STNS_ID:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 44);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(59, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "STNS_FN:";
            // 
            // lblFDATA
            // 
            this.lblFDATA.AutoSize = true;
            this.lblFDATA.Location = new System.Drawing.Point(76, 20);
            this.lblFDATA.Name = "lblFDATA";
            this.lblFDATA.Size = new System.Drawing.Size(58, 13);
            this.lblFDATA.TabIndex = 1;
            this.lblFDATA.Text = "[Initializing]";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 20);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(45, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "FDATA:";
            // 
            // fmDataProcessDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(470, 576);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btnAction);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.lstLogs);
            this.Name = "fmDataProcessDialog";
            this.Text = "Data Process Dialog";
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
        private System.Windows.Forms.Label lblFDATA;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox cmbRegions;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label lblDAT;
        private System.Windows.Forms.Label lblFBRDF;
        private System.Windows.Forms.Label lblFIPT;
        private System.Windows.Forms.Label lblTEMP;
        private System.Windows.Forms.Label lblFOUT;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.TextBox txtSTNS_ID;
        private System.Windows.Forms.Label lblVal_STNS_ID;
        private System.Windows.Forms.Label lblVal_STNS_FN;
        private System.Windows.Forms.Label lblVal_FDATA;
        private System.Windows.Forms.Label lblVal_TEMP;
        private System.Windows.Forms.Label lblVal_FOUT;
        private System.Windows.Forms.Label lblVal_FDAT;
        private System.Windows.Forms.Label lblVal_FBRDF;
        private System.Windows.Forms.Label lblVal_FIPT;
    }
}