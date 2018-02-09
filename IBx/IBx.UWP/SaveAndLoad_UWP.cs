using IBx.UWP;
using Newtonsoft.Json;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Windows.Storage;
using Xamarin.Forms;

[assembly: Dependency(typeof(SaveAndLoad_UWP))]
namespace IBx.UWP
{    
    public class SaveAndLoad_UWP : ISaveAndLoad
    {
        public void SaveText(string fullPath, string text)
        {
            StorageFolder storageFolder = ApplicationData.Current.LocalFolder;
            string convertedFullPath = storageFolder.Path + ConvertFullPath(fullPath, "\\");
            string dir = Path.GetDirectoryName(convertedFullPath);
            Directory.CreateDirectory(dir);
            using (StreamWriter sw = File.CreateText(convertedFullPath))
            {
                sw.Write(text);
            }
        }        
        
        public string LoadStringFromUserFolder(string fullPath)
        {
            string text = "";
            //check in module folder first
            StorageFolder storageFolder = ApplicationData.Current.LocalFolder;
            string convertedFullPath = storageFolder.Path + ConvertFullPath(fullPath, "\\");
            if (File.Exists(convertedFullPath))
            {
                text = File.ReadAllText(convertedFullPath);
                return text;
            }
            return text;
        }
        public string LoadStringFromAssetFolder(string fullPath)
        {
            string text = "";
            //check in Assests folder last
            Assembly assembly = GetType().GetTypeInfo().Assembly;
            Stream stream = assembly.GetManifestResourceStream("IBx.UWP.Assets" + ConvertFullPath(fullPath, "."));
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
            StorageFolder storageFolder = ApplicationData.Current.LocalFolder;
            string convertedFullPath = storageFolder.Path + ConvertFullPath(userFolderpath, "\\");
            if (File.Exists(convertedFullPath))
            {
                text = File.ReadAllText(convertedFullPath);
                return text;
            }
            //check in Assests folder last
            Assembly assembly = GetType().GetTypeInfo().Assembly;
            Stream stream = assembly.GetManifestResourceStream("IBx.UWP.Assets" + ConvertFullPath(assetFolderpath, "."));
            using (var reader = new System.IO.StreamReader(stream))
            {
                text = reader.ReadToEnd();
            }
            return text;
        }
        public string[] LoadStringLinesFromEitherFolder(string assetFolderpath, string userFolderpath)
        {
            string[] lines;
            //check in module folder first
            StorageFolder storageFolder = ApplicationData.Current.LocalFolder;
            string convertedFullPath = storageFolder.Path + ConvertFullPath(userFolderpath, "\\");
            if (File.Exists(convertedFullPath))
            {
                lines = File.ReadAllLines(convertedFullPath);
                return lines;
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
            }
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
                StorageFolder storageFolder = ApplicationData.Current.LocalFolder;
                string modFolder = Path.GetFileNameWithoutExtension(modFilename);
                
                //try from personal folder first
                //var documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                var filePath = storageFolder.Path + "\\modules\\" + modFolder + "\\" + modFilename;
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
            SKBitmap bm = null;
            try
            {
                StorageFolder storageFolder = ApplicationData.Current.LocalFolder;
                if ((mdl.currentArea.sourceBitmapName != "") && (File.Exists(storageFolder.Path + "\\modules\\" + mdl.moduleName + "\\graphics\\" + mdl.currentArea.sourceBitmapName + "\\" + filename + ".png")))
                {
                    bm = SKBitmap.Decode(storageFolder.Path + "\\modules\\" + mdl.moduleName + "\\graphics\\" + mdl.currentArea.sourceBitmapName + "\\" + filename + ".png");
                }
                else if ((mdl.currentArea.sourceBitmapName != "") && (File.Exists(storageFolder.Path + "\\modules\\" + mdl.moduleName + "\\graphics\\" + mdl.currentArea.sourceBitmapName + "\\" + filename + ".PNG")))
                {
                    bm = SKBitmap.Decode(storageFolder.Path + "\\modules\\" + mdl.moduleName + "\\graphics\\" + mdl.currentArea.sourceBitmapName + "\\" + filename + ".PNG");
                }
                else if ((mdl.currentArea.sourceBitmapName != "") && (File.Exists(storageFolder.Path + "\\modules\\" + mdl.moduleName + "\\graphics\\" + mdl.currentArea.sourceBitmapName + "\\" + filename + ".jpg")))
                {
                    bm = SKBitmap.Decode(storageFolder.Path + "\\modules\\" + mdl.moduleName + "\\graphics\\" + mdl.currentArea.sourceBitmapName + "\\" + filename + ".jpg");
                }
                else if (File.Exists(storageFolder.Path + "\\modules\\" + mdl.moduleName + "\\tiles\\" + filename + ".png"))
                {
                    bm = SKBitmap.Decode(storageFolder.Path + "\\modules\\" + mdl.moduleName + "\\tiles\\" + filename + ".png");
                }
                else if (File.Exists(storageFolder.Path + "\\modules\\" + mdl.moduleName + "\\tiles\\" + filename + ".PNG"))
                {
                    bm = SKBitmap.Decode(storageFolder.Path + "\\modules\\" + mdl.moduleName + "\\tiles\\" + filename + ".PNG");
                }
                else if (File.Exists(storageFolder.Path + "\\modules\\" + mdl.moduleName + "\\tiles\\" + filename))
                {
                    bm = SKBitmap.Decode(storageFolder.Path + "\\modules\\" + mdl.moduleName + "\\tiles\\" + filename);
                }
                else if (File.Exists(storageFolder.Path + "\\modules\\" + mdl.moduleName + "\\graphics\\" + filename + ".png"))
                {
                    bm = SKBitmap.Decode(storageFolder.Path + "\\modules\\" + mdl.moduleName + "\\graphics\\" + filename + ".png");
                }
                else if (File.Exists(storageFolder.Path + "\\modules\\" + mdl.moduleName + "\\graphics\\" + filename + ".PNG"))
                {
                    bm = SKBitmap.Decode(storageFolder.Path + "\\modules\\" + mdl.moduleName + "\\graphics\\" + filename + ".PNG");
                }
                else if (File.Exists(storageFolder.Path + "\\modules\\" + mdl.moduleName + "\\graphics\\" + filename + ".jpg"))
                {
                    bm = SKBitmap.Decode(storageFolder.Path + "\\modules\\" + mdl.moduleName + "\\graphics\\" + filename + ".jpg");
                }
                else if (File.Exists(storageFolder.Path + "\\modules\\" + mdl.moduleName + "\\graphics\\" + filename))
                {
                    bm = SKBitmap.Decode(storageFolder.Path + "\\modules\\" + mdl.moduleName + "\\graphics\\" + filename);
                }
                else if (File.Exists(storageFolder.Path + "\\modules\\" + mdl.moduleName + "\\ui\\" + filename + ".png"))
                {
                    bm = SKBitmap.Decode(storageFolder.Path + "\\modules\\" + mdl.moduleName + "\\ui\\" + filename + ".png");
                }
                else if (File.Exists(storageFolder.Path + "\\modules\\" + mdl.moduleName + "\\ui\\" + filename + ".PNG"))
                {
                    bm = SKBitmap.Decode(storageFolder.Path + "\\modules\\" + mdl.moduleName + "\\ui\\" + filename + ".PNG");
                }
                else if (File.Exists(storageFolder.Path + "\\modules\\" + mdl.moduleName + "\\ui\\" + filename))
                {
                    bm = SKBitmap.Decode(storageFolder.Path + "\\modules\\" + mdl.moduleName + "\\ui\\" + filename);
                }
                else if (File.Exists(storageFolder.Path + "\\modules\\" + mdl.moduleName + "\\pctokens\\" + filename + ".png"))
                {
                    bm = SKBitmap.Decode(storageFolder.Path + "\\modules\\" + mdl.moduleName + "\\pctokens\\" + filename + ".png");
                }
                else if (File.Exists(storageFolder.Path + "\\modules\\" + mdl.moduleName + "\\pctokens\\" + filename + ".PNG"))
                {
                    bm = SKBitmap.Decode(storageFolder.Path + "\\modules\\" + mdl.moduleName + "\\pctokens\\" + filename + ".PNG");
                }
                else if (File.Exists(storageFolder.Path + "\\modules\\" + mdl.moduleName + "\\pctokens\\" + filename))
                {
                    bm = SKBitmap.Decode(storageFolder.Path + "\\modules\\" + mdl.moduleName + "\\pctokens\\" + filename);
                }
                else if (File.Exists(storageFolder.Path + "\\modules\\" + mdl.moduleName + "\\portraits\\" + filename + ".png"))
                {
                    bm = SKBitmap.Decode(storageFolder.Path + "\\modules\\" + mdl.moduleName + "\\portraits\\" + filename + ".png");
                }
                else if (File.Exists(storageFolder.Path + "\\modules\\" + mdl.moduleName + "\\portraits\\" + filename + ".PNG"))
                {
                    bm = SKBitmap.Decode(storageFolder.Path + "\\modules\\" + mdl.moduleName + "\\portraits\\" + filename + ".PNG");
                }
                else if (File.Exists(storageFolder.Path + "\\modules\\" + mdl.moduleName + "\\portraits\\" + filename))
                {
                    bm = SKBitmap.Decode(storageFolder.Path + "\\modules\\" + mdl.moduleName + "\\portraits\\" + filename);
                }
                //STOP here if already found bitmap
                if (bm != null)
                {
                    return bm;
                }
                //If not found then try in Asset folder
                Assembly assembly = GetType().GetTypeInfo().Assembly;
                Stream stream = assembly.GetManifestResourceStream("IBx.UWP.Assets.graphics." + filename);
                if (stream == null)
                {
                    stream = assembly.GetManifestResourceStream("IBx.UWP.Assets.graphics." + filename + ".png");
                }
                if (stream == null)
                {
                    stream = assembly.GetManifestResourceStream("IBx.UWP.Assets.graphics." + filename + ".jpg");
                }
                if (stream == null)
                {
                    stream = assembly.GetManifestResourceStream("IBx.UWP.Assets.tiles." + filename);
                }
                if (stream == null)
                {
                    stream = assembly.GetManifestResourceStream("IBx.UWP.Assets.tiles." + filename + ".png");
                }
                if (stream == null)
                {
                    stream = assembly.GetManifestResourceStream("IBx.UWP.Assets.tiles." + filename + ".jpg");
                }
                if (stream == null)
                {
                    stream = assembly.GetManifestResourceStream("IBx.UWP.Assets.ui." + filename);
                }
                if (stream == null)
                {
                    stream = assembly.GetManifestResourceStream("IBx.UWP.Assets.ui." + filename + ".png");
                }
                if (stream == null)
                {
                    stream = assembly.GetManifestResourceStream("IBx.UWP.Assets.ui." + filename + ".jpg");
                }
                if (stream == null)
                {
                    stream = assembly.GetManifestResourceStream("IBx.UWP.Assets.graphics.ui_missingtexture.png");
                }
                SKManagedStream skStream = new SKManagedStream(stream);
                return SKBitmap.Decode(skStream);
            }        
            catch (Exception ex)
            {
                Assembly assembly = GetType().GetTypeInfo().Assembly;
                Stream stream = assembly.GetManifestResourceStream("IBx.UWP.Assets.graphics.ui_missingtexture.png");                
                SKManagedStream skStream = new SKManagedStream(stream);
                return SKBitmap.Decode(skStream);
            }
        }

        public List<string> GetAllFilesWithExtensionFromUserFolder(string folderpath, string extension)
        {
            List<string> list = new List<string>();
            StorageFolder storageFolder = ApplicationData.Current.LocalFolder;
            string[] files = Directory.GetFiles(storageFolder.Path + ConvertFullPath(folderpath, "\\"), "*" + extension, SearchOption.AllDirectories);
            foreach (string file in files)
            {
                list.Add(Path.GetFileName(file));                
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
                    list.Add(res);
                }
            }
            return list;
        }
        public List<string> GetAllFilesWithExtensionFromBothFolders(string assetFolderpath, string userFolderpath, string extension)
        {
            List<string> list = new List<string>();
            StorageFolder storageFolder = ApplicationData.Current.LocalFolder;
            string[] files = Directory.GetFiles(storageFolder.Path + ConvertFullPath(userFolderpath, "\\"), "*" + extension, SearchOption.AllDirectories);
            foreach (string file in files)
            {
                list.Add(Path.GetFileName(file));
            }
            Assembly assembly = GetType().GetTypeInfo().Assembly;
            foreach (var res in assembly.GetManifestResourceNames())
            {
                if ((res.Contains(ConvertFullPath(assetFolderpath, "."))) && (res.EndsWith(extension)))
                {
                    list.Add(res);
                }
            }
            return list;
        }
        public List<string> GetAllModuleFiles()
        {
            List<string> list = new List<string>();
            //search in assets
            Assembly assembly = GetType().GetTypeInfo().Assembly;
            foreach (var res in assembly.GetManifestResourceNames())
            {
                if (res.EndsWith(".mod"))
                {
                    list.Add(res);
                }
            }
            //search in personal folder
            StorageFolder storageFolder = ApplicationData.Current.LocalFolder;
            string[] files = Directory.GetFiles(storageFolder.Path + "\\modules", "*.mod", SearchOption.AllDirectories);
            foreach (string file in files)
            {
                if (Path.GetFileName(file) != "NewModule.mod")
                {
                    list.Add(Path.GetFileName(file));
                }
            }
            
            return list;
        }

        //Android.Media.MediaPlayer playerAreaMusic;
        public void CreateAreaMusicPlayer()
        {
            //playerAreaMusic = new Android.Media.MediaPlayer();
            //playerAreaMusic.Looping = true;
            //playerAreaMusic.SetVolume(0.5f, 0.5f);
        }
        public void LoadAreaMusicFile(string fileName)
        {
            //playerAreaMusic.Reset();
            //AssetFileDescriptor afd = Android.App.Application.Context.Assets.OpenFd(fileName);
            //playerAreaMusic.SetDataSource(afd.FileDescriptor, afd.StartOffset, afd.Length);
        }
        public void PlayAreaMusic()
        {
            /*
            if (playerAreaMusic == null)
            {
                return;
            }
            if (playerAreaMusic.IsPlaying)
            {
                playerAreaMusic.Pause();
                playerAreaMusic.SeekTo(0);
            }
            playerAreaMusic.Start();
            */
        }
        public void StopAreaMusic()
        {
            //playerAreaMusic.Pause();
            //playerAreaMusic.SeekTo(0);
        }
        public void PauseAreaMusic()
        {
            //playerAreaMusic.Pause();
        }
    }
}
