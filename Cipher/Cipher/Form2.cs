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
            string[] tmp = new string[newRingCount + 1];
            for (int i = 1; i < owner.ringCount; ++i)
                tmp[i] = alphs[i - 1].TrimEnd('\r');
            owner.cipher.setup = CipherSetup.createCustomSetup(Material.createMaterialByName(textureComboBox.Text),
                                                               newRingCount + 1,
                                                               tmp);
            owner.updateRings();
            this.Close();
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            diskCountNumeric.Value = owner.ringCount - 1;
            alphabetsTextBox.Text = string.Join("\n", owner.alphs, 1, owner.ringCount - 1);
            /*if (owner.cipher.setup.material == MaterialName.Paper)
                textureComboBox.SelectedIndex = 0;
            else if (owner.cipher.material == MaterialName.Wood)
                textureComboBox.SelectedIndex = 1;
            else if (owner.cipher.material == MaterialName.Metal)
                textureComboBox.SelectedIndex = 2;
            else
                textureComboBox.SelectedIndex = 3;*/
            textureComboBox.SelectedItem = owner.cipher.setup.material.getName();
        }
    }
}
