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
    /// <summary>
    /// Класс формы окна исторической справки
    /// </summary>
    public partial class HistoryForm : Form
    {
        /// <summary>
        /// Класс статьи справки
        /// </summary>
        class Help
        {
            public string title,    // Заголовок
                        content;    // Содержание статьи

            /// <summary>
            /// Конструктор
            /// </summary>
            /// <param name="title">Заголовок</param>
            /// <param name="content">Содержание статьи</param>
            public Help(string title, string content)
            {
                this.title = title;
                this.content = content;
            }
        }

        private Help[] helpTexts;                                           // Статьи справки
        private Help defaultText = new Help("Ошибка", "Ошибка загрузки");   // Статья по умолчанию

        /// <summary>
        /// Получить статью по заголовку
        /// </summary>
        /// <param name="title">Заголовок</param>
        /// <returns>Статья с указанным заголовком</returns>
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
            helpTexts = new Help[8];
            helpTexts[0] = new Help("Диск Альберти", Resources.Диск_Альберти);
            helpTexts[1] = new Help("Диск Д.Водсворда", Resources.Диск_Д_Водсворда);
            helpTexts[2] = new Help("Диски Ч.Уитстона и Плетта", Resources.Диски_Ч_Уитстона_и_Плетта);
            helpTexts[3] = new Help("Устройство Дж.Фонтаны", Resources.Устройство_Дж_Фонтаны);
            helpTexts[4] = new Help("Диск Болтона", Resources.Диск_Болтона);
            helpTexts[5] = new Help("Машина фон Крихи", Resources.Машина_фон_Крихи);
            helpTexts[6] = new Help("Диск А.Майера", Resources.Диск_А_Майера);
            helpTexts[7] = new Help("Датские часы-шифратор", Resources.Датские_часы_шифратор);

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
