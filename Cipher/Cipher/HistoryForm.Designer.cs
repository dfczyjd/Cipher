namespace Cipher
{
    partial class HistoryForm
    {
        /// <summary>
        /// Требуется переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Обязательный метод для поддержки конструктора - не изменяйте
        /// содержимое данного метода при помощи редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.contentTreeView = new System.Windows.Forms.TreeView();
            this.textBox = new System.Windows.Forms.RichTextBox();
            this.SuspendLayout();
            // 
            // contentTreeView
            // 
            this.contentTreeView.Location = new System.Drawing.Point(17, 16);
            this.contentTreeView.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.contentTreeView.Name = "contentTreeView";
            this.contentTreeView.Size = new System.Drawing.Size(132, 291);
            this.contentTreeView.TabIndex = 0;
            this.contentTreeView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.contentTreeView_AfterSelect);
            // 
            // textBox
            // 
            this.textBox.Location = new System.Drawing.Point(157, 16);
            this.textBox.Name = "textBox";
            this.textBox.ReadOnly = true;
            this.textBox.Size = new System.Drawing.Size(406, 291);
            this.textBox.TabIndex = 1;
            this.textBox.Text = "";
            // 
            // HistoryForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(575, 322);
            this.Controls.Add(this.textBox);
            this.Controls.Add(this.contentTreeView);
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "HistoryForm";
            this.Text = "HistoryForm";
            this.Load += new System.EventHandler(this.HistoryForm_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TreeView contentTreeView;
        private System.Windows.Forms.RichTextBox textBox;
    }
}