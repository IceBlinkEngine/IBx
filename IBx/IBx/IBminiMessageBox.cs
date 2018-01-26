using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBx
{
    public class IBminiMessageBox
    {
        /*
        public GameView gv;
        public string tag = "";
        public List<string> tagStack = new List<string>();
        public List<FormattedLine> logLinesList = new List<FormattedLine>();
        public int currentTopLineIndex = 0;
        public int numberOfLinesToShow = 17;
        public int currentLocX = 0;
        public int currentLocY = 0;        
        public int Width = 0;
        public int Height = 0;
        public float xLoc = 0;
        public int startY = 0;
        public int moveDeltaY = 0;
        public int tbHeight = 200;
        public int tbWidth = 300;
        public int tbXloc = 10;
        public int tbYloc = 10;
        public float fontHeightToWidthRatio = 1.0f;
        public IbbButton btnReturn = null;
        public bool touchIsDown = false;
        public int touchMoveDeltaY = 0;
        public int lastTouchMoveLocationY = 0;

        public IBminiMessageBox()
        {

        }

        public IBminiMessageBox(GameView g)
        {
            gv = g;            
        }

        public void setupIBminiMessageBox()
        {
            setControlsStart();
        }

        public void setControlsStart()
        {
            int pH = (int)((float)gv.screenHeight / 100.0f);

            if (btnReturn == null)
            {
                btnReturn = new IbbButton(gv, 1.0f);
                btnReturn.Img = "btn_large";
                btnReturn.Glow = "btn_large_glow";
                btnReturn.Text = "Return";
                btnReturn.Height = (int)(gv.ibbheight * gv.scaler);
                btnReturn.Width = (int)(gv.ibbwidthL * gv.scaler);
                btnReturn.X = (int)(currentLocX * gv.scaler) + (int)((Width * gv.scaler) / 2) - (int)((gv.ibbwidthL * gv.scaler) / 2);
                btnReturn.Y = (int)(currentLocY * gv.scaler) + (int)(Height * gv.scaler) - (int)(gv.ibbheight * gv.scaler) - (int)((gv.ibbheight / 4) * gv.scaler);                
            }            
        }

        public void DrawString(string text, float x, float y, string fontColor)
        {
            if ((y > -2) && (y <= (tbHeight * gv.scaler) - gv.fontHeight))
            {
                gv.DrawText(text, x + tbXloc + gv.pS, y, fontColor);
            }
        }

        public void AddHtmlTextToLog(string htmlText)
        {
            //Remove any '\r\n' hard returns from message
            htmlText = htmlText.Replace("\r\n", "<br>");
            htmlText = htmlText.Replace("\n\n", "<br>");
            htmlText = htmlText.Replace("\"", "'");

            if ((htmlText.EndsWith("<br>")) || (htmlText.EndsWith("<BR>")))
            {
                List<FormattedLine> lnList = gv.cc.ProcessHtmlString(htmlText, (int)(tbWidth * gv.scaler), tagStack, true);
                foreach (FormattedLine fl in lnList)
                {
                    logLinesList.Add(fl);
                }
            }
            else
            {
                List<FormattedLine> lnList = gv.cc.ProcessHtmlString(htmlText + "<br>", (int)(tbWidth * gv.scaler), tagStack, true);
                foreach (FormattedLine fl in lnList)
                {
                    logLinesList.Add(fl);
                }
            }
            scrollToEnd();
        }
        public void onDrawLogBox()
        {
            IbRect src = new IbRect(0, 0, gv.cc.GetFromBitmapList("ui_bg_log").Width, gv.cc.GetFromBitmapList("ui_bg_log").Height);
            IbRect dst = new IbRect((int)(currentLocX * gv.scaler), (int)(currentLocY * gv.scaler), (int)(Width * gv.scaler), (int)(Height * gv.scaler));
            gv.DrawBitmap(gv.cc.GetFromBitmapList("ui_bg_log"), src, dst);

            //ratio of #lines to #pixels
            float ratio = (float)(logLinesList.Count) / (float)(tbHeight * gv.scaler);
            if (ratio < 1.0f) { ratio = 1.0f; }
            if (moveDeltaY != 0)
            {
                int lineMove = (startY + moveDeltaY) * (int)ratio;
                SetCurrentTopLineAbsoluteIndex(lineMove);
            }
            //only draw lines needed to fill textbox
            float xLoc = 0.0f;
            float yLoc = 15.0f;
            int totalFontSpacing = gv.fontHeight + gv.fontLineSpacing;
            numberOfLinesToShow = (int)(((float)(tbHeight * gv.scaler) - (btnReturn.Height * 1.25f)) / (totalFontSpacing));
            int maxLines = currentTopLineIndex + numberOfLinesToShow;

            if (maxLines > logLinesList.Count) { maxLines = logLinesList.Count; }
            for (int i = currentTopLineIndex; i < maxLines; i++)
            {
                //loop through each line and print each word
                foreach (FormattedWord word in logLinesList[i].wordsList)
                {
                    int xLoc2 = (int)((currentLocX * gv.scaler + xLoc));
                    int yLoc2 = (int)((currentLocY * gv.scaler + yLoc));
                    DrawString(word.text + " ", xLoc2, yLoc2, word.color);
                    xLoc += (word.text.Length + 1) * (gv.fontWidth + gv.fontCharSpacing);
                }
                xLoc = 0;
                yLoc += gv.fontHeight + gv.fontLineSpacing;
            }
            btnReturn.Draw();
        }

        public void scrollToEnd()
        {
            SetCurrentTopLineIndex(logLinesList.Count);
        }
        public void SetCurrentTopLineIndex(int changeValue)
        {
            currentTopLineIndex += changeValue;
            if (currentTopLineIndex > logLinesList.Count - numberOfLinesToShow)
            {
                currentTopLineIndex = logLinesList.Count - numberOfLinesToShow;
            }
            if (currentTopLineIndex < 0)
            {
                currentTopLineIndex = 0;
            }
        }
        public void SetCurrentTopLineAbsoluteIndex(int absoluteValue)
        {
            currentTopLineIndex = absoluteValue;
            if (currentTopLineIndex > logLinesList.Count - numberOfLinesToShow)
            {
                currentTopLineIndex = logLinesList.Count - numberOfLinesToShow;
            }
            if (currentTopLineIndex < 0)
            {
                currentTopLineIndex = 0;
            }
        }
        private bool isTouchWithinTextBox(int eX, int eY)
        {
            if ((eX > (int)(currentLocX * gv.scaler))
                    && (eX < (int)(tbWidth * gv.scaler) + (int)(currentLocX * gv.scaler))
                    && (eY > (int)(currentLocY * gv.scaler))
                    && (eY < (int)(tbHeight * gv.scaler) + (int)(currentLocY * gv.scaler)))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public void onTouchSwipe(int eX, int eY, MouseEventType.EventType eventType)
        {
            if (isTouchWithinTextBox(eX, eY))
            {
                switch (eventType)
                {
                    case MouseEventType.EventType.MouseDown:

                        touchIsDown = true;
                        lastTouchMoveLocationY = eY;
                        touchMoveDeltaY = 0;
                        break;

                    case MouseEventType.EventType.MouseMove:

                        touchMoveDeltaY = lastTouchMoveLocationY - eY;
                        if (touchMoveDeltaY > gv.fontHeight)
                        {
                            SetCurrentTopLineIndex(1);
                            touchMoveDeltaY = 0;
                            lastTouchMoveLocationY = eY;
                        }
                        else if (touchMoveDeltaY< -1 * gv.fontHeight)
                        {
                            SetCurrentTopLineIndex(-1);
                            touchMoveDeltaY = 0;
                            lastTouchMoveLocationY = eY;
                        }
                        break;

                    case MouseEventType.EventType.MouseUp:

                        touchIsDown = false;
                        touchMoveDeltaY = 0;
                        break;
                }
            }
        }
        */
    }
}
