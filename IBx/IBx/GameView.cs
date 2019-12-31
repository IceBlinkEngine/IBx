using System;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;
using Newtonsoft.Json;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using Xamarin.Forms;
using System.Threading.Tasks;
using System.Reflection;

namespace IBx
{
    public class GameView
    {
        public ContentPage cp;
        public SKCanvas canvas;
        public bool textFormatSet = false;
        public int elapsed = 0;
        public int elapsed2 = 0;
        public int elapsed3 = 30;
        public float screenDensity;
        public int screenWidth;
        public int screenHeight;
        public int squareSizeInPixels = 100;
        public int squareSize; //in dp (squareSizeInPixels * screenDensity)
        public int pS; // = squareSize / 10 ... used for small UI and text location adjustments based on squaresize
        public int squaresInWidth = 19;
        public int squaresInHeight = 10;
        public int ibbwidthL = 340;
        public int ibbwidthR = 100;
        public int ibbheight = 100;
        public int ibpwidth = 110;
        public int ibpheight = 170;
        public int playerOffset = 4;
        public int playerOffsetX = 9;
        //changed playerOffsetY from 4 to 5 in order to show map full screen (i.e. underneath UI, too)
        public int playerOffsetY = 5;
        public int oXshift = 0;
        public int oYshift = 35;
        public string mainDirectory;
        public bool showHotKeys = false;
        public int mousePositionX = 0;
        public int mousePositionY = 0;
        public bool moveTimerRuns = false;
        public float moveTimerCounter = 0;
        public int standardTokenSize = 100;
        public SKRect srcRect = new SKRect();
        public SKRect dstRect = new SKRect();
        public SKPaint drawPaint = new SKPaint();
        public SKPaint textPaint = new SKPaint();
        public SKRect textBounds = new SKRect();
                
        public string versionNum = "v1.00";
        public string fixedModule = "";
        public float drawFontRegHeight;
        public float drawFontLargeHeight;
        public float drawFontSmallHeight;
        public float drawFontRegWidth;
        public string screenType = "splash"; //launcher, title, moreGames, main, party, inventory, combatInventory, shop, journal, combat, combatCast, convo
        public AnimationState animationState = AnimationState.None;
        public int triggerIndex = 0;
        public int triggerPropIndex = 0;

        public IB2HtmlLogBox log;
        public IBminiMessageBox messageBox;
        public IbbHtmlTextBox drawTextBox;
        public bool showMessageBox = false;
        //public IBminiItemListSelector itemListSelector;
        public CommonCode cc;
        public Module mod;
        public ScriptFunctions sf;
        public PathFinderAreas pfa;
        public ScreenParty screenParty;
        public ScreenInventory screenInventory;
        public ScreenItemSelector screenItemSelector;
        public ScreenPortraitSelector screenPortraitSelector;
        public ScreenTokenSelector screenTokenSelector;
        public ScreenPcSelector screenPcSelector;
        public ScreenJournal screenJournal;
        public ScreenShop screenShop;
        public ScreenCastSelector screenCastSelector;
        public ScreenTraitUseSelector screenTraitUseSelector;
        public ScreenConvo screenConvo;
        public ScreenTitle screenTitle;
        public ScreenPcCreation screenPcCreation;
        public ScreenSpellLevelUp screenSpellLevelUp;
        public ScreenTraitLevelUp screenTraitLevelUp;
        public ScreenLauncher screenLauncher;
        public ScreenCombat screenCombat;
        public ScreenMainMap screenMainMap;
        public ScreenPartyBuild screenPartyBuild;
        public ScreenPartyRoster screenPartyRoster;
        public bool touchEnabled = true;
        //public WMPLib.WindowsMediaPlayer areaMusic;
        //public WMPLib.WindowsMediaPlayer areaSounds;
        //public WMPLib.WindowsMediaPlayer weatherSounds1;
        //public WMPLib.WindowsMediaPlayer weatherSounds2;
        //public WMPLib.WindowsMediaPlayer weatherSounds3;
       
        //public SoundPlayer soundPlayer = new SoundPlayer();
        //public Dictionary<string, Stream> oSoundStreams = new Dictionary<string, Stream>();
        //public System.Media.SoundPlayer playerButtonEnter = new System.Media.SoundPlayer();
        //public System.Media.SoundPlayer playerButtonClick = new System.Media.SoundPlayer();
       
        public string currentMainMusic = "";
        public string currentAmbientMusic = "";
        public string currentCombatMusic = "";

        //timers
        public Stopwatch gameTimerStopwatch = new Stopwatch();
        public long previousTime = 0;
        public bool stillProcessingGameLoop = false;
        public float fps = 0;
        public int MouseX = 0;
        public int MouseY = 0;
        public int reportFPScount = 0;
        public bool animationTimerOn = false;
        public long animationStartTime = 0;
        public int animationDelayTime = 0;

        //public Timer areaMusicTimer = new Timer();
        //public Timer areaSoundsTimer = new Timer();
        //public Timer weatherSounds1Timer = new Timer();
        //public Timer weatherSounds2Timer = new Timer();
        //public Timer weatherSounds3Timer = new Timer();

        public float floatPixMovedPerTick = 4f;
        public int realTimeTimerMilliSecondsEllapsed = 0;
        public int smoothMoveTimerLengthInMilliSeconds = 16;
        public int fullScreenEffectTimerMilliSecondsElapsedRain = 0;
        public int fullScreenEffectTimerMilliSecondsElapsedSnow = 0;
        public int fullScreenEffectTimerMilliSecondsElapsedSandstorm = 0;
        public int fullScreenEffectTimerMilliSecondsElapsedClouds = 0;
        public string rainType = "";
        public string cloudType = "";
        public string fogType = "";
        public string snowType = "";
        public string sandstormType = "";
        public int smoothMoveCounter = 0;
        public bool useLargeLayout = true;
        public bool aTimer = false;
        public bool a2Timer = false;

        public GameView(ContentPage conPage)
        {
            //InitializeComponent();
            cp = conPage;
            cc = new CommonCode(this);
            mod = new Module();

            
            //this.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.GameView_MouseWheel);
            mainDirectory = Directory.GetCurrentDirectory();
            CreateUserFolders();

            try
            {
                //playerButtonClick.SoundLocation = mainDirectory + "\\default\\NewModule\\sounds\\btn_click.wav";
                //playerButtonClick.Load();
            }
            catch (Exception ex) { errorLog(ex.ToString()); } 
            try
            {
                //playerButtonEnter.SoundLocation = mainDirectory + "\\default\\NewModule\\sounds\\btn_hover.wav";
                //playerButtonEnter.Load();
            }
            catch (Exception ex) { errorLog(ex.ToString()); }

            //this is the standard way, comment out the next 3 lines if manually forcing a screen resolution for testing UI layouts
            //this.WindowState = FormWindowState.Maximized;
            //this.Width = Screen.PrimaryScreen.Bounds.Width;
            //this.Height = Screen.PrimaryScreen.Bounds.Height;
            
            //for testing other screen sizes, manually enter a resolution here
            //typical resolutions: 1366x768, 1920x1080, 1280x1024, 1280x800, 1024x768, 800x600, 1440x900
            //this.Width = 1366;
            //this.Height = 768;

            //screenWidth = this.Width; //getResources().getDisplayMetrics().widthPixels;
            //screenHeight = this.Height; //getResources().getDisplayMetrics().heightPixels;
            screenWidth = App.ScreenWidth;
            screenHeight = App.ScreenHeight;

            float sqrW = (float)screenWidth / (squaresInWidth + 2f/10f);
            float sqrH = (float)screenHeight / (squaresInHeight + 3f/10f);
            if (sqrW > sqrH)
            {
                squareSize = (int)(sqrH);
            }
            else
            {
                squareSize = (int)(sqrW);
            }
            if ((squareSize >= 99) && (squareSize < 105))
            {
                squareSize = 100;
            }
            //makes it a straight number in any case
            //better for working with half fractions of it (two half ints always form a complete whole)
            //squareSize = 2 * (int)(squareSize / 2f);
            screenDensity = (float)squareSize / (float)squareSizeInPixels;
            oXshift = (screenWidth - (squareSize * squaresInWidth)) / 2;

            pS = squareSize / 10; //used for small UI and text location adjustments based on squaresize for consistent look on all devices/screen resolutions

            //InitializeRenderer(); //uncomment this for DIRECT2D ADDITIONS

            //CREATES A FONTFAMILY
            ResetFont();
            //ResetDirect2DFont();
                        
            log = new IB2HtmlLogBox(this);
            log.numberOfLinesToShow = 20;
            cc.addLogText("red", "screenDensity: " + screenDensity);
            cc.addLogText("fuchsia", "screenWidth: " + screenWidth);
            cc.addLogText("lime", "screenHeight: " + screenHeight);
            cc.addLogText("yellow", "squareSize: " + squareSize);
            cc.addLogText("yellow", "sqrW: " + sqrW);
            cc.addLogText("yellow", "sqrH: " + sqrH);
            cc.addLogText("yellow", "");
            cc.addLogText("red", "Welcome to IceBlink 2");
            cc.addLogText("fuchsia", "You can scroll this message log box, use mouse wheel or scroll bar");

            //TODO
            //setup messageBox defaults
            //messageBox = new IBminiMessageBox(this);
            //messageBox.currentLocX = 20;
            //messageBox.currentLocY = 10;
            //messageBox.numberOfLinesToShow = 17;
            //messageBox.tbWidth = 344;
            //messageBox.Width = 344;
            //messageBox.Height = 220;
            //messageBox.tbHeight = 212;
            //messageBox.setupIBminiMessageBox();
                        
            drawTextBox = new IbbHtmlTextBox(this, 320, 100, 500, 300);
            drawTextBox.showBoxBorder = false;

            setupMusicPlayers();
            
            if (fixedModule.Equals("")) //this is the IceBlink Engine app
            {
                screenLauncher = new ScreenLauncher(mod, this);
                screenLauncher.loadModuleFiles();
                screenType = "launcher";
            }
            else //this is a fixed module
            {
                mod = cc.LoadModule(fixedModule + "/" + fixedModule + ".mod", false);
                resetGame();
                cc.LoadSaveListItems();
                screenType = "title";
            }
            //gameTimer.Interval = 16; //~60 fps
            //gameTimer.Tick += new System.EventHandler(this.gameTimer_Tick);
            gameTimerStopwatch.Start();
            previousTime = gameTimerStopwatch.ElapsedMilliseconds;
            //gameTimer.Start();
        }

        public void onTimedEvent2()
        {
            //left slice
            //(x < screenWidth/2) && (screenWidth/2 - x <= screenHeight/2 - y && screenWidth/2 - x >= ((-1)*screenHeight/2 - y))

            bool isLeftSlice = false;
            bool isRightSlice = false;
            bool isTopSlice = false;
            bool isBottomSlice = false;

            if (mousePositionX <= (screenWidth / 2 + (int)(squareSize * 10f / 100f)))
            {
                if (mousePositionY <= (screenHeight / 2 + (int)(squareSize * 55f / 100f)))
                {
                    if (((screenWidth / 2 + (int)(squareSize * 10f / 100f)) - mousePositionX) >= ((screenHeight / 2 + (int)(squareSize * 55f / 100f)) - mousePositionY))
                    {
                        isLeftSlice = true;
                    }
                }
                else
                {
                    if (((screenWidth / 2 + (int)(squareSize * 10f / 100f)) - mousePositionX) >= ((-1) * ((screenHeight / 2 + (int)(squareSize * 55f / 100f)) - mousePositionY)))
                    {
                        isLeftSlice = true;
                    }
                }
            }

            //top slice
            if (!isLeftSlice)
            {
                if (mousePositionY <= (screenHeight / 2 + (int)(squareSize * 55f / 100f)))
                {
                    if (mousePositionX <= (screenWidth / 2 + (int)(squareSize * 10f / 100f)))
                    {
                        if (((screenHeight / 2 + (int)(squareSize * 55f / 100f)) - mousePositionY) >= ((screenWidth / 2 + (int)(squareSize * 10f / 100f)) - mousePositionX))
                        {
                            isTopSlice = true;
                        }
                    }
                    else
                    {
                        if (((screenHeight / 2 + (int)(squareSize * 55f / 100f)) - mousePositionY) >= ((-1) * ((screenWidth / 2 + (int)(squareSize * 10f / 100f)) - mousePositionX)))
                        {
                            isTopSlice = true;
                        }
                    }
                }
            }

            //right slice
            if (!isLeftSlice && !isTopSlice)
            {
                if (mousePositionX > (screenWidth / 2 + (int)(squareSize * 10f / 100f)))
                {
                    if (mousePositionY <= (screenHeight / 2 + (int)(squareSize * 55f / 100f)))
                    {
                        if ((((screenWidth / 2 + (int)(squareSize * 10f / 100f)) - mousePositionX) * (-1)) >= ((screenHeight / 2 + (int)(squareSize * 55f / 100f)) - mousePositionY))
                        {
                            isRightSlice = true;
                        }
                    }
                    else
                    {
                        if ((((screenWidth / 2 + (int)(squareSize * 10f / 100f)) - mousePositionX) * (-1)) >= ((-1) * ((screenHeight / 2 + (int)(squareSize * 55f / 100f)) - mousePositionY)))
                        {
                            isRightSlice = true;
                        }
                    }
                }
            }

            //botttom slice
            if (!isLeftSlice && !isRightSlice && !isTopSlice)
            {
                isBottomSlice = true;
            }

            //int x2 = mousePositionX + (int)(squareSize * 15f / 100f);
            int x2 = mousePositionX - (int)(squareSize * 10f / 100f);
            int y2 = mousePositionY - (int)(squareSize * 55f / 100f);

            int xDistanceFromCenter = x2 - (int)(screenWidth / 2f);
            if (xDistanceFromCenter < 0)
            {
                xDistanceFromCenter = xDistanceFromCenter * -1;
            }

            int yDistanceFromCenter = y2 - (int)(screenHeight / 2f);
            if (yDistanceFromCenter < 0)
            {
                yDistanceFromCenter = yDistanceFromCenter * -1;
            }

            float moveSpeed = 1;
            int rawValue = 0;
            if (yDistanceFromCenter >= xDistanceFromCenter)
            {
                rawValue = (int)(yDistanceFromCenter);
            }
            else
            {
                rawValue = (int)(xDistanceFromCenter);
            }
            /*
            if (rawValue <= 0.75f * squareSize)
            {
                moveSpeed = 0.4f;
            }
            if (rawValue >= 2.25 * squareSize)
            {
                moveSpeed = 1.75f;
            }
            mod.scrollModeSpeed = 0.5f * moveSpeed;
            */
            float disQ = (rawValue * 0.4f) / squareSize;
            if (disQ > 1.4f)
            {
                disQ = 1.4f;
            }
            //mod.scrollModeSpeed = 0.5f * (0.15f + disQ);
            mod.scrollModeSpeed = 0.7f * (0.15f + disQ);

            int gridX = x2 / squareSize;
            int gridY = y2 / squareSize;
            int actualx2 = mod.PlayerLocationX + (gridX - playerOffsetX);
            int actualy2 = mod.PlayerLocationY + (gridY - playerOffsetY);

            //up/north
            if (isTopSlice)
            //if (mod.PlayerLocationX == actualx2 && mod.PlayerLocationY - 1 >= actualy2)
            {
                //a2Timer.Stop();
                if (!moveTimerRuns)
                {

                    mod.doTriggerInspiteOfScrolling = false;
                    bool blockMoveBecausOfCurrentScrolling = false;

                    if (mod.useScrollingSystem)
                    {
                        if (mod.scrollingTimer != 100 && mod.scrollingTimer != 0)
                        {
                            blockMoveBecausOfCurrentScrolling = true;
                        }
                    }

                    blockMoveBecausOfCurrentScrolling = false;

                    if (!blockMoveBecausOfCurrentScrolling)
                    {
                        if (mod.useScrollingSystem)
                        {
                            //single press
                            if (!mod.isScrollingNow)
                            {
                                mod.isScrollingNow = true;
                                //mod.scrollingTimer = 100 + mod.scrollingOverhang2;
                                mod.scrollingTimer = 100;
                                mod.scrollingDirection = "up";
                                mod.doTriggerInspiteOfScrolling = true;
                                bool isTransition = cc.goNorth();
                                if (!isTransition)
                                {
                                    mod.breakActiveSearch = false;
                                    //mod.wasJustCalled = false;
                                    //if (!mod.wasJustCalled)
                                    //{
                                    //if (screenType == "main")
                                    //{
                                    screenMainMap.moveUp(true);
                                    //}
                                    //mod.wasJustCalled = true;
                                    //}
                                }
                            }
                            //continued press
                            //else if (moveDelay2())
                            else if (mod.scrollingTimer <= mod.lastScrollStep || mod.scrollingTimer >= 100)
                            {
                                mod.isScrollingNow = true;
                                mod.scrollingTimer = 100 + mod.scrollingOverhang2;
                                mod.scrollingOverhang2 = 0;
                                mod.scrollingDirection = "up";
                                mod.doTriggerInspiteOfScrolling = true;
                                bool isTransition = cc.goNorth();
                                if (!isTransition)
                                {
                                    mod.breakActiveSearch = false;
                                    //mod.wasJustCalled = false;
                                    //if (!mod.wasJustCalled)
                                    //{
                                    //if (screenType == "main")
                                    //{
                                    screenMainMap.moveUp(true);
                                    //}
                                    //mod.wasJustCalled = true;
                                    //}
                                }
                            }
                        }


                    }


                }
                //a2Timer.Start();
                //mod.scrollModeSpeed = 0.5f;
            }

            //down/south
            else if (isBottomSlice)
            //else if (mod.PlayerLocationX == actualx2 && mod.PlayerLocationY + 1 <= actualy2)
            //else if (rtn5.Equals("ctrlDownArrow"))

            {
                //a2Timer.Stop();
                if (!moveTimerRuns)
                {
                    mod.doTriggerInspiteOfScrolling = false;
                    bool blockMoveBecausOfCurrentScrolling = false;

                    if (mod.useScrollingSystem)
                    {
                        if (mod.scrollingTimer != 100 && mod.scrollingTimer != 0)
                        {
                            blockMoveBecausOfCurrentScrolling = true;
                        }
                    }

                    blockMoveBecausOfCurrentScrolling = false;

                    if (!blockMoveBecausOfCurrentScrolling)
                    {
                        if (mod.useScrollingSystem)
                        {
                            //single press
                            if (!mod.isScrollingNow)
                            {
                                mod.isScrollingNow = true;
                                //mod.scrollingTimer = 100 + mod.scrollingOverhang2;
                                mod.scrollingTimer = 100;
                                mod.scrollingDirection = "down";
                                mod.doTriggerInspiteOfScrolling = true;
                                bool isTransition = cc.goSouth();
                                if (!isTransition)
                                {
                                    mod.breakActiveSearch = false;
                                    //mod.wasJustCalled = false;
                                    //if (!mod.wasJustCalled)
                                    //{
                                    //if (screenType == "main")
                                    //{
                                    screenMainMap.moveDown(true);
                                    //}
                                    //mod.wasJustCalled = true;
                                    //}
                                }
                            }
                            //continued press
                            //else if (moveDelay2())
                            else if (mod.scrollingTimer <= mod.lastScrollStep || mod.scrollingTimer >= 100)
                            {
                                mod.isScrollingNow = true;
                                mod.scrollingTimer = 100 + mod.scrollingOverhang2;
                                mod.scrollingOverhang2 = 0;
                                mod.scrollingDirection = "down";
                                mod.doTriggerInspiteOfScrolling = true;
                                bool isTransition = cc.goSouth();
                                if (!isTransition)
                                {
                                    mod.breakActiveSearch = false;
                                    //mod.wasJustCalled = false;
                                    //if (!mod.wasJustCalled)
                                    //{
                                    //if (screenType == "main")
                                    //{
                                    screenMainMap.moveDown(true);
                                    //}
                                    //mod.wasJustCalled = true;
                                    //}
                                }
                            }
                        }


                    }

                }
                //a2Timer.Start();
                //mod.scrollModeSpeed = 0.5f;
            }

            //left/west
            else if (isLeftSlice)
            //else if (mod.PlayerLocationX - 1 >= actualx2 && mod.PlayerLocationY == actualy2)
            //else if (rtn.Equals("ctrlLeftArrow"))

            {
                //a2Timer.Stop();
                if (!moveTimerRuns)
                {
                    mod.doTriggerInspiteOfScrolling = false;
                    bool blockMoveBecausOfCurrentScrolling = false;

                    if (mod.useScrollingSystem)
                    {
                        if (mod.scrollingTimer != 100 && mod.scrollingTimer != 0)
                        {
                            blockMoveBecausOfCurrentScrolling = true;
                        }
                    }

                    blockMoveBecausOfCurrentScrolling = false;

                    if (!blockMoveBecausOfCurrentScrolling)
                    {
                        if (mod.useScrollingSystem)
                        {
                            //single press
                            if (!mod.isScrollingNow)
                            {
                                mod.isScrollingNow = true;
                                //mod.scrollingTimer = 100 + mod.scrollingOverhang2;
                                mod.scrollingTimer = 100;
                                mod.scrollingDirection = "left";
                                mod.doTriggerInspiteOfScrolling = true;
                                bool isTransition = cc.goWest();
                                if (!isTransition)
                                {
                                    mod.breakActiveSearch = false;
                                    //mod.wasJustCalled = false;
                                    //if (!mod.wasJustCalled)
                                    //{
                                    //if (screenType == "main")
                                    //{
                                    screenMainMap.moveLeft(true);
                                    //}
                                    //mod.wasJustCalled = true;
                                    //}
                                }
                            }
                            //continued press
                            //else if (moveDelay2())
                            else if (mod.scrollingTimer <= mod.lastScrollStep || mod.scrollingTimer >= 100)
                            {
                                mod.isScrollingNow = true;
                                mod.scrollingTimer = 100 + mod.scrollingOverhang2;
                                mod.scrollingOverhang2 = 0;
                                mod.scrollingDirection = "left";
                                mod.doTriggerInspiteOfScrolling = true;
                                bool isTransition = cc.goWest();
                                if (!isTransition)
                                {
                                    mod.breakActiveSearch = false;
                                    //mod.wasJustCalled = false;
                                    //if (!mod.wasJustCalled)
                                    //{
                                    //if (screenType == "main")
                                    //{
                                    screenMainMap.moveLeft(true);
                                    //}
                                    //mod.wasJustCalled = true;
                                    //}
                                }
                            }
                        }


                    }

                }
                //a2Timer.Start();
                //mod.scrollModeSpeed = 0.5f;
            }
            else if (isRightSlice)
            //else if (mod.PlayerLocationX + 1 <= actualx2 && mod.PlayerLocationY == actualy2)
            //else if (rtn.Equals("ctrlRightArrow"))

            {
                //a2Timer.Stop();
                if (!moveTimerRuns)
                {
                    mod.doTriggerInspiteOfScrolling = false;
                    bool blockMoveBecausOfCurrentScrolling = false;

                    if (mod.useScrollingSystem)
                    {
                        if (mod.scrollingTimer != 100 && mod.scrollingTimer != 0)
                        {
                            blockMoveBecausOfCurrentScrolling = true;
                        }
                    }

                    blockMoveBecausOfCurrentScrolling = false;

                    if (!blockMoveBecausOfCurrentScrolling)
                    {
                        if (mod.useScrollingSystem)
                        {
                            //single press
                            if (!mod.isScrollingNow)
                            {
                                mod.isScrollingNow = true;
                                //mod.scrollingTimer = 100 + mod.scrollingOverhang2;
                                mod.scrollingTimer = 100;
                                mod.scrollingDirection = "right";
                                mod.doTriggerInspiteOfScrolling = true;
                                bool isTransition = cc.goEast();
                                if (!isTransition)
                                {
                                    mod.breakActiveSearch = false;
                                    //mod.wasJustCalled = false;
                                    //if (!mod.wasJustCalled)
                                    //{
                                    //if (screenType == "main")
                                    //{
                                    screenMainMap.moveRight(true);
                                    //}
                                    //mod.wasJustCalled = true;
                                    //}
                                }
                            }
                            //continued press
                            //else if (moveDelay2())
                            else if (mod.scrollingTimer <= mod.lastScrollStep || mod.scrollingTimer >= 100)
                            {
                                mod.isScrollingNow = true;
                                mod.scrollingTimer = 100 + mod.scrollingOverhang2;
                                mod.scrollingOverhang2 = 0;
                                mod.scrollingDirection = "right";
                                mod.doTriggerInspiteOfScrolling = true;
                                bool isTransition = cc.goEast();
                                if (!isTransition)
                                {
                                    mod.breakActiveSearch = false;
                                    //mod.wasJustCalled = false;
                                    //if (!mod.wasJustCalled)
                                    //{
                                    //if (screenType == "main")
                                    //{
                                    screenMainMap.moveRight(true);
                                    //}
                                    //mod.wasJustCalled = true;
                                    //}
                                }
                            }
                        }


                    }

                }
                //a2Timer.Start();
                //mod.scrollModeSpeed = 0.5f;
            }

        }

        public void onTimedEvent()
        {
            //check for diretion (set direction on button down)
            //move code for each specific direction
            //int zuz = 0;
            //else if (gv.mod.currentArea.GetBlocked(gv.mod.PlayerLocationX + 1, gv.mod.PlayerLocationY, gv.mod.PlayerLocationX, gv.mod.PlayerLocationY, gv.mod.PlayerLastLocationX + gv.mod.lastXadjustment, gv.mod.PlayerLastLocationY + gv.mod.lastYadjustment) == false || gv.mod.currentArea.isOverviewMap)

            if (this.mod.scrollingDirection == "up")
            {


                mod.doTriggerInspiteOfScrolling = false;
                bool blockMoveBecausOfCurrentScrolling = false;

                if (mod.useScrollingSystem)
                {
                    if (mod.scrollingTimer != 100 && mod.scrollingTimer != 0)
                    {
                        blockMoveBecausOfCurrentScrolling = true;
                    }
                }

                blockMoveBecausOfCurrentScrolling = false;

                if (!blockMoveBecausOfCurrentScrolling)
                {
                    if (mod.useScrollingSystem)
                    {
                        //single press
                        if (!mod.isScrollingNow)
                        {
                            mod.isScrollingNow = true;
                            //gv.mod.scrollingTimer = 100 + gv.mod.scrollingOverhang2;
                            mod.scrollingTimer = 100;
                            mod.scrollingDirection = "up";
                            mod.doTriggerInspiteOfScrolling = true;
                            bool isTransition = cc.goNorth();
                            if (!isTransition)
                            {
                                //if (mod.currentArea.GetBlocked(mod.PlayerLocationX, mod.PlayerLocationY - 1, mod.PlayerLocationX, mod.PlayerLocationY, mod.PlayerLastLocationX + mod.lastXadjustment, mod.PlayerLastLocationY + mod.lastYadjustment) == false || mod.currentArea.isOverviewMap)
                                //{
                                mod.breakActiveSearch = false;
                                //gv.mod.wasJustCalled = false;
                                //if (!gv.mod.wasJustCalled)
                                //{
                                //if (gv.screenType == "main")
                                //{
                                screenMainMap.moveUp(true);
                                //}
                                //gv.mod.wasJustCalled = true;
                                //}
                                //}
                            }
                        }
                        //continued press
                        //else if (moveDelay2())
                        else if (mod.scrollingTimer <= mod.lastScrollStep || mod.scrollingTimer >= 100)
                        {
                            mod.isScrollingNow = true;
                            mod.scrollingTimer = 100 + mod.scrollingOverhang2;
                            mod.scrollingOverhang2 = 0;
                            mod.scrollingDirection = "up";
                            mod.doTriggerInspiteOfScrolling = true;
                            bool isTransition = cc.goNorth();
                            if (!isTransition)
                            {
                                //if (mod.currentArea.GetBlocked(mod.PlayerLocationX, mod.PlayerLocationY - 1, mod.PlayerLocationX, mod.PlayerLocationY, mod.PlayerLastLocationX + mod.lastXadjustment, mod.PlayerLastLocationY + mod.lastYadjustment) == false || mod.currentArea.isOverviewMap)
                                //{
                                mod.breakActiveSearch = false;
                                //gv.mod.wasJustCalled = false;
                                //if (!gv.mod.wasJustCalled)
                                //{
                                //if (gv.screenType == "main")
                                //{
                                screenMainMap.moveUp(true);
                                //}
                                //gv.mod.wasJustCalled = true;
                                //}
                                //}
                            }
                        }
                    }
                }


            }
            //down
            if (this.mod.scrollingDirection == "down")
            {
                mod.doTriggerInspiteOfScrolling = false;
                bool blockMoveBecausOfCurrentScrolling = false;

                if (mod.useScrollingSystem)
                {
                    if (mod.scrollingTimer != 100 && mod.scrollingTimer != 0)
                    {
                        blockMoveBecausOfCurrentScrolling = true;
                    }
                }

                blockMoveBecausOfCurrentScrolling = false;

                if (!blockMoveBecausOfCurrentScrolling)
                {
                    if (mod.useScrollingSystem)
                    {
                        //single press
                        if (!mod.isScrollingNow)
                        {
                            mod.isScrollingNow = true;
                            //gv.mod.scrollingTimer = 100 + gv.mod.scrollingOverhang2;
                            mod.scrollingTimer = 100;
                            mod.scrollingDirection = "down";
                            mod.doTriggerInspiteOfScrolling = true;
                            bool isTransition = cc.goSouth();
                            if (!isTransition)
                            {
                                //if (mod.currentArea.GetBlocked(mod.PlayerLocationX, mod.PlayerLocationY + 1, mod.PlayerLocationX, mod.PlayerLocationY, mod.PlayerLastLocationX + mod.lastXadjustment, mod.PlayerLastLocationY + mod.lastYadjustment) == false || mod.currentArea.isOverviewMap)
                                //{
                                mod.breakActiveSearch = false;
                                //gv.mod.wasJustCalled = false;
                                //if (!gv.mod.wasJustCalled)
                                //{
                                //if (gv.screenType == "main")
                                //{
                                screenMainMap.moveDown(true);
                                //}
                                //gv.mod.wasJustCalled = true;
                                //}
                                //}
                            }
                        }
                        //continued press
                        //else if (moveDelay2())
                        else if (mod.scrollingTimer <= mod.lastScrollStep || mod.scrollingTimer >= 100)
                        {
                            mod.isScrollingNow = true;
                            mod.scrollingTimer = 100 + mod.scrollingOverhang2;
                            mod.scrollingOverhang2 = 0;
                            mod.scrollingDirection = "down";
                            mod.doTriggerInspiteOfScrolling = true;
                            bool isTransition = cc.goSouth();
                            if (!isTransition)
                            {
                                //if (mod.currentArea.GetBlocked(mod.PlayerLocationX, mod.PlayerLocationY + 1, mod.PlayerLocationX, mod.PlayerLocationY, mod.PlayerLastLocationX + mod.lastXadjustment, mod.PlayerLastLocationY + mod.lastYadjustment) == false || mod.currentArea.isOverviewMap)
                                //{
                                mod.breakActiveSearch = false;
                                //gv.mod.wasJustCalled = false;
                                //if (!gv.mod.wasJustCalled)
                                //{
                                //if (gv.screenType == "main")
                                //{
                                screenMainMap.moveDown(true);
                                //}
                                //gv.mod.wasJustCalled = true;
                                //}
                                //}
                            }
                        }
                    }


                }
            }
            //left
            if (this.mod.scrollingDirection == "left")
            {



                mod.doTriggerInspiteOfScrolling = false;
                bool blockMoveBecausOfCurrentScrolling = false;

                if (mod.useScrollingSystem)
                {
                    if (mod.scrollingTimer != 100 && mod.scrollingTimer != 0)
                    {
                        blockMoveBecausOfCurrentScrolling = true;
                    }
                }

                blockMoveBecausOfCurrentScrolling = false;

                if (!blockMoveBecausOfCurrentScrolling)
                {
                    if (mod.useScrollingSystem)
                    {
                        //single press
                        if (!mod.isScrollingNow)
                        {
                            mod.isScrollingNow = true;
                            //gv.mod.scrollingTimer = 100 + gv.mod.scrollingOverhang2;
                            mod.scrollingTimer = 100;
                            mod.scrollingDirection = "left";
                            mod.doTriggerInspiteOfScrolling = true;
                            bool isTransition = cc.goWest();
                            if (!isTransition)
                            {
                                //if (mod.currentArea.GetBlocked(mod.PlayerLocationX - 1, mod.PlayerLocationY, mod.PlayerLocationX, mod.PlayerLocationY, mod.PlayerLastLocationX + mod.lastXadjustment, mod.PlayerLastLocationY + mod.lastYadjustment) == false || mod.currentArea.isOverviewMap)
                                //{
                                mod.breakActiveSearch = false;
                                //gv.mod.wasJustCalled = false;
                                //if (!gv.mod.wasJustCalled)
                                //{
                                //if (gv.screenType == "main")
                                //{
                                screenMainMap.moveLeft(true);
                                //}
                                //gv.mod.wasJustCalled = true;
                                //}
                                //}
                            }
                        }
                        //continued press
                        //else if (moveDelay2())
                        else if (mod.scrollingTimer <= mod.lastScrollStep || mod.scrollingTimer >= 100)
                        {
                            mod.isScrollingNow = true;
                            mod.scrollingTimer = 100 + mod.scrollingOverhang2;
                            mod.scrollingOverhang2 = 0;
                            mod.scrollingDirection = "left";
                            mod.doTriggerInspiteOfScrolling = true;
                            bool isTransition = cc.goWest();
                            if (!isTransition)
                            {
                                //if (mod.currentArea.GetBlocked(mod.PlayerLocationX - 1, mod.PlayerLocationY, mod.PlayerLocationX, mod.PlayerLocationY, mod.PlayerLastLocationX + mod.lastXadjustment, mod.PlayerLastLocationY + mod.lastYadjustment) == false || mod.currentArea.isOverviewMap)
                                //{
                                mod.breakActiveSearch = false;
                                //gv.mod.wasJustCalled = false;
                                //if (!gv.mod.wasJustCalled)
                                //{
                                //if (gv.screenType == "main")
                                //{
                                screenMainMap.moveLeft(true);
                                //}
                                //gv.mod.wasJustCalled = true;
                                //}
                                //}
                            }
                        }
                    }

                }
            }
            //right
            if (this.mod.scrollingDirection == "right")
            {

                mod.doTriggerInspiteOfScrolling = false;
                bool blockMoveBecausOfCurrentScrolling = false;

                if (mod.useScrollingSystem)
                {
                    if (mod.scrollingTimer != 100 && mod.scrollingTimer != 0)
                    {
                        blockMoveBecausOfCurrentScrolling = true;
                    }
                }

                blockMoveBecausOfCurrentScrolling = false;

                if (!blockMoveBecausOfCurrentScrolling)
                {
                    if (mod.useScrollingSystem)
                    {
                        //single press
                        if (!mod.isScrollingNow)
                        {
                            mod.isScrollingNow = true;
                            //gv.mod.scrollingTimer = 100 + gv.mod.scrollingOverhang2;
                            mod.scrollingTimer = 100;
                            mod.scrollingDirection = "right";
                            mod.doTriggerInspiteOfScrolling = true;
                            bool isTransition = cc.goEast();
                            if (!isTransition)
                            {
                                //if (mod.currentArea.GetBlocked(mod.PlayerLocationX + 1, mod.PlayerLocationY, mod.PlayerLocationX, mod.PlayerLocationY, mod.PlayerLastLocationX + mod.lastXadjustment, mod.PlayerLastLocationY + mod.lastYadjustment) == false || mod.currentArea.isOverviewMap)
                                //{
                                mod.breakActiveSearch = false;
                                //gv.mod.wasJustCalled = false;
                                //if (!gv.mod.wasJustCalled)
                                //{
                                //if (gv.screenType == "main")
                                //{
                                screenMainMap.moveRight(true);
                                //}
                                //gv.mod.wasJustCalled = true;
                                //}
                                //}
                            }
                        }
                        //continued press
                        //else if (moveDelay2())
                        else if (mod.scrollingTimer <= mod.lastScrollStep || mod.scrollingTimer >= 100)
                        {
                            mod.isScrollingNow = true;
                            mod.scrollingTimer = 100 + mod.scrollingOverhang2;
                            mod.scrollingOverhang2 = 0;
                            mod.scrollingDirection = "right";
                            mod.doTriggerInspiteOfScrolling = true;
                            bool isTransition = cc.goEast();
                            if (!isTransition)
                            {
                                //if (mod.currentArea.GetBlocked(mod.PlayerLocationX + 1, mod.PlayerLocationY, mod.PlayerLocationX, mod.PlayerLocationY, mod.PlayerLastLocationX + mod.lastXadjustment, mod.PlayerLastLocationY + mod.lastYadjustment) == false || mod.currentArea.isOverviewMap)
                                //{
                                mod.breakActiveSearch = false;
                                //gv.mod.wasJustCalled = false;
                                //if (!gv.mod.wasJustCalled)
                                //{
                                //if (gv.screenType == "main")
                                //{
                                screenMainMap.moveRight(true);
                                //}
                                //gv.mod.wasJustCalled = true;
                                //}
                                //}
                            }
                        }
                    }
                }
            }

        }

        public void createScreens()
	    {
		    sf = new ScriptFunctions(mod, this);
		    pfa = new PathFinderAreas(mod);
		    screenParty = new ScreenParty(mod, this);
		    screenInventory = new ScreenInventory(mod, this);
            screenItemSelector = new ScreenItemSelector(mod, this);
            screenPortraitSelector = new ScreenPortraitSelector(mod, this);
            screenTokenSelector = new ScreenTokenSelector(mod, this);
            screenPcSelector = new ScreenPcSelector(mod, this);
		    screenJournal = new ScreenJournal(mod, this);	
		    screenShop = new ScreenShop(mod, this);
		    screenCastSelector = new ScreenCastSelector(mod, this);
            screenTraitUseSelector = new ScreenTraitUseSelector(mod, this);
            screenConvo = new ScreenConvo(mod, this);		    
		    screenMainMap = new ScreenMainMap(mod, this);
            screenCombat = new ScreenCombat(mod, this);
            screenTitle = new ScreenTitle(mod, this);
		    screenPcCreation = new ScreenPcCreation(mod, this);
		    screenSpellLevelUp = new ScreenSpellLevelUp(mod, this);
		    screenTraitLevelUp = new ScreenTraitLevelUp(mod, this);		
		    screenLauncher = new ScreenLauncher(mod, this);
		    screenPartyBuild = new ScreenPartyBuild(mod, this);
            screenPartyRoster = new ScreenPartyRoster(mod,this);
	    }
        public void LoadStandardImages()
        {

            cc.downStairFlankShadowLeft = cc.LoadBitmap("downStairFlankShadowLeft");
            cc.downStairShadow = cc.LoadBitmap("downStairShadow");
            cc.downStairFlankShadowRight = cc.LoadBitmap("downStairFlankShadowRight");
            cc.bridgeShadow = cc.LoadBitmap("bridgeShadow");
            cc.highlight90 = cc.LoadBitmap("highlight90");
            cc.highlightGreen = cc.LoadBitmap("highlightGreen");
            cc.leftCurtain = cc.LoadBitmap("leftCurtain");
            cc.rightCurtain = cc.LoadBitmap("rightCurtain");
            cc.longShadow = cc.LoadBitmap("longShadow");
            cc.longShadowCorner = cc.LoadBitmap("longShadowCorner");
            cc.shortShadow = cc.LoadBitmap("shortShadow");
            cc.shortShadowCorner = cc.LoadBitmap("shortShadowCorner");
            cc.longShadowCornerHalf = cc.LoadBitmap("longShadowCornerHalf");
            cc.longShadowCornerHalfMirror = cc.LoadBitmap("longShadowCornerHalfMirror");
            cc.shortShadowCorner2 = cc.LoadBitmap("shortShadowCorner2");
            cc.smallStairNEMirror = cc.LoadBitmap("smallStairNEMirror");
            cc.smallStairNENormal = cc.LoadBitmap("smallStairNENormal");
            cc.corner3 = cc.LoadBitmap("corner3");
            cc.entranceLightNorth2 = cc.LoadBitmap("entranceLightNorth2");

            cc.tooHigh = cc.LoadBitmap("tooHigh");
            cc.tooDeep = cc.LoadBitmap("tooDeep");

            cc.btnIni = cc.LoadBitmap("btn_ini");
            cc.btnIniGlow = cc.LoadBitmap("btn_ini_glow");
            cc.walkPass = cc.LoadBitmap("walk_pass");
            cc.walkBlocked = cc.LoadBitmap("walk_block");
            //gehörtdiewelt
            cc.encounter_indicator = cc.LoadBitmap("encounter_indicator");
            cc.mandatory_conversation_indicator = cc.LoadBitmap("mandatory_conversation_indicator");
            cc.optional_conversation_indicator = cc.LoadBitmap("optional_conversation_indicator");
            cc.challengeHidden = cc.LoadBitmap("challengeHidden");
            cc.challengeSkull = cc.LoadBitmap("challengeSkull");
            cc.isChasingSymbol = cc.LoadBitmap("chasing");
            cc.swiftSymbol = cc.LoadBitmap("swift");
            cc.slowSymbol = cc.LoadBitmap("slow");

            cc.losBlocked = cc.LoadBitmap("los_block");
            cc.black_tile = cc.LoadBitmap("black_tile");
            cc.black_tile_NE = cc.LoadBitmap("black_tile_NE");
            cc.black_tile_NW = cc.LoadBitmap("black_tile_NW");
            cc.black_tile_SE = cc.LoadBitmap("black_tile_SW");
            cc.black_tile_SW = cc.LoadBitmap("black_tile_SE");
            cc.black_tile2 = cc.LoadBitmap("black_tile2");
            cc.turn_marker = cc.LoadBitmap("turn_marker");
            cc.pc_dead = cc.LoadBitmap("pc_dead");
            cc.pc_stealth = cc.LoadBitmap("pc_stealth");
            cc.offScreen = cc.LoadBitmap("offScreen");
            cc.offScreenBlack = cc.LoadBitmap("offScreenBlack");
            cc.offScreen5 = cc.LoadBitmap("offScreen5");
            cc.black_tile4 = cc.LoadBitmap("black_tile4");
            cc.black_tile5 = cc.LoadBitmap("black_tile5");
            cc.offScreen6 = cc.LoadBitmap("offScreen6");
            cc.offScreen7 = cc.LoadBitmap("offScreen7");
            cc.offScreenTrans = cc.LoadBitmap("offScreenTrans");
            cc.death_fx = cc.LoadBitmap("death_fx");
            cc.hitSymbol = cc.LoadBitmap("hit_symbol");
            cc.missSymbol = cc.LoadBitmap("miss_symbol");
            cc.highlight_green = cc.LoadBitmap("highlight_green");
            cc.highlight_red = cc.LoadBitmap("highlight_red");
            cc.tint_dawn = cc.LoadBitmap("tint_dawn");
            cc.tint_sunrise = cc.LoadBitmap("tint_sunrise");
            cc.tint_sunset = cc.LoadBitmap("tint_sunset");
            cc.tint_dusk = cc.LoadBitmap("tint_dusk");
            cc.tint_night = cc.LoadBitmap("tint_night");
            cc.night_tile_NW = cc.LoadBitmap("night_tile_NW");
            cc.night_tile_NE = cc.LoadBitmap("night_tile_NE");
            cc.night_tile_SE = cc.LoadBitmap("night_tile_SW");
            cc.night_tile_SW = cc.LoadBitmap("night_tile_SE");
            cc.light_torch = cc.LoadBitmap("light_torch");
            cc.prp_lightYellow = cc.LoadBitmap("prp_lightYellow");
            cc.prp_lightRed = cc.LoadBitmap("prp_lightRed");
            cc.prp_lightGreen = cc.LoadBitmap("prp_lightGreen");
            cc.prp_lightBlue = cc.LoadBitmap("prp_lightBlue");
            cc.prp_lightPurple = cc.LoadBitmap("prp_lightPurple");
            cc.prp_lightOrange = cc.LoadBitmap("prp_lightOrange");

            cc.light_torchOLD = cc.LoadBitmap("light_torchOLD");
            //off for now
            //cc.tint_rain = cc.LoadBitmap("tint_rain");
            cc.ui_portrait_frame = cc.LoadBitmap("ui_portrait_frame");
            cc.ui_bg_fullscreen = cc.LoadBitmap("ui_bg_fullscreen");
            cc.facing1 = cc.LoadBitmap("facing1");
            cc.facing2 = cc.LoadBitmap("facing2");
            cc.facing3 = cc.LoadBitmap("facing3");
            cc.facing4 = cc.LoadBitmap("facing4");
            cc.facing6 = cc.LoadBitmap("facing6");
            cc.facing7 = cc.LoadBitmap("facing7");
            cc.facing8 = cc.LoadBitmap("facing8");
            cc.facing9 = cc.LoadBitmap("facing9");
        }	
	    public void resetGame()
	    {
		    //mod = new Module();
		    mod = cc.LoadModule(mod.moduleName + ".mod", false);
            log.tagStack.Clear();
            log.logLinesList.Clear();
            log.currentTopLineIndex = 0;
            if (mod.useSmoothMovement == true)
            {
                //16 milliseconds a tick, equals - theoretically - about 60 FPS
                
                //these are the pix moved per tick, designed so that a square is traversed within realTimeTimerLengthInMilliSeconds 
                //update: actually as the 60 FPS are never reached, we will see little stops between prop moves with realtime timer on
                floatPixMovedPerTick = ((float)squareSize / 90f) * mod.allAnimationSpeedMultiplier;
                //IBMessageBox.Show(this, "floatPixMovedPerTick after first is:" + floatPixMovedPerTick.ToString());
                //due to a mistake of mine 4 pix were moved always beforehand, trying a dynamically calculated average of 7.5 pix now, increases speed by 90%
                //floatPixMovedPerTick = floatPixMovedPerTick / (((float)mod.realTimeTimerLengthInMilliSeconds / 1000f * 2f / 3f)) * 6.675f;
                floatPixMovedPerTick = floatPixMovedPerTick / ((1500f / 1000f * 2f / 3f)) * 6.675f;
                //IBMessageBox.Show(this, "floatPixMovedPerTick after second is is:" + floatPixMovedPerTick.ToString());
                //IBMessageBox.Show(this, "real time timer length is:" + realTimeTimerLengthInMilliSeconds.ToString());
            }

            //reset fonts
            ResetFont();
            //reset log number of lines based on the value from the Module's mod file
            log.numberOfLinesToShow = mod.logNumberOfLines;            
                        
		    mod.debugMode = false;
		    mod.loadAreas(this);
		    //mod.setCurrentArea(mod.startingArea, this);
            bool foundArea = mod.setCurrentArea(mod.startingArea, this);
            if (!foundArea)
            {
                //sf.MessageBoxHtml("Area: " + mod.startingArea + " does not exist in the module...check the spelling or make sure your are pointing to the correct starting area that you intended");
            }

            mod.PlayerLocationX = mod.startingPlayerPositionX;
		    mod.PlayerLocationY = mod.startingPlayerPositionY;
		    cc.title = cc.LoadBitmap("title");
            LoadStandardImages();
		    cc.LoadRaces();
		    cc.LoadPlayerClasses();
		    cc.LoadItems();
		    cc.LoadContainers();
		    cc.LoadShops();
		    cc.LoadEffects();
		    cc.LoadSpells();
		    cc.LoadTraits();
            cc.LoadFactions();
            cc.LoadWeathers();
            cc.LoadWeatherEffects();
		    cc.LoadCreatures();
		    cc.LoadEncounters();
		    cc.LoadJournal();

            //Add new methods for laoding entTiles and Tiles diretcly from the module file
            //already in via LaodEncounters();
            //

            //hurghj
            //if (!mod.useAllTileSystem)
            //{
            //    cc.LoadTileBitmapList();
            //}
				
		    foreach (Container c in mod.moduleContainersList)
            {
                c.initialContainerItemRefs.Clear();
                foreach (ItemRefs i in c.containerItemRefs)
                {
                    c.initialContainerItemRefs.Add(i.DeepCopy());
                }
            }
            foreach (Shop s in mod.moduleShopsList)
            {
                s.initialShopItemRefs.Clear();
                foreach (ItemRefs i in s.shopItemRefs)
                {
                    s.initialShopItemRefs.Add(i.DeepCopy());
                }
            }
            foreach (Area a in mod.moduleAreasObjects)
            {
                a.InitialAreaPropTagsList.Clear();
                foreach (Prop p in a.Props)
                {
                    a.InitialAreaPropTagsList.Add(p.PropTag);
                }            
            }
        
		    cc.nullOutControls();
            cc.setPanelsStart();
		    cc.setControlsStart();
            cc.setPortraitsStart();
		    cc.setToggleButtonsStart();
            //TODO log.ResetLogBoxUiBitmaps();

		    createScreens();
		    initializeSounds();
		
		    cc.LoadTestParty();

            //load all the message box helps/tutorials
            cc.stringStrength = cc.loadTextToString("MessageStrengthExplanation.txt");
            cc.stringDexterity = cc.loadTextToString("MessageDexterityExplanation.txt");
            cc.stringConstitution = cc.loadTextToString("MessageConstitutionExplanation.txt");
            cc.stringIntelligence = cc.loadTextToString("MessageIntelligenceExplanation.txt");
            cc.stringWisdom = cc.loadTextToString("MessageWisdomExplanation.txt");
            cc.stringCharisma = cc.loadTextToString("MessageCharismaExplanation.txt");
            cc.stringBeginnersGuide = cc.loadTextToString("MessageBeginnersGuide.txt");
            cc.stringPlayersGuide = cc.loadTextToString("MessagePlayersGuide.txt");
            cc.stringPcCreation = cc.loadTextToString("MessagePcCreation.txt");
            cc.stringMessageCombat = cc.loadTextToString("MessageCombat.txt");
            cc.stringMessageInventory = cc.loadTextToString("MessageInventory.txt");
            cc.stringMessageParty = cc.loadTextToString("MessageParty.txt");
            cc.stringMessageMainMap = cc.loadTextToString("MessageMainMap.txt");
        }

        private void ResetFont()
        {
            //loop for testing/debugging of resource names (uncomment the next four lines to use)
            /*foreach (var res in GetType().GetTypeInfo().Assembly.GetManifestResourceNames())
            {
                if (res.Contains("meta"))
                {
                    int x = 1;
                }
                System.Diagnostics.Debug.WriteLine("found resource: " + res);
            }*/

            textPaint.Color = SKColors.White;
            textPaint.TextSize = 60;
            textPaint.Typeface = SKTypeface.FromFamilyName("Arial");

            /*string assetName = "IBx.UWP.metamorphous_regular.ttf";
            //string assetName = "IBx.iOS.metamorphous_regular.ttf";
            //string assetName = "IBx.Droid.metamorphous_regular.ttf";

            using (var stream = GetType().GetTypeInfo().Assembly.GetManifestResourceStream(assetName))
            {
                using (var managedStream = new SKManagedStream(stream, true))
                {                    
                    textPaint.Color = SKColors.White;
                    textPaint.TextSize = 60;
                    textPaint.Typeface = SKTypeface.FromStream(managedStream);                    
                }
            }*/
            
            drawFontLargeHeight = (int)(ibbheight * screenDensity * mod.fontD2DScaleMultiplier * 0.5);
            drawFontRegHeight = (int)(ibbheight * screenDensity * mod.fontD2DScaleMultiplier * 0.333);
            drawFontSmallHeight = (int)(ibbheight * screenDensity * mod.fontD2DScaleMultiplier * 0.25);
            drawFontRegWidth = 10.0f;
        }

        //MUSIC and SOUNDS
        public void setupMusicPlayers()
        {
            CreateAreaMusicPlayer();
            /*try
            {
                areaMusic = new WMPLib.WindowsMediaPlayer();
                areaMusic.PlayStateChange += new WMPLib._WMPOCXEvents_PlayStateChangeEventHandler(AreaMusic_PlayStateChange);
                areaMusic.MediaError += new WMPLib._WMPOCXEvents_MediaErrorEventHandler(Player_MediaError);
                areaMusic.settings.setMode("Loop", true);
                areaMusic.settings.volume = 50;

                areaSounds = new WMPLib.WindowsMediaPlayer();
                areaSounds.PlayStateChange += new WMPLib._WMPOCXEvents_PlayStateChangeEventHandler(AreaSounds_PlayStateChange);
                areaSounds.MediaError += new WMPLib._WMPOCXEvents_MediaErrorEventHandler(Player_MediaError);
                areaSounds.settings.setMode("Loop", true);

                //for winds
                weatherSounds1 = new WMPLib.WindowsMediaPlayer();
                //weatherSounds1.PlayStateChange += new WMPLib._WMPOCXEvents_PlayStateChangeEventHandler(WeatherSounds1_PlayStateChange);
                weatherSounds1.MediaError += new WMPLib._WMPOCXEvents_MediaErrorEventHandler(Player_MediaError);
                weatherSounds1.settings.setMode("Loop", true);
                weatherSounds1.settings.volume = 50;
                //for rain
                weatherSounds2 = new WMPLib.WindowsMediaPlayer();
                //weatherSounds2.PlayStateChange += new WMPLib._WMPOCXEvents_PlayStateChangeEventHandler(WeatherSounds2_PlayStateChange);
                weatherSounds2.MediaError += new WMPLib._WMPOCXEvents_MediaErrorEventHandler(Player_MediaError);
                weatherSounds2.settings.setMode("Loop", true);
                weatherSounds2.settings.volume = 50;
                //for lightning
                weatherSounds3 = new WMPLib.WindowsMediaPlayer();
                weatherSounds3.settings.volume = 50;
                //channel 3 is for lightning, no loop needed
                //weatherSounds3.settings.setMode("Loop", true);

                startMusic();
                startAmbient();
            }
            catch (Exception ex)
            {
                cc.addLogText("red","Failed to setup Music Player...Audio will be disabled. Most likely due to not having Windows Media Player installed or having an incompatible version.");
                errorLog(ex.ToString());
            }*/
        }
        public void startMusic()
        {
            try
            {
                if (currentMainMusic.Equals(mod.currentArea.AreaMusic))
                {
                    PlayAreaMusic(mod.currentArea.AreaMusic);
                }
                else
                {
                    LoadAreaMusicFile("\\modules\\" + this.mod.moduleName + "\\music\\" + mod.currentArea.AreaMusic);
                    PlayAreaMusic(mod.currentArea.AreaMusic);
                }
            }
            catch (Exception ex)
            {
                cc.addLogText("red", "Failed on startMusic(): " + ex.ToString());
                errorLog(ex.ToString());
            }       
        }
        public void startAmbient()
        {
            /*try
            {
                if ((currentAmbientMusic.Equals(mod.currentArea.AreaSounds)) && (areaSounds != null))
                {
                    areaSounds.controls.play();
                }
                else
                {
                    areaSounds.controls.stop();

                    if (mod.currentArea.AreaSounds != "none")
                    {
                        if (File.Exists(mainDirectory + "\\modules\\" + mod.moduleName + "\\music\\" + mod.currentArea.AreaSounds))
                        {
                            areaSounds.URL = mainDirectory + "\\modules\\" + mod.moduleName + "\\music\\" + mod.currentArea.AreaSounds;
                        }
                        else if (File.Exists(mainDirectory + "\\modules\\" + mod.moduleName + "\\music\\" + mod.currentArea.AreaSounds + ".mp3"))
                        {
                            areaSounds.URL = mainDirectory + "\\modules\\" + mod.moduleName + "\\music\\" + mod.currentArea.AreaSounds + ".mp3";
                        }
                        else if (File.Exists(mainDirectory + "\\default\\NewModule\\music\\" + mod.currentArea.AreaSounds))
                        {
                            areaSounds.URL = mainDirectory + "\\default\\NewModule\\music\\" + mod.currentArea.AreaSounds;
                        }
                        else if (File.Exists(mainDirectory + "\\default\\NewModule\\music\\" + mod.currentArea.AreaSounds + ".mp3"))
                        {
                            areaSounds.URL = mainDirectory + "\\default\\NewModule\\music\\" + mod.currentArea.AreaSounds + ".mp3";
                        }
                        else
                        {
                            areaSounds.URL = "";
                        }
                        if (areaSounds.URL != "")
                        {
                            areaSounds.controls.stop();
                            areaSounds.controls.play();
                        }
                    }
                    else
                    {
                        areaSounds.URL = "";
                    }
                }
            }
            catch (Exception ex)
            {
                cc.addLogText("red", "Failed on startAmbient(): " + ex.ToString());
                errorLog(ex.ToString());
            }*/
        }
        public void startCombatMusic()
        {
            /*try
            {
                if ((currentCombatMusic.Equals(mod.currentEncounter.AreaMusic)) && (areaMusic != null))
                {
                    areaMusic.controls.play();
                }
                else
                {
                    areaMusic.controls.stop();
                    
                    if (mod.currentEncounter.AreaMusic != "none")
                    {
                        if (File.Exists(mainDirectory + "\\modules\\" + mod.moduleName + "\\music\\" + mod.currentEncounter.AreaMusic + ".mp3"))
                        {
                            areaMusic.URL = mainDirectory + "\\modules\\" + mod.moduleName + "\\music\\" + mod.currentEncounter.AreaMusic + ".mp3";
                        }
                        else if (File.Exists(mainDirectory + "\\modules\\" + mod.moduleName + "\\music\\" + mod.currentEncounter.AreaMusic))
                        {
                            areaMusic.URL = mainDirectory + "\\modules\\" + mod.moduleName + "\\music\\" + mod.currentEncounter.AreaMusic;
                        }
                        else if (File.Exists(mainDirectory + "\\default\\NewModule\\music\\" + mod.currentEncounter.AreaMusic + ".mp3"))
                        {
                            areaMusic.URL = mainDirectory + "\\default\\NewModule\\music\\" + mod.currentEncounter.AreaMusic + ".mp3";
                        }
                        else if (File.Exists(mainDirectory + "\\default\\NewModule\\music\\" + mod.currentEncounter.AreaMusic))
                        {
                            areaMusic.URL = mainDirectory + "\\default\\NewModule\\music\\" + mod.currentEncounter.AreaMusic;
                        }
                        else
                        {
                            areaMusic.URL = "";
                        }
                        if (areaMusic.URL != "")
                        {
                            areaMusic.controls.stop();
                            areaMusic.controls.play();
                        }
                    }
                    else
                    {
                        areaMusic.URL = "";
                    }
                }
            }
            catch (Exception ex)
            {
                cc.addLogText("red", "Failed on playCombatAreaMusicSounds()" + ex.ToString());
                errorLog(ex.ToString());
            }*/
        }
        private void AreaMusic_PlayStateChange(int NewState)
        {
            /*try
            {
                if ((WMPLib.WMPPlayState)NewState == WMPLib.WMPPlayState.wmppsStopped)
                {
                    delayMusic();
                }
            }
            catch (Exception ex)
            {
                cc.addLogText("Failed on AreaMusic_PlayStateChange()" + ex.ToString());
                errorLog(ex.ToString());
            }*/
        }
        private void AreaSounds_PlayStateChange(int NewState)
        {
            /*try
            {
                if ((WMPLib.WMPPlayState)NewState == WMPLib.WMPPlayState.wmppsStopped)
                {
                    delaySounds();
                }
            }
            catch (Exception ex)
            {
                cc.addLogText("Failed on AreaSounds_PlayStateChange()" + ex.ToString());
                errorLog(ex.ToString());
            }*/
        }
        private void Player_MediaError(object pMediaObject)
        {
            cc.addLogText("Cannot play media file.");
        }
        private void delayMusic()
        {
            /*try
            {
                int rand = sf.RandInt(mod.currentArea.AreaMusicDelayRandomAdder);
                areaMusicTimer.Enabled = false;
                areaMusic.controls.stop();
                areaMusicTimer.Interval = mod.currentArea.AreaMusicDelay + rand;
                areaMusicTimer.Enabled = true;
            }
            catch (Exception ex)
            {
                cc.addLogText("Failed on delayMusic()" + ex.ToString());
                errorLog(ex.ToString());
            }*/
        }
        private void areaMusicTimer_Tick(object sender, EventArgs e)
        {
            /*try
            {
                if (areaMusic.URL != "")
                {
                    areaMusic.controls.play();
                }
                areaMusicTimer.Enabled = false;
            }
            catch (Exception ex)
            {
                cc.addLogText("Failed on areaMusicTimer_Tick()" + ex.ToString());
                errorLog(ex.ToString());
            }*/
        }
        private void delaySounds()
        {
            /*try
            {
                int rand = sf.RandInt(mod.currentArea.AreaSoundsDelayRandomAdder);
                areaSoundsTimer.Enabled = false;
                areaSounds.controls.stop();
                areaSoundsTimer.Interval = mod.currentArea.AreaSoundsDelay + rand;
                areaSoundsTimer.Enabled = true;
            }
            catch (Exception ex)
            {
                cc.addLogText("Failed on delaySounds()" + ex.ToString());
                errorLog(ex.ToString());
            }*/
        }
        private void areaSoundsTimer_Tick(object sender, EventArgs e)
        {
            /*try
            {
                if (areaSounds.URL != "")
                {
                    areaSounds.controls.play();
                }
                areaSoundsTimer.Enabled = false;
            }
            catch (Exception ex)
            {
                cc.addLogText("Failed on areaSoundsTimer_Tick()" + ex.ToString());
                errorLog(ex.ToString());
            }*/
        }
	    public void stopMusic()
	    {
            StopAreaMusic();
            //areaMusic.controls.pause();
	    }
	    public void stopAmbient()
	    {
            //areaSounds.controls.pause();
	    }
	    public void stopCombatMusic()
	    {
            //areaMusic.controls.pause();
	    }
	    public void initializeSounds()
	    {
            /*oSoundStreams.Clear();
            string jobDir = "";
            jobDir = this.mainDirectory + "\\modules\\" + mod.moduleName + "\\sounds";
            foreach (string f in Directory.GetFiles(jobDir, "*.*", SearchOption.AllDirectories))
            {
                oSoundStreams.Add(Path.GetFileNameWithoutExtension(f), File.OpenRead(Path.GetFullPath(f)));
            }*/
	    }
	    /*public void PlaySound(string filenameNoExtension)
	    {            
            if ((filenameNoExtension.Equals("none")) || (filenameNoExtension.Equals("")) || (!mod.playSoundFx))
            {
                //play nothing
                return;
            }
            else
            {
                try
                {
                    soundPlayer.Stream = oSoundStreams[filenameNoExtension];
                    soundPlayer.Play();
                }
                catch (Exception ex)
                {
                    errorLog(ex.ToString());
                    if (mod.debugMode) //SD_20131102
                    {
                        cc.addLogText("<font color='yellow'>failed to play sound" + filenameNoExtension + "</font><BR>");
                    }
                    initializeSounds();
                }
            }            
	    }*/
       
        //ANIMATION TIMER STUFF
        public void postDelayed(string type, int delay)
        {
            animationTimerOn = true;
            animationStartTime = gameTimerStopwatch.ElapsedMilliseconds; //get the current total amount of ms since the game launched
            if (delay < 1)
            {
                delay = 1;
            }
            if (mod.useCombatSmoothMovement)
            {
                delay = 1;
            }
            animationDelayTime = delay;
            /*if (type.Equals("doAnimation"))
            {
                animationTimer.Enabled = true;
                if (delay < 1)
                {
                    delay = 1;
                }
                animationTimer.Interval = delay;
                //animationTimer.Interval = 1;
                animationTimer.Start();
            }*/
        }
        private void AnimationTimer_Tick()
        {
            //screenCombat.blockAnimationBridge = false;
            if ((!screenCombat.blockAnimationBridge) || (!mod.useCombatSmoothMovement))
            {
                animationTimerOn = false;
                screenCombat.doAnimationController();
            }
            else
            {
                animationTimerOn = true;
                animationStartTime = gameTimerStopwatch.ElapsedMilliseconds; //get the current total amount of ms since the game launched
            }
        }        

        //GAME LOOP
        public void gameTimer_Tick(SKCanvasView sk_canvas)
        {
            if (!stillProcessingGameLoop)
            {
                squaresInHeight = screenHeight / squareSize;
                squaresInWidth = screenWidth / squareSize;
                double rawOffsetX = squaresInWidth / 2;
                double rawOffsetY = squaresInHeight / 2;
                double rawOffset = squaresInHeight / 2 - 1;
                playerOffset = (int)Math.Ceiling(rawOffset);
                playerOffsetX = (int)Math.Ceiling(rawOffsetX);

                playerOffsetY = (int)Math.Ceiling(rawOffsetY);
                stillProcessingGameLoop = true; //starting the game loop so do not allow another tick call to run until finished with this tick call.
                long current = gameTimerStopwatch.ElapsedMilliseconds; //get the current total amount of ms since the game launched
                elapsed = (int)(current - previousTime); //calculate the total ms elapsed since the last time through the game loop
                if (animationTimerOn) //do combat animation stuff
                {
                    long timePassed = current - animationStartTime;
                    if (timePassed > animationDelayTime)
                    {                        
                        AnimationTimer_Tick();
                    }
                }
                if (aTimer)
                {
                    onTimedEvent();
                }
                if (a2Timer)
                {
                    onTimedEvent2();
                }
                Update(elapsed); //runs AI and physics
                sk_canvas.InvalidateSurface(); //draw the screen frame
                if (reportFPScount >= 10)
                {
                    reportFPScount = 0;
                    fps = 1000 / (current - previousTime);
                }
                reportFPScount++;
                previousTime = current; //remember the current time at the beginning of this tick call for the next time through the game loop to calculate elapsed time
                stillProcessingGameLoop = false; //finished game loop so okay to let the next tick call enter the game loop      
            }
        }
        private void Update(int elapsed)
        {            
            //iterate through spriteList and handle any sprite location and animation frame calculations
            if (screenType.Equals("main"))
            {
                screenMainMap.Update(elapsed);
            }
            else if (screenType.Equals("combat"))
            {
                screenCombat.Update(elapsed);
            }
        }

        //DRAW ROUTINES
        //new, for all stuff outlined using rect
        public void DrawTextOutlinedRect(string text, IbRect rect, float scaler, string fontColor)
        {
            DrawTextOutlined(text, rect.Left, rect.Top, "normal", "regular", fontColor, false);
        }
        public void DrawTextCenterOutlinedRect(string text, IbRect rect, float scaler, string fontColor)
        {
            DrawTextOutlined(text, rect.Left, rect.Top, "normal", "regular", fontColor, false);
        }
        public void DrawTextLeftOutlinedRect(string text, IbRect rect, float scaler, string fontColor)
        {
            DrawTextOutlined(text, rect.Left, rect.Top, "normal", "regular", fontColor, false);
        }

        //new, for all stuff outlined using locx, locy
        public void DrawTextOutlined(string text, float xLoc, float yLoc, float scaler, string fontColor)
        {
            DrawTextOutlined(text, xLoc, yLoc, "normal", "regular", fontColor, false);
        }

        //our new method for everything outlined
        public void DrawTextOutlined(string text, float x, float y, string style, string size, string color, bool isUnderlined)
        {
            for (int xx = -2; xx <= 2; xx++)
            {
                for (int yy = -2; yy <= 2; yy++)
                {
                    DrawText(text, x + xx, y + yy, size, "black", style, 255, isUnderlined);
                }
            }
            DrawText(text, x, y, size, color, style, 255, isUnderlined);
        }

        public void DrawText(string text, IbRect rect, string fontColor)
        {
            if (fontColor.Equals("black"))
            {
                text = "<font color = 'black'>" + text + "</font>";
            }
            else if (fontColor.Equals("red"))
            {
                text = "<font color = 'red'>" + text + "</font>";
            }
            else if (fontColor.Equals("blue"))
            {
                text = "<font color = 'blue'>" + text + "</font>";
            }
            else if (fontColor.Equals("green"))
            {
                text = "<font color = 'green'>" + text + "</font>";
            }
            else if (fontColor.Equals("yellow"))
            {
                text = "<font color = 'yellow'>" + text + "</font>";
            }
            drawTextBox.tbXloc = rect.Left;
            drawTextBox.tbYloc = rect.Top;
            drawTextBox.tbWidth = rect.Width;
            drawTextBox.tbHeight = rect.Height;
            drawTextBox.logLinesList.Clear();
            drawTextBox.AddHtmlTextToLog(text);
            drawTextBox.onDrawLogBox();
            //DrawText(text, rect, FontWeight.Normal, SharpDX.DirectWrite.FontStyle.Normal, scaler, fontColor);
        }
        public void DrawText(string text, float xLoc, float yLoc)
        {
            DrawText(text, xLoc, yLoc, "regular", "white", "normal", 255, false);
        }
        public void DrawText(string text, float xLoc, float yLoc, string color)
        {
            DrawText(text, xLoc, yLoc, "regular", color, "normal", 255, false);
        }
        public void DrawText(string text, float xLoc, float yLoc, float scaler, string color)
        {
            DrawText(text, xLoc, yLoc, "regular", color, "normal", 255, false);
        }
        public void DrawText(string text, float xLoc, float yLoc, string size, string color)
        {
            DrawText(text, xLoc, yLoc, size, color, "normal", 255, false);
        }
        public void DrawText(string text, float xLoc, float yLoc, string size, string color, string style)
        {
            DrawText(text, xLoc, yLoc, size, color, style, 255, false);
        }
        public void DrawText(string text, float xLoc, float yLoc, string size, string color, string style, byte opacity, bool isUnderlined)
        {
            //underline is no longer available in skiaSharp. Google removed it from skia
            //From Google: "In Skia the text decorations(underline and strike - through) only affect drawText calls,
            //and this is done mostly just for backward compatibility and will probably be removed soon.
            //Text decoration can get quite complex with layout, and since different users will disagree 
            //about what the behavior should be it's best that the user do the drawing."
            
            //TODO So we in IB will need to create our own underlining function by drawing a line under text after getting text bounds

            //Font size  (small, regular, large)          
            if (size.Equals("small"))
            {
                textPaint.TextSize = drawFontSmallHeight;
            }
            else if (size.Equals("large"))
            {
                textPaint.TextSize = drawFontLargeHeight;
            }
            else
            {
                textPaint.TextSize = drawFontRegHeight;
            }

            if (textPaint.Typeface == null)
            {
                textPaint.Typeface = SKTypeface.FromFamilyName("Arial");
            }
            //Font Style (bold, bolditalic, italic, normal)
            if (style.Equals("bold"))
            {
                textPaint.Typeface = SKTypeface.FromTypeface(textPaint.Typeface, SKTypefaceStyle.Bold);
            }
            else if (style.Equals("italic"))
            {
                textPaint.Typeface = SKTypeface.FromTypeface(textPaint.Typeface, SKTypefaceStyle.Italic);
            }
            else if (style.Equals("bolditalic"))
            {
                textPaint.Typeface = SKTypeface.FromTypeface(textPaint.Typeface, SKTypefaceStyle.BoldItalic);
            }
            else
            {
                textPaint.Typeface = SKTypeface.FromTypeface(textPaint.Typeface, SKTypefaceStyle.Normal);
            }

            //color            
            if (color.Equals("red"))
            {
                textPaint.Color = SKColors.Red.WithAlpha(opacity);
            }
            else if (color.Equals("lime"))
            {
                textPaint.Color = SKColors.Lime.WithAlpha(opacity);
            }
            else if (color.Equals("black"))
            {
                textPaint.Color = SKColors.Black.WithAlpha(opacity);
            }
            else if (color.Equals("white"))
            {
                textPaint.Color = SKColors.White.WithAlpha(opacity);
            }
            else if (color.Equals("silver"))
            {
                textPaint.Color = SKColors.Silver.WithAlpha(opacity);
            }
            else if (color.Equals("grey"))
            {
                textPaint.Color = SKColors.DimGray.WithAlpha(opacity);
            }
            else if (color.Equals("aqua"))
            {
                textPaint.Color = SKColors.Aqua.WithAlpha(opacity);
            }
            else if (color.Equals("fuchsia"))
            {
                textPaint.Color = SKColors.Fuchsia.WithAlpha(opacity);
            }
            else if (color.Equals("yellow"))
            {
                textPaint.Color = SKColors.Yellow.WithAlpha(opacity);
            }
            else if (color.Equals("magenta"))
            {
                textPaint.Color = SKColors.Magenta.WithAlpha(opacity);
            }
            else if (color.Equals("green"))
            {
                textPaint.Color = SKColors.Green.WithAlpha(opacity);
            }
            else if (color.Equals("gray"))
            {
                textPaint.Color = SKColors.Gray.WithAlpha(opacity);
            }            
            else
            {
                textPaint.Color = SKColors.White.WithAlpha(opacity);
            }

            canvas.DrawText(text, xLoc + oXshift, yLoc + oYshift + textPaint.TextSize, textPaint);
        }
        public float MeasureString(string text)
        {            
            return MeasureString(text, "regular", "normal");
        }
        public float MeasureString(string text, string size, string style)
        {
            if (text.Equals(""))
            {
                return 0f;
            }
            // Measure string width.
            //Font size  (small, regular, large)          
            if (size.Equals("small"))
            {
                textPaint.TextSize = drawFontSmallHeight;
            }
            else if (size.Equals("large"))
            {
                textPaint.TextSize = drawFontLargeHeight;
            }
            else
            {
                textPaint.TextSize = drawFontRegHeight;
            }

            if (textPaint.Typeface == null)
            {
                textPaint.Typeface = SKTypeface.FromFamilyName("Arial");
            }
            //Font Style (bold, bolditalic, italic, normal)
            if (style.Equals("bold"))
            {
                textPaint.Typeface = SKTypeface.FromTypeface(textPaint.Typeface, SKTypefaceStyle.Bold);
            }
            else if (style.Equals("italic"))
            {
                textPaint.Typeface = SKTypeface.FromTypeface(textPaint.Typeface, SKTypefaceStyle.Italic);
            }
            else if (style.Equals("bolditalic"))
            {
                textPaint.Typeface = SKTypeface.FromTypeface(textPaint.Typeface, SKTypefaceStyle.BoldItalic);
            }
            else
            {
                textPaint.Typeface = SKTypeface.FromTypeface(textPaint.Typeface, SKTypefaceStyle.Normal);
            }
            return textPaint.MeasureText(text);
            
        }
        public CoordinateF MeasureStringSize(string text, string size, string style)
        {
            if (text.Equals(""))
            {
                return new CoordinateF(0, 0);
            }
            // Measure string width.
            //Font size  (small, regular, large)   
            try
            {
                if (size.Equals("small"))
                {
                    textPaint.TextSize = drawFontSmallHeight;
                }
                else if (size.Equals("large"))
                {
                    textPaint.TextSize = drawFontLargeHeight;
                }
                else
                {
                    textPaint.TextSize = drawFontRegHeight;
                }

                if (textPaint.Typeface == null)
                {
                    textPaint.Typeface = SKTypeface.FromFamilyName("Arial");
                }
                //Font Style (bold, bolditalic, italic, normal)
                if (style.Equals("bold"))
                {
                    textPaint.Typeface = SKTypeface.FromTypeface(textPaint.Typeface, SKTypefaceStyle.Bold);
                }
                else if (style.Equals("italic"))
                {
                    textPaint.Typeface = SKTypeface.FromTypeface(textPaint.Typeface, SKTypefaceStyle.Italic);
                }
                else if (style.Equals("bolditalic"))
                {
                    textPaint.Typeface = SKTypeface.FromTypeface(textPaint.Typeface, SKTypefaceStyle.BoldItalic);
                }
                else
                {
                    textPaint.Typeface = SKTypeface.FromTypeface(textPaint.Typeface, SKTypefaceStyle.Normal);
                }
                textPaint.MeasureText(text, ref textBounds);
                CoordinateF returnSize = new CoordinateF(textBounds.Width, textBounds.Height);
                return returnSize;
            }
            catch
            {
                return new CoordinateF(0, 0);
            }
            
        }

        public void DrawRectangle(IbRect rect, SKColor penColor, int penWidth)
        {
            using (SKPaint skp = new SKPaint())
            {
                SKRect SKRectangle = new SKRect();
                rect.Left = rect.Left + oXshift;
                rect.Top = rect.Top + oYshift;

                skp.IsAntialias = true;
                skp.Style = SKPaintStyle.Stroke;
                skp.Color = penColor;
                skp.StrokeWidth = penWidth;
                canvas.DrawRect(SKRectangle, skp);
            }
        }
        public void DrawLine(int lastX, int lastY, int nextX, int nextY, string penColor, int penWidth)
        {
            using (SKPaint skp = new SKPaint())
            {
                skp.IsAntialias = true;
                skp.Style = SKPaintStyle.Stroke;
                if (penColor == "green")
                {
                    skp.Color = SKColors.Lime;
                }
                else if (penColor == "red")
                {
                    skp.Color = SKColors.Red;
                }
                else
                {
                    skp.Color = SKColors.White;
                }
                skp.StrokeWidth = penWidth;
                canvas.DrawLine(lastX + oXshift, lastY + oYshift, nextX + oXshift, nextY + oYshift, skp);
                //renderTarget2D.DrawLine(new Vector2(lastX + oXshift, lastY + oYshift), new Vector2(nextX + oXshift, nextY + oYshift), scb, penWidth);
            }
        }
        
        public void DrawBitmap(SKBitmap bitmap, IbRect source, IbRect target)
        {
            DrawBitmap(bitmap, source, target, 0f, false, 1.0f, 0, 0, 1, 1);
        }
        public void DrawBitmap(SKBitmap bitmap, IbRectF source, IbRectF target)
        {
            DrawBitmap(bitmap, source, target, 0f, false, 1.0f, 0, 0, 1, 1);
        }
        public void DrawBitmap(SKBitmap bitmap, IbRect source, IbRect target, bool mirror)
        {
            DrawBitmap(bitmap, source, target, 0f, mirror, 1.0f, 0, 0, 1, 1);
        }
        public void DrawBitmap(SKBitmap bitmap, IbRect source, IbRect target, bool mirror, bool isParty)
        {
            //chnaged to recognize party being drawn
            //TODO add isParty part
            DrawBitmap(bitmap, source, target, 0f, mirror, 1.0f, 0, 0, 1, 1);
        }
        public void DrawBitmap(SKBitmap bitmap, IbRectF source, IbRectF target, bool mirror)
        {
            DrawBitmap(bitmap, source, target, 0f, mirror, 1.0f, 0, 0, 1, 1);
        }
        public void DrawBitmap(SKBitmap bitmap, IbRect source, IbRect target, int angleInDegrees, bool mirror)
        {
            DrawBitmap(bitmap, source, target, angleInDegrees, mirror, 1.0f, 0, 0, 1, 1);
        }
        public void DrawBitmap(SKBitmap bitmap, IbRectF source, IbRectF target, int angleInDegrees, bool mirror)
        {
            DrawBitmap(bitmap, source, target, angleInDegrees, mirror, 1.0f, 0, 0, 1, 1);
        }
        public void DrawBitmap(SKBitmap bitmap, IbRect source, IbRect target, float angleInRadians, bool mirror)
        {
            float angleInDegrees = (angleInRadians * 360.0f) / (float)(Math.PI * 2);
            DrawBitmap(bitmap, source, target, angleInDegrees, mirror, 1.0f, 0, 0, 1, 1);
        }
        public void DrawBitmap(SKBitmap bitmap, IbRect source, IbRect target, float angleInRadians, bool mirror, float opacity)
        {
            float angleInDegrees = (angleInRadians * 360.0f) / (float)(Math.PI * 2);
            DrawBitmap(bitmap, source, target, angleInDegrees, mirror, opacity, 0, 0, 1, 1);
        }
        public void DrawBitmap(SKBitmap bitmap, IbRect source, IbRect target, float angleInRadians, bool mirror, float opacity, bool isParty)
        {
            float angleInDegrees = (angleInRadians * 360.0f) / (float)(Math.PI * 2);
            DrawBitmap(bitmap, source, target, angleInDegrees, mirror, opacity, 0, 0, 1, 1);
        }
        public void DrawBitmap(SKBitmap bitmap, IbRect source, IbRect target, int angleInDegrees, bool mirror, int Xshift, int Yshift)
        {
            DrawBitmap(bitmap, source, target, angleInDegrees, mirror, 1.0f, Xshift, Yshift, 1, 1);
        }
        public void DrawBitmap(SKBitmap bitmap, IbRect source, IbRect target, float angleInRadians, bool mirror, int Xshift, int Yshift)
        {
            float angleInDegrees = (angleInRadians * 360.0f) / (float)(Math.PI * 2);
            DrawBitmap(bitmap, source, target, angleInDegrees, mirror, 1.0f, Xshift, Yshift, 1, 1);
        }
        public void DrawBitmap(SKBitmap bitmap, IbRect source, IbRect target, int angleInDegrees, bool mirror, int Xshift, int Yshift, int Xscale, int Yscale)
        {
            DrawBitmap(bitmap, source, target, angleInDegrees, mirror, 1.0f, Xshift, Yshift, Xscale, Yscale);
        }
        public void DrawBitmap(SKBitmap bitmap, IbRect source, IbRect target, int angleInDegrees, bool mirror, int Xshift, int Yshift, int Xscale, int Yscale, float opacity)
        {
            DrawBitmap(bitmap, source, target, angleInDegrees, mirror, opacity, Xshift, Yshift, Xscale, Yscale);
        }
        public void DrawBitmap(SKBitmap bitmap, IbRect source, IbRect target, float angleInRadians, bool mirror, int Xshift, int Yshift, int Xscale, int Yscale)
        {
            float angleInDegrees = (angleInRadians * 360.0f) / (float)(Math.PI * 2);
            DrawBitmap(bitmap, source, target, angleInDegrees, mirror, 1.0f, Xshift, Yshift, Xscale, Yscale);
        }
        public void DrawBitmap(SKBitmap bitmap, IbRect source, IbRect target, bool mirror, float opacity)
        {
            DrawBitmap(bitmap, source, target, 0f, mirror, opacity, 0, 0, 1, 1);
        }
        public void DrawBitmap(SKBitmap bitmap, IbRectF source, IbRectF target, bool mirror, float opacity)
        {
            DrawBitmap(bitmap, source, target, 0f, mirror, opacity, 0, 0, 1, 1);
        }
        public void DrawBitmap(SKBitmap bitmap, IbRect source, IbRect destination, float angleInDegrees, bool mirror, float opacity, int Xshift, int Yshift, int Xscale, int Yscale)
        {
            int mir = 1;
            if (mirror) { mir = -1; }
            float xshf = (float)Xshift * 2 * screenDensity;
            float yshf = (float)Yshift * 2 * screenDensity;
            float xscl = 1f + (((float)Xscale * 2 * screenDensity) / squareSize);
            float yscl = 1f + (((float)Yscale * 2 * screenDensity) / squareSize);            

            canvas.Save();
            canvas.Scale(mir * xscl, yscl, (destination.Left + oXshift) + (destination.Width / 2), (destination.Top + oYshift) + (destination.Height / 2));
            canvas.RotateDegrees(angleInDegrees, (destination.Left + oXshift) + (destination.Width / 2), (destination.Top + oYshift) + (destination.Height / 2));
            canvas.Translate(xshf, yshf);
            dstRect.Left = destination.Left + oXshift;
            dstRect.Top = destination.Top + oYshift;
            dstRect.Right = destination.Width + destination.Left + oXshift;
            dstRect.Bottom = destination.Height + destination.Top + oYshift;
            srcRect.Left = source.Left;
            srcRect.Top = source.Top;
            srcRect.Right = source.Left + source.Width;
            srcRect.Bottom = source.Top + source.Height;
            drawPaint.Color = SKColors.Black.WithAlpha((byte)(255 * opacity));
            //used for sprites
            canvas.DrawBitmap(bitmap, srcRect, dstRect, drawPaint);
            canvas.Restore();
        }
        public void DrawBitmap(SKBitmap bitmap, IbRectF source, IbRectF destination, float angleInDegrees, bool mirror, float opacity, int Xshift, int Yshift, int Xscale, int Yscale)
        {
            int mir = 1;
            if (mirror) { mir = -1; }
            float xshf = (float)Xshift * 2 * screenDensity;
            float yshf = (float)Yshift * 2 * screenDensity;
            float xscl = 1f + (((float)Xscale * 2 * screenDensity) / squareSize);
            float yscl = 1f + (((float)Yscale * 2 * screenDensity) / squareSize);

            canvas.Save();
            canvas.Scale(mir * xscl, yscl, (destination.Left + oXshift) + (destination.Width / 2), (destination.Top + oYshift) + (destination.Height / 2));
            canvas.RotateDegrees(angleInDegrees, (destination.Left + oXshift) + (destination.Width / 2), (destination.Top + oYshift) + (destination.Height / 2));
            canvas.Translate(xshf, yshf);
            dstRect.Left = destination.Left + oXshift;
            dstRect.Top = destination.Top + oYshift;
            dstRect.Right = destination.Width + destination.Left + oXshift;
            dstRect.Bottom = destination.Height + destination.Top + oYshift;
            srcRect.Left = source.Left;
            srcRect.Top = source.Top;
            srcRect.Right = source.Left + source.Width;
            srcRect.Bottom = source.Top + source.Height;
            drawPaint.Color = SKColors.Black.WithAlpha((byte)(255 * opacity));
            //used for sprites
            canvas.DrawBitmap(bitmap, srcRect, dstRect, drawPaint);
            canvas.Restore();
        }
        
        public void Render(SKCanvas c)
        {
            canvas = c;
            
            if ((mod.useUIBackground) && (!screenType.Equals("main")) && (!screenType.Equals("combat")) && (!screenType.Equals("launcher")) && (! screenType.Equals("title")))
            {
                drawUIBackground();
            }            
            if (screenType.Equals("title"))
            {
                screenTitle.redrawTitle();
            }
            else if (screenType.Equals("launcher"))
            {
                screenLauncher.redrawLauncher();
            }
            else if (screenType.Equals("pcCreation"))
            {
                screenPcCreation.redrawPcCreation();
            }
            else if (screenType.Equals("learnSpellCreation"))
            {
                screenSpellLevelUp.redrawSpellLevelUp(true);
                
            }
            else if (screenType.Equals("learnSpellLevelUp") || screenType.Equals("learnSpellLevelUpCombat"))
            {
                screenSpellLevelUp.redrawSpellLevelUp(false);
            }
            else if (screenType.Equals("learnTraitCreation"))
            {
                screenTraitLevelUp.redrawTraitLevelUp(true);
            }
            else if (screenType.Equals("learnTraitLevelUp") || screenType.Equals("learnTraitLevelUpCombat"))
            {
                screenTraitLevelUp.redrawTraitLevelUp(false);
            }
            else if (screenType.Equals("main"))
            {
                screenMainMap.redrawMain(elapsed);
            }
            else if (screenType.Equals("party"))
            {
                screenParty.redrawParty();
            }
            else if (screenType.Equals("combatParty"))
            {
                screenParty.redrawParty();
            }
            else if (screenType.Equals("inventory"))
            {
                screenInventory.redrawInventory();
            }
            else if (screenType.Equals("itemSelector"))
            {
                screenItemSelector.redrawItemSelector();
            }
            else if (screenType.Equals("portraitSelector"))
            {
                screenPortraitSelector.redrawPortraitSelector();
            }
            else if (screenType.Equals("tokenSelector"))
            {
                screenTokenSelector.redrawTokenSelector();
            }
            else if (screenType.Equals("pcSelector"))
            {
                screenPcSelector.redrawPcSelector();
            }
            else if (screenType.Equals("combatInventory"))
            {
                screenInventory.redrawInventory();
            }
            else if (screenType.Equals("journal"))
            {
                screenJournal.redrawJournal();
            }
            else if (screenType.Equals("shop"))
            {
                screenShop.redrawShop();
            }
            else if (screenType.Equals("combat"))
            {
                screenCombat.redrawCombat(elapsed);
            }
            else if (screenType.Equals("combatCast"))
            {
                screenCastSelector.redrawCastSelector(true);
            }
            else if (screenType.Equals("mainMapCast"))
            {
                screenCastSelector.redrawCastSelector(false);
            }
            else if (screenType.Equals("combatTraitUse"))
            {
                screenTraitUseSelector.redrawTraitUseSelector(true);
            }
            else if (screenType.Equals("mainMapTraitUse"))
            {
                screenTraitUseSelector.redrawTraitUseSelector(false);
            }
            else if (screenType.Equals("convo"))
            {
                screenConvo.redrawConvo();
            }
            else if (screenType.Equals("partyBuild"))
            {
                screenPartyBuild.redrawPartyBuild();
            }
            else if (screenType.Equals("partyRoster"))
            {
                screenPartyRoster.redrawPartyRoster();
            }
            mod.debugMode = true; //TODO turn off, only for testing
            if (mod.debugMode)
            {
                int txtH = (int)drawFontRegHeight;
                for (int x = -2; x <= 2; x++)
                {
                    for (int y = -2; y <= 2; y++)
                    {
                        DrawText("FPS:" + fps.ToString() + "(" + MouseX + "," + MouseY + ")", x + 5, screenHeight - txtH - 5 + y - oYshift, "black");
                    }
                }
                DrawText("FPS:" + fps.ToString() + "(" + MouseX + "," + MouseY + ")", 5, screenHeight - txtH - 5 - oYshift, "white");
            }

            //EndDraw(); //uncomment this for DIRECT2D ADDITIONS
        }

        public void drawUIBackground()
        {
            try
            {
                IbRect src = new IbRect(0, 0, cc.ui_bg_fullscreen.Width, cc.ui_bg_fullscreen.Height);
                IbRect dst = new IbRect(0, 0, screenWidth, screenHeight - oYshift);
                DrawBitmap(cc.ui_bg_fullscreen, src, dst);
            }
            catch
            { }
        }

        //INPUT STUFF
        /*private void GameView_MouseWheel(object sender, MouseEventArgs e)
        {
            if ((screenType.Equals("main")) || (screenType.Equals("combat")))
            {
                log.onMouseWheel(sender, e);
            }
            onMouseEvent(sender, e, MouseEventType.EventType.MouseWheel);
        }*/
        /*private void GameView_MouseDown(object sender, MouseEventArgs e)
        {
            if ((screenType.Equals("main")) || (screenType.Equals("combat")))
            {
                //TODO log.onMouseDown(sender, e);
            }
            onMouseEvent(sender, e, MouseEventType.EventType.MouseDown);
        }*/
        /*private void GameView_MouseUp(object sender, MouseEventArgs e)
        {
            if ((screenType.Equals("main")) || (screenType.Equals("combat")))
            {
                //TODO log.onMouseUp(sender, e);
            }
            onMouseEvent(sender, e, MouseEventType.EventType.MouseUp);
        }*/
        /*private void GameView_MouseMove(object sender, MouseEventArgs e)
        {
            if ((screenType.Equals("main")) || (screenType.Equals("combat")))
            {
                mousePositionX = e.X;
                mousePositionY = e.Y;

                //TODO log.onMouseMove(sender, e);
            }
            onMouseEvent(sender, e, MouseEventType.EventType.MouseMove);
        }*/
        /*private void GameView_MouseClick(object sender, MouseEventArgs e)
        {
            onMouseEvent(sender, e, MouseEventType.EventType.MouseClick);
        }*/

        public void onMouseEvent(object sender, SKTouchEventArgs e)
        {
            MouseEventType.EventType eventType = MouseEventType.EventType.MouseMove;
            if (e.ActionType == SKTouchAction.Moved)
            {
                eventType = MouseEventType.EventType.MouseMove;
            }
            else if (e.ActionType == SKTouchAction.Pressed)
            {
                eventType = MouseEventType.EventType.MouseDown;
            }
            else if (e.ActionType == SKTouchAction.Released)
            {
                eventType = MouseEventType.EventType.MouseUp;
            }
            try 
            {
                int eX = (int)e.Location.X - oXshift;
                int eY = (int)e.Location.Y - oYshift;
                MouseX = eX;
                MouseY = eY;
                //do only itemListSelector if visible
                /*if (itemListSelector.showIBminiItemListSelector)
                {
                    itemListSelector.onTouchItemListSelection(eX, eY, eventType);
                    return;
                }*/
                if (touchEnabled)
                {
                    //do touch scrolling if in a scrolling text box
                    if (showMessageBox)
                    {
                        //TODO messageBox.onTouchSwipe(eX, eY, eventType);
                    }
                    else if ((screenType.Equals("main")) || (screenType.Equals("combat")))
                    {
                        log.onTouchSwipe(eX, eY, eventType);
                    }

                    //GAME SCREENS
                    if (screenType.Equals("main"))
                    {
                        screenMainMap.onTouchMain(eX, eY, eventType);	
                    }
                    else if (screenType.Equals("launcher"))
                    {
                        screenLauncher.onTouchLauncher(eX, eY, eventType);
                    }
                    else if (screenType.Equals("pcCreation"))
                    {
                        screenPcCreation.onTouchPcCreation(eX, eY, eventType);
                    }
                    else if (screenType.Equals("learnSpellCreation"))
                    {
                        screenSpellLevelUp.onTouchSpellLevelUp(eX, eY, eventType, true, false);   	
                    }
                    else if (screenType.Equals("learnSpellLevelUp"))
                    {
                        screenSpellLevelUp.onTouchSpellLevelUp(eX, eY, eventType, false, false);     	
                    }
                    else if (screenType.Equals("learnSpellLevelUpCombat"))
                    {
                        screenSpellLevelUp.onTouchSpellLevelUp(eX, eY, eventType, false, true);
                    }
                    else if (screenType.Equals("learnTraitCreation"))
                    {
                        screenTraitLevelUp.onTouchTraitLevelUp(eX, eY, eventType, true, false);   	
                    }
                    else if (screenType.Equals("learnTraitLevelUp"))
                    {
                        screenTraitLevelUp.onTouchTraitLevelUp(eX, eY, eventType, false, false);     	
                    }
                    else if (screenType.Equals("learnTraitLevelUpCombat"))
                    {
                        screenTraitLevelUp.onTouchTraitLevelUp(eX, eY, eventType, false, true);
                    }
                    else if (screenType.Equals("title"))
                    {
                        screenTitle.onTouchTitle(eX, eY, eventType);
                    }
                    else if (screenType.Equals("party"))
                    {
                        screenParty.onTouchParty(eX, eY, eventType, false);
                    }
                    else if (screenType.Equals("combatParty"))
                    {
                        screenParty.onTouchParty(eX, eY, eventType, true);
                    }
                    else if (screenType.Equals("inventory"))
                    {
                        screenInventory.onTouchInventory(eX, eY, eventType, false);
                    }
                    else if (screenType.Equals("combatInventory"))
                    {
                        screenInventory.onTouchInventory(eX, eY, eventType, true);
                    }
                    else if (screenType.Equals("itemSelector"))
                    {
                        screenItemSelector.onTouchItemSelector(eX, eY, eventType);
                    }
                    else if (screenType.Equals("portraitSelector"))
                    {
                        screenPortraitSelector.onTouchPortraitSelector(eX, eY, eventType);
                    }
                    else if (screenType.Equals("tokenSelector"))
                    {
                        screenTokenSelector.onTouchTokenSelector(eX, eY, eventType);
                    }
                    else if (screenType.Equals("pcSelector"))
                    {
                        screenPcSelector.onTouchPcSelector(eX, eY, eventType);
                    }
                    else if (screenType.Equals("journal"))
                    {
                        screenJournal.onTouchJournal(eX, eY, eventType);
                    }
                    else if (screenType.Equals("shop"))
                    {
                        screenShop.onTouchShop(eX, eY, eventType);
                    }
                    else if (screenType.Equals("combat"))
                    {
                        screenCombat.onTouchCombat(eX, eY, eventType);
                    }
                    else if (screenType.Equals("combatCast"))
                    {
                        screenCastSelector.onTouchCastSelector(eX, eY, eventType, true);
                    }
                    else if (screenType.Equals("mainMapCast"))
                    {
                        screenCastSelector.onTouchCastSelector(eX, eY, eventType, false);
                    }
                    else if (screenType.Equals("combatTraitUse"))
                    {
                        screenTraitUseSelector.onTouchCastSelector(eX, eY, eventType, true);
                    }
                    else if (screenType.Equals("mainMapTraitUse"))
                    {
                        screenTraitUseSelector.onTouchCastSelector(eX, eY, eventType, false);
                    }
                    else if (screenType.Equals("convo"))
                    {
                        screenConvo.onTouchConvo(eX, eY, eventType);
                    }
                    else if (screenType.Equals("partyBuild"))
                    {
                        screenPartyBuild.onTouchPartyBuild(eX, eY, eventType);
                    }
                    else if (screenType.Equals("partyRoster"))
                    {
                        screenPartyRoster.onTouchPartyRoster(eX, eY, eventType);
                    }
                }
            }
            catch (Exception ex) 
            {
                errorLog(ex.ToString());   		
            }		
        }

        /*public void onKeyboardEvent(Keys keyData)
        {
            try
            {
                if (touchEnabled)
                {
                    if (keyData == Keys.H)
                    {
                        if (showHotKeys) { showHotKeys = false; }
                        else { showHotKeys = true; }
                    }
                    if (screenType.Equals("main"))
                    {
                        screenMainMap.onKeyUp(keyData);
                    }
                    else if (screenType.Equals("combat"))
                    {
                        screenCombat.onKeyUp(keyData);
                    }
                    else if (screenType.Equals("convo"))
                    {
                        screenConvo.onKeyUp(keyData);
                    }
                }
            }
            catch (Exception ex)
            {
                errorLog(ex.ToString());
            }
        }*/
        /*protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            onKeyboardEvent(keyData);
                
            return base.ProcessCmdKey(ref msg, keyData);
        }*/

        //DIALOGS
        public Task<string> ListViewPage(List<string> list, string headerText)
        {
            // wait in this proc, until user did his input 
            var tcs = new TaskCompletionSource<string>();

            // create and show page
            var page = new ListViewPageIBx(tcs, list, headerText);
            cp.Navigation.PushModalAsync(page);

            // code is waiting her, until result is passed with tcs.SetResult() in btn-Clicked
            // then proc returns the result
            return tcs.Task;
        }
        public int GetSelectedIndex(string selectedString, List<string> itemList)
        {
            int index = 0;
            foreach(string s in itemList)
            {
                if (s.Equals(selectedString))
                {
                    return index;
                }
                index++;
            }
            return -1;
        }
        public Task<string> StringInputBox(string headerText, string existingTextInputValue)
        {
            // wait in this proc, until user did his input 
            var tcs = new TaskCompletionSource<string>();

            var lblTitle = new Label { Text = "Text Entry", HorizontalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold };
            var lblMessage = new Label { Text = headerText };
            var txtInput = new Editor { Text = existingTextInputValue, BackgroundColor = Color.White, TextColor = Color.Black };
            //txtInput.HorizontalOptions = LayoutOptions.FillAndExpand;
            txtInput.VerticalOptions = LayoutOptions.FillAndExpand;

            var btnOk = new Button
            {
                Text = "Ok",
                WidthRequest = 100,
                BackgroundColor = Xamarin.Forms.Color.FromRgb(0.8, 0.8, 0.8),
            };
            btnOk.Clicked += async (s, e) =>
            {
                // close page
                var result = txtInput.Text;
                await cp.Navigation.PopModalAsync();
                // pass result
                tcs.SetResult(result);
            };

            var btnCancel = new Button
            {
                Text = "Cancel",
                WidthRequest = 100,
                BackgroundColor = Xamarin.Forms.Color.FromRgb(0.8, 0.8, 0.8)
            };
            btnCancel.Clicked += async (s, e) =>
            {
                // close page
                await cp.Navigation.PopModalAsync();
                // pass empty result
                tcs.SetResult(existingTextInputValue);
            };

            var slButtons = new StackLayout
            {
                Orientation = StackOrientation.Horizontal,
                Children = { btnOk, btnCancel },
            };

            var layout = new StackLayout
            {
                Padding = new Thickness(0, 40, 0, 0),
                VerticalOptions = LayoutOptions.FillAndExpand,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                Orientation = StackOrientation.Vertical,
                Children = { lblTitle, lblMessage, txtInput, slButtons },
            };

            // create and show page
            var page = new ContentPage();
            page.Content = layout;
            cp.Navigation.PushModalAsync(page);
            // open keyboard
            //txtInput.Focus();

            // code is waiting her, until result is passed with tcs.SetResult() in btn-Clicked
            // then proc returns the result
            return tcs.Task;
        }

        //PLATFORM SPECIFIC CALLS
        public void CreateUserFolders() //called at start-up
        {
            DependencyService.Get<ISaveAndLoad>().CreateUserFolders();
        }
        public void SaveText(string fullPath, string text)
        {
            DependencyService.Get<ISaveAndLoad>().SaveText(fullPath, text);
        }
        public string LoadStringFromUserFolder(string fullPath)
        {
            return DependencyService.Get<ISaveAndLoad>().LoadStringFromUserFolder(fullPath);
        }
        public string LoadStringFromAssetFolder(string fullPath)
        {
            return DependencyService.Get<ISaveAndLoad>().LoadStringFromAssetFolder(fullPath);
        }
        public string LoadStringFromEitherFolder(string assetFolderpath, string userFolderpath)
        {
            return DependencyService.Get<ISaveAndLoad>().LoadStringFromEitherFolder(assetFolderpath, userFolderpath);
        }
        public string[] LoadStringLinesFromEitherFolder(string assetFolderpath, string userFolderpath)
        {
            return DependencyService.Get<ISaveAndLoad>().LoadStringLinesFromEitherFolder(assetFolderpath, userFolderpath);
        }
        public List<string> GetAllFilesWithExtensionFromUserFolder(string folderpath, string extension)
        {
            return DependencyService.Get<ISaveAndLoad>().GetAllFilesWithExtensionFromUserFolder(folderpath, extension);
        }
        public List<string> GetAllFilesWithExtensionFromAssetFolder(string folderpath, string extension)
        {
            return DependencyService.Get<ISaveAndLoad>().GetAllFilesWithExtensionFromAssetFolder(folderpath, extension);
        }
        public List<string> GetAllFilesWithExtensionFromBothFolders(string assetFolderpath, string userFolderpath, string extension)
        {
            return DependencyService.Get<ISaveAndLoad>().GetAllFilesWithExtensionFromBothFolders(assetFolderpath, userFolderpath, extension);
        }        
        public string GetModuleFileString(string modFilename)
        {
            return DependencyService.Get<ISaveAndLoad>().GetModuleFileString(modFilename);
        }
        public SKBitmap LoadBitmap(string filename, Module mdl)
        {
            return DependencyService.Get<ISaveAndLoad>().LoadBitmap(filename, mdl);
        }
        public List<string> GetAllModuleFiles(bool userOnly)
        {
            return DependencyService.Get<ISaveAndLoad>().GetAllModuleFiles(userOnly);
        }

        public void RestartAreaMusicIfEnded()
        {
            DependencyService.Get<ISaveAndLoad>().RestartAreaMusicIfEnded(this);
        }
        public void PlaySound(string filenameNoExtension)
        {
            //TODO DependencyService.Get<ISaveAndLoad>().PlaySound(this, filenameNoExtension);
        }
        public void CreateAreaMusicPlayer()
        {
            //DependencyService.Get<ISaveAndLoad>().CreateAreaMusicPlayer();
        }
        public void LoadAreaMusicFile(string fileName)
        {
            //DependencyService.Get<ISaveAndLoad>().LoadAreaMusicFile(fileName);
        }
        public void PlayAreaMusic(string filenameNoExtension)
        {
            DependencyService.Get<ISaveAndLoad>().PlayAreaMusic(this, filenameNoExtension);
        }
        public void PlayAreaAmbientSounds(string filenameNoExtension)
        {
            DependencyService.Get<ISaveAndLoad>().PlayAreaAmbientSounds(this, filenameNoExtension);
        }
        public void StopAreaMusic()
        {
            DependencyService.Get<ISaveAndLoad>().StopAreaMusic();
        }
        public void PauseAreaMusic()
        {
            //DependencyService.Get<ISaveAndLoad>().PlayAreaMusic();
        }

        //ERROR REPORTING FILE
        public void errorLog(string text)
        {
            string filename = "\\IB2ErrorLog.txt";
            SaveText(filename, text);
        }
    }
}
