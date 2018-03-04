﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace IBx
{
    public class ScreenJournal 
    {

	    //public gv.module gv.mod;
	    public GameView gv;
	    private int journalScreenQuestIndex = 0;
	    private int journalScreenEntryIndex = 0;	
	    private string journalBack;
	    private IbbButton btnReturnJournal = null;
	    public IbbButton ctrlUpArrow = null;
	    public IbbButton ctrlDownArrow = null;
	    public IbbButton ctrlLeftArrow = null;
	    public IbbButton ctrlRightArrow = null;
        private IbbHtmlTextBox description;
	
	    public ScreenJournal(Module m, GameView g)
	    {
		    //gv.mod = m;
		    gv = g;
	    }
	    public void setControlsStart()
	    {		
		    int pW = (int)((float)gv.screenWidth / 100.0f);
		    int pH = (int)((float)gv.screenHeight / 100.0f);
		    int padW = gv.squareSize/6;		
		    int xShift = gv.squareSize/2;
		    int yShift = gv.squareSize/2;

            description = new IbbHtmlTextBox(gv, 320, 100, 500, 300);
            description.showBoxBorder = false;

		    if (ctrlUpArrow == null)
		    {
			    ctrlUpArrow = new IbbButton(gv, 1.0f);
			    ctrlUpArrow.Img = "btn_small"; // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_small);
			    ctrlUpArrow.Img2 = "ctrl_up_arrow"; // BitmapFactory.decodeResource(gv.getResources(), R.drawable.ctrl_up_arrow);
			    ctrlUpArrow.Glow = "btn_small_glow"; // BitmapFactory.decodeResource(gv.getResources(), R.drawable.arrow_glow);
			    ctrlUpArrow.X = 12 * gv.squareSize;
			    ctrlUpArrow.Y = 1 * gv.squareSize + pH * 2;
                ctrlUpArrow.Height = (int)(gv.ibbheight * gv.screenDensity);
                ctrlUpArrow.Width = (int)(gv.ibbwidthR * gv.screenDensity);
		    }
		    if (ctrlDownArrow == null)
		    {
			    ctrlDownArrow = new IbbButton(gv, 1.0f);	
			    ctrlDownArrow.Img = "btn_small"; // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_small);
			    ctrlDownArrow.Img2 = "ctrl_down_arrow"; // BitmapFactory.decodeResource(gv.getResources(), R.drawable.ctrl_down_arrow);
			    ctrlDownArrow.Glow = "btn_small_glow"; // BitmapFactory.decodeResource(gv.getResources(), R.drawable.arrow_glow);
			    ctrlDownArrow.X = 12 * gv.squareSize;
			    ctrlDownArrow.Y = 2 * gv.squareSize + pH * 3;
                ctrlDownArrow.Height = (int)(gv.ibbheight * gv.screenDensity);
                ctrlDownArrow.Width = (int)(gv.ibbwidthR * gv.screenDensity);
		    }
		    if (ctrlLeftArrow == null)
		    {
			    ctrlLeftArrow = new IbbButton(gv, 1.0f);
			    ctrlLeftArrow.Img = "btn_small"; // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_small);
			    ctrlLeftArrow.Img2 = "ctrl_left_arrow"; // BitmapFactory.decodeResource(gv.getResources(), R.drawable.ctrl_left_arrow);
			    ctrlLeftArrow.Glow = "btn_small_glow"; // BitmapFactory.decodeResource(gv.getResources(), R.drawable.arrow_glow);
			    ctrlLeftArrow.X = 10 * gv.squareSize + xShift;
			    ctrlLeftArrow.Y = pH * 34;
                ctrlLeftArrow.Height = (int)(gv.ibbheight * gv.screenDensity);
                ctrlLeftArrow.Width = (int)(gv.ibbwidthR * gv.screenDensity);
		    }
		    if (ctrlRightArrow == null)
		    {
			    ctrlRightArrow = new IbbButton(gv, 1.0f);
			    ctrlRightArrow.Img = "btn_small"; // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_small);
			    ctrlRightArrow.Img2 = "ctrl_right_arrow"; // BitmapFactory.decodeResource(gv.getResources(), R.drawable.ctrl_right_arrow);
			    ctrlRightArrow.Glow = "btn_small_glow"; // BitmapFactory.decodeResource(gv.getResources(), R.drawable.arrow_glow);
			    ctrlRightArrow.X = 11 * gv.squareSize + pW * 2 + xShift;
			    ctrlRightArrow.Y = pH * 34;
                ctrlRightArrow.Height = (int)(gv.ibbheight * gv.screenDensity);
                ctrlRightArrow.Width = (int)(gv.ibbwidthR * gv.screenDensity);			
		    }				
		    if (btnReturnJournal == null)
		    {
			    btnReturnJournal = new IbbButton(gv, 1.2f);	
			    btnReturnJournal.Text = "RETURN";
			    btnReturnJournal.Img = "btn_large"; // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_large);
			    btnReturnJournal.Glow = "btn_large_glow"; // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_large_glow);
                btnReturnJournal.X = (gv.screenWidth / 2) - (int)(gv.ibbwidthL * gv.screenDensity / 2.0f);
			    btnReturnJournal.Y = 9 * gv.squareSize + pH * 2;
                btnReturnJournal.Height = (int)(gv.ibbheight * gv.screenDensity);
                btnReturnJournal.Width = (int)(gv.ibbwidthL * gv.screenDensity);			
		    }
		
	    }

        public void redrawJournal()
        {
    	    int pW = (int)((float)gv.screenWidth / 100.0f);
		    int pH = (int)((float)gv.screenHeight / 100.0f);
		
    	    int locY = pH * 5;
            int locX = 4 * gv.squareSize;
            int spacing = (int)gv.drawFontRegHeight + pH;
            int leftStartY = pH * 4;
    	    int tabStartY = pH * 40;
    	
    	    //IF BACKGROUND IS NULL, LOAD IMAGE
    	    if (journalBack == null)
    	    {
                journalBack = "journalback";
    	    }
    	    //IF BUTTONS ARE NULL, LOAD BUTTONS
    	    if (btnReturnJournal == null)
    	    {
    		    setControlsStart();
    	    }
    	
    	    //DRAW BACKGROUND IMAGE
            IbRect src = new IbRect(0, 0, gv.cc.GetFromBitmapList(journalBack).Width, gv.cc.GetFromBitmapList(journalBack).Height);
            IbRect dst = new IbRect(2 * gv.squareSize, 0, (gv.squaresInWidth - 4) * gv.squareSize, (gv.squaresInHeight - 1) * gv.squareSize);
            gv.DrawBitmap(gv.cc.GetFromBitmapList(journalBack), src, dst);
        
            //MAKE SURE NO OUT OF INDEX ERRORS
    	    if (gv.mod.partyJournalQuests.Count > 0)
    	    {
	    	    if (journalScreenQuestIndex >= gv.mod.partyJournalQuests.Count)
	    	    {
	    		    journalScreenQuestIndex = 0;
	    	    }    	
	    	    if (journalScreenEntryIndex >= gv.mod.partyJournalQuests[journalScreenQuestIndex].Entries.Count)
	    	    {
	    		    journalScreenEntryIndex = 0;
	    	    }
    	    }
			
    	    //DRAW QUESTS
            string color = "black";
		    gv.DrawText("Active Quests:", locX, locY += leftStartY, "black");
		    gv.DrawText("--------------", locX, locY += spacing, "black");
		    if (gv.mod.partyJournalQuests.Count > 0)
    	    {
                int minQuestNumber = journalScreenQuestIndex - 3;
                int maxQuestNumber = journalScreenQuestIndex + 3;
                if (minQuestNumber < 0)
                {
                    maxQuestNumber -= minQuestNumber;
                    if (maxQuestNumber > gv.mod.partyJournalQuests.Count -1)
                    {
                        maxQuestNumber = gv.mod.partyJournalQuests.Count - 1;
                    }
                    minQuestNumber = 0;
                }
                if (maxQuestNumber > gv.mod.partyJournalQuests.Count - 1)
                {
                    minQuestNumber -= (maxQuestNumber - (gv.mod.partyJournalQuests.Count - 1));
                    if (minQuestNumber < 0)
                    {
                        minQuestNumber = 0;
                    }
                    maxQuestNumber = gv.mod.partyJournalQuests.Count - 1;
                }

                for (int i = minQuestNumber; i <= maxQuestNumber;i++)
                {
                    if (journalScreenQuestIndex == i) { color = "lime"; }
                    else { color = "black"; }
                    gv.DrawText(gv.mod.partyJournalQuests[i].Name, locX, locY += spacing, color);
                }
            }
		
		    //DRAW QUEST ENTRIES
		    locY = tabStartY;
		    gv.DrawText("Quest Entry:", locX, locY, "black");
		    gv.DrawText("--------------", locX, locY += spacing, "black");	
		    if (gv.mod.partyJournalQuests.Count > 0)
    	    {
                //Description
                string textToSpan = "<font color='black'><i><b>" + gv.mod.partyJournalQuests[journalScreenQuestIndex].Entries[journalScreenEntryIndex].EntryTitle + "</b></i></font><br>";
                textToSpan += gv.mod.partyJournalQuests[journalScreenQuestIndex].Entries[journalScreenEntryIndex].EntryText;
	                            
                int yLoc = pH * 18;

                description.tbXloc = locX;
                description.tbYloc = locY + spacing;
                description.tbWidth = pW * 30;
                description.tbHeight = pH * 50;
                description.logLinesList.Clear();
                description.AddHtmlTextToLog(textToSpan);
                description.onDrawLogBox();
    	    }
		
		    //DRAW ALL CONTROLS
		    ctrlUpArrow.Draw();
		    ctrlDownArrow.Draw();
		    ctrlLeftArrow.Draw();
		    ctrlRightArrow.Draw();
		    btnReturnJournal.Draw();
        }

        public void onTouchJournal(int eX, int eY, MouseEventType.EventType eventType)
        {//1
            try
            {//2 
                ctrlUpArrow.glowOn = false;
                ctrlDownArrow.glowOn = false;
                ctrlLeftArrow.glowOn = false;
                ctrlRightArrow.glowOn = false;
                btnReturnJournal.glowOn = false;

                switch (eventType)
                {//3
                    case MouseEventType.EventType.MouseDown:
                    case MouseEventType.EventType.MouseMove:
                        int x = (int)eX;
                        int y = (int)eY;
                        if (ctrlUpArrow.getImpact(x, y))
                        {
                            ctrlUpArrow.glowOn = true;
                        }
                        else if (ctrlDownArrow.getImpact(x, y))
                        {
                            ctrlDownArrow.glowOn = true;
                        }
                        else if (ctrlLeftArrow.getImpact(x, y))
                        {
                            ctrlLeftArrow.glowOn = true;
                        }
                        else if (ctrlRightArrow.getImpact(x, y))
                        {
                            ctrlRightArrow.glowOn = true;
                        }
                        else if (btnReturnJournal.getImpact(x, y))
                        {
                            btnReturnJournal.glowOn = true;
                        }

                        break;

                    case MouseEventType.EventType.MouseUp:
                        x = (int)eX;
                        y = (int)eY;

                        ctrlUpArrow.glowOn = false;
                        ctrlDownArrow.glowOn = false;
                        ctrlLeftArrow.glowOn = false;
                        ctrlRightArrow.glowOn = false;
                        btnReturnJournal.glowOn = false;

                        if (ctrlUpArrow.getImpact(x, y))
                        {
                            if (journalScreenQuestIndex > 0)
                            {
                                journalScreenQuestIndex--;
                                journalScreenEntryIndex = gv.mod.partyJournalQuests[journalScreenQuestIndex].Entries.Count - 1;
                            }
                        }
                        else if (ctrlDownArrow.getImpact(x, y))
                        {
                            if (journalScreenQuestIndex < gv.mod.partyJournalQuests.Count - 1)
                            {
                                journalScreenQuestIndex++;
                                journalScreenEntryIndex = gv.mod.partyJournalQuests[journalScreenQuestIndex].Entries.Count - 1;
                            }
                        }
                        else if (ctrlLeftArrow.getImpact(x, y))
                        {
                            if (journalScreenEntryIndex > 0)
                            {
                                journalScreenEntryIndex--;
                            }
                        }
                        else if (ctrlRightArrow.getImpact(x, y))
                        {
                            if (journalScreenEntryIndex < gv.mod.partyJournalQuests[journalScreenQuestIndex].Entries.Count - 1)
                            {
                                journalScreenEntryIndex++;
                            }
                        }
                        else if (btnReturnJournal.getImpact(x, y))
                        {
                            gv.screenType = "main";
                            journalBack = null;
                            btnReturnJournal = null;
                            ctrlUpArrow = null;
                            ctrlDownArrow = null;
                            ctrlLeftArrow = null;
                            ctrlRightArrow = null;
                        }
                        break;
                }//3
            }//2
            catch
            { }
        }	    
    }
}
