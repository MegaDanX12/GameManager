using GameManager.LauncherData;
using IWshRuntimeLibrary;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace GameManager.Models
{
    /// <summary>
    /// Informazioni su un gioco EA.
    /// </summary>
    public class EAGameData : GameData, INotifyPropertyChanged
    {
        /// <summary>
        /// Indica se il gioco è installato.
        /// </summary>
        public bool IsInstalled { get; } = true;

        /// <summary>
        /// Inizializza una nuova istanza di <see cref="EAGameData"/>.
        /// </summary>
        /// <param name="GameFolderPath">Percorso cartella di gioco.</param>
        public EAGameData(string GameFolderPath)
        {
            try
            {
                AppID = null;
                GamePath = GameFolderPath;
                XDocument InstallerManifest = XDocument.Load(GameFolderPath + "\\__Installer\\installerdata.xml");
                string CurrentLocale = CultureInfo.CurrentCulture.Name.Replace('-', '_');
                IEnumerable<XElement> GameTitles = InstallerManifest.Descendants("gameTitle");
                IEnumerable<XElement> LauncherData = InstallerManifest.Descendants("launcher");
                IEnumerable<XElement> LauncherElements;
                foreach (XElement launcherdata in LauncherData)
                {
                    LauncherElements = launcherdata.Elements();
                    XElement LauncherElement = LauncherElements.FirstOrDefault((element) => element.Name == "trial")!;
                    if (LauncherElement.Value is "0")
                    {
                        XElement FilepathElement = LauncherElements.FirstOrDefault((element) => element.Name == "filePath")!;
                        ExecutableName = FilepathElement.Value.Remove(0, FilepathElement.Value.LastIndexOf(']') + 1);
                        XElement? ArgumentsElement = LauncherElements.FirstOrDefault((element) => element.Name == "parameters");
                        if (ArgumentsElement is not null)
                        {
                            LaunchArguments = ArgumentsElement.Value;
                        }
                        break;
                    }
                }
                foreach (XElement title in GameTitles)
                {
                    if (title.Attribute("locale")!.Value.Contains(CurrentLocale))
                    {
                        Title = title.Value;
                        break;
                    }
                }
                XElement UninstallElement = InstallerManifest.Descendants().FirstOrDefault((element) => element.Name == "uninstall")!;
                string UninstallData = UninstallElement.Element("path")!.Value;
                string RegistryPath = UninstallData[..UninstallData.LastIndexOf('\\')].Remove(0, 1);
                string RootKey = RegistryPath[..RegistryPath.IndexOf('\\')];
                RegistryHive? Hive = RootKey switch
                {
                    "HKEY_LOCAL_MACHINE" => (RegistryHive?)RegistryHive.LocalMachine,
                    "HKEY_CURRENT_USER" => (RegistryHive?)RegistryHive.CurrentUser,
                    _ => null,
                };
                if (Hive is not null)
                {
                    RegistryKey? UninstallKey = RegistryKey.OpenBaseKey(Hive.Value, RegistryView.Registry32).OpenSubKey(RegistryPath.Replace(RootKey + "\\", string.Empty));
                    if (UninstallKey is not null)
                    {
                        UninstallString = (string)UninstallKey.GetValue("UninstallString")!;
                        UninstallKey.Dispose();
                    }
                    else
                    {
                        UninstallKey = RegistryKey.OpenBaseKey(Hive.Value, RegistryView.Registry64).OpenSubKey(RegistryPath.Replace(RootKey + "\\", string.Empty));
                        if (UninstallKey is not null)
                        {
                            UninstallString = (string)UninstallKey.GetValue("UninstallString")!;
                            UninstallKey.Dispose();
                        }
                        else
                        {
                            UninstallString = null;
                        }
                    }
                }
                else
                {
                    UninstallString = null;
                }
                InstallDate = Directory.GetCreationTime(GameFolderPath);
                Platform = GamePlatform.EA;
            }
            catch
            {
                IsInstalled = false;
            }
        }

        public override bool StartGame()
        {
            if (!Process.GetProcessesByName("EADesktop").Any())
            {
                using Process? LauncherProcess = Process.Start(EALauncherData.LauncherPath!);
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
            ProcessStartInfo StartInfo = new(GamePath + "\\" + ExecutableName, LaunchArguments is not null ? LaunchArguments : string.Empty);
            using Process? GameProcess = Process.Start(StartInfo);
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

        private void GameProcess_Exited(object? sender, System.EventArgs e)
        {
            IsRunning = false;
        }

        public override bool UninstallGame()
        {
            if (UninstallString is not null)
            {
                string FileName = (UninstallString[..(UninstallString.LastIndexOf("\"") + 1)].Replace("\"", string.Empty));
                string Parameters = UninstallString.Replace(FileName, string.Empty).Replace("\"", string.Empty).TrimStart();
                using Process UninstallerProcess = Process.Start(FileName, Parameters);
                if (UninstallerProcess is not null)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public override bool CreateShortcut()
        {
            string DesktopPath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
            WshShell Shell = new();
            IWshShortcut Shortcut = Shell.CreateShortcut(DesktopPath + "\\" + Title + ".lnk");
            Shortcut.TargetPath = GamePath + "\\" + ExecutableName;
            Shortcut.Save();
            return true;
        }
    }
}