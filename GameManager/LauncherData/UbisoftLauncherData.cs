using Microsoft.Win32;
using System;
using System.IO;

namespace GameManager.LauncherData
{
    /// <summary>
    /// Informazioni sul launcher Ubisoft.
    /// </summary>
    public static class UbisoftLauncherData
    {
        /// <summary>
        /// Percorso di installazione del launcher.
        /// </summary>
        public static string? LauncherPath { get; private set; }

        /// <summary>
        /// Percorso della libreria di giochi.
        /// </summary>
        public static string? LibraryPath { get; private set; }

        /// <summary>
        /// Recupera informazioni sul launcher Ubisoft.
        /// </summary>
        public static void RetrieveUbisoftLauncherData()
        {
            LauncherPath = FindUbisoftLauncherPath();
            if (LauncherPath is not null)
            {

            }
            else
            {
                LibraryPath = null;
            }
        }

        /// <summary>
        /// Recupera il percorso di installazione del Launcher Ubisoft.
        /// </summary>
        /// <returns>Il percorso di installazione.</returns>
        private static string? FindUbisoftLauncherPath()
        {
            using RegistryKey? UbisoftKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64).OpenSubKey(@"SOFTWARE\WOW6432Node\Ubisoft\Launcher");
            if (UbisoftKey is not null)
            {
                return (string?)UbisoftKey.GetValue("InstallDir");
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Recupera il percorso della libreria di giochi Ubisoft.
        /// </summary>
        /// <returns>Il percorso della libreria.</returns>
        private static string? FindUbisoftLauncherLibraryPath()
        {
            using StreamReader ConfigFile = new(File.OpenRead(Environment.GetEnvironmentVariable("LocalAppData") + "\\Ubisoft Game Launcher\\settings.yaml"));
            string Line = string.Empty;
            while (!Line.Contains("game_installation_path"))
            {
                Line = ConfigFile.ReadLine()!;
            }
            return Line.Replace("game_installation_path: ", string.Empty).Replace('/', '\\');
        }
    }
}