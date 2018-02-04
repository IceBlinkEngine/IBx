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

[assembly: Dependency(typeof(SaveAndLoad_iOS))]
namespace IBx.iOS
{
    public class SaveAndLoad_iOS : ISaveAndLoad
    {
        #region ISaveAndLoad Text implementation
        public void SaveText(string filename, string text)
        {
            /*string path = CreatePathToFile(filename);
            using (StreamWriter sw = File.CreateText(path))
                await sw.WriteAsync(text);*/
        }        
        public void SaveCharacter(string modName, string filename, Player pc)
        {
            var documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            var filePath = documentsPath + "/saves/" + modName + "/characters/" + filename;
            string json = JsonConvert.SerializeObject(pc, Newtonsoft.Json.Formatting.Indented);
            using (StreamWriter sw = new StreamWriter(filePath))
            {
                sw.Write(json.ToString());
            }
        }
        public void SaveModuleAssetFile(string modFolder, string assetFilenameWithExtension, string json)
        {

        }
        /*public void SaveSaveGame(string modName, string filename, SaveGame save)
        {
            var documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            var filePath = documentsPath + "/saves/" + modName + "/" + filename;
            System.IO.FileInfo file = new System.IO.FileInfo(filePath);
            file.Directory.Create(); // If the directory already exists, this method does nothing.
            string json = JsonConvert.SerializeObject(save, Newtonsoft.Json.Formatting.Indented);
            using (StreamWriter sw = new StreamWriter(filePath))
            {
                sw.Write(json.ToString());
            }
        }*/

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
        public string GetModuleAssetFileString(string modFolder, string assetFilename)
        {
            //try asset area            
            Assembly assembly = GetType().GetTypeInfo().Assembly;
            Stream stream = assembly.GetManifestResourceStream("IBx.iOS." + assetFilename);
            if (stream != null)
            {
                using (var reader = new System.IO.StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
            //try from personal folder first
            var documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            //string modFolder = Path.GetFileNameWithoutExtension(areaFilename);
            var filePath = documentsPath + "/modules/" + modFolder + "/" + assetFilename;
            if (File.Exists(filePath))
            {
                return File.ReadAllText(filePath);
            }
            else //try from external folder
            {
                /*Java.IO.File sdCard = Android.OS.Environment.ExternalStorageDirectory;
                filePath = sdCard.AbsolutePath + "/IBx/modules/" + modFolder + "/" + areaFilename + ".are";
                if (File.Exists(filePath))
                {
                    return File.ReadAllText(filePath);
                }*/
            }
            return "";
        }
        public string GetDataAssetFileString(string assetFilename)
        {
            //try asset area            
            Assembly assembly = GetType().GetTypeInfo().Assembly;
            Stream stream = assembly.GetManifestResourceStream("IBx.iOS." + assetFilename);
            if (stream != null)
            {
                using (var reader = new System.IO.StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
            return "";
        }
        public string GetSettingsString()
        {
            //try from personal folder first
            var documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            //string modFolder = Path.GetFileNameWithoutExtension(areaFilename);
            var filePath = documentsPath + "/settings.json";
            if (File.Exists(filePath))
            {
                return File.ReadAllText(filePath);
            }
            /*else //try from external folder
            {
                Java.IO.File sdCard = Android.OS.Environment.ExternalStorageDirectory;
                filePath = sdCard.AbsolutePath + "/IBx/settings.json";
                if (File.Exists(filePath))
                {
                    return File.ReadAllText(filePath);
                }
            }*/
            return "";
        }
        public string GetSaveFileString(string modName, string filename)
        {
            //try from personal folder first
            var documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            var filePath = documentsPath + "/saves/" + modName + "/" + filename;
            if (File.Exists(filePath))
            {
                return File.ReadAllText(filePath);
            }
            /*else //try from external folder
            {
                Java.IO.File sdCard = Android.OS.Environment.ExternalStorageDirectory;
                filePath = sdCard.AbsolutePath + "/IBx/saves/" + filename;
                if (File.Exists(filePath))
                {
                    return File.ReadAllText(filePath);
                }
            }*/
            return "";
        }
        #endregion        

        #region ISaveAndLoad implementation
        public void SaveBitmap(string filename, SKBitmap bmp)
        {
            //StorageFolder localFolder = ApplicationData.Current.LocalFolder;
            //StorageFile sampleFile = await localFolder.CreateFileAsync(filename, CreationCollisionOption.ReplaceExisting);
            //await FileIO.WriteTextAsync(sampleFile, bmp);
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
        #endregion

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
            /*var documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            Java.IO.File directory = new Java.IO.File(documentsPath + "/modules");
            directory.Mkdirs();
            foreach (Java.IO.File d in directory.ListFiles())
            {
                if (d.IsDirectory)
                {
                    Java.IO.File modDirectory = new Java.IO.File(directory.Path + "/" + d.Name);
                    foreach (Java.IO.File f in modDirectory.ListFiles())
                    {
                        try
                        {
                            if (f.Name.EndsWith(".mod"))
                            {
                                list.Add(f.Name);
                            }
                        }
                        catch (Exception ex)
                        {

                        }
                    }
                }
            }*/

            //search in external folder
            /*Java.IO.File sdCard = Android.OS.Environment.ExternalStorageDirectory;
            directory = new Java.IO.File(sdCard.AbsolutePath + "/IBx/modules");
            directory.Mkdirs();
            //check to see if Lanterna2 exists, if not copy it over
            foreach (Java.IO.File d in directory.ListFiles())
            {
                if (d.IsDirectory)
                {
                    Java.IO.File modDirectory = new Java.IO.File(directory.Path + "/" + d.Name);
                    foreach (Java.IO.File f in modDirectory.ListFiles())
                    {
                        try
                        {
                            if (f.Name.EndsWith(".mod"))
                            {
                                list.Add(f.Name);
                            }
                        }
                        catch (Exception ex)
                        {

                        }
                    }
                }
            }*/
            return list;
        }
        public List<string> GetAllAreaFilenames(string modFolder)
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
            /*var documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            Java.IO.File directory = new Java.IO.File(documentsPath + "/modules");
            directory.Mkdirs();
            foreach (Java.IO.File d in directory.ListFiles())
            {
                if (d.IsDirectory)
                {
                    Java.IO.File modDirectory = new Java.IO.File(directory.Path + "/" + d.Name);
                    foreach (Java.IO.File f in modDirectory.ListFiles())
                    {
                        try
                        {
                            if (f.Name.EndsWith(".mod"))
                            {
                                list.Add(f.Name);
                            }
                        }
                        catch (Exception ex)
                        {

                        }
                    }
                }
            }*/

            //search in external folder
            /*Java.IO.File sdCard = Android.OS.Environment.ExternalStorageDirectory;
            directory = new Java.IO.File(sdCard.AbsolutePath + "/IBx/modules");
            directory.Mkdirs();
            //check to see if Lanterna2 exists, if not copy it over
            foreach (Java.IO.File d in directory.ListFiles())
            {
                if (d.IsDirectory)
                {
                    Java.IO.File modDirectory = new Java.IO.File(directory.Path + "/" + d.Name);
                    foreach (Java.IO.File f in modDirectory.ListFiles())
                    {
                        try
                        {
                            if (f.Name.EndsWith(".mod"))
                            {
                                list.Add(f.Name);
                            }
                        }
                        catch (Exception ex)
                        {

                        }
                    }
                }
            }*/
            return list;
        }
        public List<string> GetAllConvoFilenames(string modFolder)
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
            /*var documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            Java.IO.File directory = new Java.IO.File(documentsPath + "/modules");
            directory.Mkdirs();
            foreach (Java.IO.File d in directory.ListFiles())
            {
                if (d.IsDirectory)
                {
                    Java.IO.File modDirectory = new Java.IO.File(directory.Path + "/" + d.Name);
                    foreach (Java.IO.File f in modDirectory.ListFiles())
                    {
                        try
                        {
                            if (f.Name.EndsWith(".mod"))
                            {
                                list.Add(f.Name);
                            }
                        }
                        catch (Exception ex)
                        {

                        }
                    }
                }
            }*/

            //search in external folder
            /*Java.IO.File sdCard = Android.OS.Environment.ExternalStorageDirectory;
            directory = new Java.IO.File(sdCard.AbsolutePath + "/IBx/modules");
            directory.Mkdirs();
            //check to see if Lanterna2 exists, if not copy it over
            foreach (Java.IO.File d in directory.ListFiles())
            {
                if (d.IsDirectory)
                {
                    Java.IO.File modDirectory = new Java.IO.File(directory.Path + "/" + d.Name);
                    foreach (Java.IO.File f in modDirectory.ListFiles())
                    {
                        try
                        {
                            if (f.Name.EndsWith(".mod"))
                            {
                                list.Add(f.Name);
                            }
                        }
                        catch (Exception ex)
                        {

                        }
                    }
                }
            }*/
            return list;
        }
        public List<string> GetAllEncounterFilenames(string modFolder)
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
            /*var documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            Java.IO.File directory = new Java.IO.File(documentsPath + "/modules");
            directory.Mkdirs();
            foreach (Java.IO.File d in directory.ListFiles())
            {
                if (d.IsDirectory)
                {
                    Java.IO.File modDirectory = new Java.IO.File(directory.Path + "/" + d.Name);
                    foreach (Java.IO.File f in modDirectory.ListFiles())
                    {
                        try
                        {
                            if (f.Name.EndsWith(".mod"))
                            {
                                list.Add(f.Name);
                            }
                        }
                        catch (Exception ex)
                        {

                        }
                    }
                }
            }*/

            //search in external folder
            /*Java.IO.File sdCard = Android.OS.Environment.ExternalStorageDirectory;
            directory = new Java.IO.File(sdCard.AbsolutePath + "/IBx/modules");
            directory.Mkdirs();
            //check to see if Lanterna2 exists, if not copy it over
            foreach (Java.IO.File d in directory.ListFiles())
            {
                if (d.IsDirectory)
                {
                    Java.IO.File modDirectory = new Java.IO.File(directory.Path + "/" + d.Name);
                    foreach (Java.IO.File f in modDirectory.ListFiles())
                    {
                        try
                        {
                            if (f.Name.EndsWith(".mod"))
                            {
                                list.Add(f.Name);
                            }
                        }
                        catch (Exception ex)
                        {

                        }
                    }
                }
            }*/
            return list;
        }
        public List<string> GetGraphicsFiles(string modFolder, string endsWith)
        {
            List<string> list = new List<string>();

            //search in assets
            Assembly assembly = GetType().GetTypeInfo().Assembly;
            foreach (var res in assembly.GetManifestResourceNames())
            {
                if ((res.EndsWith(endsWith)) && (res.Contains(".graphics.")))
                {
                    list.Add(res);
                }
            }
            /*
            //search in personal folder
            var documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            Java.IO.File directory = new Java.IO.File(documentsPath + "/" + path);
            directory.Mkdirs();
            foreach (Java.IO.File f in directory.ListFiles())
            {
                if (f.Name.EndsWith(endsWith))
                {
                    list.Add(f.Name);
                }
            }

            //search in external folder
            Java.IO.File sdCard = Android.OS.Environment.ExternalStorageDirectory;
            directory = new Java.IO.File(sdCard.AbsolutePath + "/IBx/" + path);
            directory.Mkdirs();
            //check to see if Lanterna2 exists, if not copy it over
            foreach (Java.IO.File f in directory.ListFiles())
            {
                if (f.Name.EndsWith(endsWith))
                {
                    list.Add(f.Name);
                }
            }*/
            return list;
        }
        public List<string> GetTileFiles(string modFolder, string endsWith)
        {
            List<string> list = new List<string>();

            //search in assets
            Assembly assembly = GetType().GetTypeInfo().Assembly;
            foreach (var res in assembly.GetManifestResourceNames())
            {
                if ((res.EndsWith(endsWith)) && (res.Contains(".tiles.")))
                {
                    list.Add(res);
                }
            }
            /*
            //search in personal folder
            var documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            Java.IO.File directory = new Java.IO.File(documentsPath + "/" + path);
            directory.Mkdirs();
            foreach (Java.IO.File f in directory.ListFiles())
            {
                if (f.Name.EndsWith(endsWith))
                {
                    list.Add(f.Name);
                }
            }

            //search in external folder
            Java.IO.File sdCard = Android.OS.Environment.ExternalStorageDirectory;
            directory = new Java.IO.File(sdCard.AbsolutePath + "/IBx/" + path);
            directory.Mkdirs();
            //check to see if Lanterna2 exists, if not copy it over
            foreach (Java.IO.File f in directory.ListFiles())
            {
                if (f.Name.EndsWith(endsWith))
                {
                    list.Add(f.Name);
                }
            }*/
            return list;
        }
        public List<string> GetCharacterFiles(string modFolder, string endsWith)
        {
            List<string> list = new List<string>();

            //search in assets
            Assembly assembly = GetType().GetTypeInfo().Assembly;
            foreach (var res in assembly.GetManifestResourceNames())
            {
                if ((res.EndsWith(endsWith)) && (res.Contains(".saves." + modFolder + ".characters")))
                {
                    list.Add(res);
                }
            }
            /*
            //search in personal folder
            var documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            Java.IO.File directory = new Java.IO.File(documentsPath + "/" + path);
            directory.Mkdirs();
            foreach (Java.IO.File f in directory.ListFiles())
            {
                if (f.Name.EndsWith(endsWith))
                {
                    list.Add(f.Name);
                }
            }

            //search in external folder
            Java.IO.File sdCard = Android.OS.Environment.ExternalStorageDirectory;
            directory = new Java.IO.File(sdCard.AbsolutePath + "/IBx/" + path);
            directory.Mkdirs();
            //check to see if Lanterna2 exists, if not copy it over
            foreach (Java.IO.File f in directory.ListFiles())
            {
                if (f.Name.EndsWith(endsWith))
                {
                    list.Add(f.Name);
                }
            }*/
            return list;
        }

        public bool FileExists(string filename)
        {
            return File.Exists(CreatePathToFile(filename));
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