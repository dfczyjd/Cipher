using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.VisualBasic.FileIO;
using Cipher.Properties;

namespace Cipher
{
    /// <summary>
    /// Перечисление материалов для модели
    /// </summary>
    public enum MaterialName
    {
        Paper, Wood, Metal, Bronze
    }

    /// <summary>
    /// Класс материала
    /// </summary>
    public class Material
    {
        public static Material paper, wood, metal, bronze;  // Варианты материала
        public MaterialName name;                           // Название
        public Bitmap texture;                              // Текстура
        public Pen pen;                                     // Объект типа Pen для отрисовки символов
        public Brush brush;                                 // Объект типа Brush для отрисовки границ

        /// <summary>
        /// Инициализация встроенных материалов
        /// </summary>
        static Material()
        {
            // Объекты типа Pen для инициализации материалов (параметры подобраны вручную)
            Pen paperPen = new Pen(Color.Black),
                metalPen = new Pen(Color.FromArgb(48, 48, 48), 3),
                woodPen = new Pen(Color.FromArgb(0x80, 0x40, 0x20), 3),
                bronzePen = new Pen(Color.FromArgb(115, 56, 28), 3);

            paper = new Material
            {
                name = MaterialName.Paper,
                pen = paperPen,
                brush = paperPen.Brush,
                texture = new Bitmap(Resources.paperTexture, CipherType.IMAGE_WIDTH, CipherType.IMAGE_HEIGHT)
            };
            wood = new Material
            {
                name = MaterialName.Wood,
                pen = woodPen,
                brush = woodPen.Brush,
                texture = new Bitmap(Resources.woodTexture, CipherType.IMAGE_WIDTH, CipherType.IMAGE_HEIGHT)
            };
            metal = new Material
            {
                name = MaterialName.Metal,
                pen = metalPen,
                brush = metalPen.Brush,
                texture = new Bitmap(Resources.metalTexture, CipherType.IMAGE_WIDTH, CipherType.IMAGE_HEIGHT)
            };
            bronze = new Material
            {
                name = MaterialName.Bronze,
                pen = bronzePen,
                brush = bronzePen.Brush,
                texture = new Bitmap(Resources.bronzeTexture, CipherType.IMAGE_WIDTH, CipherType.IMAGE_HEIGHT)
            };
        }

        /// <summary>
        /// Получить материал по названию
        /// </summary>
        /// <param name="RussianName">Название на русском языке</param>
        /// <returns>Материал с указанным названием</returns>
        public static Material getMaterialByName(string RussianName)
        {
            switch (RussianName)
            {
                case "Бумага":
                    return paper;

                case "Дерево":
                    return wood;

                case "Металл":
                    return metal;

                case "Бронза":
                    return bronze;

                default:
                    throw new InvalidEnumArgumentException(string.Format("Материал {0} не существует", RussianName));
            }
        }

        /// <summary>
        /// Получить название материала на русском языке
        /// </summary>
        /// <returns>Название материала на русском языке</returns>
        public string getName()
        {
            switch (name)
            {
                case MaterialName.Paper:
                    return "Бумага";

                case MaterialName.Wood:
                    return "Дерево";

                case MaterialName.Metal:
                    return "Металл";

                case MaterialName.Bronze:
                    return "Бронза";
            }
            throw new InvalidEnumArgumentException(string.Format("Материал {0} не существует", name));
        }
    }

    /// <summary>
    /// Класс параметров шифратора
    /// </summary>
    public class CipherSetup
    {
        // Название для пользовательских наборов параметров
        public const string CUSTOM_SETUP_NAME = "Пользовательский";

        // Массив загружаемых наборов параметров 
        public static CipherSetup[] loadedSetups;

        public string name;             // Название
        public Material material;       // Материал
        public int ringCount;           // Количество дисков
        public string[] alphabets;      // Алфавиты дисков
        public bool autoEncrypt;        // Наличие фенкции автошифрования

        /// <summary>
        /// Угловой размер прорези
        /// </summary>
        public float WindowAngle
        {
            get
            {
                return 240f / alphabets.Last().Length;
            }
        }

        /// <summary>
        /// Загрузка наборов параметров из файла
        /// </summary>
        static CipherSetup()
        {
            try
            {
                using (TextFieldParser parser = new TextFieldParser("config.csv", Encoding.UTF8))
                {
                    int setupCount = int.Parse(parser.ReadLine());
                    parser.SetDelimiters("\t", ";");
                    loadedSetups = new CipherSetup[setupCount];
                    for (int i = 0; i < setupCount; ++i)
                    {
                        string[] data = parser.ReadFields();
                        CipherSetup setup = new CipherSetup();
                        setup.name = data[0];
                        switch (data[1])
                        {
                            case "Бумага":
                                setup.material = Material.paper;
                                break;

                            case "Дерево":
                                setup.material = Material.wood;
                                break;

                            case "Металл":
                                setup.material = Material.metal;
                                break;

                            case "Бронза":
                                setup.material = Material.bronze;
                                break;

                            default:
                                throw new FormatException("Материала под названием {data[1]} (строка {i}) не существует.");
                        }
                        setup.autoEncrypt = bool.Parse(data[2]);
                        setup.ringCount = int.Parse(data[3]) + 1;
                        setup.alphabets = new string[setup.ringCount];
                        for (int j = 1; j < setup.ringCount; ++j)
                            setup.alphabets[j] = data[j + 3];
                        loadedSetups[i] = setup;
                    }
                }
            }
            catch   // Если произошла ошибка, установить тестовый набор
            {
                loadedSetups = new CipherSetup[4];
                int ringCount = 6;
                string[] alphs = new string[ringCount];
                for (int i = 0; i < ringCount; ++i)
                    alphs[i] = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
                loadedSetups[0] = createSetup(Material.paper, ringCount, alphs, true, "Бумажный тестовый");
                loadedSetups[1] = createSetup(Material.wood, ringCount, alphs, true, "Деревянный тестовый");
                loadedSetups[2] = createSetup(Material.metal, ringCount, alphs, true, "Металлический тестовый");
                loadedSetups[3] = createSetup(Material.bronze, ringCount, alphs, true, "Бронзовый тестовый");
                MessageBox.Show("Не удалось загрузить набор шифраторов из файла. Загружен встроенный набор.");
            }
        }

        /// <summary>
        /// Получить набор параметров по названию
        /// </summary>
        /// <param name="name">Название набора</param>
        /// <returns>Набор параметров</returns>
        public static CipherSetup getSetupByName(string name)
        {
            foreach (var setup in loadedSetups)
                if (setup.name == name)
                    return setup;
            throw new ArgumentException(string.Format("Встроенного шифратора с названием \"{0}\" не найдено", name));
        }

        /// <summary>
        /// Создать пользовательский набор параметров
        /// </summary>
        /// <param name="material">Материал</param>
        /// <param name="ringCount">Количество дисков</param>
        /// <param name="alphabets">Алфавиты дисков</param>
        /// <param name="autoEncrypt">Наличие функции автошифрования</param>
        /// <param name="name">Название</param>
        /// <returns>Пользвательский набор указанных параметров</returns>
        public static CipherSetup createSetup(Material material, int ringCount, string[] alphabets,
                                                bool autoEncrypt, string name = CUSTOM_SETUP_NAME)
        {
            CipherSetup custom = new CipherSetup
            {
                name = name,
                material = material,
                ringCount = ringCount,
                alphabets = alphabets,
                autoEncrypt = autoEncrypt
            };
            return custom;
        }

        /// <summary>
        /// Найти символ на диске шифратора
        /// </summary>
        /// <param name="ringIndex">Диск, на котором производится поиск</param>
        /// <param name="symbol">Символ для поиска</param>
        /// <returns>Индекс символа в алфавите диска или -1, если символ не найден</returns>
        public int findSymbol(int ringIndex, char symbol)
        {
            for (int i = 0; i < alphabets[ringIndex].Length; ++i)
                if (char.ToUpper(alphabets[ringIndex][i]) == char.ToUpper(symbol))
                    return i;
            return -1;
        }
    }

    /// <summary>
    /// Класс модели шифратора
    /// </summary>
    public class CipherType
    {
        public const int RING_WIDTH = 50;                           // толщина диска
        public const int IMAGE_WIDTH = 700;                         // ширина изображения модели
        public const int IMAGE_HEIGHT = 700;                        // высота изображения модели
        public const int HALF_IMAGE_WIDTH = IMAGE_WIDTH / 2;        // половина ширины изображения модели
        public const int HALF_IMAGE_HEIGHT = IMAGE_HEIGHT / 2;      // половина высоты изображения модели

        // Объесты типа Brush для создания "трафаретов" для удаления частей изображения
        static Brush BLACK_BRUSH = new Pen(Color.Black).Brush,
                    WHITE_BRUSH = new Pen(Color.White).Brush;

        private MainForm owner;         // форма, на которой отрисовывается модель
        private Bitmap[] rings;         // текстуры дисков
        private Bitmap window;          // текстура прорези
        private CipherSetup setup;      // набор параметров
        public int encryptCount;        // количество символов, прошедших шифрование

        // Индексы дисков и символов для подсветки
        public int highlightFromRing = -1, highlightFromCell = -1,
                   highlightToRing = -1, highlightToCell = -1;

        /// <summary>
        /// Свойство для работы с набором параметров
        /// </summary>
        public CipherSetup Setup
        {
            get
            {
                return setup;
            }

            set
            {
                if (rings != null)
                {
                    foreach (var ring in rings)
                        ring.Dispose();
                }
                if (window != null)
                    window.Dispose();
                setup = value;
                window = new Bitmap((int)(RING_WIDTH * (setup.ringCount * 2 + 3f / 8)),
                                     (int)(RING_WIDTH * (setup.ringCount * 2 + 3f / 8)));
                createWindow();
            }
        }

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="owner">форма-владелец</param>
        /// <param name="setup">Набор параметров модели</param>
        public CipherType(MainForm owner, CipherSetup setup)
        {
            this.owner = owner;
            this.Setup = setup;
        }

        /// <summary>
        /// Вырезать круг в центре из указанного изображения
        /// </summary>
        /// <param name="bmp">Исходное изображение</param>
        /// <param name="r">Радиус круга</param>
        /// <returns>Вырезанный круг</returns>
        private Bitmap cutCircle(Bitmap bmp, int r)
        {
            Bitmap result = new Bitmap(bmp),
                tmp = new Bitmap(bmp.Width, bmp.Height);
            // Создание "трафарета" для вырезания
            using (Graphics g = Graphics.FromImage(tmp))
            {
                g.FillRectangle(WHITE_BRUSH, new Rectangle(0, 0, bmp.Width, bmp.Height));
                g.FillEllipse(BLACK_BRUSH, new Rectangle(bmp.Width / 2 - r, bmp.Height / 2 - r, 2 * r, 2 * r));
            }
            tmp.MakeTransparent(Color.Black);
            using (Graphics g = Graphics.FromImage(result))
            {
                g.DrawImage(tmp, 0, 0);
                g.DrawEllipse(Setup.material.pen, new Rectangle(bmp.Width / 2 - r, bmp.Height / 2 - r, 2 * r, 2 * r));
            }
            result.MakeTransparent(Color.White);
            return result;
        }

        /// <summary>
        /// Закрасить круг в центре в указанном изображении чёрным
        /// </summary>
        /// <param name="bmp">Исходное изображение</param>
        /// <param name="r">Радиус круга</param>
        /// <returns>Изображение с закрашенным кругом</returns>
        private Bitmap removeCircle(Bitmap bmp, int r)
        {
            Bitmap result = new Bitmap(bmp);
            using (Graphics g = Graphics.FromImage(result))
            {
                g.FillEllipse(BLACK_BRUSH, new Rectangle(bmp.Width / 2 - r, bmp.Height / 2 - r, 2 * r, 2 * r));
            }
            return result;
        }

        /// <summary>
        /// Обнуление переменных, связанных с подсветкой ячеек
        /// </summary>
        public void clearHighlight()
        {
            highlightFromCell = highlightFromRing = -1;
            highlightToCell = highlightToRing = -1;
        }

        /// <summary>
        /// Повернуть изображение в объекте Graphics относительно заданной точки
        /// </summary>
        /// <param name="g">Объект Graphics</param>
        /// <param name="angle">Угол поворота</param>
        /// <param name="x">x-координата точки</param>
        /// <param name="y">y-координата точки</param>
        public static void rotate(Graphics g, float angle, int x, int y)
        {
            g.TranslateTransform(x, y);
            g.RotateTransform(angle);
            g.TranslateTransform(-x, -y);
        }

        /// <summary>
        /// Подсветить ячейку
        /// </summary>
        /// <param name="ringIndex">Индекс диска</param>
        /// <param name="cellIndex">Индекс ячейки</param>
        /// <param name="color">Цвет</param>
        private void highligthCell(int ringIndex, int cellIndex, Color color)
        {
            float delta = 180f / Setup.alphabets[ringIndex].Length,
                  angle = cellIndex * (360f / Setup.alphabets[ringIndex].Length);
            Bitmap result = new Bitmap(IMAGE_WIDTH, IMAGE_HEIGHT);
            using (Graphics g = Graphics.FromImage(result))
            {
                rotate(g, angle, HALF_IMAGE_WIDTH, HALF_IMAGE_HEIGHT);
                rotate(g, delta, HALF_IMAGE_WIDTH, HALF_IMAGE_HEIGHT);
                int inRadius = RING_WIDTH * ringIndex,
                    outRadius = inRadius + RING_WIDTH;
                Pen pen = new Pen(color, 2);
                g.DrawLine(pen, HALF_IMAGE_WIDTH, HALF_IMAGE_WIDTH - inRadius,
                                HALF_IMAGE_WIDTH, HALF_IMAGE_WIDTH - outRadius);
                rotate(g, -2 * delta, HALF_IMAGE_WIDTH, HALF_IMAGE_HEIGHT);
                g.DrawLine(pen, HALF_IMAGE_WIDTH,
                                HALF_IMAGE_WIDTH - inRadius,
                                HALF_IMAGE_WIDTH, HALF_IMAGE_WIDTH - outRadius);
                rotate(g, delta, HALF_IMAGE_WIDTH, HALF_IMAGE_HEIGHT);
                pen.Width = 5;
                g.DrawArc(pen, HALF_IMAGE_WIDTH - inRadius, HALF_IMAGE_HEIGHT - inRadius,
                            2 * inRadius, 2 * inRadius, -90 - delta, 2 * delta);
                g.DrawArc(pen, HALF_IMAGE_WIDTH - outRadius, HALF_IMAGE_HEIGHT - outRadius,
                            2 * outRadius, 2 * outRadius, -90 - delta, 2 * delta);
                
            }
            using (Graphics g = Graphics.FromImage(owner.output))
            {
                g.DrawImage(result, 0, 0);
            }
        }

        /// <summary>
        /// Создать текстуру прорези
        /// </summary>
        private void createWindow()
        {
            float delta = Setup.WindowAngle;
            using (Graphics g = Graphics.FromImage(window))
            {
                g.FillPie(Setup.material.brush, 0, 0, window.Width, window.Height, -90 - delta, 2 * delta);
                g.FillPie(WHITE_BRUSH, 20, 10, window.Width - 40, window.Height - 40, -90 - delta * 2 / 3, delta * 4 / 3);
                window.MakeTransparent(Color.White);
            }
        }

        /// <summary>
        /// Заполнить массив текстур дисков
        /// </summary>
        public void createRings()
        {
            rings = new Bitmap[Setup.ringCount];
            for (int i = 0; i < Setup.ringCount; ++i)
            {
                rings[i] = removeCircle(Setup.material.texture, RING_WIDTH * i);
                rings[i] = cutCircle(rings[i], RING_WIDTH * (i + 1));
            }
            createWindow();
        }

        /// <summary>
        /// Вывести символы алфавитов на текстуру диска
        /// </summary>
        /// <param name="ringIndex">Индекс диска</param>
        public void printSymbols(int ringIndex)
        {
            using (Graphics g = Graphics.FromImage(rings[ringIndex]))
            {
                for (int j = 0; j < Setup.alphabets[ringIndex].Length; ++j)
                {
                    // Повернуть диск нужной ячейкой вверх и вывести символ в неё
                    float angle = 180.0F / Setup.alphabets[ringIndex].Length;
                    Font f = new Font("Consolas", ringIndex * 2 + 10, FontStyle.Bold);
                    float y = HALF_IMAGE_WIDTH - RING_WIDTH * (ringIndex + 0.5F), x = HALF_IMAGE_WIDTH;
                    g.DrawString(Setup.alphabets[ringIndex].Substring(j, 1), f, Setup.material.brush, x - f.SizeInPoints / 2, y - f.Height / 2);
                    rotate(g, angle, HALF_IMAGE_WIDTH, HALF_IMAGE_HEIGHT);
                    g.DrawLine(new Pen(Setup.material.brush, 1), HALF_IMAGE_WIDTH, HALF_IMAGE_HEIGHT,
                        HALF_IMAGE_WIDTH, HALF_IMAGE_WIDTH - RING_WIDTH * (ringIndex + 1));
                    rotate(g, angle, HALF_IMAGE_WIDTH, HALF_IMAGE_HEIGHT);
                }
            }
        }

        /// <summary>
        /// Обновить текстуры дисков
        /// </summary>
        public void refreshRings()
        {
            Bitmap turned;
            using (Graphics res = Graphics.FromImage(owner.output))
            {
                res.Clear(Color.Transparent);
                for (int i = Setup.ringCount - 1; i >= 0; --i)
                {
                    turned = new Bitmap(IMAGE_WIDTH, IMAGE_HEIGHT);
                    using (Graphics g = Graphics.FromImage(turned))
                    {
                        // Повернуть диск и отрисовать его 
                        rotate(g, owner.degreeRotations[i], HALF_IMAGE_WIDTH, HALF_IMAGE_HEIGHT);
                        g.DrawImage(rings[i], 0, 0);
                    }
                    res.DrawImage(turned, 0, 0);
                    turned.Dispose();
                }
                // Отрисовать прорезь
                res.DrawImage(window, HALF_IMAGE_WIDTH - window.Width / 2, HALF_IMAGE_HEIGHT - window.Height / 2);
            }
            // Подсветить ячейки, если требуется
            if (highlightFromRing != -1)
            {
                highligthCell(highlightFromRing, highlightFromCell, Color.Green);
                highligthCell(highlightToRing, highlightToCell, Color.Red);
            }
        }

        /// <summary>
        /// Операция % для отрицательных чисел
        /// </summary>
        /// <param name="left">Левый операнд</param>
        /// <param name="right">Правый операнд</param>
        /// <returns>Результат операции</returns>
        public static int mod(int left, int right)
        {
            return (left % right + right) % right;
        }

        /// <summary>
        /// Зашифровать символ
        /// </summary>
        /// <param name="c">Символ для шифрования</param>
        /// <returns>Результат шифрования</returns>
        public char encrypt(char c)
        {
            int alphabetLength = setup.alphabets[1].Length;
            int foundIndex = Setup.findSymbol(Setup.ringCount - 1, c);  // Найти символ открытого текста в алфавите
            if (foundIndex == -1)
                return c;
            int diskIndex = (setup.ringCount - 2) - encryptCount;   // Индекс диска для символа шифротекста
            // Вычислить расстояние от прорези до ячейки с символом шифротекста
            int position = mod(foundIndex + owner.cellRotations.Last(), alphabetLength) - owner.cellRotations[diskIndex];
            // Подсветить ячейки с обоими символами
            highlightFromRing = setup.ringCount - 1;
            highlightFromCell = mod(foundIndex + owner.cellRotations.Last(), alphabetLength);
            highlightToRing = diskIndex;
            highlightToCell = mod(position + owner.cellRotations[diskIndex], alphabetLength);
            // Изменить счётчик зашифрованных символов
            encryptCount = mod(encryptCount + 1, setup.ringCount - 2);
            // Вернуть результат
            return setup.alphabets[diskIndex][mod(position, alphabetLength)];
        }

        /// <summary>
        /// Дешифровать символ
        /// </summary>
        /// <param name="c">Символ для дешифрования</param>
        /// <returns>Результат дешифрования</returns>
        public char decrypt(char c)
        {
            int alphabetLength = setup.alphabets[1].Length;
            int diskIndex = (setup.ringCount - 2) - encryptCount;
            int foundIndex = Setup.findSymbol(diskIndex, c);    // Найти символ шифротекста в алфавите
            if (foundIndex == -1)
                return c;
            // Вычислить расстояние от прорези до ячейки с символом открытого текста
            int position = mod(foundIndex + owner.cellRotations[diskIndex], alphabetLength) - owner.cellRotations.Last();
            // Подсветить ячейки с обоими символами
            highlightToRing = setup.ringCount - 1;
            highlightToCell = mod(position + owner.cellRotations.Last(), alphabetLength);
            highlightFromRing = diskIndex;
            highlightFromCell = mod(foundIndex + owner.cellRotations[diskIndex], alphabetLength);
            // Изменить счётчик дешифрованных символов
            encryptCount = mod(encryptCount + 1, setup.ringCount - 2);
            // Вернуть результат
            return setup.alphabets.Last()[mod(position, alphabetLength)];
        }
    }
}
