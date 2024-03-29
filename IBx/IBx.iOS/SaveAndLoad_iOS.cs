﻿using System.Linq;
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
using UIKit;
using SkiaSharp.Views.iOS;

[assembly: Dependency(typeof(SaveAndLoad_iOS))]
namespace IBx.iOS
{
    public class SaveAndLoad_iOS : ISaveAndLoad
    {
        public string TrackingId = "UA-60615839-5";
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
            directoryname = Path.Combine(documents, "user");
            Directory.CreateDirectory(directoryname);
            /* for testing
            Assembly assembly = GetType().GetTypeInfo().Assembly;            
            foreach (var res in assembly.GetManifestResourceNames())
            {
                System.Diagnostics.Debug.WriteLine(res);
            }*/
        }

        public void SaveText(string fullPath, string text)
        {
            var documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string convertedFullPath = documents + ConvertFullPath(fullPath, "/");
            string dir = Path.GetDirectoryName(convertedFullPath);
            try
            {
                Directory.CreateDirectory(dir);
                using (StreamWriter sw = File.CreateText(convertedFullPath))
                {
                    sw.Write(text);
                }
            }
            catch (Exception ex)
            {

            }
        }        
        
        public string LoadStringFromUserFolder(string fullPath)
        {
            string text = "";
            //check in app module folder first
            Assembly assembly = GetType().GetTypeInfo().Assembly;
            Stream stream = assembly.GetManifestResourceStream("IBx.iOS.Assets" + ConvertFullPath(fullPath, "."));
            if (stream != null)
            {
                using (var reader = new System.IO.StreamReader(stream))
                {
                    text = reader.ReadToEnd();
                }
                return text;
            }

            //check in user module folder next
            var documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string convertedFullPath = documents + ConvertFullPath(fullPath, "/");
            if (File.Exists(convertedFullPath))
            {
                try
                {
                    text = File.ReadAllText(convertedFullPath);
                }
                catch (Exception ex)
                {

                }
                return text;
            }
            return text;

            //check in module folder first
            /*StorageFolder storageFolder = ApplicationData.Current.LocalFolder;
            string convertedFullPath = storageFolder.Path + ConvertFullPath(fullPath, "\\");
            if (File.Exists(convertedFullPath))
            {
                text = File.ReadAllText(convertedFullPath);
                return text;
            }*/           
            
        }
        public string LoadStringFromAssetFolder(string fullPath)
        {
            string text = "";
            //string filename = Path.GetFileName("C:" + fullPath);
            int pos = fullPath.LastIndexOf("\\") + 1;
            string filename = fullPath.Substring(pos, fullPath.Length - pos);
            //check in Assests folder last
            Assembly assembly = GetType().GetTypeInfo().Assembly;
            foreach (var res in assembly.GetManifestResourceNames())
            {
                //System.Diagnostics.Debug.WriteLine(res);
            }
            Stream stream = assembly.GetManifestResourceStream("IBx.iOS.Assets" + ConvertFullPath(fullPath, "."));
            if (stream == null)
            {
                stream = assembly.GetManifestResourceStream("IBx.iOS." + filename);
            }
            if (stream != null)
            {
                using (var reader = new System.IO.StreamReader(stream))
                {
                    text = reader.ReadToEnd();
                }
            }
            return text;
        }
        public string LoadStringFromEitherFolder(string assetFolderpath, string userFolderpath)
        {
            string text = "";
            int pos = userFolderpath.LastIndexOf("\\") + 1;
            string filename = userFolderpath.Substring(pos, userFolderpath.Length - pos);
            //check in module folder first
            var documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string convertedFullPath = documents + ConvertFullPath(userFolderpath, "/");
            if (File.Exists(convertedFullPath))
            {
                try
                {
                    text = File.ReadAllText(convertedFullPath);
                }
                catch (Exception ex)
                {

                }
                return text;
            }
            //check in Assests folder last
            Assembly assembly = GetType().GetTypeInfo().Assembly;
            Stream stream = assembly.GetManifestResourceStream("IBx.iOS.Assets" + ConvertFullPath(assetFolderpath, "."));
            if (stream == null)
            {
                stream = assembly.GetManifestResourceStream("IBx.iOS." + filename);
            }
            if (stream != null)
            {
                using (var reader = new System.IO.StreamReader(stream))
                {
                    text = reader.ReadToEnd();
                }
            }
            return text;
        }
        public string[] LoadStringLinesFromEitherFolder(string assetFolderpath, string userFolderpath)
        {
            string[] lines;
            List<string> linesArray = new List<string>();
            
            int pos = userFolderpath.LastIndexOf("\\") + 1;
            string filename = userFolderpath.Substring(pos, userFolderpath.Length - pos);
            //check in module folder first
            var documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string convertedFullPath = documents + ConvertFullPath(userFolderpath, "/");
            if (File.Exists(convertedFullPath))
            {
                try
                {
                    lines = File.ReadAllLines(convertedFullPath);
                }
                catch (Exception ex)
                {
                    linesArray.Add("");
                    lines = linesArray.ToArray();
                }
                
                return lines;
            }
            //check in Assests folder last
            Assembly assembly = GetType().GetTypeInfo().Assembly;
            Stream stream = assembly.GetManifestResourceStream("IBx.iOS.Assets" + ConvertFullPath(assetFolderpath, "."));
            if (stream == null)
            {
                stream = assembly.GetManifestResourceStream("IBx.iOS." + filename);
            }
            if (stream != null)
            {
                using (var reader = new System.IO.StreamReader(stream))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        linesArray.Add(line);
                    }
                }
                lines = linesArray.ToArray();
                return lines;
            }
            linesArray.Add("");
            lines = linesArray.ToArray();
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
                Stream stream = assembly.GetManifestResourceStream("IBx.iOS.Assets.modules." + modFolder + "." + modFilename);
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
            //MODULE'S GRAPHICS FOLDER
            var documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            var modulesDir = Path.Combine(documents, "modules");
            var modFolder = Path.Combine(modulesDir, mdl.moduleName);
            var modGraphicsFolder = Path.Combine(modFolder, "graphics");
            var filePath = Path.Combine(modGraphicsFolder, filename);

            if (File.Exists(filePath))
            {
                SKBitmap bm = UIImage.FromFile(filePath).ToSKBitmap();
                if (bm != null) { return bm; }
            }
            else if (File.Exists(filePath + ".png"))
            {
                SKBitmap bm = UIImage.FromFile(filePath + ".png").ToSKBitmap();
                if (bm != null) { return bm; }
            }
            else if (File.Exists(filePath + ".jpg"))
            {
                SKBitmap bm = UIImage.FromFile(filePath + ".jpg").ToSKBitmap();
                if (bm != null) { return bm; }
            }
            modGraphicsFolder = Path.Combine(modFolder, "pctokens");
            filePath = Path.Combine(modGraphicsFolder, filename);
            if (File.Exists(filePath))
            {
                SKBitmap bm = UIImage.FromFile(filePath).ToSKBitmap();
                if (bm != null) { return bm; }
            }
            else if (File.Exists(filePath + ".png"))
            {
                SKBitmap bm = UIImage.FromFile(filePath + ".png").ToSKBitmap();
                if (bm != null) { return bm; }
            }
            else if (File.Exists(filePath + ".PNG"))
            {
                SKBitmap bm = UIImage.FromFile(filePath + ".PNG").ToSKBitmap();
                if (bm != null) { return bm; }
            }
            else if (File.Exists(filePath + ".jpg"))
            {
                SKBitmap bm = UIImage.FromFile(filePath + ".jpg").ToSKBitmap();
                if (bm != null) { return bm; }
            }
            modGraphicsFolder = Path.Combine(modFolder, "portraits");
            filePath = Path.Combine(modGraphicsFolder, filename);
            if (File.Exists(filePath))
            {
                SKBitmap bm = UIImage.FromFile(filePath).ToSKBitmap();
                if (bm != null) { return bm; }
            }
            else if (File.Exists(filePath + ".png"))
            {
                SKBitmap bm = UIImage.FromFile(filePath + ".png").ToSKBitmap();
                if (bm != null) { return bm; }
            }
            else if (File.Exists(filePath + ".PNG"))
            {
                SKBitmap bm = UIImage.FromFile(filePath + ".PNG").ToSKBitmap();
                if (bm != null) { return bm; }
            }
            else if (File.Exists(filePath + ".jpg"))
            {
                SKBitmap bm = UIImage.FromFile(filePath + ".jpg").ToSKBitmap();
                if (bm != null) { return bm; }
            }
            modGraphicsFolder = Path.Combine(modFolder, "tiles");
            filePath = Path.Combine(modGraphicsFolder, filename);
            if (File.Exists(filePath))
            {
                SKBitmap bm = UIImage.FromFile(filePath).ToSKBitmap();
                if (bm != null) { return bm; }
            }
            else if (File.Exists(filePath + ".png"))
            {
                SKBitmap bm = UIImage.FromFile(filePath + ".png").ToSKBitmap();
                if (bm != null) { return bm; }
            }
            else if (File.Exists(filePath + ".jpg"))
            {
                SKBitmap bm = UIImage.FromFile(filePath + ".jpg").ToSKBitmap();
                if (bm != null) { return bm; }
            }
            modGraphicsFolder = Path.Combine(modFolder, "ui");
            filePath = Path.Combine(modGraphicsFolder, filename);
            if (File.Exists(filePath))
            {
                SKBitmap bm = UIImage.FromFile(filePath).ToSKBitmap();
                if (bm != null) { return bm; }
            }
            else if (File.Exists(filePath + ".png"))
            {
                SKBitmap bm = UIImage.FromFile(filePath + ".png").ToSKBitmap();
                if (bm != null) { return bm; }
            }
            else if (File.Exists(filePath + ".jpg"))
            {
                SKBitmap bm = UIImage.FromFile(filePath + ".jpg").ToSKBitmap();
                if (bm != null) { return bm; }
            }

            //USER FOLDER
            documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            var userDir = Path.Combine(documents, "user");
            filePath = Path.Combine(userDir, filename);

            if (File.Exists(filePath))
            {
                SKBitmap bm = UIImage.FromFile(filePath).ToSKBitmap();
                if (bm != null) { return bm; }
            }
            else if (File.Exists(filePath + ".png"))
            {
                SKBitmap bm = UIImage.FromFile(filePath + ".png").ToSKBitmap();
                if (bm != null) { return bm; }
            }
            else if (File.Exists(filePath + ".jpg"))
            {
                SKBitmap bm = UIImage.FromFile(filePath + ".jpg").ToSKBitmap();
                if (bm != null) { return bm; }
            }

            //DEFAULT ASSETS
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
                stream = assembly.GetManifestResourceStream("IBx.iOS.Assets.portraits." + filename);
            }
            if (stream == null)
            {
                stream = assembly.GetManifestResourceStream("IBx.iOS.Assets.portraits." + filename + ".png");
            }
            if (stream == null)
            {
                stream = assembly.GetManifestResourceStream("IBx.iOS.Assets.portraits." + filename + ".PNG");
            }
            if (stream == null)
            {
                stream = assembly.GetManifestResourceStream("IBx.iOS.Assets.portraits." + filename + ".jpg");
            }
            if (stream == null)
            {
                stream = assembly.GetManifestResourceStream("IBx.iOS.Assets.pctokens." + filename);
            }
            if (stream == null)
            {
                stream = assembly.GetManifestResourceStream("IBx.iOS.Assets.pctokens." + filename + ".png");
            }
            if (stream == null)
            {
                stream = assembly.GetManifestResourceStream("IBx.iOS.Assets.pctokens." + filename + ".PNG");
            }
            if (stream == null)
            {
                stream = assembly.GetManifestResourceStream("IBx.iOS.Assets.pctokens." + filename + ".jpg");
            }
            if (stream == null)
            {
                stream = assembly.GetManifestResourceStream("IBx.iOS.Assets.graphics.ui_missingtexture.png");
            }
            SKManagedStream skStream = new SKManagedStream(stream);

            //Stream fileStream = File.OpenRead("btn_small_on.png");
            //return SKBitmap.Decode(skStream);
            try
            {
                return SKBitmap.Decode(skStream);
            }
            catch (Exception ex)
            {
                Stream stream2 = assembly.GetManifestResourceStream("IBx.iOS.Assets.graphics.ui_missingtexture.png");
                SKManagedStream skStream2 = new SKManagedStream(stream2);
                return SKBitmap.Decode(skStream2);
            }
            //StorageFolder storageFolder = ApplicationData.Current.LocalFolder;
            //StorageFile sampleFile = await storageFolder.GetFileAsync(filename);
            //SKBitmap text = await Windows.Storage.FileIO.ReadTextAsync(sampleFile);
            //return text;


        }
        
        public List<string> GetAllFilesWithExtensionFromUserFolder(string folderpath, string extension)
        {
            List<string> list = new List<string>();

            //FROM ASSETS
            Assembly assembly = GetType().GetTypeInfo().Assembly;                        
            foreach (var res in assembly.GetManifestResourceNames())
            {
                if ((res.Contains(ConvertFullPath(folderpath, "."))) && (res.EndsWith(extension)))
                {
                    string[] split = res.Split('.');
                    list.Add(split[split.Length - 2]);
                }
            }

            //FROM USER FOLDER
            var documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            if (Directory.Exists(documents + ConvertFullPath(folderpath, "/")))
            {
                string[] files = Directory.GetFiles(documents + ConvertFullPath(folderpath, "/"), "*" + extension, SearchOption.AllDirectories);
                foreach (string file in files)
                {
                    list.Add(Path.GetFileNameWithoutExtension(file));
                }
            }

            return list;
        }
        public List<string> GetAllFilesWithExtensionFromAssetFolder(string folderpath, string extension)
        {
            List<string> list = new List<string>();
            Assembly assembly = GetType().GetTypeInfo().Assembly;
            foreach (var res in assembly.GetManifestResourceNames())
            {
                if ((res.Contains(ConvertFullPath(folderpath, "."))) && (res.EndsWith(extension)))
                {
                    string[] split = res.Split('.');
                    list.Add(split[split.Length - 2]);
                }
            }
            return list;
        }
        public List<string> GetAllFilesWithExtensionFromBothFolders(string assetFolderpath, string userFolderpath, string extension)
        {
            List<string> list = new List<string>();
            string uppercase = extension.ToUpper();
            string lowercase = extension.ToLower();

            //FROM USER FOLDER
            var documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            if (Directory.Exists(documents + ConvertFullPath(userFolderpath, "/")))
            {
                string[] files = Directory.GetFiles(documents + ConvertFullPath(userFolderpath, "/"), "*" + uppercase, SearchOption.AllDirectories);
                foreach (string file in files)
                {
                    if (!list.Contains(Path.GetFileName(file)))
                    {
                        list.Add(Path.GetFileName(file));
                    }
                }
                files = Directory.GetFiles(documents + ConvertFullPath(userFolderpath, "/"), "*" + lowercase, SearchOption.AllDirectories);
                foreach (string file in files)
                {
                    if (!list.Contains(Path.GetFileName(file)))
                    {
                        list.Add(Path.GetFileName(file));
                    }
                }
                files = Directory.GetFiles(documents + ConvertFullPath(userFolderpath, "/"), "*" + extension, SearchOption.AllDirectories);
                foreach (string file in files)
                {
                    if (!list.Contains(Path.GetFileName(file)))
                    {
                        list.Add(Path.GetFileName(file));
                    }
                }
            }

            //FROM ASSETS
            Assembly assembly = GetType().GetTypeInfo().Assembly;

            //DEBUGGING RESOURCE PATH
            //foreach (var res in assembly.GetManifestResourceNames())
            //{
            //    int x3 = 0;
            //}
            //module folder in app 
            foreach (var res in assembly.GetManifestResourceNames())
            {
                if ((res.Contains("IBx.iOS.Assets" + ConvertFullPath(userFolderpath, "."))) && (res.EndsWith(uppercase)))
                {
                    string[] split = res.Split('.');
                    list.Add(split[split.Length - 2] + "." + split[split.Length - 1]);
                }
                else if ((res.Contains("IBx.iOS.Assets" + ConvertFullPath(userFolderpath, "."))) && (res.EndsWith(lowercase)))
                {
                    string[] split = res.Split('.');
                    list.Add(split[split.Length - 2] + "." + split[split.Length - 1]);
                }
                else if ((res.Contains("IBx.iOS.Assets" + ConvertFullPath(userFolderpath, "."))) && (res.EndsWith(extension)))
                {
                    string[] split = res.Split('.');
                    list.Add(split[split.Length - 2] + "." + split[split.Length - 1]);
                }
            }
            //from main asset folder
            foreach (var res in assembly.GetManifestResourceNames())
            {
                if ((res.Contains("IBx.iOS.Assets" + ConvertFullPath(assetFolderpath, "."))) && (res.EndsWith(uppercase)))
                {
                    string[] split = res.Split('.');
                    list.Add(split[split.Length - 2] + "." + split[split.Length - 1]);
                }
                else if ((res.Contains("IBx.iOS.Assets" + ConvertFullPath(assetFolderpath, "."))) && (res.EndsWith(lowercase)))
                {
                    string[] split = res.Split('.');
                    list.Add(split[split.Length - 2] + "." + split[split.Length - 1]);
                }
                else if ((res.Contains("IBx.iOS.Assets" + ConvertFullPath(assetFolderpath, "."))) && (res.EndsWith(extension)))
                {
                    string[] split = res.Split('.');
                    list.Add(split[split.Length - 2] + "." + split[split.Length - 1]);
                }
            }
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
            //MODULE'S GRAPHICS FOLDER
            var documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            var modulesDir = Path.Combine(documents, "modules");
            var modFolder = Path.Combine(modulesDir, gv.mod.moduleName);
            var modSoundFolder = Path.Combine(modFolder, "sounds");
            var filePath = Path.Combine(modSoundFolder, filename);

            if (File.Exists(filePath))
            {
                return File.OpenRead(filePath);
            }
            else if (File.Exists(filePath + ".wav"))
            {
                return File.OpenRead(filePath + ".wav");
            }
            else if (File.Exists(filePath + ".mp3"))
            {
                return File.OpenRead(filePath + ".mp3");
            }
            modSoundFolder = Path.Combine(modFolder, "music");
            filePath = Path.Combine(modSoundFolder, filename);
            if (File.Exists(filePath))
            {
                return File.OpenRead(filePath);
            }
            else if (File.Exists(filePath + ".wav"))
            {
                return File.OpenRead(filePath + ".wav");
            }
            else if (File.Exists(filePath + ".mp3"))
            {
                return File.OpenRead(filePath + ".mp3");
            }

            //DEFAULT ASSETS
            Assembly assembly = GetType().GetTypeInfo().Assembly;
            var stream = assembly.GetManifestResourceStream("IBx.iOS.Assets.modules." + gv.mod.moduleName + ".sounds." + filename);
            if (stream == null)
            {
                stream = assembly.GetManifestResourceStream("IBx.iOS.Assets.modules." + gv.mod.moduleName + ".sounds." + filename + ".wav");
            }
            if (stream == null)
            {
                stream = assembly.GetManifestResourceStream("IBx.iOS.Assets.modules." + gv.mod.moduleName + ".sounds." + filename + ".mp3");
            }
            if (stream == null)
            {
                stream = assembly.GetManifestResourceStream("IBx.iOS.Assets.modules." + gv.mod.moduleName + ".music." + filename);
            }
            if (stream == null)
            {
                stream = assembly.GetManifestResourceStream("IBx.iOS.Assets.modules." + gv.mod.moduleName + ".music." + filename + ".wav");
            }
            if (stream == null)
            {
                stream = assembly.GetManifestResourceStream("IBx.iOS.Assets.modules." + gv.mod.moduleName + ".music." + filename + ".mp3");
            }
            if (stream == null)
            {
                stream = assembly.GetManifestResourceStream("IBx.iOS.Assets.sounds." + filename);
            }
            if (stream == null)
            {
                stream = assembly.GetManifestResourceStream("IBx.iOS.Assets.sounds." + filename + ".wav");
            }
            if (stream == null)
            {
                stream = assembly.GetManifestResourceStream("IBx.iOS.Assets.sounds." + filename + ".mp3");
            }
            if (stream == null)
            {
                stream = assembly.GetManifestResourceStream("IBx.iOS.Assets.music." + filename);
            }
            if (stream == null)
            {
                stream = assembly.GetManifestResourceStream("IBx.iOS.Assets.music." + filename + ".wav");
            }
            if (stream == null)
            {
                stream = assembly.GetManifestResourceStream("IBx.iOS.Assets.music." + filename + ".mp3");
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
                    areaMusicPlayer.PlaybackEnded += AreaMusicPlayer_PlaybackEnded;
                }
                if (!areaMusicPlayer.IsPlaying)
                {
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
        }

        private void AreaMusicPlayer_PlaybackEnded(object sender, EventArgs e)
        {
            RestartAreaMusicIfEnded();
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
                if (!areaAmbientSoundsPlayer.IsPlaying)
                {
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
                            gv.cc.addLogText("<yl>failed to play area ambient sounds" + filenameNoExtension + "</yl><BR>");
                        }
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
        public void RestartAreaMusicIfEnded()
        {
            //restart area music
            if (areaMusicPlayer == null)
            {
                areaMusicPlayer = CrossSimpleAudioPlayer.CreateSimpleAudioPlayer();
            }
            try
            {
                if (!areaMusicPlayer.IsPlaying)
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
                if (!areaAmbientSoundsPlayer.IsPlaying)
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

            if (areaAmbientSoundsPlayer == null)
            {
                areaAmbientSoundsPlayer = CrossSimpleAudioPlayer.CreateSimpleAudioPlayer();
            }
            try
            {
                if (areaAmbientSoundsPlayer.IsPlaying)
                {
                    areaAmbientSoundsPlayer.Stop();
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