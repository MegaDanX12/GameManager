using GameManager.LauncherData;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using ValveKeyValue;

namespace GameManager.Models
{
    /// <summary>
    /// Informazioni su un gioco di Steam.
    /// </summary>
    public class SteamGameData : GameData
    {

        private const uint Magic28 = 0x07_56_44_28;

        private const uint Magic = 0x07_56_44_27;

        /// <summary>
        /// Indica se la classe rappresenta un gioco.
        /// </summary>
        public bool IsGame { get; private init; } = true;

        /// <summary>
        /// Stringa di avvio del gioco.
        /// </summary>
        public string? AppLaunchString { get; private init; }

        /// <summary>
        /// Inizializza una nuova istanza di <see cref="SteamGameData"/>.
        /// </summary>
        /// <param name="ManifestFilePath">Percorso del manifesto che descrive il gioco.</param>
        /// <param name="LibraryPath">Percorso della libreria di cui il gioco fa parte.</param>
        /// <exception cref="InvalidDataException"></exception>
        public SteamGameData(string ManifestFilePath, string LibraryPath)
        {
            FileStream ManifestFile = File.OpenRead(ManifestFilePath);
            KVSerializer Deserializer = KVSerializer.Create(KVSerializationFormat.KeyValues1Text);
            KVObject Data = Deserializer.Deserialize(ManifestFile);
            AppID = (string)Data["appid"];
            Title = (string)Data["name"];
            GamePath = LibraryPath + "\\common\\" + (string)Data["installdir"];
            using FileStream AppInfoFile = File.OpenRead(SteamLauncherData.LauncherPath + "\\appcache\\appinfo.vdf");
            using BinaryReader Reader = new(AppInfoFile, Encoding.Default, true);
            uint MagicValue = Reader.ReadUInt32();
            if (MagicValue is not Magic && MagicValue is not Magic28)
            {
                throw new InvalidDataException("Invalid AppInfo file.");
            }
            AppInfoFile.Position += 4;
            uint SectionSize;
            uint AppIDNumber = Convert.ToUInt32(AppID);
            uint AppIDRead;
            do
            {
                AppIDRead = Reader.ReadUInt32();
                if (AppIDRead != AppIDNumber)
                {
                    SectionSize = Reader.ReadUInt32();
                    AppInfoFile.Position += SectionSize;
                }
                else
                {
                    if (MagicValue is Magic28)
                    {
                        AppInfoFile.Position += 64;
                    }
                    else
                    {
                        AppInfoFile.Position += 44;
                    }
                }
            }
            while (AppIDRead != AppIDNumber);
            Deserializer = KVSerializer.Create(KVSerializationFormat.KeyValues1Binary);
            KVObject AppInfoData = Deserializer.Deserialize(AppInfoFile);
            if ((string)AppInfoData["common"]["type"] is "Game" or "game")
            {
                List<KVObject> LaunchData = ((IEnumerable<KVObject>)AppInfoData["config"]["launch"]).ToList();
                for (byte i = 0; i < LaunchData.Count; i++)
                {
                    if (Path.GetExtension((string)LaunchData[i]["executable"]) is ".exe" or ".bat" or ".cmd")
                    {
                        Executables.Add((string)LaunchData[i]["executable"]);
                        LaunchArguments = LaunchArguments is null && LaunchData[i]["arguments"] is not null ? (string)LaunchData[i]["arguments"] : null;
                        ExecutableName ??= Executables[0];
                    }
                }
                AppLaunchString = "steam://rungameid/" + AppID;
            }
            else
            {
                AppLaunchString = null;
                ExecutableName = null;
                IsGame = false;
            }
            InstallDate = Directory.GetCreationTime(GamePath);
            Platform = GamePlatform.Steam;
        }
    }
}