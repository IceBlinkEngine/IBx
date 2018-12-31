using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace IBx
{
    public class IbbHtmlTextBox
    {
        public GameView gv;
        public List<string> tagStack = new List<string>();
        public List<FormattedLine> logLinesList = new List<FormattedLine>();
        float xLoc = 0;
        public int tbHeight = 200;
        public int tbWidth = 300;
        public int tbXloc = 10;
        public int tbYloc = 10;
        public float fontHeightToWidthRatio = 1.0f;
        public bool showBoxBorder = false;

        public IbbHtmlTextBox(GameView g, int locX, int locY, int width, int height)
        {
            gv = g;
            //fontfamily = gv.family;
            //font = new Font(fontfamily, 20.0f * (float)gv.squareSize / 100.0f);
            //font = gv.drawFontReg;
            tbXloc = locX;
            tbYloc = locY;
            tbWidth = width;
            tbHeight = height;
            //brush.Color = Color.Red;
        }
        public IbbHtmlTextBox(GameView g)
        {
            gv = g;
            //fontfamily = gv.family;
            //font = new Font(fontfamily, 20.0f * (float)gv.squareSize / 100.0f);
            //font = gv.drawFontReg;
            //brush.Color = Color.Red;
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
            if ((y > -10) && (y <= tbHeight - fontH))
            {
                gv.DrawText(text, x + tbXloc, y + tbYloc, fontHeight, fontColor, style, opacity, isUnderlined);
            }
        }

        public void AddHtmlTextToLog(string htmlText)
        {            
            htmlText = htmlText.Replace("\r\n", "<br>");
            htmlText = htmlText.Replace("\n\n", "<br>");
            htmlText = htmlText.Replace("\"", "'");

            if ((htmlText.EndsWith("<br>")) || (htmlText.EndsWith("<BR>")))
            {
                //ProcessHtmlString(htmlText, tbWidth);
                List<FormattedLine> linesList = gv.cc.ProcessHtmlString(htmlText, tbWidth, tagStack);
                foreach (FormattedLine fl in linesList)
                {
                    logLinesList.Add(fl);
                }
            }
            else
            {
                //ProcessHtmlString(htmlText + "<br>", tbWidth);
                List<FormattedLine> linesList = gv.cc.ProcessHtmlString(htmlText + "<br>", tbWidth, tagStack);
                foreach (FormattedLine fl in linesList)
                {
                    logLinesList.Add(fl);
                }
            }
        }
        

        public void onDrawLogBox()
        {
            //only draw lines needed to fill textbox
            float xLoc = 0;
            float yLoc = 0;
            //loop through 5 lines from current index point
            for (int i = 0; i < logLinesList.Count; i++)
            {
                //loop through each line and print each word
                foreach (FormattedWord word in logLinesList[i].wordsList)
                {
                    float fontsize = 0;
                    if (word.fontSize.Equals("small"))
                    {
                        fontsize = gv.drawFontSmallHeight;
                    }
                    else if (word.fontSize.Equals("large"))
                    {
                        fontsize = gv.drawFontLargeHeight;
                    }
                    else
                    {
                        fontsize = gv.drawFontRegHeight;
                    }
                    int difYheight = logLinesList[i].lineHeight - (int)fontsize;                    
                    DrawString(word.text + " ", xLoc, yLoc + difYheight, word.style, word.color, word.fontSize, 255, word.underlined);
                    xLoc += gv.MeasureString(word.text + " ", word.fontSize, word.style);
                }
                xLoc = 0;
                yLoc += logLinesList[i].lineHeight + (logLinesList[i].lineHeight / 4);
            }
        }        
    }
}
