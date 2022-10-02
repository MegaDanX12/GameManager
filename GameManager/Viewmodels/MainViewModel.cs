using GameLib;
using GameLib.Core;
using GameManager.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameManager.Viewmodels
{
    /// <summary>
    /// Viewmodel principale.
    /// </summary>
    public class MainViewModel
    {
        /// <summary>
        /// Gestore delle informazioni sui lanucher.
        /// </summary>
        private readonly LauncherManager LauncherManager;

        /// <summary>
        /// Launcher disponibili.
        /// </summary>
        private readonly List<ILauncher> Launchers;

        /// <summary>
        /// Giochi installati.
        /// </summary>
        private readonly ObservableCollection<GameData> Games;

        /// <summary>
        /// Lista di giochi filtrata.
        /// </summary>
        public ObservableCollection<GameData> FilteredGames { get; init; }


        public MainViewModel()
        {
            Games = new();
            LauncherOptions Options = new()
            {
                LoadLocalCatalogData = true,
                SearchGameConfigStore = false,
                QueryOnlineData = true,
                OnlineQueryTimeout = TimeSpan.FromSeconds(5),
                SearchExecutables = true
            };
            LauncherManager = new(Options);
            Launchers = LauncherManager.GetLaunchers().ToList();
            GamePlatform Platform;
            foreach (ILauncher launcher in Launchers)
            {
                Platform = launcher.Name switch
                {
                    "Steam" => GamePlatform.Steam,
                    "Origin" => GamePlatform.Origin,
                    "Epic Games" => GamePlatform.EpicGames,
                    "GOG Galaxy" => GamePlatform.GOG,
                    "Rockstar Games" => GamePlatform.Rockstar,
                    "Ubisoft Connect" => GamePlatform.Ubisoft,
                    "Battle.net" => GamePlatform.BattleNet,
                    _ => GamePlatform.None,
                };
                foreach (IGame game in launcher.Games)
                {
                    Games.Add(new(game, Platform));
                }
            }
            FilteredGames = new(Games);
        }
    }
}