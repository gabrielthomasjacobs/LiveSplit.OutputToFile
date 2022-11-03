namespace LiveSplit.UI.Components
{
    partial class OutputToFileSettings
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
			this.txtFolderPath = new System.Windows.Forms.TextBox();
			this.btnSelectFolder = new System.Windows.Forms.Button();
			this.labelFolderPath = new System.Windows.Forms.Label();
			this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			this.numericSplitsBefore = new System.Windows.Forms.NumericUpDown();
			this.numericSplitsAfter = new System.Windows.Forms.NumericUpDown();
			this.labelSplitsBefore = new System.Windows.Forms.Label();
			this.labelSplitsAfter = new System.Windows.Forms.Label();
			this.tableLayoutPanel1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.numericSplitsBefore)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.numericSplitsAfter)).BeginInit();
			this.SuspendLayout();
			// 
			// txtFolderPath
			// 
			this.txtFolderPath.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
			this.txtFolderPath.Location = new System.Drawing.Point(86, 4);
			this.txtFolderPath.Name = "txtFolderPath";
			this.txtFolderPath.Size = new System.Drawing.Size(293, 20);
			this.txtFolderPath.TabIndex = 0;
			// 
			// btnSelectFolder
			// 
			this.btnSelectFolder.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
			this.btnSelectFolder.Location = new System.Drawing.Point(385, 3);
			this.btnSelectFolder.Name = "btnSelectFolder";
			this.btnSelectFolder.Size = new System.Drawing.Size(74, 23);
			this.btnSelectFolder.TabIndex = 1;
			this.btnSelectFolder.Text = "Browse...";
			this.btnSelectFolder.UseVisualStyleBackColor = true;
			this.btnSelectFolder.Click += new System.EventHandler(this.btnSelectFile_Click);
			// 
			// labelFolderPath
			// 
			this.labelFolderPath.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
			this.labelFolderPath.AutoSize = true;
			this.labelFolderPath.Location = new System.Drawing.Point(3, 8);
			this.labelFolderPath.Name = "labelFolderPath";
			this.labelFolderPath.Size = new System.Drawing.Size(77, 13);
			this.labelFolderPath.TabIndex = 3;
			this.labelFolderPath.Text = "Output Folder:";
			// 
			// tableLayoutPanel1
			// 
			this.tableLayoutPanel1.AutoSize = true;
			this.tableLayoutPanel1.ColumnCount = 3;
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 83F));
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.tableLayoutPanel1.Controls.Add(this.labelSplitsBefore, 0, 1);
			this.tableLayoutPanel1.Controls.Add(this.labelFolderPath, 0, 0);
			this.tableLayoutPanel1.Controls.Add(this.btnSelectFolder, 2, 0);
			this.tableLayoutPanel1.Controls.Add(this.txtFolderPath, 1, 0);
			this.tableLayoutPanel1.Controls.Add(this.numericSplitsBefore, 1, 1);
			this.tableLayoutPanel1.Controls.Add(this.numericSplitsAfter, 1, 3);
			this.tableLayoutPanel1.Controls.Add(this.labelSplitsAfter, 0, 3);
			this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tableLayoutPanel1.Location = new System.Drawing.Point(7, 7);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			this.tableLayoutPanel1.RowCount = 5;
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 29F));
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
			this.tableLayoutPanel1.Size = new System.Drawing.Size(462, 233);
			this.tableLayoutPanel1.TabIndex = 1;
			// 
			// numericSplitsBefore
			// 
			this.numericSplitsBefore.Location = new System.Drawing.Point(86, 32);
			this.numericSplitsBefore.Name = "numericSplitsBefore";
			this.numericSplitsBefore.Size = new System.Drawing.Size(120, 20);
			this.numericSplitsBefore.TabIndex = 4;
			// 
			// numericSplitsAfter
			// 
			this.numericSplitsAfter.Location = new System.Drawing.Point(86, 58);
			this.numericSplitsAfter.Name = "numericSplitsAfter";
			this.numericSplitsAfter.Size = new System.Drawing.Size(120, 20);
			this.numericSplitsAfter.TabIndex = 5;
			// 
			// labelSplitsBefore
			// 
			this.labelSplitsBefore.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
			this.labelSplitsBefore.AutoSize = true;
			this.labelSplitsBefore.Location = new System.Drawing.Point(3, 35);
			this.labelSplitsBefore.Name = "labelSplitsBefore";
			this.labelSplitsBefore.Size = new System.Drawing.Size(77, 13);
			this.labelSplitsBefore.TabIndex = 6;
			this.labelSplitsBefore.Text = "Output Folder:";
			// 
			// labelSplitsAfter
			// 
			this.labelSplitsAfter.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
			this.labelSplitsAfter.AutoSize = true;
			this.labelSplitsAfter.Location = new System.Drawing.Point(3, 61);
			this.labelSplitsAfter.Name = "labelSplitsAfter";
			this.labelSplitsAfter.Size = new System.Drawing.Size(77, 13);
			this.labelSplitsAfter.TabIndex = 7;
			this.labelSplitsAfter.Text = "Output Folder:";
			// 
			// OutputToFileSettings
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.tableLayoutPanel1);
			this.Name = "OutputToFileSettings";
			this.Padding = new System.Windows.Forms.Padding(7);
			this.Size = new System.Drawing.Size(476, 247);
			this.tableLayoutPanel1.ResumeLayout(false);
			this.tableLayoutPanel1.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.numericSplitsBefore)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.numericSplitsAfter)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

        }

		#endregion

		public System.Windows.Forms.TextBox txtFolderPath;
		private System.Windows.Forms.Button btnSelectFolder;
		private System.Windows.Forms.Label labelFolderPath;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
		private System.Windows.Forms.Label labelSplitsBefore;
		private System.Windows.Forms.NumericUpDown numericSplitsBefore;
		private System.Windows.Forms.NumericUpDown numericSplitsAfter;
		private System.Windows.Forms.Label labelSplitsAfter;
	}
}