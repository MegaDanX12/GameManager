using System.IO;

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
            AppLaunchString = "com.epicgames.launcher://apps/" + CatalogNamespace + "%3A" + CatalogItemId + "%3A" + AppName + "?action=launch&silent=true";
        }
    }
}