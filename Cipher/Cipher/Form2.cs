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
            if (cipherComboBox.SelectedIndex == CipherSetup.builtInSetups.Length)
            {
                var alphs = alphabetsTextBox.Text.Split('\n');
                int newRingCount = (int)diskCountNumeric.Value;
                if (alphs.Length != newRingCount)
                {
                    MessageBox.Show("Количество алфавитов дисков должно быть равно количеству дисков");
                    return;
                }
                string[] tmp = new string[newRingCount + 1];
                for (int i = 1; i < newRingCount + 1; ++i)
                    tmp[i] = alphs[i - 1].TrimEnd('\r');
                cipher.setup = CipherSetup.createSetup(Material.getMaterialByName(textureComboBox.Text),
                                                               newRingCount + 1,
                                                               tmp);
            }
            else
                cipher.setup = CipherSetup.builtInSetups[cipherComboBox.SelectedIndex];
            owner.updateRings();
            this.Close();
        }

        private void loadParams(CipherSetup setup)
        {
            diskCountNumeric.Value = setup.ringCount - 1;
            alphabetsTextBox.Text = string.Join("\n", setup.alphabets, 1, setup.ringCount - 1);
            textureComboBox.SelectedItem = setup.material.getName();
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            cipherComboBox.Items.Clear();
            foreach (var setup in CipherSetup.builtInSetups)
                cipherComboBox.Items.Add(setup.name);
            cipherComboBox.Items.Add(Constants.CUSTOM_SETUP_NAME);
            loadParams(cipher.setup);
            cipherComboBox.SelectedItem = cipher.setup.name;
        }

        private void cipherComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cipherComboBox.SelectedIndex == CipherSetup.builtInSetups.Length)
            {
                textureComboBox.Enabled = true;
                diskCountNumeric.Enabled = true;
                alphabetsTextBox.Enabled = true;
            }
            else
            {
                textureComboBox.Enabled = false;
                diskCountNumeric.Enabled = false;
                alphabetsTextBox.Enabled = false;
                loadParams(CipherSetup.builtInSetups[cipherComboBox.SelectedIndex]);
            }
        }
    }
}
