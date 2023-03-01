using Microsoft.Win32;
using System.Collections.Generic;
using System.IO;
using ValveKeyValue;

namespace GameManager.LauncherData
{
    /// <summary>
    /// Informazioni sul launcher di Steam.
    /// </summary>
    public static class SteamLauncherData
    {
        /// <summary>
        /// Percorso di installazione del launcher.
        /// </summary>
        public static string? LauncherPath { get; private set; }

        /// <summary>
        /// Percorsi librerie.
        /// </summary>
        public static List<string>? LibrariesPath { get; private set; } = new();

        /// <summary>
        /// Recupera informazioni sul launcher di Steam.
        /// </summary>
        public static void RetrieveSteamLauncherData()
        {
            LauncherPath = FindSteamLauncherPath();
            if (LauncherPath is not null)
            {
                FindSteamLibrariesPath();
            }
            else
            {
                LibrariesPath = null;
            }
        }

        /// <summary>
        /// Recupera il percorso della cartella di installazione di Steam.
        /// </summary>
        /// <returns>Il percorso di installazione di Steam.</returns>
        private static string? FindSteamLauncherPath()
        {
            using RegistryKey? SteamKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64).OpenSubKey(@"SOFTWARE\WOW6432Node\Valve\Steam");
            if (SteamKey is not null)
            {
                return (string?)SteamKey.GetValue("InstallPath");
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Recupera i percorsi delle librerie di Steam.
        /// </summary>
        private static void FindSteamLibrariesPath()
        {
            FileStream LibrariesDataFile = File.OpenRead(LauncherPath + "\\steamapps\\libraryfolders.vdf");
            KVSerializer Deserializer = KVSerializer.Create(KVSerializationFormat.KeyValues1Text);
            KVObject Data = Deserializer.Deserialize(LibrariesDataFile);
            string LibraryPath;
            foreach (KVObject child in Data.Children)
            {
                LibraryPath = (string)child["path"];
                if (Directory.Exists(LibraryPath))
                {
                    LibrariesPath!.Add(LibraryPath);
                }
            }
        }
    }
}