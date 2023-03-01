﻿using Microsoft.Win32;
using System;
using System.IO;

namespace GameManager.LauncherData
{
    /// <summary>
    /// Informazioni sul launcher di Epic Games.
    /// </summary>
    public static class EpicGamesLauncherData
    {
        /// <summary>
        /// Percorso di installazione del launcher di Epic Games.
        /// </summary>
        public static string? LauncherPath { get; private set; }

        /// <summary>
        /// Percorso della libreria di giochi.
        /// </summary>
        public static string? LibraryPath { get; private set; }

        /// <summary>
        /// Recupera informazioni sul launcher di Epic Games.
        /// </summary>
        public static void RetrieveEpicGamesLauncherData()
        {
            LauncherPath = FindEpicGamesLauncherPath();
            if (LauncherPath is not null)
            {
                LibraryPath = FindEpicGamesLauncherLibraryPath();
            }
            else
            {
                LibraryPath = null;
            }
        }

        /// <summary>
        /// Recupera il percorso del launcher di Epic Games.
        /// </summary>
        /// <returns>Il percorso di installazione.</returns>
        public static string? FindEpicGamesLauncherPath()
        {
            RegistryKey UninstallKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64).OpenSubKey(@"SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\Uninstall")!;
            string[] SubKeys = UninstallKey.GetSubKeyNames();
            RegistryKey ApplicationKey;
            string DisplayName;
            foreach (string subkey in SubKeys)
            {
                ApplicationKey = UninstallKey.OpenSubKey(subkey)!;
                DisplayName = (string)ApplicationKey.GetValue("DisplayName")!;
                if (DisplayName is not null && DisplayName is "Epic Games Launcher")
                {
                    UninstallKey.Close();
                    return (string)ApplicationKey.GetValue("InstallLocation")! + "Launcher\\Portal\\";
                }
            }
            UninstallKey.Close();
            UninstallKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64).OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall")!;
            SubKeys = UninstallKey.GetSubKeyNames();
            foreach (string subkey in SubKeys)
            {
                ApplicationKey = UninstallKey.OpenSubKey(subkey)!;
                DisplayName = (string)ApplicationKey.GetValue("DisplayName")!;
                if (DisplayName is not null && DisplayName is "Epic Games Launcher")
                {
                    UninstallKey.Close();
                    return (string)ApplicationKey.GetValue("InstallLocation")! + "Launcher\\Portal\\";
                }
            }
            UninstallKey.Close();
            return null;
        }

        /// <summary>
        /// Recupera il percorso della libreria di giochi Epic Games.
        /// </summary>
        /// <returns>Il percorso della libreria.</returns>
        public static string? FindEpicGamesLauncherLibraryPath()
        {
            using StreamReader ConfigFile = new(File.OpenRead(Environment.GetEnvironmentVariable("LocalAppData") + "\\EpicGamesLauncher\\Saved\\Config\\Windows\\GameUserSettings.ini"));
            string Line = string.Empty;
            while (!Line.Contains("DefaultAppInstallLocation"))
            {
                Line = ConfigFile.ReadLine()!;
            }
            return Line.Replace("DefaultAppInstallLocation=", string.Empty);
        }
    }
}