using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace GameManager.Models
{
    /// <summary>
    /// Informazioni su un gioco EA.
    /// </summary>
    public class EAGameData : GameData
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
                InstallDate = Directory.GetCreationTime(GameFolderPath);
                Platform = GamePlatform.EA;
            }
            catch
            {
                IsInstalled = false;
            }
        }
    }
}