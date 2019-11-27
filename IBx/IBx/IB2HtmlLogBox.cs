using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IBx
{
    public class IB2HtmlLogBox
    {
        [JsonIgnore]
        public GameView gv;
        public string tag = "";
        [JsonIgnore]
        public List<string> tagStack = new List<string>();
        [JsonIgnore]
        public List<FormattedLine> logLinesList = new List<FormattedLine>();
        [JsonIgnore]
        public int currentTopLineIndex = 0;
        public int numberOfLinesToShow = 17;
        public float xLoc = 0;
        public int startY = 0;
        public int moveDeltaY = 0;
        public int tbHeight = 200;
        public int tbWidth = 300;
        public int tbXloc = 10;
        public int tbYloc = 10;
        public float fontHeightToWidthRatio = 1.0f;
        public bool touchIsDown = false;
        public int touchMoveDeltaY = 0;
        public int lastTouchMoveLocationY = 0;

        public IB2HtmlLogBox()
        {

        }

        public IB2HtmlLogBox(GameView g)
        {
            gv = g;
        }

        public void setupIB2HtmlLogBox(GameView g)
        {
            gv = g;
        }

        public void DrawString(string text, float x, float y, string style, string fontColor, string fontHeight, byte opacity, bool isUnderlined)
        {
            float fontH = 0;
            if (fontHeight.Equals("small"))
            {
                fontH = gv.drawFontSmallHeight;
            }
            else if (fontHeight.Equals("large"))
            {
                fontH = gv.drawFontLargeHeight;
            }
            else
            {
                fontH = gv.drawFontRegHeight;
            }
            if ((y > -2) && (y <= (int)(tbHeight * gv.screenDensity) - fontH))
            {
                //hurgh21
                if (gv.mod.useMinimalisticUI)
                {
                    gv.DrawText(text, x + (int)(tbXloc * gv.screenDensity) + 2 * gv.pS, y, fontHeight, fontColor, style, opacity, isUnderlined);
                }
                else 
                {
                    gv.DrawText(text, x + (int)(tbXloc * gv.screenDensity) + gv.pS, y, fontHeight, fontColor, style, opacity, isUnderlined);
                }
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
                List<FormattedLine> linesList = gv.cc.ProcessHtmlString(htmlText, (int)(tbWidth * gv.screenDensity), tagStack);
                foreach (FormattedLine fl in linesList)
                {
                    logLinesList.Add(fl);
                }
            }
            else
            {
                List<FormattedLine> linesList = gv.cc.ProcessHtmlString(htmlText + "<br>", (int)(tbWidth * gv.screenDensity), tagStack);
                foreach (FormattedLine fl in linesList)
                {
                    logLinesList.Add(fl);
                }
            }
            gv.mod.logFadeCounter = 120;
            gv.mod.logOpacity = 1f;
            scrollToEnd();
        }
        public void onDrawLogBox(IB2Panel parentPanel)
        {
            //ratio of #lines to #pixels
            float ratio = (float)(logLinesList.Count) / (float)(tbHeight * gv.screenDensity);
            if (ratio < 1.0f) { ratio = 1.0f; }
            if (moveDeltaY != 0)
            {
                int lineMove = (startY + moveDeltaY) * (int)ratio;
                SetCurrentTopLineAbsoluteIndex(lineMove);
            }
            //only draw lines needed to fill textbox
            float xLoc = 0.0f;
            float yLoc = 3.0f;
            int maxLines = 0;

            if (gv.screenType.Equals("combat") && (gv.mod.useMinimalisticUI))
            {
                numberOfLinesToShow = 20;
            }

            if (gv.screenType.Equals("combat") && (gv.screenCombat.showIniBar) && (!gv.mod.useMinimalisticUI))
            {
                numberOfLinesToShow = 22;
            }

            if (gv.screenType.Equals("combat") && (!gv.screenCombat.showIniBar) && (!gv.mod.useMinimalisticUI))
            {
                numberOfLinesToShow = 26;
            }

            maxLines = currentTopLineIndex + numberOfLinesToShow;
            
            if (maxLines > logLinesList.Count) { maxLines = logLinesList.Count; }
            for (int i = currentTopLineIndex; i < maxLines; i++)
            {
                //loop through each line and print each word
                foreach (FormattedWord word in logLinesList[i].wordsList)
                {
                    int difYheight = logLinesList[i].lineHeight - (int)gv.drawFontRegHeight;
                    int xLoc2 = (int)((parentPanel.currentLocX * gv.screenDensity + xLoc));
                    int yLoc2 = (int)((parentPanel.currentLocY * gv.screenDensity + yLoc + difYheight));
                    byte logOpac = (byte)(255f * gv.mod.logOpacity);
                    int yPositionModifier = 0;
                    if (gv.screenType.Equals("combat") && (!gv.mod.useMinimalisticUI) && (gv.screenCombat.showIniBar))
                    {
                        yPositionModifier = gv.squareSize + 4 * gv.pS;
                    }
                   
                    DrawString(word.text + " ", xLoc2, yLoc2 + yPositionModifier, word.style, word.color, word.fontSize, logOpac, word.underlined);
                    xLoc += gv.MeasureString(word.text + " ", word.fontSize, word.style);
                }
                xLoc = 0;
                yLoc += logLinesList[i].lineHeight;
            }            
            //draw border for debug info
            //gv.DrawRectangle(new IbRect(parentPanel.currentLocX + tbXloc, parentPanel.currentLocY + tbYloc, tbWidth, tbHeight), SharpDX.Color.DimGray, 1);
        }

        public void scrollToEnd()
        {
            SetCurrentTopLineIndex(logLinesList.Count);
            //gv.Invalidate();
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
        /*private bool isMouseWithinTextBox(MouseEventArgs e)
        {
            if ((e.X > (int)(tbXloc * gv.screenDensity)) && (e.X < (int)(tbWidth * gv.screenDensity) + (int)(tbXloc * gv.screenDensity)) && (e.Y > (int)(tbYloc * gv.screenDensity)) && (e.Y < (int)(tbHeight * gv.screenDensity) + (int)(tbYloc * gv.screenDensity)))
            {
                return true;
            }
            return false;
        }*/
        /*public void onMouseWheel(object sender, MouseEventArgs e)
        {
            if (isMouseWithinTextBox(e))
            {
                // Update the drawing based upon the mouse wheel scrolling. 
                gv.mod.logFadeCounter = 120;
                gv.mod.logOpacity = 1f;
                int numberOfTextLinesToMove = e.Delta * SystemInformation.MouseWheelScrollLines / 120;

                if (numberOfTextLinesToMove != 0)
                {
                    SetCurrentTopLineIndex(-numberOfTextLinesToMove);
                    //gv.Invalidate();
                    gv.Render(0);
                }
            }
        }*/
        private bool isTouchWithinTextBox(int eX, int eY)
        {
            if ((eX > (int)((tbXloc)))
                    && (eX < (int)(tbWidth) + (int)((tbXloc)))
                    && (eY > (int)((tbYloc)))
                    && (eY < (int)(tbHeight) + (int)((tbYloc))))
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
            switch (eventType)
            {
                case MouseEventType.EventType.MouseDown:

                    if (isTouchWithinTextBox(eX, eY))
                    {
                        gv.mod.logFadeCounter = 120;
                        gv.mod.logOpacity = 1f;
                        touchIsDown = true;
                        lastTouchMoveLocationY = eY;
                        touchMoveDeltaY = 0;
                    }
                    break;

                case MouseEventType.EventType.MouseMove:

                    if (touchIsDown)
                    {
                        if (isTouchWithinTextBox(eX, eY))
                        {
                            gv.mod.logFadeCounter = 120;
                            gv.mod.logOpacity = 1f;
                            touchMoveDeltaY = lastTouchMoveLocationY - eY;
                            if (touchMoveDeltaY > gv.drawFontRegHeight)
                            {
                                SetCurrentTopLineIndex(1);
                                touchMoveDeltaY = 0;
                                lastTouchMoveLocationY = eY;
                            }
                            else if (touchMoveDeltaY < -1 * gv.drawFontRegHeight)
                            {
                                SetCurrentTopLineIndex(-1);
                                touchMoveDeltaY = 0;
                                lastTouchMoveLocationY = eY;
                            }
                        }
                    }
                    break;

                case MouseEventType.EventType.MouseUp:

                    touchIsDown = false;
                    touchMoveDeltaY = 0;
                    break;
            }
        }

    }
}
