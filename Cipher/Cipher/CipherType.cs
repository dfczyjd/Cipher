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
    public enum MaterialName
    {
        Paper, Wood, Metal, Bronze
    }

    public class Material
    {
        public static Material paper, wood, metal, bronze;
        public MaterialName name;
        public Bitmap texture;
        public Pen pen;
        public Brush brush;

        static Material()
        {
            paper = new Material();
            paper.name = MaterialName.Paper;
            paper.pen = MainForm.paperPen;
            paper.brush = MainForm.paperPen.Brush;
            paper.texture = new Bitmap(Resources.paperTexture, Constants.IMAGE_WIDTH, Constants.IMAGE_HEIGHT);
            wood = new Material();
            wood.name = MaterialName.Wood;
            wood.pen = MainForm.woodPen;
            wood.brush = MainForm.woodPen.Brush;
            wood.texture = new Bitmap(Resources.woodTexture, Constants.IMAGE_WIDTH, Constants.IMAGE_HEIGHT);
            metal = new Material();
            metal.name = MaterialName.Metal;
            metal.pen = MainForm.metalPen;
            metal.brush = MainForm.metalPen.Brush;
            metal.texture = new Bitmap(Resources.metalTexture, Constants.IMAGE_WIDTH, Constants.IMAGE_HEIGHT);
            bronze = new Material();
            bronze.name = MaterialName.Bronze;
            bronze.pen = MainForm.bronzePen;
            bronze.brush = MainForm.bronzePen.Brush;
            bronze.texture = new Bitmap(Resources.bronzeTexture, Constants.IMAGE_WIDTH, Constants.IMAGE_HEIGHT);
        }

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

    public class CipherSetup
    {
        public static CipherSetup[] builtInSetups;

        public string name;
        public Material material;
        public int ringCount;
        public string[] alphabets;

        public float WindowAngle
        {
            get
            {
                return 240f / alphabets[1].Length;
            }
        }

        static CipherSetup()
        {
            // TODO: настроить путь
            try
            {
                using (TextFieldParser parser = new TextFieldParser("../../config.csv"))
                {
                    int setupCount = int.Parse(parser.ReadLine());
                    parser.SetDelimiters(";");
                    builtInSetups = new CipherSetup[setupCount];
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
                                throw new FormatException($"Материала под названием {data[1]} (строка {i}) не существует.");
                        }
                        setup.ringCount = int.Parse(data[2]);
                        setup.alphabets = new string[setup.ringCount];
                        for (int j = 1; j < setup.ringCount; ++j)
                            setup.alphabets[j] = data[j + 2];
                        builtInSetups[i] = setup;
                    }
                }
            }
            catch
            {
                builtInSetups = new CipherSetup[4];
                string[] alphs = new string[Constants.DEFAULT_RING_COUNT];
                for (int i = 0; i < Constants.DEFAULT_RING_COUNT; ++i)
                    alphs[i] = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
                builtInSetups[0] = createSetup(Material.paper, Constants.DEFAULT_RING_COUNT, alphs, "Бумажный тестовый");
                builtInSetups[1] = createSetup(Material.wood, Constants.DEFAULT_RING_COUNT, alphs, "Деревянный тестовый");
                builtInSetups[2] = createSetup(Material.metal, Constants.DEFAULT_RING_COUNT, alphs, "Металлический тестовый");
                builtInSetups[3] = createSetup(Material.bronze, Constants.DEFAULT_RING_COUNT, alphs, "Бронзовый тестовый");
                MessageBox.Show("Не удалось загрузить встроенный набор шифраторов из файла. Загружен встроенный набор.");
            }
        }

        public static CipherSetup getSetupByName(string name)
        {
            foreach (var setup in builtInSetups)
                if (setup.name == name)
                    return setup;
            throw new ArgumentException(string.Format("Встроенного шифратора с названием \"{0}\" не найдено", name));
        }

        public static CipherSetup createSetup(Material material, int ringCount, string[] alphabets, string name = Constants.CUSTOM_SETUP_NAME)
        {
            CipherSetup custom = new CipherSetup();
            custom.name = name;
            custom.material = material;
            custom.ringCount = ringCount;
            custom.alphabets = alphabets;
            return custom;
        }
    }

    public class CipherType
    {
        private MainForm owner;
        private Bitmap[] slices;
        private Bitmap window;
        private CipherSetup setup;

        public CipherSetup Setup
        {
            get
            {
                return setup;
            }

            set
            {
                setup = value;
                window = new Bitmap((int)(Constants.RING_WIDTH * (setup.ringCount * 2 + 2f / 3)),
                                     (int)(Constants.RING_WIDTH * (setup.ringCount * 2 + 2f / 3)));
                cutWindow();
            }
        }

        public CipherType(MainForm owner, CipherSetup setup)
        {
            this.owner = owner;
            this.Setup = setup;
        }

        private Bitmap cutCircle(Bitmap bmp, int r)
        {
            Bitmap result = new Bitmap(bmp),
                tmp = new Bitmap(bmp.Width, bmp.Height);
            using (Graphics g = Graphics.FromImage(tmp))
            {
                g.FillRectangle(MainForm.white, new Rectangle(0, 0, tmp.Width, tmp.Height));
                g.FillEllipse(MainForm.black, new Rectangle(bmp.Width / 2 - r, bmp.Height / 2 - r, 2 * r, 2 * r));
                tmp.MakeTransparent(Color.Black);
            }
            using (Graphics g = Graphics.FromImage(result))
            {
                g.DrawImage(tmp, 0, 0);
                g.DrawEllipse(Setup.material.pen, new Rectangle(bmp.Width / 2 - r, bmp.Height / 2 - r, 2 * r, 2 * r));
            }
            result.MakeTransparent(Color.White);
            return result;
        }

        private Bitmap removeCircle(Bitmap bmp, int r)
        {
            Bitmap result = new Bitmap(bmp);
            using (Graphics g = Graphics.FromImage(result))
            {
                g.FillEllipse(MainForm.black, new Rectangle(bmp.Width / 2 - r, bmp.Height / 2 - r, 2 * r, 2 * r));
            }
            return result;
        }

        private void cutWindow()
        {
            float delta = Setup.WindowAngle;
            using (Graphics g = Graphics.FromImage(window))
            {
                g.FillPie(Setup.material.brush, 0, 0, window.Width, window.Height, -90 - delta, 2 * delta);
                g.FillPie(MainForm.white, 20, 10, window.Width - 40, window.Height - 40, -90 - delta * 2 / 3, delta * 4 / 3);
                window.MakeTransparent(Color.White);
            }
        }

        public void slice()
        {
            slices = new Bitmap[Setup.ringCount];
            for (int i = 0; i < Setup.ringCount; ++i)
            {
                slices[i] = removeCircle(Setup.material.texture, Constants.RING_WIDTH * i);
                slices[i] = cutCircle(slices[i], Constants.RING_WIDTH * (i + 1));
            }
            cutWindow();
        }

        public void inscribe(int ringIndex)
        {
            using (Graphics g = Graphics.FromImage(slices[ringIndex]))
            {
                for (int j = 0; j < Setup.alphabets[ringIndex].Length; ++j)
                {
                    float angle = 180.0F / Setup.alphabets[ringIndex].Length;
                    Font f = new Font("Consolas", ringIndex * 2 + 6);
                    float y = Constants.HALF_IMAGE_WIDTH - Constants.RING_WIDTH * (ringIndex + 0.5F), x = Constants.HALF_IMAGE_WIDTH;
                    g.DrawString(Setup.alphabets[ringIndex].Substring(j, 1), f, Setup.material.brush, x - f.SizeInPoints / 2, y - f.Height / 2);
                    MainForm.rotate(g, angle, Constants.HALF_IMAGE_WIDTH, Constants.HALF_IMAGE_HEIGHT);
                    g.DrawLine(new Pen(Setup.material.brush, 1), Constants.HALF_IMAGE_WIDTH, Constants.HALF_IMAGE_HEIGHT,
                        Constants.HALF_IMAGE_WIDTH, Constants.HALF_IMAGE_WIDTH - Constants.RING_WIDTH * (ringIndex + 1));
                    MainForm.rotate(g, angle, Constants.HALF_IMAGE_WIDTH, Constants.HALF_IMAGE_HEIGHT);
                }
            }
        }

        static int iterationCount = 0;

        public void refreshRings()
        {
            Bitmap turned;
            using (Graphics res = Graphics.FromImage(owner.output))
            {
                res.FillRectangle(MainForm.white, new Rectangle(new Point(0, 0), owner.output.Size));
                for (int i = Setup.ringCount - 1; i >= 0; --i)
                {
                    turned = new Bitmap(Constants.IMAGE_WIDTH, Constants.IMAGE_HEIGHT);
                    using (Graphics g = Graphics.FromImage(turned))
                    {
                        MainForm.rotate(g, owner.flrot[i], Constants.HALF_IMAGE_WIDTH, Constants.HALF_IMAGE_HEIGHT);
                        g.DrawImage(slices[i], 0, 0);
                    }
                    res.DrawImage(turned, owner.center.X - Constants.HALF_IMAGE_WIDTH, owner.center.Y - Constants.HALF_IMAGE_HEIGHT);
                }
                turned = new Bitmap(window.Width, window.Height);
                using (Graphics g = Graphics.FromImage(turned))
                {
                    MainForm.rotate(g, owner.windowRotation, window.Width / 2, window.Height / 2);
                    g.DrawImage(window, 0, 0);
                }
                res.DrawImage(turned, Constants.HALF_IMAGE_WIDTH - window.Width / 2, Constants.HALF_IMAGE_HEIGHT - window.Height / 2);
            }
            ++iterationCount;
            if (iterationCount % 100 == 0)
                GC.Collect();
        }
    }
}
