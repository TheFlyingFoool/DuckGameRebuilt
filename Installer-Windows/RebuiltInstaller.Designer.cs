namespace Installer_Windows
{
    partial class RebuiltInstaller
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RebuiltInstaller));
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.lblAutoSearch = new System.Windows.Forms.Label();
            this.txtPath = new System.Windows.Forms.TextBox();
            this.btnChoosePath = new System.Windows.Forms.Button();
            this.btnInstall = new System.Windows.Forms.Button();
            this.lblPath = new System.Windows.Forms.Label();
            this.chkReplace = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize) (this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.Color.Transparent;
            this.pictureBox1.Image = ((System.Drawing.Image) (resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(12, 12);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(752, 255);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // lblAutoSearch
            // 
            this.lblAutoSearch.BackColor = System.Drawing.Color.Transparent;
            this.lblAutoSearch.Font = new System.Drawing.Font("Consolas", 21.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte) (0)));
            this.lblAutoSearch.ForeColor = System.Drawing.Color.White;
            this.lblAutoSearch.Location = new System.Drawing.Point(12, 283);
            this.lblAutoSearch.Name = "lblAutoSearch";
            this.lblAutoSearch.Size = new System.Drawing.Size(752, 59);
            this.lblAutoSearch.TabIndex = 1;
            this.lblAutoSearch.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // txtPath
            // 
            this.txtPath.BackColor = System.Drawing.Color.LightGray;
            this.txtPath.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.txtPath.Font = new System.Drawing.Font("Consolas", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte) (0)));
            this.txtPath.ForeColor = System.Drawing.Color.FromArgb(((int) (((byte) (60)))), ((int) (((byte) (60)))), ((int) (((byte) (60)))));
            this.txtPath.Location = new System.Drawing.Point(12, 381);
            this.txtPath.Name = "txtPath";
            this.txtPath.ReadOnly = true;
            this.txtPath.Size = new System.Drawing.Size(662, 32);
            this.txtPath.TabIndex = 0;
            this.txtPath.TabStop = false;
            // 
            // btnChoosePath
            // 
            this.btnChoosePath.BackColor = System.Drawing.Color.LightGray;
            this.btnChoosePath.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnChoosePath.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte) (0)));
            this.btnChoosePath.ForeColor = System.Drawing.Color.Black;
            this.btnChoosePath.Location = new System.Drawing.Point(674, 381);
            this.btnChoosePath.Margin = new System.Windows.Forms.Padding(0);
            this.btnChoosePath.Name = "btnChoosePath";
            this.btnChoosePath.Size = new System.Drawing.Size(90, 31);
            this.btnChoosePath.TabIndex = 3;
            this.btnChoosePath.Text = "Choose Path";
            this.btnChoosePath.UseVisualStyleBackColor = false;
            this.btnChoosePath.Click += new System.EventHandler(this.btnChoosePath_Click);
            // 
            // btnInstall
            // 
            this.btnInstall.BackColor = System.Drawing.Color.LightGray;
            this.btnInstall.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnInstall.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte) (0)));
            this.btnInstall.ForeColor = System.Drawing.SystemColors.WindowText;
            this.btnInstall.Location = new System.Drawing.Point(597, 428);
            this.btnInstall.Name = "btnInstall";
            this.btnInstall.Size = new System.Drawing.Size(167, 35);
            this.btnInstall.TabIndex = 4;
            this.btnInstall.Text = "Install";
            this.btnInstall.UseVisualStyleBackColor = false;
            this.btnInstall.Click += new System.EventHandler(this.btnNext_Click);
            // 
            // lblPath
            // 
            this.lblPath.Font = new System.Drawing.Font("Consolas", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte) (0)));
            this.lblPath.ForeColor = System.Drawing.Color.White;
            this.lblPath.Location = new System.Drawing.Point(12, 355);
            this.lblPath.Name = "lblPath";
            this.lblPath.Size = new System.Drawing.Size(371, 23);
            this.lblPath.TabIndex = 7;
            this.lblPath.Text = "Path to current Duck Game install:";
            // 
            // chkReplace
            // 
            this.chkReplace.Checked = true;
            this.chkReplace.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkReplace.Font = new System.Drawing.Font("Consolas", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte) (0)));
            this.chkReplace.Location = new System.Drawing.Point(12, 434);
            this.chkReplace.Name = "chkReplace";
            this.chkReplace.Size = new System.Drawing.Size(432, 29);
            this.chkReplace.TabIndex = 8;
            this.chkReplace.Text = "Replace Current Duck Game Install";
            this.chkReplace.UseVisualStyleBackColor = true;
            // 
            // RebuiltInstaller
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int) (((byte) (70)))), ((int) (((byte) (70)))), ((int) (((byte) (70)))));
            this.ClientSize = new System.Drawing.Size(776, 475);
            this.Controls.Add(this.chkReplace);
            this.Controls.Add(this.lblPath);
            this.Controls.Add(this.btnInstall);
            this.Controls.Add(this.btnChoosePath);
            this.Controls.Add(this.txtPath);
            this.Controls.Add(this.lblAutoSearch);
            this.Controls.Add(this.pictureBox1);
            this.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte) (0)));
            this.ForeColor = System.Drawing.Color.White;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon) (resources.GetObject("$this.Icon")));
            this.Name = "RebuiltInstaller";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Duck Game Rebuilt Installer";
            ((System.ComponentModel.ISupportInitialize) (this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private System.Windows.Forms.CheckBox chkReplace;

        private System.Windows.Forms.Label lblPath;

        private System.Windows.Forms.Button btnInstall;

        private System.Windows.Forms.Button btnChoosePath;

        private System.Windows.Forms.TextBox txtPath;

        private System.Windows.Forms.Label lblAutoSearch;

        private System.Windows.Forms.PictureBox pictureBox1;

        #endregion
    }
}