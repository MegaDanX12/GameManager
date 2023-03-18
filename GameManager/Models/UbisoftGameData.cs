using GameManager.LauncherData;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace GameManager.Models
{
    /// <summary>
    /// Informazioni su un gioco Ubisoft.
    /// </summary>
    public class UbisoftGameData : GameData
    {
        /// <summary>
        /// Stringa di lancio applicazione.
        /// </summary>
        public string? AppLaunchString { get; set; }

        /// <summary>
        /// Inizializza una nuova istanza di <see cref="UbisoftGameData"/>.
        /// </summary>
        /// <param name="GamePath">Percorso della cartella di installazione del gioco.</param>
        public UbisoftGameData(string GamePath, string AppID)
        {
            this.GamePath = GamePath;
            this.AppID = AppID;
            StringBuilder TitleBuilder = new(GamePath);
            TitleBuilder.Remove(0, GamePath.LastIndexOf('\\'));
            TitleBuilder.Remove(0, 1);
            Title = TitleBuilder.ToString();
            string[] ExecutableFiles = Directory.GetFiles(GamePath, "*.exe", SearchOption.AllDirectories);
            foreach (string path in ExecutableFiles)
            {
                Executables.Add(path.Replace(GamePath + "\\", string.Empty));
            }
            if (Executables.Count is 1)
            {
                ExecutableName = Executables[0];
                AppLaunchString = "uplay://launch/" + AppID + "/0";
            }
            InstallDate = Directory.GetCreationTime(GamePath);
            Platform = GamePlatform.Ubisoft;
        }

        public override bool StartGame()
        {
            if (!Process.GetProcessesByName("upc").Any())
            {
                using Process? LauncherProcess = Process.Start(UbisoftLauncherData.LauncherPath!);
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
            using Process? UpcProcess = Process.Start(Path.GetDirectoryName(UbisoftLauncherData.LauncherPath!) + "\\upc.exe", "uplay://uninstall/" + AppID);
            if (UpcProcess is not null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public override bool CreateShortcut()
        {
            if (AppLaunchString is not null && ExecutableName is not null)
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
            else
            {
                return false;
            }
        }
    }
}