namespace ComputingProject_UI
{
    partial class Form1
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
            this.update = new System.Windows.Forms.Timer(this.components);
            this.Forces = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // update
            // 
            this.update.Tick += new System.EventHandler(this.update_Tick);
            // 
            // Forces
            // 
            this.Forces.AutoSize = true;
            this.Forces.Location = new System.Drawing.Point(548, 13);
            this.Forces.Name = "Forces";
            this.Forces.Size = new System.Drawing.Size(35, 13);
            this.Forces.TabIndex = 0;
            this.Forces.Text = "label1";
            this.Forces.Click += new System.EventHandler(this.Forces_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(705, 579);
            this.Controls.Add(this.Forces);
            this.Name = "Form1";
            this.Text = "MainCanvas";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Timer update;
        private System.Windows.Forms.Label Forces;
    }
}

