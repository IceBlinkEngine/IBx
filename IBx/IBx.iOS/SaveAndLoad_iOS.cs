using System.Linq;
using Foundation;
using System.IO;
using System.Threading.Tasks;
using Xamarin.Forms;
using IBx.iOS;
using SkiaSharp;
using System.Reflection;
using System.Collections.Generic;
using System;
using Newtonsoft.Json;
using Google.Analytics;
using Plugin.SimpleAudioPlayer;

[assembly: Dependency(typeof(SaveAndLoad_iOS))]
namespace IBx.iOS
{
    public class SaveAndLoad_iOS : ISaveAndLoad
    {
        public string TrackingId = "UA-60615839-12";
        public ITracker Tracker;
        const string AllowTrackingKey = "AllowTracking";
        int numOfTrackerEventHitsInThisSession = 0;
        public ISimpleAudioPlayer soundPlayer;
        public ISimpleAudioPlayer areaMusicPlayer;
        public ISimpleAudioPlayer areaAmbientSoundsPlayer;

        #region Instantition...
        private static SaveAndLoad_iOS thisRef;
        public SaveAndLoad_iOS()
        {
            // no code req'd
        }

        public static SaveAndLoad_iOS GetGASInstance()
        {
            if (thisRef == null)
                // it's ok, we can call this constructor
                thisRef = new SaveAndLoad_iOS();
            return thisRef;
        }
        #endregion

        public bool AllowReadWriteExternal()
        {
            return true;
        }

        public void CreateUserFolders()
        {
            var documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            var directoryname = Path.Combine(documents, "modules");
            Directory.CreateDirectory(directoryname);
            directoryname = Path.Combine(documents, "saves");
            Directory.CreateDirectory(directoryname);
        }

        public void SaveText(string filename, string text)
        {
            /*string path = CreatePathToFile(filename);
            using (StreamWriter sw = File.CreateText(path))
                await sw.WriteAsync(text);*/
        }        
        
        public string LoadStringFromUserFolder(string fullPath)
        {
            string text = "";
            //check in module folder first
            /*StorageFolder storageFolder = ApplicationData.Current.LocalFolder;
            string convertedFullPath = storageFolder.Path + ConvertFullPath(fullPath, "\\");
            if (File.Exists(convertedFullPath))
            {
                text = File.ReadAllText(convertedFullPath);
                return text;
            }*/
            return text;
        }
        public string LoadStringFromAssetFolder(string fullPath)
        {
            string text = "";
            //check in Assests folder last
            Assembly assembly = GetType().GetTypeInfo().Assembly;
            Stream stream = assembly.GetManifestResourceStream("IBx.iOS.Assets." + ConvertFullPath(fullPath, "."));
            using (var reader = new System.IO.StreamReader(stream))
            {
                text = reader.ReadToEnd();
            }
            return text;
        }
        public string LoadStringFromEitherFolder(string assetFolderpath, string userFolderpath)
        {
            string text = "";
            //check in module folder first
            /*StorageFolder storageFolder = ApplicationData.Current.LocalFolder;
            string convertedFullPath = storageFolder.Path + ConvertFullPath(userFolderpath, "\\");
            if (File.Exists(convertedFullPath))
            {
                text = File.ReadAllText(convertedFullPath);
                return text;
            }*/
            //check in Assests folder last
            Assembly assembly = GetType().GetTypeInfo().Assembly;
            Stream stream = assembly.GetManifestResourceStream("IBx.iOS.Assets." + ConvertFullPath(assetFolderpath, "."));
            using (var reader = new System.IO.StreamReader(stream))
            {
                text = reader.ReadToEnd();
            }
            return text;
        }
        public string[] LoadStringLinesFromEitherFolder(string assetFolderpath, string userFolderpath)
        {
            string[] lines;
            List<string> linesArray = new List<string>();
            linesArray.Add("");            
            lines = linesArray.ToArray();
            //check in module folder first
            /*Java.IO.File sdCard = Android.OS.Environment.ExternalStorageDirectory;
            string filePath = sdCard.AbsolutePath + "/IBx" + ConvertFullPath(userFolderpath, "/");
            if (File.Exists(filePath))
            {
                return File.ReadAllLines(filePath);
            }
            //check in Assests folder last
            Assembly assembly = GetType().GetTypeInfo().Assembly;
            Stream stream = assembly.GetManifestResourceStream("IBx.UWP.Assets" + ConvertFullPath(assetFolderpath, "."));
            using (var reader = new System.IO.StreamReader(stream))
            {
                List<string> linesArray = new List<string>();
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    linesArray.Add(line);
                }
                lines = linesArray.ToArray();
            }*/
            return lines;
        }

        public string ConvertFullPath(string fullPath, string replaceWith)
        {
            string convertedFullPath = "";
            convertedFullPath = fullPath.Replace("\\", replaceWith);
            return convertedFullPath;
        }

        public string GetModuleFileString(string modFilename)
        {
            //asset module
            if (modFilename.StartsWith("IBx."))
            {
                Assembly assembly = GetType().GetTypeInfo().Assembly;
                Stream stream = assembly.GetManifestResourceStream(modFilename);
                using (var reader = new System.IO.StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
            else
            {
                string modFolder = Path.GetFileNameWithoutExtension(modFilename);
                //try asset area            
                Assembly assembly = GetType().GetTypeInfo().Assembly;
                Stream stream = assembly.GetManifestResourceStream("IBx.iOS." + modFilename);
                if (stream != null)
                {
                    using (var reader = new System.IO.StreamReader(stream))
                    {
                        return reader.ReadToEnd();
                    }
                }

                //try from personal folder first
                var documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                var filePath = documentsPath + "/modules/" + modFolder + "/" + modFilename;
                if (File.Exists(filePath))
                {
                    return File.ReadAllText(filePath);
                }
                else //try from external folder
                {
                    /*Java.IO.File sdCard = Android.OS.Environment.ExternalStorageDirectory;
                    filePath = sdCard.AbsolutePath + "/IBx/modules/" + modFolder + "/" + modFilename;
                    if (File.Exists(filePath))
                    {
                        return File.ReadAllText(filePath);
                    }*/
                }
            }
            return "";
        }
        
        public SKBitmap LoadBitmap(string filename, Module mdl)
        {
            Assembly assembly = GetType().GetTypeInfo().Assembly;
            Stream stream = assembly.GetManifestResourceStream("IBx.iOS." + filename);
            if (stream == null)
            {
                stream = assembly.GetManifestResourceStream("IBx.iOS." + filename + ".png");
            }
            if (stream == null)
            {
                stream = assembly.GetManifestResourceStream("IBx.iOS." + filename + ".jpg");
            }
            if (stream == null)
            {
                stream = assembly.GetManifestResourceStream("IBx.iOS.Assets.graphics." + filename);
            }
            if (stream == null)
            {
                stream = assembly.GetManifestResourceStream("IBx.iOS.Assets.graphics." + filename + ".png");
            }
            if (stream == null)
            {
                stream = assembly.GetManifestResourceStream("IBx.iOS.Assets.graphics." + filename + ".jpg");
            }
            if (stream == null)
            {
                stream = assembly.GetManifestResourceStream("IBx.iOS.Assets.tiles." + filename);
            }
            if (stream == null)
            {
                stream = assembly.GetManifestResourceStream("IBx.iOS.Assets.tiles." + filename + ".png");
            }
            if (stream == null)
            {
                stream = assembly.GetManifestResourceStream("IBx.iOS.Assets.tiles." + filename + ".jpg");
            }
            if (stream == null)
            {
                stream = assembly.GetManifestResourceStream("IBx.iOS.Assets.ui." + filename);
            }
            if (stream == null)
            {
                stream = assembly.GetManifestResourceStream("IBx.iOS.Assets.ui." + filename + ".png");
            }
            if (stream == null)
            {
                stream = assembly.GetManifestResourceStream("IBx.iOS.Assets.ui." + filename + ".jpg");
            }
            if (stream == null)
            {
                stream = assembly.GetManifestResourceStream("IBx.iOS.Assets.graphics.ui_missingtexture.png");
            }
            SKManagedStream skStream = new SKManagedStream(stream);

            //Stream fileStream = File.OpenRead("btn_small_on.png");
            return SKBitmap.Decode(skStream);

            //StorageFolder storageFolder = ApplicationData.Current.LocalFolder;
            //StorageFile sampleFile = await storageFolder.GetFileAsync(filename);
            //SKBitmap text = await Windows.Storage.FileIO.ReadTextAsync(sampleFile);
            //return text;


        }
        
        public List<string> GetAllFilesWithExtensionFromUserFolder(string folderpath, string extension)
        {
            List<string> list = new List<string>();

            return list;
        }
        public List<string> GetAllFilesWithExtensionFromAssetFolder(string folderpath, string extension)
        {
            List<string> list = new List<string>();

            return list;
        }
        public List<string> GetAllFilesWithExtensionFromBothFolders(string assetFolderpath, string userFolderpath, string extension)
        {
            List<string> list = new List<string>();

            return list;
        }
        public List<string> GetAllModuleFiles(bool userOnly)
        {
            List<string> list = new List<string>();

            if (!userOnly)
            {
                //search in assets
                Assembly assembly = GetType().GetTypeInfo().Assembly;
                foreach (var res in assembly.GetManifestResourceNames())
                {
                    if ((res.EndsWith(".mod")) && (!res.EndsWith("NewModule.mod")))
                    {
                        list.Add(res);
                    }
                }
            }

            //search in personal folder
            var documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string[] files = Directory.GetFiles(documents + "/modules", "*.mod", SearchOption.AllDirectories);
            foreach (string file in files)
            {
                if (Path.GetFileName(file) != "NewModule.mod")
                {
                    list.Add(Path.GetFileName(file));
                }
            }

            return list;
        }

        public void TrackAppEvent(string Category, string EventAction, string EventLabel)
        {
            try
            {
                if (numOfTrackerEventHitsInThisSession > 300)
                {
                    Gai.SharedInstance.DefaultTracker.Send(DictionaryBuilder.CreateScreenView().Build());
                    Gai.SharedInstance.Dispatch(); // Manually dispatch the event immediately
                    numOfTrackerEventHitsInThisSession = 0;
                }
                else
                {
                    numOfTrackerEventHitsInThisSession++;
                }
                Gai.SharedInstance.DefaultTracker.Send(DictionaryBuilder.CreateEvent("iOS_" + Category, "iOS_" + EventAction, "iOS_" + EventLabel, null).Build());
                Gai.SharedInstance.Dispatch(); // Manually dispatch the event immediately
            }
            catch
            {

            }
        }
        public void InitializeNativeGAS()
        {
            try
            {
                var optionsDict = NSDictionary.FromObjectAndKey(new NSString("YES"), new NSString(AllowTrackingKey));
                NSUserDefaults.StandardUserDefaults.RegisterDefaults(optionsDict);

                Gai.SharedInstance.OptOut = !NSUserDefaults.StandardUserDefaults.BoolForKey(AllowTrackingKey);

                Gai.SharedInstance.DispatchInterval = 10;
                Gai.SharedInstance.TrackUncaughtExceptions = true;

                Tracker = Gai.SharedInstance.GetTracker("TestApp", TrackingId);
            }
            catch
            {

            }
        }

        Stream GetStreamFromFile(GameView gv, string filename)
        {
            Assembly assembly = GetType().GetTypeInfo().Assembly;
            var stream = assembly.GetManifestResourceStream("IBbasic.iOS.Assets.modules." + gv.mod.moduleName + "." + filename);
            if (stream == null)
            {
                stream = assembly.GetManifestResourceStream("IBbasic.iOS.Assets.modules." + gv.mod.moduleName + "." + filename + ".wav");
            }
            if (stream == null)
            {
                stream = assembly.GetManifestResourceStream("IBbasic.iOS.Assets.modules." + gv.mod.moduleName + "." + filename + ".mp3");
            }
            if (stream == null)
            {
                stream = assembly.GetManifestResourceStream("IBbasic.iOS.Assets.sounds." + filename);
            }
            if (stream == null)
            {
                stream = assembly.GetManifestResourceStream("IBbasic.iOS.Assets.sounds." + filename + ".wav");
            }
            if (stream == null)
            {
                stream = assembly.GetManifestResourceStream("IBbasic.iOS.Assets.sounds." + filename + ".mp3");
            }
            return stream;
        }
        public void PlaySound(GameView gv, string filenameNoExtension)
        {
            if ((filenameNoExtension.Equals("none")) || (filenameNoExtension.Equals("")) || (!gv.mod.playSoundFx))
            {
                //play nothing
                return;
            }
            else
            {
                if (soundPlayer == null)
                {
                    soundPlayer = CrossSimpleAudioPlayer.CreateSimpleAudioPlayer();
                }
                try
                {
                    soundPlayer.Loop = false;
                    soundPlayer.Load(GetStreamFromFile(gv, filenameNoExtension));
                    soundPlayer.Play();
                }
                catch (Exception ex)
                {
                    if (gv.mod.debugMode) //SD_20131102
                    {
                        gv.cc.addLogText("<yl>failed to play sound" + filenameNoExtension + "</yl><BR>");
                    }
                }
            }
        }
        public void PlayAreaMusic(GameView gv, string filenameNoExtension)
        {
            if ((filenameNoExtension.Equals("none")) || (filenameNoExtension.Equals("")) || (!gv.mod.playSoundFx))
            {
                //play nothing
                return;
            }
            else
            {
                if (areaMusicPlayer == null)
                {
                    areaMusicPlayer = CrossSimpleAudioPlayer.CreateSimpleAudioPlayer();
                }
                try
                {
                    areaMusicPlayer.Loop = true;
                    areaMusicPlayer.Load(GetStreamFromFile(gv, filenameNoExtension));
                    areaMusicPlayer.Play();
                }
                catch (Exception ex)
                {
                    if (gv.mod.debugMode) //SD_20131102
                    {
                        gv.cc.addLogText("<yl>failed to play area music" + filenameNoExtension + "</yl><BR>");
                    }
                }
            }
        }
        public void PlayAreaAmbientSounds(GameView gv, string filenameNoExtension)
        {
            if ((filenameNoExtension.Equals("none")) || (filenameNoExtension.Equals("")) || (!gv.mod.playSoundFx))
            {
                //play nothing
                return;
            }
            else
            {
                if (areaAmbientSoundsPlayer == null)
                {
                    areaAmbientSoundsPlayer = CrossSimpleAudioPlayer.CreateSimpleAudioPlayer();
                }
                try
                {
                    areaAmbientSoundsPlayer.Loop = true;
                    areaAmbientSoundsPlayer.Load(GetStreamFromFile(gv, filenameNoExtension));
                    areaAmbientSoundsPlayer.Play();
                }
                catch (Exception ex)
                {
                    if (gv.mod.debugMode) //SD_20131102
                    {
                        gv.cc.addLogText("<yl>failed to play area music" + filenameNoExtension + "</yl><BR>");
                    }
                }
            }
        }
        public void RestartAreaMusicIfEnded(GameView gv)
        {
            //restart area music
            if (areaMusicPlayer == null)
            {
                areaMusicPlayer = CrossSimpleAudioPlayer.CreateSimpleAudioPlayer();
            }
            try
            {
                if ((!areaMusicPlayer.IsPlaying) && (gv.mod.playSoundFx))
                {
                    try
                    {
                        areaMusicPlayer.Play();
                    }
                    catch (Exception ex)
                    {

                    }
                }
            }
            catch (Exception ex)
            {

            }

            //restart area ambient sounds
            if (areaAmbientSoundsPlayer == null)
            {
                areaAmbientSoundsPlayer = CrossSimpleAudioPlayer.CreateSimpleAudioPlayer();
            }
            try
            {
                if ((!areaAmbientSoundsPlayer.IsPlaying) && (gv.mod.playSoundFx))
                {
                    try
                    {
                        areaAmbientSoundsPlayer.Play();
                    }
                    catch (Exception ex)
                    {

                    }
                }
            }
            catch (Exception ex)
            {

            }
        }
        public void StopAreaMusic()
        {
            if (areaMusicPlayer == null)
            {
                areaMusicPlayer = CrossSimpleAudioPlayer.CreateSimpleAudioPlayer();
            }
            try
            {
                if (areaMusicPlayer.IsPlaying)
                {
                    areaMusicPlayer.Stop();
                }
            }
            catch (Exception ex)
            {

            }
        }
        public void PauseAreaMusic()
        {
            //playerAreaMusic.Pause();
        }

        static string CreatePathToFile(string fileName)
        {
            return Path.Combine(DocumentsPath, fileName);
        }
        public static string DocumentsPath
        {
            get
            {
                var documentsDirUrl = NSFileManager.DefaultManager.GetUrls(NSSearchPathDirectory.DocumentDirectory, NSSearchPathDomain.User).Last();
                return documentsDirUrl.Path;
            }
        }
    }
}