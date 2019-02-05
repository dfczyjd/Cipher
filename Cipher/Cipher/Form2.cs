using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Cipher
{
    public partial class Form2 : Form
    {
        public Form1 owner;

        public Form2()
        {
            InitializeComponent();
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            var alphs = alphabetsTextBox.Text.Split('\n');
            int newRingCount = (int)diskCountNumeric.Value;
            if (alphs.Length != newRingCount)
            {
                MessageBox.Show("Количество алфавитов дисков должно быть равно количеству дисков");
                return;
            }
            owner.ringCount = newRingCount + 1;
            owner.selectTexture((string)textureComboBox.SelectedItem);
            for (int i = 1; i < owner.ringCount; ++i)
                owner.alphs[i] = alphs[i - 1];
            owner.updateRings();
            this.Close();
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            diskCountNumeric.Value = owner.ringCount - 1;
            alphabetsTextBox.Text = string.Join("\n", owner.alphs, 1, owner.ringCount - 1);
            if (owner.cipher is Paper)
                textureComboBox.SelectedIndex = 0;
            else if (owner.cipher is Wood_old)
                textureComboBox.SelectedIndex = 1;
            else
                textureComboBox.SelectedIndex = 2;
        }
    }
}
