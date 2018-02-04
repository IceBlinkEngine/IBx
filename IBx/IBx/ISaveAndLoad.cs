using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IBx
{
    /// <summary>
    /// Define an API for loading and saving a text file. Reference this interface
    /// in the common code, and implement this interface in the app projects for
    /// iOS, Android and WinPhone. Remember to use the 
    ///     [assembly: Dependency (typeof (SaveAndLoad_IMPLEMENTATION_CLASSNAME))]
    /// attribute on each of the implementations.
    /// </summary>
    public interface ISaveAndLoad
    {
        void SaveText(string filename, string text);

        string LoadStringFromUserFolder(string fullPath);
        string LoadStringFromAssetFolder(string fullPath);
        string LoadStringFromEitherFolder(string assetFolderpath, string userFolderpath);

        string GetModuleFileString(string modFilename);
        string GetModuleAssetFileString(string modFolder, string assetFilename);
        string GetSettingsString();
        string GetDataAssetFileString(string assetFilename);
        string GetSaveFileString(string modName, string filename);
        List<string> GetGraphicsFiles(string modFolder, string endsWith);
        List<string> GetTileFiles(string modFolder, string endsWith);
        List<string> GetCharacterFiles(string modFolder, string endsWith);
        void SaveCharacter(string modName, string filename, Player pc);
        void SaveModuleAssetFile(string modFolder, string assetFilenameWithExtension, string json);
        //void SaveSaveGame(string modName, string filename, SaveGame save);

        void SaveBitmap(string filename, SKBitmap bmp);
        SKBitmap LoadBitmap(string filename, Module mdl);

        List<string> GetAllFilesWithExtensionFromUserFolder(string folderpath, string extension);
        List<string> GetAllFilesWithExtensionFromAssetFolder(string folderpath, string extension);
        List<string> GetAllFilesWithExtensionFromBothFolders(string assetFolderpath, string userFolderpath, string extension);

        List<string> GetAllModuleFiles();
        List<string> GetAllAreaFilenames(string modFolder);
        List<string> GetAllConvoFilenames(string modFolder);
        List<string> GetAllEncounterFilenames(string modFolder);

        bool FileExists(string filename);
    }
}
