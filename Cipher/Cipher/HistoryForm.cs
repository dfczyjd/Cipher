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
        private Help defaultText = new Help("Ошибка", "Ошибка загрузки");

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
            helpTexts = new Help[7];
            helpTexts[0] = new Help("Code-O-Graph", Resources.Code_O_Graph);
            helpTexts[1] = new Help("Диск Альберти", Resources.Диск_Альберти);
            helpTexts[2] = new Help("Диск Д.Водсворда", Resources.Диск_Д_Водсворда);
            helpTexts[3] = new Help("Диски Ч.Уитстона и Плетта", Resources.Диски_Ч_Уитстона_и_Плетта);
            helpTexts[4] = new Help("Диск Г.Шотта", Resources.Диск_Г_Шотта);
            helpTexts[5] = new Help("Диск Дж.Фонтана", Resources.Устройство_Дж_Фонтана);
            helpTexts[6] = new Help("Шифратор Ч.Беббиджа", Resources.Шифратор_Ч_Бэббиджа);

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
