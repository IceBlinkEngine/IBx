﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace IBx
{
    public class IbbHtmlLogBox
    {
        /*
        public GameView gv = null;
        public Bitmap btn_up;
        public Bitmap btn_down;
        public Bitmap btn_scroll;
        public Bitmap bg_scroll;
        
        public List<string> tagStack = new List<string>();
        public List<FormattedLine> logLinesList = new List<FormattedLine>();
        public int currentTopLineIndex = 0;
        public int numberOfLinesToShow = 17;
        public float xLoc = 0;
        public int moveEY = 0;
        public int moveScrollingStartY = 0;
        public int scrollButtonYLoc = 0;
        public int startY = 0;
        public int moveDeltaY = 0;
        public int tbHeight = 200;
        public int tbWidth = 300;
        
        public int tbXloc = 10;
        public int tbYloc = 10;
        public float fontHeightToWidthRatio = 1.0f;

        public IbbHtmlLogBox()
        {

        }

        public IbbHtmlLogBox(GameView g, int locX, int locY, int width, int height)
        {
            gv = g;
            btn_up = ""btn_up.png");
            btn_down = ""btn_down.png");
            btn_scroll = ""btn_scroll.png");
            bg_scroll = ""bg_scroll.png");
            //fontfamily = gv.family;
            //font = new Font(fontfamily, 20.0f * (float)gv.squareSize / 100.0f);
            tbXloc = locX;
            //the log box was placed to high on laptp, moved it down a bit, tried it dynamically
            //tbYloc = locY - (int)((gv.Height - 1080)/20);
            float heightShiftAdjustment = (gv.oYshift * (gv.Height/1080f) / 3);
            tbYloc = locY + (int)heightShiftAdjustment;
            tbWidth = width;
            //had to make the box sleightly longer to get height increased text in
            tbHeight = height + gv.oYshift;
            //brush.Color = Color.Red;
        }

        public void setupIbbHtmlLogBox(GameView g)
        {
            gv = g;
            btn_up = ""btn_up.png");
            btn_down = ""btn_down.png");
            btn_scroll = ""btn_scroll.png");
            bg_scroll = ""bg_scroll.png");
        }

        public void ResetLogBoxUiBitmaps()
        {
            btn_up = ""btn_up.png");
            btn_down = ""btn_down.png");
            btn_scroll = ""btn_scroll.png");
            bg_scroll = ""bg_scroll.png");
        }
        
        public void DrawBitmap(Bitmap bmp, int x, int y)
        {
            IbRect src = new IbRect(0, 0, bmp.Width, bmp.Height);
            IbRect dst = new IbRect(x + tbXloc, y + tbYloc - gv.oYshift, bmp.Width, bmp.Height);
            gv.DrawBitmap(bmp, src, dst);
        }
        public void DrawString(string text, float x, float y, FontWeight fw, SharpDX.DirectWrite.FontStyle fs, SharpDX.Color fontColor, float fontHeight, bool isUnderlined)
        {
            if ((y > -2) && (y <= tbHeight - fontHeight))
            {
                //gv.DrawText(text, f, sb, x + tbXloc, y + tbYloc - gv.oYshift);
                gv.DrawText(text, x + tbXloc + gv.pS, y, fw, fs, 1.0f, fontColor, isUnderlined);
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
                List<FormattedLine> linesList = gv.cc.ProcessHtmlString(htmlText, tbWidth - btn_up.Width, tagStack);
                foreach (FormattedLine fl in linesList)
                {
                    logLinesList.Add(fl);
                }
            }
            else
            {
                List<FormattedLine> linesList = gv.cc.ProcessHtmlString(htmlText + "<br>", tbWidth - btn_up.Width, tagStack);
                foreach (FormattedLine fl in linesList)
                {
                    logLinesList.Add(fl);
                }
            }        
            scrollToEnd();
        }
        
        public void onDrawLogBox()
        {
            //ratio of #lines to #pixels
            float ratio = (float)(logLinesList.Count) / (float)(tbHeight - btn_down.Height - btn_up.Height - btn_scroll.Height);
            if (ratio < 1.0f) { ratio = 1.0f; }
            if (moveDeltaY != 0)
            {
                int lineMove = (startY + moveDeltaY) * (int)ratio;
                SetCurrentTopLineAbsoluteIndex(lineMove);
            }
            //only draw lines needed to fill textbox
            float xLoc = 0;
            float yLoc = 3.0f;
            int maxLines = currentTopLineIndex + numberOfLinesToShow;
            if (maxLines > logLinesList.Count) { maxLines = logLinesList.Count; }
            for (int i = currentTopLineIndex; i < maxLines; i++)
            {
                //loop through each line and print each word
                foreach (FormattedWord word in logLinesList[i].wordsList)
                {
                    if (gv.textFormat != null)
                    {
                        gv.textFormat.Dispose();
                        gv.textFormat = null;
                    }

                    if (gv.textLayout != null)
                    {
                        gv.textLayout.Dispose();
                        gv.textLayout = null;
                    }
                    gv.textFormat = new SharpDX.DirectWrite.TextFormat(gv.factoryDWrite, gv.family.Name, gv.CurrentFontCollection, word.fontWeight, word.fontStyle, FontStretch.Normal, word.fontSize) { TextAlignment = TextAlignment.Leading, ParagraphAlignment = ParagraphAlignment.Near };
                    gv.textLayout = new SharpDX.DirectWrite.TextLayout(gv.factoryDWrite, word.text + " ", gv.textFormat, gv.Width, gv.Height);
                    int difYheight = logLinesList[i].lineHeight - (int)word.fontSize;
                    if (word.underlined)
                    {
                        gv.textLayout.SetUnderline(true, new TextRange(0, word.text.Length - 1));
                    }
                    DrawString(word.text + " ", xLoc, yLoc + difYheight, word.fontWeight, word.fontStyle, word.color, word.fontSize, word.underlined);
                    xLoc += gv.textLayout.Metrics.WidthIncludingTrailingWhitespace;

                    //OLD STUFF
                    //print each word and move xLoc
                    //font = new Font(fontfamily, word.fontSize, word.fontStyle);
                    //int wordWidth = (int)(gv.gCanvas.MeasureString(word.text, font)).Width;
                    //int wordWidth = 12;
                    //brush.Color = word.color;
                    //int difYheight = logLinesList[i].lineHeight - font.Height;
                    //DrawString(word.text, font, brush, xLoc, yLoc + difYheight);
                    //xLoc += wordWidth;
                }
                xLoc = 0;
                yLoc += logLinesList[i].lineHeight;
            }

            //determine the scrollbutton location            
            scrollButtonYLoc = (currentTopLineIndex / (int)ratio);
            if (scrollButtonYLoc > tbHeight - btn_down.Height - btn_scroll.Height)
            {
                scrollButtonYLoc = tbHeight - btn_down.Height - btn_scroll.Height;
            }
            if (scrollButtonYLoc < 0 + btn_up.Height)
            {
                scrollButtonYLoc = 0 + btn_up.Height;
            }

            //draw scrollbar
            for (int y = 0; y < tbHeight - 10; y += 10)
            {
                DrawBitmap(bg_scroll, tbWidth - bg_scroll.Width - 5, y);
            }
            DrawBitmap(btn_up, tbWidth - btn_up.Width, 0);
            DrawBitmap(btn_down, tbWidth - btn_down.Width, tbHeight - btn_down.Height);
            DrawBitmap(btn_scroll, tbWidth - btn_scroll.Width - 1, scrollButtonYLoc);

            //draw border for debug info
            gv.DrawRectangle(new IbRect(tbXloc, tbYloc - gv.oYshift, tbWidth, tbHeight), Color.DimGray, 1);
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
            //if (currentTopLineIndex < 0)
            //{
                //currentTopLineIndex = 0;
            //}
            if (currentTopLineIndex > logLinesList.Count - numberOfLinesToShow)
            {
                currentTopLineIndex = logLinesList.Count - numberOfLinesToShow;
            }
            if (currentTopLineIndex < 0)
            {
                currentTopLineIndex = 0;
            }
        }
        
        private bool isMouseWithinTextBox(MouseEventArgs e)
        {
            if ((e.X > tbXloc) && (e.X < tbWidth + tbXloc) && (e.Y > tbYloc) && (e.Y < tbHeight + tbYloc))
            {
                return true;
            }
            return false;
        }
        private bool isMouseWithinScrollBar(MouseEventArgs e)
        {
            if ((e.X > tbWidth + tbXloc - btn_up.Width) && (e.X < tbWidth + tbXloc) && (e.Y > tbYloc) && (e.Y < tbHeight + tbYloc))
            {
                return true;
            }
            return false;
        }

        public void onMouseWheel(object sender, MouseEventArgs e)
        {
            if (isMouseWithinTextBox(e))
            {
                // Update the drawing based upon the mouse wheel scrolling. 
                int numberOfTextLinesToMove = e.Delta * SystemInformation.MouseWheelScrollLines / 120;

                if (numberOfTextLinesToMove != 0)
                {
                    SetCurrentTopLineIndex(-numberOfTextLinesToMove);
                    //gv.Invalidate();
                    gv.Render(0);
                }
            }
        }
        public void onMouseDown(object sender, MouseEventArgs e)
        {
            moveDeltaY = 0;
            if (isMouseWithinScrollBar(e))
            {
                if (e.Y - tbYloc < scrollButtonYLoc)
                {
                    //if mouse is above scroll button, move up a bit
                    xLoc = 0;
                }
                else if (e.Y - tbYloc > scrollButtonYLoc + btn_scroll.Height)
                {
                    //if mouse is below scroll button, move down a bit
                    xLoc = 0;
                }
                else
                {
                    //if mouse is on scroll button, set moveScrollingStartY = e.Y
                    moveScrollingStartY = e.Y;
                    startY = scrollButtonYLoc;
                }
            }
            else if (isMouseWithinTextBox(e))
            {
                //moveScrollingStartY = e.Y;
            }
        }
        public void onMouseMove(object sender, MouseEventArgs e)
        {
            if (isMouseWithinScrollBar(e))
            {
                //if button is down and was on scroll button then move button and scroll text accordingly
                if (e.Button == System.Windows.Forms.MouseButtons.Left)
                {
                    moveEY = e.Y;
                    xLoc = 0;
                    moveDeltaY = e.Y - moveScrollingStartY;
                    //float multiplier = -(float)(tbHeight - btn_down.Height) / (float)(tbHeight - totalTextHeight);
                    //startYLocForTextDrawing = startY - (int)((float)(e.Y - moveScrollingStartY) / multiplier);
                    //gv.Invalidate();
                }
            }
            else if (isMouseWithinTextBox(e))
            {
                if (e.Button == System.Windows.Forms.MouseButtons.Left)
                {
                    //moveEY = e.Y;
                    //xLoc = 0;
                    //startYLocForTextDrawing = startY + (e.Y - moveScrollingStartY);
                    //gv.Invalidate();
                }
            }
        }
        public void onMouseUp(object sender, MouseEventArgs e)
        {
            moveDeltaY = 0;
            moveScrollingStartY = 0;
            if (isMouseWithinScrollBar(e))
            {
                //if click on top button, move 5 lines up
                if (e.Y < tbYloc + btn_up.Height)
                {
                    SetCurrentTopLineIndex(-5);
                    //currentTopLineIndex -= 5;
                    //if (currentTopLineIndex < 0) { currentTopLineIndex = 0; }
                    //if (currentTopLineIndex > logLinesList.Count - 1) { currentTopLineIndex = logLinesList.Count - 1; }
                    //gv.Invalidate();
                    gv.Render(0);
                }
                else if (e.Y > tbYloc + tbHeight - btn_down.Height)
                {
                    SetCurrentTopLineIndex(5);
                    //currentTopLineIndex += 5;
                    //if (currentTopLineIndex < 0) { currentTopLineIndex = 0; }
                    //if (currentTopLineIndex > logLinesList.Count - 1) { currentTopLineIndex = logLinesList.Count - 1; }
                    //gv.Invalidate();
                    gv.Render(0);
                }
            }
            else if (isMouseWithinTextBox(e))
            {
                //gv.Invalidate();
            }
        }
        */
    }
}
