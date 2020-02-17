using System;
using System.Collections.Generic;
using System.Linq;

using Foundation;
using UIKit;

namespace IBx.iOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the 
    // User Interface of the application, as well as listening (and optionally responding) to 
    // application events from iOS.
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        //
        // This method is invoked when the application has loaded and is ready to run. In this 
        // method you should instantiate the window, load the UI into it and then make the window
        // visible.
        //
        // You have 17 seconds to return from this method, or iOS will terminate your application.
        //
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            global::Xamarin.Forms.Forms.Init();

            int dim1 = (int)UIScreen.MainScreen.NativeBounds.Width;
            int dim2 = (int)UIScreen.MainScreen.NativeBounds.Height;

            if (dim1 >= dim2)
            {
                App.ScreenWidth = dim1;
                App.ScreenHeight = dim2;
            }
            else
            {
                App.ScreenWidth = dim2;
                App.ScreenHeight = dim1;
            }

            SaveAndLoad_iOS.GetGASInstance().InitializeNativeGAS();

            LoadApplication(new App());

            return base.FinishedLaunching(app, options);
        }
    }
}
