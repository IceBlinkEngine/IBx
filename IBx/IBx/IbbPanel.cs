using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IBx
{
    public class IbbPanel
    {
        //this class is handled differently than Android version
        public string ImgBG = "";
        public int LocX = 0;
        public int LocY = 0;
        public int Width = 0;
        public int Height = 0;
        public GameView gv;

        public IbbPanel(GameView g)
        {
            gv = g;
        }

        public void Draw()
        {
            IbRect src = new IbRect(0, 0, gv.cc.GetFromBitmapList(ImgBG).Width, gv.cc.GetFromBitmapList(ImgBG).Height);
            IbRect dst = new IbRect(this.LocX, this.LocY, Width, Height);            
            gv.DrawBitmap(gv.cc.GetFromBitmapList(ImgBG), src, dst);
        }
    }
}
