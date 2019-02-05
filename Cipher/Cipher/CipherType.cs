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
    public abstract class CipherType
    {
        protected Form1 owner;
        public Bitmap bmp;
        protected Bitmap[] slices;

        public CipherType(Form1 owner)
        {
            this.owner = owner;
        }

        public abstract void slice(Bitmap bmp, int pieces);
        public abstract void inscribe(int ringIndex);
        public abstract void refreshAllRings();
        public abstract void refreshRing(int ringIndex);
    }
}
