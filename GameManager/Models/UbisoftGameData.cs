using System.IO;
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
    }
}