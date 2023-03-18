using GameManager.LauncherData;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace GameManager.Models
{
    /// <summary>
    /// Informazioni su un gioco Epic Games.
    /// </summary>
    public class EGLGameData : GameData
    {

        private readonly string CatalogNamespace;


        private readonly string CatalogItemId;


        private readonly string AppName;

        /// <summary>
        /// Stringa di avvio del gioco.
        /// </summary>
        public string? AppLaunchString { get; private init; }

        /// <summary>
        /// Inizializza una nuova istanza di <see cref="EGLGameData"/>.
        /// </summary>
        /// <param name="ManifestFilePath">Percorso del file di manifesto che descrive il gioco.</param>
        public EGLGameData(string ManifestFilePath) 
        {
            StreamReader Reader = new(File.OpenRead(ManifestFilePath));
            string? Line = Reader.ReadLine();
            while (Line is not null)
            {
                if (Line.Contains("LaunchExecutable"))
                {
                    ExecutableName = Line.TrimStart().Replace("\"", string.Empty).Replace("LaunchExecutable: ", string.Empty).Replace('/', '\\').Replace(",", string.Empty);
                    Executables.Add(ExecutableName);
                }
                if (Line.Contains("LaunchCommand"))
                {
                    LaunchArguments = Line.TrimStart().Replace("\"", string.Empty).Replace("LaunchCommand: ", string.Empty).Replace('/', '\\').Replace(",", string.Empty);
                }
                if (Line.Contains("DisplayName"))
                {
                    Title = Line.TrimStart().Replace("\"", string.Empty).Replace("DisplayName: ", string.Empty).Replace('/', '\\').Replace(",", string.Empty);
                }
                if (Line.Contains("InstallLocation"))
                {
                    GamePath = Line.TrimStart().Replace("\"", string.Empty).Replace("InstallLocation: ", string.Empty).Replace('/', '\\').Replace("\\\\", "\\").Replace(",", string.Empty);
                }
                if (Line.Contains("MainGameCatalogNamespace"))
                {
                    CatalogNamespace = Line.TrimStart().Replace("\"", string.Empty).Replace("MainGameCatalogNamespace: ", string.Empty).Replace('/', '\\').Replace(",", string.Empty);
                }
                if (Line.Contains("MainGameCatalogItemId"))
                {
                    CatalogItemId = Line.TrimStart().Replace("\"", string.Empty).Replace("MainGameCatalogItemId: ", string.Empty).Replace('/', '\\').Replace(",", string.Empty);
                }
                if (Line.Contains("MainGameAppName"))
                {
                    AppName = Line.TrimStart().Replace("\"", string.Empty).Replace("MainGameAppName: ", string.Empty).Replace('/', '\\').Replace(",", string.Empty);
                }
                Line = Reader.ReadLine();
            }
            InstallDate = Directory.GetCreationTime(GamePath);
            Platform = GamePlatform.EpicGames;
            AppLaunchString = "com.epicgames.launcher://apps/" + CatalogNamespace + "%3A" + CatalogItemId + "%3A" + AppName + "?action=launch";
        }

        public override bool StartGame()
        {
            if (!Process.GetProcessesByName("EpicGamesLauncher").Any())
            {
                using Process? LauncherProcess = Process.Start(EpicGamesLauncherData.LauncherPath!);
                if (LauncherProcess is not null)
                {
                    if (!LauncherProcess.WaitForInputIdle(10000))
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            using Process? GameProcess = Process.Start(AppLaunchString!);
            if (GameProcess is not null)
            {
                GameProcess.EnableRaisingEvents = true;
                GameProcess.Exited += GameProcess_Exited;
                IsRunning = true;
                return true;
            }
            else
            {
                return false;
            }
        }

        private void GameProcess_Exited(object? sender, EventArgs e)
        {
            IsRunning = false;
        }

        public override bool UninstallGame()
        {
            throw new NotImplementedException();
        }

        public override bool CreateShortcut()
        {
            string DesktopPath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
            using StreamWriter Writer = new(DesktopPath + "\\" + Title + ".url");
            Writer.WriteLine("[InternetShortcut]");
            Writer.WriteLine("URL=" + AppLaunchString);
            Writer.WriteLine("IconIndex=0");
            Writer.WriteLine("IconFile=" + GamePath + "\\" + ExecutableName);
            Writer.Flush();
            return true;
        }
    }
}