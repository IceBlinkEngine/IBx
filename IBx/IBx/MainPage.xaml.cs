using SkiaSharp;
using SkiaSharp.Views.Forms;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace IBx
{
	public partial class MainPage : ContentPage
	{
        GameView gv;
        SKPaint backFill = new SKPaint { Style = SKPaintStyle.Fill };
        SKBitmap bm = new SKBitmap(100, 100, true);
        public int counter = 0;
        public Stopwatch gameTimerStopwatch = new Stopwatch();
        public long previousTime = 0;
        public float fps = 0;
        public int reportFPScount = 0;
        public string mainDirectory;
        public string actionType = "";
        public int locX = 0;
        public int locY = 0;

        public MainPage()
		{
			InitializeComponent();

            mainDirectory = Directory.GetCurrentDirectory();

            gv = new GameView(this);

            //SetUpBitmap();

            Device.StartTimer(TimeSpan.FromSeconds(1f / 60), () =>
            {
                gv.gameTimer_Tick(canvasView);
                return true;
            });

            gameTimerStopwatch.Start();
            previousTime = gameTimerStopwatch.ElapsedMilliseconds;
        }

        private void canvasView_PaintSurface(object sender, SKPaintSurfaceEventArgs e)
        {
            SKSurface surface = e.Surface;
            SKCanvas canvas = surface.Canvas;

            canvas.Clear(SKColors.Black);

            gv.Render(canvas);            
        }

        private void OnTouch(object sender, SKTouchEventArgs e)
        {
            gv.onMouseEvent(sender, e);
            switch (e.ActionType)
            {
                case SKTouchAction.Pressed:
                    // start of a stroke
                    var p = new SKPath();
                    p.MoveTo(e.Location);
                    //temporaryPaths[e.Id] = p;
                    //string file = LoadTextAndroid("test3d.mod");
                    break;
                case SKTouchAction.Moved:
                    // the stroke, while pressed
                    actionType = e.ActionType.ToString();
                    locX = (int)e.Location.X;
                    locY = (int)e.Location.Y;
                    //if (e.InContact)
                    //temporaryPaths[e.Id].LineTo(e.Location);
                    break;
                case SKTouchAction.Released:
                    // end of a stroke
                    int x = 0;
                    //paths.Add(temporaryPaths[e.Id]);
                    //temporaryPaths.Remove(e.Id);
                    break;
                case SKTouchAction.Cancelled:
                    // we don't want that stroke
                    x = 0;
                    //temporaryPaths.Remove(e.Id);
                    break;
            }

            // we have handled these events
            e.Handled = true;

            // update the UI
            ((SKCanvasView)sender).InvalidateSurface();
        }
    }
}
