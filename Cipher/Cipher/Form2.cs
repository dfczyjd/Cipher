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

        private TextBox[] alphabets;

        public Form2()
        {
            InitializeComponent();

            alphabets = new TextBox[Constants.MAX_RING_COUNT];
            alphabets[0] = textBox1;
            alphabets[1] = textBox2;
            alphabets[2] = textBox3;
            alphabets[3] = textBox4;
            alphabets[4] = textBox5;
            alphabets[5] = textBox6;
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            if (cipherComboBox.SelectedIndex == CipherSetup.builtInSetups.Length)
            {
                int newRingCount = (int)diskCountNumeric.Value;
                string[] tmp = new string[newRingCount + 1];
                for (int i = 1; i < newRingCount + 1; ++i)
                {
                    tmp[i] = alphabets[i - 1].Text;
                    if (i > 1 && tmp[i].Length != tmp[1].Length)
                    {
                        MessageBox.Show("Алфавиты должны быть одной длины.");
                        return;
                    }
                }
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
            for (int i = 1; i < setup.ringCount; ++i)
                alphabets[i - 1].Text = setup.alphabets[i];
            //alphabetsTextBox.Text = string.Join("\n", setup.alphabets, 1, setup.ringCount - 1);
            textureComboBox.SelectedItem = setup.material.getName();
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            diskCountNumeric.Maximum = Constants.MAX_RING_COUNT;

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
                foreach (var elem in alphabets)
                    elem.Enabled = true;
                //alphabetsTextBox.Enabled = true;
            }
            else
            {
                textureComboBox.Enabled = false;
                diskCountNumeric.Enabled = false;
                foreach (var elem in alphabets)
                    elem.Enabled = false;
                //alphabetsTextBox.Enabled = false;
                loadParams(CipherSetup.builtInSetups[cipherComboBox.SelectedIndex]);
            }
        }

        private void diskCountNumeric_ValueChanged(object sender, EventArgs e)
        {
            for (int i = 0; i < alphabets.Length; ++i)
                alphabets[i].Visible = (i < diskCountNumeric.Value);
        }
    }
}
