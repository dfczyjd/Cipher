﻿using System;
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
            paper.texture = new Bitmap(Resources.paperTexture, CipherType.IMAGE_WIDTH, CipherType.IMAGE_HEIGHT);
            wood = new Material();
            wood.name = MaterialName.Wood;
            wood.pen = MainForm.woodPen;
            wood.brush = MainForm.woodPen.Brush;
            wood.texture = new Bitmap(Resources.woodTexture, CipherType.IMAGE_WIDTH, CipherType.IMAGE_HEIGHT);
            metal = new Material();
            metal.name = MaterialName.Metal;
            metal.pen = MainForm.metalPen;
            metal.brush = MainForm.metalPen.Brush;
            metal.texture = new Bitmap(Resources.metalTexture, CipherType.IMAGE_WIDTH, CipherType.IMAGE_HEIGHT);
            bronze = new Material();
            bronze.name = MaterialName.Bronze;
            bronze.pen = MainForm.bronzePen;
            bronze.brush = MainForm.bronzePen.Brush;
            bronze.texture = new Bitmap(Resources.bronzeTexture, CipherType.IMAGE_WIDTH, CipherType.IMAGE_HEIGHT);
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
        public const string CUSTOM_SETUP_NAME = "Пользовательский";
        const int DEFAULT_RING_COUNT = 6;

        public static CipherSetup[] builtInSetups;

        public string name;
        public Material material;
        public int ringCount;
        public string[] alphabets;
        public bool autoEncrypt;

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
                using (TextFieldParser parser = new TextFieldParser("../../config.csv", Encoding.UTF8))
                {
                    int setupCount = int.Parse(parser.ReadLine());
                    parser.SetDelimiters("\t", ";");
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
                                throw new FormatException("Материала под названием {data[1]} (строка {i}) не существует.");
                        }
                        setup.autoEncrypt = bool.Parse(data[2]);
                        setup.ringCount = int.Parse(data[3]) + 1;
                        setup.alphabets = new string[setup.ringCount];
                        for (int j = 1; j < setup.ringCount; ++j)
                            setup.alphabets[j] = data[j + 3];
                        builtInSetups[i] = setup;
                    }
                }
            }
            catch
            {
                builtInSetups = new CipherSetup[4];
                string[] alphs = new string[DEFAULT_RING_COUNT];
                for (int i = 0; i < DEFAULT_RING_COUNT; ++i)
                    alphs[i] = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
                builtInSetups[0] = createSetup(Material.paper, DEFAULT_RING_COUNT, alphs, true, "Бумажный тестовый");
                builtInSetups[1] = createSetup(Material.wood, DEFAULT_RING_COUNT, alphs, true, "Деревянный тестовый");
                builtInSetups[2] = createSetup(Material.metal, DEFAULT_RING_COUNT, alphs, true, "Металлический тестовый");
                builtInSetups[3] = createSetup(Material.bronze, DEFAULT_RING_COUNT, alphs, true, "Бронзовый тестовый");
                MessageBox.Show("Не удалось загрузить встроенный набор шифраторов из файла. Загружен стандартный набор.");
            }
        }

        public static CipherSetup getSetupByName(string name)
        {
            foreach (var setup in builtInSetups)
                if (setup.name == name)
                    return setup;
            throw new ArgumentException(string.Format("Встроенного шифратора с названием \"{0}\" не найдено", name));
        }

        public static CipherSetup createSetup(Material material, int ringCount, string[] alphabets,
                                                bool autoEncrypt, string name = CUSTOM_SETUP_NAME)
        {
            CipherSetup custom = new CipherSetup();
            custom.name = name;
            custom.material = material;
            custom.ringCount = ringCount;
            custom.alphabets = alphabets;
            custom.autoEncrypt = autoEncrypt;
            return custom;
        }

        public int findSymbol(int ringIndex, char symbol)
        {
            for (int i = 0; i < alphabets[ringIndex].Length; ++i)
                if (char.ToUpper(alphabets[ringIndex][i]) == char.ToUpper(symbol))
                    return i;
            return -1;
        }
    }

    public class CipherType
    {
        public const int RING_WIDTH = 50;
        public const int IMAGE_WIDTH = 700;
        public const int IMAGE_HEIGHT = 700;
        const int HALF_IMAGE_WIDTH = IMAGE_WIDTH / 2;
        const int HALF_IMAGE_HEIGHT = IMAGE_HEIGHT / 2;

        private MainForm owner;
        private Bitmap[] slices;
        private Bitmap window;
        private CipherSetup setup;
        public int encryptCount;

        public int highlightFromRing = -1, highlightFromCell = -1,
                   highlightToRing = -1, highlightToCell = -1;

        public CipherSetup Setup
        {
            get
            {
                return setup;
            }

            set
            {
                if (slices != null)
                {
                    foreach (var ring in slices)
                        ring.Dispose();
                }
                if (window != null)
                    window.Dispose();
                setup = value;
                window = new Bitmap((int)(RING_WIDTH * (setup.ringCount * 2 + 3f / 8)),
                                     (int)(RING_WIDTH * (setup.ringCount * 2 + 3f / 8)));
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
                g.FillRectangle(MainForm.white, new Rectangle(0, 0, bmp.Width, bmp.Height));
                g.FillEllipse(MainForm.black, new Rectangle(bmp.Width / 2 - r, bmp.Height / 2 - r, 2 * r, 2 * r));
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

        private Bitmap removeCircle(Bitmap bmp, int r)
        {
            Bitmap result = new Bitmap(bmp);
            using (Graphics g = Graphics.FromImage(result))
            {
                g.FillEllipse(MainForm.black, new Rectangle(bmp.Width / 2 - r, bmp.Height / 2 - r, 2 * r, 2 * r));
            }
            return result;
        }

        public void clearHighlight()
        {
            highlightFromCell = highlightFromRing = -1;
            highlightToCell = highlightToRing = -1;
        }

        private void highligthCell(int ringIndex, int cellIndex, Color color)
        {
            float delta = 180f / Setup.alphabets[ringIndex].Length,
                  angle = cellIndex * (360f / Setup.alphabets[ringIndex].Length);
            Bitmap result = new Bitmap(IMAGE_WIDTH, IMAGE_HEIGHT);
            using (Graphics g = Graphics.FromImage(result))
            {
                MainForm.rotate(g, angle, HALF_IMAGE_WIDTH, HALF_IMAGE_HEIGHT);
                MainForm.rotate(g, delta, HALF_IMAGE_WIDTH, HALF_IMAGE_HEIGHT);
                int inRadius = RING_WIDTH * ringIndex,
                    outRadius = inRadius + RING_WIDTH;
                Pen pen = new Pen(color, 2);
                g.DrawLine(pen, HALF_IMAGE_WIDTH, HALF_IMAGE_WIDTH - inRadius,
                                HALF_IMAGE_WIDTH, HALF_IMAGE_WIDTH - outRadius);
                MainForm.rotate(g, -2 * delta, HALF_IMAGE_WIDTH, HALF_IMAGE_HEIGHT);
                g.DrawLine(pen, HALF_IMAGE_WIDTH,
                                HALF_IMAGE_WIDTH - inRadius,
                                HALF_IMAGE_WIDTH, HALF_IMAGE_WIDTH - outRadius);
                MainForm.rotate(g, delta, HALF_IMAGE_WIDTH, HALF_IMAGE_HEIGHT);
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
                slices[i] = removeCircle(Setup.material.texture, RING_WIDTH * i);
                slices[i] = cutCircle(slices[i], RING_WIDTH * (i + 1));
            }
            cutWindow();
        }

        public void printSymbols(int ringIndex)
        {
            using (Graphics g = Graphics.FromImage(slices[ringIndex]))
            {
                for (int j = 0; j < Setup.alphabets[ringIndex].Length; ++j)
                {
                    float angle = 180.0F / Setup.alphabets[ringIndex].Length;
                    Font f = new Font("Consolas", ringIndex * 2 + 10, FontStyle.Bold);
                    float y = HALF_IMAGE_WIDTH - RING_WIDTH * (ringIndex + 0.5F), x = HALF_IMAGE_WIDTH;
                    g.DrawString(Setup.alphabets[ringIndex].Substring(j, 1), f, Setup.material.brush, x - f.SizeInPoints / 2, y - f.Height / 2);
                    MainForm.rotate(g, angle, HALF_IMAGE_WIDTH, HALF_IMAGE_HEIGHT);
                    g.DrawLine(new Pen(Setup.material.brush, 1), HALF_IMAGE_WIDTH, HALF_IMAGE_HEIGHT,
                        HALF_IMAGE_WIDTH, HALF_IMAGE_WIDTH - RING_WIDTH * (ringIndex + 1));
                    MainForm.rotate(g, angle, HALF_IMAGE_WIDTH, HALF_IMAGE_HEIGHT);
                }
            }
        }

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
                        MainForm.rotate(g, owner.flrot[i], HALF_IMAGE_WIDTH, HALF_IMAGE_HEIGHT);
                        g.DrawImage(slices[i], 0, 0);
                    }
                    res.DrawImage(turned, 0, 0);
                    turned.Dispose();
                }
                turned = new Bitmap(window.Width, window.Height);
                using (Graphics g = Graphics.FromImage(turned))
                {
                    MainForm.rotate(g, owner.windowRotation, window.Width / 2, window.Height / 2);
                    g.DrawImage(window, 0, 0);
                }
                res.DrawImage(turned, HALF_IMAGE_WIDTH - window.Width / 2, HALF_IMAGE_HEIGHT - window.Height / 2);
                turned.Dispose();
            }
            if (highlightFromRing != -1)
            {
                highligthCell(highlightFromRing, highlightFromCell, Color.Green);
                highligthCell(highlightToRing, highlightToCell, Color.Red);
            }
        }

        public static int mod(int left, int right)
        {
            return (left + right) % right;
        }

        public char encrypt(char c)
        {
            int alphabetLength = setup.alphabets[1].Length;
            int foundIndex = Setup.findSymbol(Setup.ringCount - 1, c);
            if (foundIndex == -1)
                return c;
            int diskIndex = (setup.ringCount - 2) - encryptCount;
            int position = mod(foundIndex + owner.rotations.Last(), alphabetLength) - owner.rotations[diskIndex];
            highlightFromRing = setup.ringCount - 1;
            highlightFromCell = mod(foundIndex + owner.rotations.Last(), alphabetLength);
            highlightToRing = diskIndex;
            highlightToCell = mod(position + owner.rotations[diskIndex], alphabetLength);
            encryptCount = mod(encryptCount + 1, setup.ringCount - 2);
            return setup.alphabets[diskIndex][mod(position, alphabetLength)];
        }

        public char decrypt(char c)
        {
            int alphabetLength = setup.alphabets[1].Length;
            int diskIndex = (setup.ringCount - 2) - encryptCount;
            int foundIndex = Setup.findSymbol(diskIndex, c);
            if (foundIndex == -1)
                return c;
            int position = mod(foundIndex + owner.rotations[diskIndex], alphabetLength) - owner.rotations.Last();
            highlightToRing = setup.ringCount - 1;
            highlightToCell = mod(position + owner.rotations.Last(), alphabetLength);
            highlightFromRing = diskIndex;
            highlightFromCell = mod(foundIndex + owner.rotations[diskIndex], alphabetLength);
            encryptCount = mod(encryptCount + 1, setup.ringCount - 2);
            return setup.alphabets.Last()[mod(position, alphabetLength)];
        }
    }
}
