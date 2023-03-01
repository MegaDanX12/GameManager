using GameManager.LauncherData;
using GameManager.Models;
using Microsoft.Win32;
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
        /// Giochi installati.
        /// </summary>
        private readonly List<GameData> Games;

        /// <summary>
        /// Lista di giochi filtrata.
        /// </summary>
        public ObservableCollection<GameData> FilteredGames { get; init; }


        public MainViewModel()
        {
            SteamLauncherData.RetrieveSteamLauncherData();
            EALauncherData.RetrieveEALauncherData();
            EpicGamesLauncherData.RetrieveEpicGamesLauncherData();
            Games = new();
            
            FilteredGames = new(Games);
        }
    }
}