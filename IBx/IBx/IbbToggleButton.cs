using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace IBx
{
    public class IbbToggleButton
    {
        //this class is handled differently than Android version
        public string ImgOn = null;
        public string ImgOff = null;
        public bool toggleOn = false;
        public int X = 0;
        public int Y = 0;
        public int Width = 0;
        public int Height = 0;
        public GameView gv;

        public IbbToggleButton(GameView g)
        {
            gv = g;
        }

        public bool getImpact(int x, int y)
        {
            if ((x >= X) && (x <= (X + this.Width)))
            {
                if ((y >= Y) && (y <= (Y + this.Height)))
                {
                    return true;
                }
            }
            return false;
        }

        public void Draw()
        {
            IbRect src = new IbRect(0, 0, gv.cc.GetFromBitmapList(ImgOn).Width, gv.cc.GetFromBitmapList(ImgOn).Height);
            IbRect dst = new IbRect(this.X, this.Y, gv.squareSize/2, gv.squareSize/2);

            if (this.toggleOn)
            {
                if (this.ImgOn != null)
                {
                    gv.DrawBitmap(gv.cc.GetFromBitmapList(ImgOn), src, dst);
                }
            }
            else
            {
                if (this.ImgOff != null)
                {
                    gv.DrawBitmap(gv.cc.GetFromBitmapList(ImgOff), src, dst);
                }
            }
        }
    }
}
