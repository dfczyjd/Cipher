namespace Cipher
{
    partial class EditForm
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
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.textureComboBox = new System.Windows.Forms.ComboBox();
            this.diskCountNumeric = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.okButton = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.cipherComboBox = new System.Windows.Forms.ComboBox();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.textBox3 = new System.Windows.Forms.TextBox();
            this.textBox4 = new System.Windows.Forms.TextBox();
            this.textBox5 = new System.Windows.Forms.TextBox();
            this.autoEncryptCheckBox = new System.Windows.Forms.CheckBox();
            this.label5 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.diskCountNumeric)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 101);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(135, 17);
            this.label1.TabIndex = 1;
            this.label1.Text = "Количество дисков";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(13, 69);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(69, 17);
            this.label2.TabIndex = 2;
            this.label2.Text = "Текстура";
            // 
            // textureComboBox
            // 
            this.textureComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.textureComboBox.FormattingEnabled = true;
            this.textureComboBox.Items.AddRange(new object[] {
            "Бумага",
            "Дерево",
            "Металл",
            "Бронза"});
            this.textureComboBox.Location = new System.Drawing.Point(161, 65);
            this.textureComboBox.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.textureComboBox.Name = "textureComboBox";
            this.textureComboBox.Size = new System.Drawing.Size(267, 24);
            this.textureComboBox.TabIndex = 3;
            // 
            // diskCountNumeric
            // 
            this.diskCountNumeric.Location = new System.Drawing.Point(161, 98);
            this.diskCountNumeric.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.diskCountNumeric.Minimum = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.diskCountNumeric.Name = "diskCountNumeric";
            this.diskCountNumeric.Size = new System.Drawing.Size(47, 22);
            this.diskCountNumeric.TabIndex = 4;
            this.diskCountNumeric.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.diskCountNumeric.ValueChanged += new System.EventHandler(this.diskCountNumeric_ValueChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(13, 142);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(125, 17);
            this.label3.TabIndex = 5;
            this.label3.Text = "Алфавиты дисков";
            // 
            // okButton
            // 
            this.okButton.Location = new System.Drawing.Point(259, 338);
            this.okButton.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(100, 28);
            this.okButton.TabIndex = 7;
            this.okButton.Text = "ОК";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(13, 34);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(77, 17);
            this.label4.TabIndex = 8;
            this.label4.Text = "Шифратор";
            // 
            // cipherComboBox
            // 
            this.cipherComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cipherComboBox.FormattingEnabled = true;
            this.cipherComboBox.Location = new System.Drawing.Point(161, 32);
            this.cipherComboBox.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.cipherComboBox.Name = "cipherComboBox";
            this.cipherComboBox.Size = new System.Drawing.Size(267, 24);
            this.cipherComboBox.TabIndex = 9;
            this.cipherComboBox.SelectedIndexChanged += new System.EventHandler(this.cipherComboBox_SelectedIndexChanged);
            // 
            // textBox1
            // 
            this.textBox1.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.textBox1.Location = new System.Drawing.Point(161, 139);
            this.textBox1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(351, 25);
            this.textBox1.TabIndex = 10;
            // 
            // textBox2
            // 
            this.textBox2.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.textBox2.Location = new System.Drawing.Point(161, 162);
            this.textBox2.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(351, 25);
            this.textBox2.TabIndex = 11;
            // 
            // textBox3
            // 
            this.textBox3.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.textBox3.Location = new System.Drawing.Point(161, 187);
            this.textBox3.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.textBox3.Name = "textBox3";
            this.textBox3.Size = new System.Drawing.Size(351, 25);
            this.textBox3.TabIndex = 12;
            // 
            // textBox4
            // 
            this.textBox4.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.textBox4.Location = new System.Drawing.Point(161, 210);
            this.textBox4.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.textBox4.Name = "textBox4";
            this.textBox4.Size = new System.Drawing.Size(351, 25);
            this.textBox4.TabIndex = 13;
            // 
            // textBox5
            // 
            this.textBox5.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.textBox5.Location = new System.Drawing.Point(161, 235);
            this.textBox5.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.textBox5.Name = "textBox5";
            this.textBox5.Size = new System.Drawing.Size(351, 25);
            this.textBox5.TabIndex = 14;
            // 
            // autoEncryptCheckBox
            // 
            this.autoEncryptCheckBox.AutoSize = true;
            this.autoEncryptCheckBox.Location = new System.Drawing.Point(161, 273);
            this.autoEncryptCheckBox.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.autoEncryptCheckBox.Name = "autoEncryptCheckBox";
            this.autoEncryptCheckBox.Size = new System.Drawing.Size(18, 17);
            this.autoEncryptCheckBox.TabIndex = 16;
            this.autoEncryptCheckBox.UseVisualStyleBackColor = true;
            this.autoEncryptCheckBox.CheckedChanged += new System.EventHandler(this.autoEncryptCheckBox_CheckedChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(12, 272);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(124, 17);
            this.label5.TabIndex = 17;
            this.label5.Text = "Автошифрование";
            // 
            // EditForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(875, 380);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.autoEncryptCheckBox);
            this.Controls.Add(this.textBox5);
            this.Controls.Add(this.textBox4);
            this.Controls.Add(this.textBox3);
            this.Controls.Add(this.textBox2);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.cipherComboBox);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.diskCountNumeric);
            this.Controls.Add(this.textureComboBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "EditForm";
            this.Text = "Изменить шифратор";
            this.Load += new System.EventHandler(this.Form2_Load);
            ((System.ComponentModel.ISupportInitialize)(this.diskCountNumeric)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox textureComboBox;
        private System.Windows.Forms.NumericUpDown diskCountNumeric;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox cipherComboBox;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.TextBox textBox3;
        private System.Windows.Forms.TextBox textBox4;
        private System.Windows.Forms.TextBox textBox5;
        private System.Windows.Forms.CheckBox autoEncryptCheckBox;
        private System.Windows.Forms.Label label5;
    }
}