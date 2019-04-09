using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Cipher.Properties;
using System.IO;

namespace Cipher
{
    public partial class HistoryForm : Form
    {
        class Help
        {
            public string title, content;

            public Help(string title, string content)
            {
                this.title = title;
                this.content = content;
            }
        }

        private Help[] helpTexts;
        private Help defaultText;

        private Help GetHelpByTitle(string title)
        {
            foreach (var text in helpTexts)
                if (text.title == title)
                    return text;
            return defaultText;
        }

        public HistoryForm()
        {
            InitializeComponent();
        }

        private void HistoryForm_Load(object sender, EventArgs e)
        {
            /*contentTreeView.Nodes.Add("Заголовок 1");
            contentTreeView.Nodes.Add("Заголовок 2");

            helpTexts = new Help[2];
            helpTexts[0] = new Help("Заголовок 1", "Текст 1");
            helpTexts[1] = new Help("Заголовок 2", "Текст 2");

            defaultText = new Help("", "Текст по умолчанию");*/
            helpTexts = new Help[2];
            helpTexts[0] = new Help("Заголовок 1", Resources.Заголовок_1);
            helpTexts[1] = new Help("Заголовок 2", Resources.Заголовок_2);

            foreach (var elem in helpTexts)
                contentTreeView.Nodes.Add(elem.title);
        }

        private void contentTreeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            Help text = GetHelpByTitle(e.Node.Text);
            textBox.ReadOnly = false;
            Clipboard.SetText(text.content, TextDataFormat.Rtf);
            textBox.Clear();
            textBox.Paste();
            textBox.ReadOnly = true;
        }
    }
}
