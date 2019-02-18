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
    public enum MaterialName
    {
        Paper, Wood, Metal, Bronze
    }

    public struct Material
    {
        public MaterialName name;
        public Bitmap texture;
        public Color color;

        public static Material createMaterialByName(string RussianName)
        {
            Material obj = new Material();
            switch (RussianName)
            {
                case "Бумага":
                    obj.name = MaterialName.Paper;
                    obj.texture = null;
                    obj.color = MainForm.paperPen.Color;
                    break;

                case "Дерево":
                    obj.name = MaterialName.Wood;
                    obj.texture = null;
                    obj.color = MainForm.woodPen.Color;
                    break;

                case "Металл":
                    obj.name = MaterialName.Metal;
                    obj.texture = null;
                    obj.color = MainForm.metalPen.Color;
                    break;

                case "Бронза":
                    obj.name = MaterialName.Bronze;
                    obj.texture = null;
                    obj.color = MainForm.bronzePen.Color;
                    break;

                default:
                    throw new InvalidEnumArgumentException(string.Format("Материал {0} не существует", RussianName));
            }
            return obj;
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

    public struct CipherSetup
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
        protected MainForm owner;
        public Bitmap bmp;
        protected Bitmap[] slices;
        public Pen pen;
        public Brush brush;
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
                g.DrawEllipse(pen, new Rectangle(bmp.Width / 2 - r, bmp.Height / 2 - r, 2 * r, 2 * r));
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

        public virtual void slice(Bitmap bmp, int pieces)
        {
            slices = new Bitmap[pieces];
            owner.ringWidth = 30;// bmp.Width / pieces / 2;
            for (int i = 0; i < pieces; ++i)
            {
                slices[i] = removeCircle(bmp, owner.ringWidth * i);
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
                    float y = bmp.Width / 2 - owner.ringWidth * (ringIndex + 0.5F), x = bmp.Width / 2;
                    g.DrawString(owner.alphs[ringIndex].Substring(j, 1), f, brush, x - f.SizeInPoints / 2, y - f.Height / 2);
                    MainForm.rotate(g, angle, bmp.Width / 2, bmp.Height / 2);
                    g.DrawLine(new Pen(brush, 1), bmp.Width / 2, bmp.Height / 2, bmp.Width / 2, bmp.Width / 2 - owner.ringWidth * (ringIndex + 1));
                    MainForm.rotate(g, angle, bmp.Width / 2, bmp.Height / 2);
                }
            }
        }

        public virtual void refreshAllRings()
        {
            Bitmap turned;
            using (Graphics res = Graphics.FromImage(owner.output))
            {
                res.FillRectangle(MainForm.white, new Rectangle(new Point(0, 0), owner.output.Size));
                for (int i = owner.ringCount - 1; i >= 0; --i)
                {
                    turned = new Bitmap(bmp.Width, bmp.Height);
                    using (Graphics g = Graphics.FromImage(turned))
                    {
                        MainForm.rotate(g, owner.flrot[i]/*rotations[i] * 360 / (float)owner.alphs[i].Length*/, bmp.Width / 2, bmp.Height / 2);
                        g.DrawImage(slices[i], 0, 0);
                    }
                    res.DrawImage(turned, owner.center.X - bmp.Width / 2, owner.center.Y - bmp.Height / 2);
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
