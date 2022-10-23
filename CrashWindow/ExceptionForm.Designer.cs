using CrashWindow;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
namespace DuckGame.src.MonoTime.Console
{
    public class ExRichTextBox : RichTextBox
    {
        public ExRichTextBox()
        {
            Selectable = false;
        }
        const int WM_SETFOCUS = 0x0007;
        const int WM_KILLFOCUS = 0x0008;

        ///<summary>
        /// Enables or disables selection highlight. 
        /// If you set `Selectable` to `false` then the selection highlight
        /// will be disabled. 
        /// It's enabled by default.
        ///</summary>
        [DefaultValue(true)]
        public bool Selectable { get; set; }
        protected override void WndProc(ref Message m)
        {
            
            //if (m.Msg == 32)
            //{
            //    m.Msg = 32;
            //    return;
            //}
            //else if (m.Msg != WM_SETFOCUS)
            //{
            //    System.Console.WriteLine(m.Msg.ToString());
            //}
            if (m.Msg == WM_SETFOCUS && !Selectable)
                m.Msg = WM_KILLFOCUS;

            base.WndProc(ref m);
        }
    }
    partial class ExceptionForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ExceptionForm));
            this.crashDescription = new System.Windows.Forms.RichTextBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.crashSourceLabel = new System.Windows.Forms.Label();
            this.forumLink = new System.Windows.Forms.LinkLabel();
            this.modDesc = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.restartButton = new System.Windows.Forms.Button();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.richTextBox2 = new DuckGame.src.MonoTime.Console.ExRichTextBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // crashDescription
            // 
            this.crashDescription.Location = new System.Drawing.Point(3, 126);
            this.crashDescription.Name = "crashDescription";
            this.crashDescription.ReadOnly = true;
            this.crashDescription.Size = new System.Drawing.Size(565, 200);
            this.crashDescription.TabIndex = 0;
            this.crashDescription.Text = "";
            this.crashDescription.LinkClicked += new System.Windows.Forms.LinkClickedEventHandler(this.crashDescription_LinkClicked);
            this.crashDescription.TextChanged += new System.EventHandler(this.crashDescription_TextChanged);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(6, 8);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(65, 65);
            this.pictureBox1.TabIndex = 2;
            this.pictureBox1.TabStop = false;
            // 
            // crashSourceLabel
            // 
            this.crashSourceLabel.AutoSize = true;
            this.crashSourceLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.crashSourceLabel.ForeColor = System.Drawing.Color.Red;
            this.crashSourceLabel.Location = new System.Drawing.Point(6, 85);
            this.crashSourceLabel.Name = "crashSourceLabel";
            this.crashSourceLabel.Size = new System.Drawing.Size(91, 13);
            this.crashSourceLabel.TabIndex = 3;
            this.crashSourceLabel.Text = "Crash Source: ";
            // 
            // forumLink
            // 
            this.forumLink.AutoSize = true;
            this.forumLink.LinkArea = new System.Windows.Forms.LinkArea(0, 0);
            this.forumLink.Location = new System.Drawing.Point(367, 84);
            this.forumLink.Name = "forumLink";
            this.forumLink.Size = new System.Drawing.Size(200, 17);
            this.forumLink.TabIndex = 4;
            this.forumLink.Text = "Duck Game Technical Support Forums";
            this.forumLink.UseCompatibleTextRendering = true;
            this.forumLink.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.forumLink_LinkClicked);
            // 
            // modDesc
            // 
            this.modDesc.AutoSize = true;
            this.modDesc.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.modDesc.ForeColor = System.Drawing.Color.Red;
            this.modDesc.Location = new System.Drawing.Point(6, 104);
            this.modDesc.Name = "modDesc";
            this.modDesc.Size = new System.Drawing.Size(303, 13);
            this.modDesc.TabIndex = 5;
            this.modDesc.Text = "Mods apparently were not responsible for this crash.";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(492, 389);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 31);
            this.button1.TabIndex = 6;
            this.button1.Text = "Close";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(411, 389);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 31);
            this.button2.TabIndex = 7;
            this.button2.Text = "Copy";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // restartButton
            // 
            this.restartButton.Location = new System.Drawing.Point(3, 389);
            this.restartButton.Name = "restartButton";
            this.restartButton.Size = new System.Drawing.Size(124, 31);
            this.restartButton.TabIndex = 8;
            this.restartButton.Text = "Restart Duck Game";
            this.restartButton.UseVisualStyleBackColor = true;
            this.restartButton.Click += new System.EventHandler(this.restartButton_Click);
            // 
            // richTextBox1
            // 
            this.richTextBox1.Location = new System.Drawing.Point(3, 328);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.ReadOnly = true;
            this.richTextBox1.Size = new System.Drawing.Size(565, 57);
            this.richTextBox1.TabIndex = 10;
            this.richTextBox1.Text = "<Information about this crash will be automatically sent to the developer, but if" +
    " you can provide any additional information please do it here!>";
            this.richTextBox1.TextChanged += new System.EventHandler(this.richTextBox1_TextChanged);
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(205, 398);
            this.checkBox1.Margin = new System.Windows.Forms.Padding(2);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(123, 17);
            this.checkBox1.TabIndex = 11;
            this.checkBox1.Text = "Submit Crash Report";
            this.checkBox1.UseVisualStyleBackColor = true;
            this.checkBox1.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // richTextBox2
            // 
            this.richTextBox2.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.None;
            this.richTextBox2.AutoSize = true;
            if (Program.IsLinux)
            {
                this.richTextBox2.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.50F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            }
            else
            {
                this.richTextBox2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            }
            this.richTextBox2.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.richTextBox2.Cursor = System.Windows.Forms.Cursors.Default;
            this.richTextBox2.Location = new System.Drawing.Point(75, 8);
            this.richTextBox2.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
            this.richTextBox2.Name = "richTextBox2";
            this.richTextBox2.Selectable = false;
            this.richTextBox2.Size = new System.Drawing.Size(493, 75);//new System.Drawing.Size(493, 65);
            this.richTextBox2.TabIndex = 12;
            this.richTextBox2.Text = resources.GetString("richTextBox2.Text");
            this.richTextBox2.TextChanged += new System.EventHandler(this.richTextBox2_TextChanged);
            // 
            // ExceptionForm
            // 
            Color DefaultBackground = Color.FromArgb(240, 240, 240);
            this.BackColor = DefaultBackground;
            this.richTextBox2.BackColor = DefaultBackground;
            this.checkBox1.BackColor = DefaultBackground;
            this.richTextBox1.BackColor = DefaultBackground;
            this.restartButton.BackColor = DefaultBackground;
            this.button2.BackColor = DefaultBackground;
            this.button1.BackColor = DefaultBackground;
            this.modDesc.BackColor = DefaultBackground;
            this.forumLink.BackColor = DefaultBackground;
            this.crashSourceLabel.BackColor = DefaultBackground;
            this.pictureBox1.BackColor = DefaultBackground;
            this.crashDescription.BackColor = DefaultBackground;

            this.Controls.Add(this.richTextBox2);
            this.Controls.Add(this.checkBox1);
            this.Controls.Add(this.richTextBox1);
            this.Controls.Add(this.restartButton);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.modDesc);
            this.Controls.Add(this.forumLink);
            this.Controls.Add(this.crashSourceLabel);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.crashDescription);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "ExceptionForm";
            this.Text = "Duck Game Crash Report";
            this.Load += new System.EventHandler(this.ExceptionForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RichTextBox crashDescription;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label crashSourceLabel;
        private System.Windows.Forms.LinkLabel forumLink;
        private Label modDesc;
        private Button button1;
        private Button button2;
        private Button restartButton;
        private RichTextBox richTextBox1;
        private CheckBox checkBox1;
        private ExRichTextBox richTextBox2;
    }
}