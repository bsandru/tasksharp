namespace TaskSharp
{
    partial class Taskbar
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
            if (disposing)
            {
                if (components != null)
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
            this._cmsRight = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.quitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this._flp = new System.Windows.Forms.FlowLayoutPanel();
            this._cmsRight.SuspendLayout();
            this.SuspendLayout();
            // 
            // _cmsRight
            // 
            this._cmsRight.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.quitToolStripMenuItem});
            this._cmsRight.Name = "_cmsRight";
            this._cmsRight.Size = new System.Drawing.Size(100, 26);
            // 
            // quitToolStripMenuItem
            // 
            this.quitToolStripMenuItem.Name = "quitToolStripMenuItem";
            this.quitToolStripMenuItem.Size = new System.Drawing.Size(99, 22);
            this.quitToolStripMenuItem.Text = "Quit";
            this.quitToolStripMenuItem.Click += new System.EventHandler(this.quitToolStripMenuItem_Click);
            // 
            // _flp
            // 
            this._flp.BackColor = System.Drawing.Color.Transparent;
            this._flp.Dock = System.Windows.Forms.DockStyle.Fill;
            this._flp.Location = new System.Drawing.Point(0, 0);
            this._flp.Margin = new System.Windows.Forms.Padding(0);
            this._flp.Padding = new System.Windows.Forms.Padding(3, 0, 3, 0);
            this._flp.Name = "_flp";
            this._flp.Size = new System.Drawing.Size(292, 273);
            this._flp.TabIndex = 1;
            // 
            // Taskbar
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(292, 273);
            this.ContextMenuStrip = this._cmsRight;
            this.Controls.Add(this._flp);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "Taskbar";
            this.ShowInTaskbar = false;
            this.Text = "Sidebar";
            this.TopMost = true;
            this._cmsRight.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ContextMenuStrip _cmsRight;
        private System.Windows.Forms.ToolStripMenuItem quitToolStripMenuItem;
        private System.Windows.Forms.FlowLayoutPanel _flp;
    }
}

