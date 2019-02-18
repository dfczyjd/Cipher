#if NO_COMPILE
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
    class Wood_old : CipherType
    {
        public Bitmap bmp2;
        private Bitmap[] sides;
        private int delx = -5, dely = 5;

        public Wood_old(MainForm owner) : base(owner) { }

        private Bitmap cutCircle(Bitmap bmp, int r, int x = 0, int y = 0)
        {
            Bitmap result = new Bitmap(bmp),
                tmp = new Bitmap(bmp.Width, bmp.Height);
            using (Graphics g = Graphics.FromImage(tmp))
            {
                g.FillRectangle(MainForm.white, new Rectangle(0, 0, tmp.Width, tmp.Height));
                g.TranslateTransform(-x, -y);
                g.FillEllipse(MainForm.black, new Rectangle(bmp.Width / 2 - r, bmp.Height / 2 - r, 2 * r, 2 * r));
                tmp.MakeTransparent(Color.Black);
            }
            using (Graphics g = Graphics.FromImage(result))
            {
                g.DrawImage(tmp, 0, 0);
                g.DrawEllipse(new Pen(Color.Black), new Rectangle(bmp.Width / 2 - r, bmp.Height / 2 - r, 2 * r, 2 * r));
            }
            result.MakeTransparent(Color.White);
            return result;
        }

        private Bitmap removeCircle(Bitmap bmp, int r)
        {
            Bitmap result = new Bitmap(bmp);
            using (Graphics g = Graphics.FromImage(result))
            {
                g.FillEllipse(MainForm.white, new Rectangle(bmp.Width / 2 - r, bmp.Height / 2 - r, 2 * r, 2 * r));
            }
            result.MakeTransparent(Color.White);
            return result;
        }

        public override void slice(Bitmap bmp, int pieces)
        {
            slices = new Bitmap[pieces];
            sides = new Bitmap[pieces - 1];
            owner.ringWidth = 30;// bmp.Width / pieces / 2;
            for (int i = 0; i < pieces; ++i)
            {
                if (i % 2 == 0)
                    slices[i] = cutCircle(bmp, owner.ringWidth * (i + 1) - 1);
                else
                    slices[i] = cutCircle(bmp2, owner.ringWidth * (i + 1) - 1);
                slices[i] = removeCircle(slices[i], owner.ringWidth * i);
                if (i == 0)
                    continue;
                if (i % 2 == 0)
                    sides[i - 1] = cutCircle(bmp2, owner.ringWidth * i - 1, delx, -dely);
                else
                    sides[i - 1] = cutCircle(bmp, owner.ringWidth * i - 1, delx, -dely);
                sides[i - 1].RotateFlip(RotateFlipType.Rotate90FlipNone);
            }
        }

        public override void inscribe(int ringIndex)
        {
            using (Graphics g = Graphics.FromImage(slices[ringIndex]))
            {
                for (int j = 0; j < owner.alphs[ringIndex].Length; ++j)
                {
                    float angle = 180.0F / owner.alphs[ringIndex].Length;
                    Font f = new Font("Times New Roman", ringIndex * 2 + 6);
                    float y = bmp.Width / 2 - owner.ringWidth * (ringIndex + 0.5F), x = bmp.Width / 2;
                    g.DrawString(owner.alphs[ringIndex].Substring(j, 1), f, MainForm.black, x - f.SizeInPoints / 2, y - f.Height / 2);
                    MainForm.rotate(g, angle, bmp.Width / 2, bmp.Height / 2);
                    //g.DrawLine(new Pen(Color.Black), bmp.Width / 2, bmp.Height / 2, bmp.Width / 2, bmp.Width / 2 - owner.ringWidth * (ringIndex + 1));
                    MainForm.rotate(g, angle, bmp.Width / 2, bmp.Height / 2);
                }
            }
        }

        public override void refreshAllRings()
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
                        if (i != owner.ringCount - 1)
                            g.DrawImage(sides[i], delx * i, dely * i);
                        MainForm.rotate(g, owner.rotations[i], bmp.Width / 2 + delx * i, bmp.Height / 2 + dely * i);
                        g.DrawImage(slices[i], delx * i, dely * i);
                    }
                    res.DrawImage(turned, owner.center.X - bmp.Width / 2, owner.center.Y - bmp.Height / 2);
                }
            }
        }

        public override void refreshRing(int ringIndex)
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

    class Wood : CipherType
    {
        private Bitmap[] texts;
        private static Pen metalPen = new Pen(Color.FromArgb(0x50, 0x20, 0x20), 3);
        private static Brush metalBrush = metalPen.Brush;

        public Wood(MainForm owner)
            : base(owner)
        {
            pen = new Pen(Color.FromArgb(0x50, 0x20, 0x20), 3);
            brush = pen.Brush;
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
                g.DrawEllipse(metalPen, new Rectangle(bmp.Width / 2 - r, bmp.Height / 2 - r, 2 * r, 2 * r));
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

        public override void slice(Bitmap bmp, int pieces)
        {
            slices = new Bitmap[pieces];
            texts = new Bitmap[pieces];
            owner.ringWidth = 30;// bmp.Width / pieces / 2;
            for (int i = 0; i < pieces; ++i)
            {
                texts[i] = new Bitmap(bmp.Width, bmp.Height);
                slices[i] = removeCircle(bmp, owner.ringWidth * i);
                slices[i] = cutCircle(slices[i], owner.ringWidth * (i + 1));
            }
        }

        public override void inscribe(int ringIndex)
        {
            using (Graphics g = Graphics.FromImage(texts[ringIndex]))
            {
                for (int j = 0; j < owner.alphs[ringIndex].Length; ++j)
                {
                    float angle = 180.0F / owner.alphs[ringIndex].Length;
                    Font f = new Font("Times New Roman", ringIndex * 2 + 6);
                    float y = bmp.Width / 2 - owner.ringWidth * (ringIndex + 0.5F), x = bmp.Width / 2;
                    g.DrawString(owner.alphs[ringIndex].Substring(j, 1), f, metalBrush, x - f.SizeInPoints / 2, y - f.Height / 2);
                    MainForm.rotate(g, angle, bmp.Width / 2, bmp.Height / 2);
                    g.DrawLine(new Pen(metalBrush, 1), bmp.Width / 2, bmp.Height / 2, bmp.Width / 2, bmp.Width / 2 - owner.ringWidth * (ringIndex + 1));
                    MainForm.rotate(g, angle, bmp.Width / 2, bmp.Height / 2);
                }
            }
        }

        public override void refreshAllRings()
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
                        g.DrawImage(texts[i], 0, 0);
                    }
                    res.DrawImage(turned, owner.center.X - bmp.Width / 2, owner.center.Y - bmp.Height / 2);
                }
            }
        }

        public override void refreshRing(int ringIndex)
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
#endif