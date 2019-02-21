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
        public MainForm owner;
        public CipherType cipher;

        public Form2()
        {
            InitializeComponent();
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            var alphs = alphabetsTextBox.Text.Split('\n');
            int newRingCount = (int)diskCountNumeric.Value;
            if (alphs.Length != newRingCount + 1)
            {
                MessageBox.Show("Количество алфавитов дисков должно быть равно количеству дисков");
                return;
            }
            string[] tmp = new string[newRingCount + 1];
            for (int i = 1; i < newRingCount; ++i)
                tmp[i] = alphs[i - 1].TrimEnd('\r');
            Material m = Material.getMaterialByName(textureComboBox.Text);
            owner.cipher.setup = CipherSetup.createCustomSetup(Material.getMaterialByName(textureComboBox.Text),
                                                               newRingCount + 1,
                                                               tmp);
            owner.updateRings();
            this.Close();
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            diskCountNumeric.Value = cipher.setup.ringCount - 1;
            //alphabetsTextBox.Text = string.Join("\n", owner.alphs, 1, cipher.setup.ringCount - 1);
            for (int i = 1; i < cipher.setup.ringCount; ++i)
                alphabetsTextBox.Text += owner.alphs[i] + "\n";
            textureComboBox.SelectedItem = cipher.setup.material.getName();
        }
    }
}
