﻿namespace Cipher
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
            this.textBox = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // contentTreeView
            // 
            this.contentTreeView.Location = new System.Drawing.Point(13, 13);
            this.contentTreeView.Name = "contentTreeView";
            this.contentTreeView.Size = new System.Drawing.Size(100, 237);
            this.contentTreeView.TabIndex = 0;
            this.contentTreeView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.contentTreeView_AfterSelect);
            // 
            // textBox
            // 
            this.textBox.Location = new System.Drawing.Point(120, 13);
            this.textBox.Multiline = true;
            this.textBox.Name = "textBox";
            this.textBox.ReadOnly = true;
            this.textBox.Size = new System.Drawing.Size(299, 237);
            this.textBox.TabIndex = 1;
            // 
            // HistoryForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(431, 262);
            this.Controls.Add(this.textBox);
            this.Controls.Add(this.contentTreeView);
            this.Name = "HistoryForm";
            this.Text = "HistoryForm";
            this.Load += new System.EventHandler(this.HistoryForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TreeView contentTreeView;
        private System.Windows.Forms.TextBox textBox;
    }
}