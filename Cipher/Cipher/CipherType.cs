using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Cipher.Properties;

namespace Cipher
{
    public enum MaterialName
    {
        Paper, Wood, Metal, Bronze
    }

    public class Material
    {
        const int WIDTH = 400, HEIGHT = 400;

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
            paper.texture = new Bitmap(Resources.paperTexture, WIDTH, HEIGHT);
            wood = new Material();
            wood.name = MaterialName.Wood;
            wood.pen = MainForm.woodPen;
            wood.brush = MainForm.woodPen.Brush;
            wood.texture = new Bitmap(Resources.woodTexture, WIDTH, HEIGHT);
            metal = new Material();
            metal.name = MaterialName.Metal;
            metal.pen = MainForm.metalPen;
            metal.brush = MainForm.metalPen.Brush;
            metal.texture = new Bitmap(Resources.metalTexture, WIDTH, HEIGHT);
            bronze = new Material();
            bronze.name = MaterialName.Bronze;
            bronze.pen = MainForm.bronzePen;
            bronze.brush = MainForm.bronzePen.Brush;
            bronze.texture = new Bitmap(Resources.bronzeTexture, WIDTH, HEIGHT);
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
        public Material material;
        public int ringCount;
        public string[] alphabets;

        public static CipherSetup createCustomSetup(Material material, int ringCount, string[] alphabets)
        {
            CipherSetup custom = new CipherSetup();
            custom.material = material;
            custom.ringCount = ringCount;
            custom.alphabets = alphabets;
            return custom;
        }
    }

    public class CipherType
    {
        const int WIDTH = 400, HEIGHT = 400;

        protected MainForm owner;
        protected Bitmap[] slices;
        public CipherSetup setup;

        public CipherType(MainForm owner, CipherSetup setup)
        {
            this.owner = owner;
            this.setup = setup;
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
                g.DrawEllipse(setup.material.pen, new Rectangle(bmp.Width / 2 - r, bmp.Height / 2 - r, 2 * r, 2 * r));
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

        public virtual void slice()
        {
            slices = new Bitmap[setup.ringCount];
            owner.ringWidth = 30;// bmp.Width / pieces / 2;
            for (int i = 0; i < setup.ringCount; ++i)
            {
                slices[i] = removeCircle(setup.material.texture, owner.ringWidth * i);
                slices[i] = cutCircle(slices[i], owner.ringWidth * (i + 1));
            }
        }

        public virtual void inscribe(int ringIndex)
        {
            using (Graphics g = Graphics.FromImage(slices[ringIndex]))
            {
                for (int j = 0; j < owner.alphs[ringIndex].Length; ++j)
                {
                    float angle = 180.0F / owner.alphs[ringIndex].Length;
                    Font f = new Font("Times New Roman", ringIndex * 2 + 6);
                    float y = WIDTH / 2 - owner.ringWidth * (ringIndex + 0.5F), x = WIDTH / 2;
                    g.DrawString(owner.alphs[ringIndex].Substring(j, 1), f, setup.material.brush, x - f.SizeInPoints / 2, y - f.Height / 2);
                    MainForm.rotate(g, angle, WIDTH / 2, HEIGHT / 2);
                    g.DrawLine(new Pen(setup.material.brush, 1), WIDTH / 2, HEIGHT / 2, WIDTH / 2, WIDTH / 2 - owner.ringWidth * (ringIndex + 1));
                    MainForm.rotate(g, angle, WIDTH / 2, HEIGHT / 2);
                }
            }
        }

        public virtual void refreshAllRings()
        {
            Bitmap turned;
            using (Graphics res = Graphics.FromImage(owner.output))
            {
                res.FillRectangle(MainForm.white, new Rectangle(new Point(0, 0), owner.output.Size));
                for (int i = setup.ringCount - 1; i >= 0; --i)
                {
                    turned = new Bitmap(WIDTH, HEIGHT);
                    using (Graphics g = Graphics.FromImage(turned))
                    {
                        MainForm.rotate(g, owner.flrot[i], WIDTH / 2, HEIGHT / 2);
                        g.DrawImage(slices[i], 0, 0);
                    }
                    res.DrawImage(turned, owner.center.X - WIDTH / 2, owner.center.Y - HEIGHT / 2);
                }
            }
        }

        public virtual void refreshRing(int ringIndex)
        {
            /*Bitmap turned;
            using (Graphics res = Graphics.FromImage(output))
            {
                turned = new Bitmap(bmp.Width, bmp.Height);
                using (Graphics g = Graphics.FromImage(turned))
                {
                    rotate(g, rotations[ringIndex], bmp.Width / 2, bmp.Height / 2);
                    g.DrawImage(slices[ringIndex], 0, 0);
                }
                res.DrawImage(turned, center.X - bmp.Width / 2, center.Y - bmp.Height / 2);
            }*/
            refreshAllRings();
        }
    }
}
