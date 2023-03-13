using GameManager.LauncherData;
using GameManager.Models;
using Microsoft.Win32;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;

namespace GameManager.Viewmodels
{
    /// <summary>
    /// Viewmodel principale.
    /// </summary>
    public class MainViewModel
    {
        /// <summary>
        /// Giochi installati.
        /// </summary>
        private readonly List<GameData> Games;

        /// <summary>
        /// Lista di giochi filtrata.
        /// </summary>
        public ObservableCollection<GameData> FilteredGames { get; init; }

        /// <summary>
        /// Inizializza una nuova istanza di <see cref="MainViewModel"/>.
        /// </summary>
        public MainViewModel()
        {
            SteamLauncherData.RetrieveSteamLauncherData();
            EALauncherData.RetrieveEALauncherData();
            EpicGamesLauncherData.RetrieveEpicGamesLauncherData();
            UbisoftLauncherData.RetrieveUbisoftLauncherData();
            Games = new();
            EnumerateSteamGames();
            EnumerateEAGames();
            EnumerateEGLGames();
            EnumerateUbisoftGames();
            FilteredGames = new(Games);
        }

        /// <summary>
        /// Enumera i giochi di Steam installati.
        /// </summary>
        private void EnumerateSteamGames()
        {
            if (SteamLauncherData.LauncherPath is not null)
            {
                if (SteamLauncherData.LibrariesPath is not null && SteamLauncherData.LibrariesPath.Count > 0)
                {
                    DirectoryInfo[] LibraryDirectories = new DirectoryInfo[SteamLauncherData.LibrariesPath!.Count];
                    for (byte i = 0; i < LibraryDirectories.Length; i++)
                    {
                        LibraryDirectories[i] = new(SteamLauncherData.LibrariesPath![i]);
                    }
                    FileInfo[] AppManifests;
                    SteamGameData GameData;
                    foreach (DirectoryInfo directory in LibraryDirectories)
                    {
                        AppManifests = directory.GetFiles("appmanifest_*.acf");
                        foreach (FileInfo file in AppManifests)
                        {
                            GameData = new SteamGameData(file.FullName, directory.FullName);
                            if (GameData.IsGame)
                            {
                                Games.Add(GameData);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Enumera i giochi EA installati.
        /// </summary>
        private void EnumerateEAGames()
        {
            if (EALauncherData.LauncherPath is not null)
            {
                if (EALauncherData.LibraryPath is not null)
                {
                    DirectoryInfo LibraryFolder = new(EALauncherData.LibraryPath);
                    DirectoryInfo[] GameFolders = LibraryFolder.GetDirectories();
                    EAGameData GameData;
                    foreach (DirectoryInfo directory in GameFolders)
                    {
                        GameData = new(directory.FullName);
                        if (GameData.IsInstalled)
                        {
                            Games.Add(GameData);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Enumera i giochi Epic Games installati.
        /// </summary>
        private void EnumerateEGLGames()
        {
            if (EpicGamesLauncherData.LauncherPath is not null)
            {
                if (EpicGamesLauncherData.LauncherDataPath is not null)
                {
                    string[] ItemFiles = Directory.GetFiles(EpicGamesLauncherData.LauncherDataPath + "Manifests", "*.item");
                    EGLGameData GameData;
                    foreach (string path in ItemFiles)
                    {
                        GameData = new(path);
                        Games.Add(GameData);
                    }
                }
            }
        }

        /// <summary>
        /// Enumera i giochi Ubisoft installati.
        /// </summary>
        private void EnumerateUbisoftGames()
        {
            if (UbisoftLauncherData.LauncherPath is not null)
            {
                if (UbisoftLauncherData.LibraryPath is not null)
                {
                    using RegistryKey? UbisoftGamesKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64).OpenSubKey(@"SOFTWARE\WOW6432Node\Ubisoft\Launcher\Installs");
                    if (UbisoftGamesKey is not null)
                    {
                        string[] SubkeyNames = UbisoftGamesKey.GetSubKeyNames();
                        UbisoftGameData GameData;
                        string InstallDirectory;
                        foreach (string name in SubkeyNames)
                        {
                            using RegistryKey? GameKey = UbisoftGamesKey.OpenSubKey(name);
                            InstallDirectory = ((string)GameKey!.GetValue("InstallDir")!).Replace('/', '\\');
                            if (InstallDirectory.Contains(UbisoftLauncherData.LibraryPath))
                            {
                                GameData = new(InstallDirectory.Remove(InstallDirectory.Length - 1, 1), name);
                                Games.Add(GameData);
                            }
                            
                        }
                    }
                }
            }
        }
    }
}