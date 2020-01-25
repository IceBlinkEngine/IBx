using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace IBx
{
    public class IbbButton
    {
        //this class is handled differently than Android version
        public string Img = null;    //this is the normal button and color intensity
        public string ImgOff = null; //this is usually a grayed out button
        public string ImgOn = null;  //useful for buttons that are toggled on like "Move"
        public string Img2 = null;   //usually used for an image on top of default button like arrows or inventory backpack image
        public string Img2Off = null;   //usually used for turned off image on top of default button like spell not available
        public string Img3 = null;   //typically used for convo plus notification icon
        public string Glow = null;   //typically the green border highlight when hoover over or press button
        public buttonState btnState = buttonState.Normal;
        public bool btnNotificationOn = true; //used to determine whether Img3 is shown or not
        public bool glowOn = false;
        public string Text = "";
        public string Quantity = "";
        public string HotKey = "";
        public int X = 0;
        public int Y = 0;
        public int Width = 0;
        public int Height = 0;
        public float scaler = 1.0f;
        public bool playedHoverSound = false;
        public bool btnOfChargedItem = false;
        public bool btnWithGold = false;
        public GameView gv;

        public IbbButton(GameView g, float sc)
        {
            gv = g;
            scaler = sc;
        }

        public bool getImpact(int x, int y)
        {
            if ((x >= X) && (x <= (X + this.Width)))
            {
                if ((y >= Y) && (y <= (Y + this.Height)))
                {
                    if (!playedHoverSound)
                    {
                        playedHoverSound = true;
                    }
                    return true;
                }
            }
            playedHoverSound = false;
            return false;
        }

        public void Draw()
        {
            int pH = (int)((float)gv.screenHeight / 200.0f);
            int pW = (int)((float)gv.screenHeight / 200.0f);
            float fSize = (float)(gv.squareSize / 4) * scaler;

            IbRect src = new IbRect(0, 0, gv.cc.GetFromBitmapList(Img).Width, gv.cc.GetFromBitmapList(Img).Height);
            IbRect src2 = new IbRect(0, 0, gv.cc.GetFromBitmapList(Img).Width, gv.cc.GetFromBitmapList(Img).Height);
            IbRect src3 = new IbRect(0, 0, gv.cc.GetFromBitmapList(Img).Width, gv.cc.GetFromBitmapList(Img).Height);

            if (this.Img2 != null)
            {
                src2 = new IbRect(0, 0, gv.cc.GetFromBitmapList(Img2).Width, gv.cc.GetFromBitmapList(Img2).Width);
            }
            if (this.Img3 != null)
            {
                src3 = new IbRect(0, 0, gv.cc.GetFromBitmapList(Img3).Width, gv.cc.GetFromBitmapList(Img3).Width);
            }
            IbRect dst = new IbRect(this.X, this.Y, (int)((float)this.Width), (int)((float)this.Height));

            IbRect srcGlow = new IbRect(0, 0, gv.cc.GetFromBitmapList(Glow).Width, gv.cc.GetFromBitmapList(Glow).Height);
            IbRect dstGlow = new IbRect(this.X - (int)(7 * gv.screenDensity), 
                                        this.Y - (int)(7 * gv.screenDensity), 
                                        (int)((float)this.Width) + (int)(15 * gv.screenDensity), 
                                        (int)((float)this.Height) + (int)(15 * gv.screenDensity));

            //draw glow first if on
            if ((this.glowOn) && (this.Glow != null))
            {
                gv.DrawBitmap(gv.cc.GetFromBitmapList(Glow), srcGlow, dstGlow);
            }
            //draw the proper button State
            if ((this.btnState == buttonState.On) && (this.ImgOn != null))
            {
                gv.DrawBitmap(gv.cc.GetFromBitmapList(ImgOn), src, dst);
            }
            else if ((this.btnState == buttonState.Off) && (this.ImgOff != null))
            {
                gv.DrawBitmap(gv.cc.GetFromBitmapList(ImgOff), src, dst);
            }
            else
            {
                gv.DrawBitmap(gv.cc.GetFromBitmapList(Img), src, dst);
            }
            //draw the standard overlay image if has one
            if ((this.btnState == buttonState.Off) && (this.Img2Off != null))
            {
                gv.DrawBitmap(gv.cc.GetFromBitmapList(Img2Off), src2, dst);
            }
            else if (this.Img2 != null)
            {
                gv.DrawBitmap(gv.cc.GetFromBitmapList(Img2), src2, dst);
            }
            //draw the notification image if turned on (like a level up or additional convo nodes image)
            if ((this.btnNotificationOn) && (this.Img3 != null))
            {
                gv.DrawBitmap(gv.cc.GetFromBitmapList(Img3), src3, dst);
            }

            float thisFontHeight = gv.drawFontRegHeight;
            string fontHeightInString = "regular";
            if (scaler > 1.05f)
            {
                thisFontHeight = gv.drawFontLargeHeight;
                fontHeightInString = "large";
            }
            else if (scaler < 0.95f)
            {
                thisFontHeight = gv.drawFontSmallHeight;
                fontHeightInString = "regular";
            }
            
            // DRAW TEXT
            float stringSize = gv.MeasureString(Text, fontHeightInString, "normal");

            //place in the center
            float ulX = ((this.Width) - stringSize) / 2;
            float ulY = ((this.Height) - thisFontHeight) / 2;

            /*for (int x = -2; x <= 2; x++)
            {
                for (int y = -2; y <= 2; y++)
                {
                    gv.DrawText(Text, this.X + ulX + x, this.Y + ulY + y , fontHeightInString, "black");
                }
            }*/
            gv.DrawText(Text, this.X + ulX + 2, this.Y + ulY + 2, fontHeightInString, "black");
            if (!this.btnWithGold)
            {
                gv.DrawText(Text, this.X + ulX, this.Y + ulY, fontHeightInString, "white");
            }
            else
            {
                gv.DrawText(Text, this.X + ulX, this.Y + ulY, fontHeightInString, "yellow");
            }
            
            // DRAW QUANTITY
            stringSize = gv.MeasureString(Quantity, fontHeightInString, "normal");
            
            //place in the bottom right quadrant
            ulX = (((this.Width) - stringSize) / 8) * 7;
            ulY = (((this.Height) - thisFontHeight) / 8) * 7;

            /*for (int x = -2; x <= 2; x++)
            {
                for (int y = -2; y <= 2; y++)
                {
                    gv.DrawText(Quantity, this.X + ulX + x, this.Y + ulY + y, fontHeightInString, "black");
                }
            }*/
            gv.DrawText(Quantity, this.X + ulX + 2, this.Y + ulY + 2, fontHeightInString, "black");
            if (!this.btnOfChargedItem)
            {
                gv.DrawText(Quantity, this.X + ulX, this.Y + ulY, fontHeightInString, "white");
            }
            else
            {
                gv.DrawText(Quantity, this.X + ulX, this.Y + ulY, fontHeightInString, "green");
            }

            // DRAW HOTKEY
            if (gv.showHotKeys)
            {
                stringSize = gv.MeasureString(HotKey, fontHeightInString, "normal");

                //place in the bottom center
                ulX = ((this.Width) - stringSize) / 2;
                ulY = (((this.Height) - thisFontHeight) / 4) * 3;

                for (int x = -2; x <= 2; x++)
                {
                    for (int y = -2; y <= 2; y++)
                    {
                        gv.DrawText(HotKey, this.X + ulX + x, this.Y + ulY + y, fontHeightInString, "black");
                    }
                }
                gv.DrawText(HotKey, this.X + ulX, this.Y + ulY, fontHeightInString, "red");
            }
        }
    }
}
