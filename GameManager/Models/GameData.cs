using GameLib.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameManager.Models
{
    /// <summary>
    /// Piattaforma di gioco.
    /// </summary>
    public enum GamePlatform 
    {
        None,
        Steam,
        Origin,
        EpicGames,
        Ubisoft,
        BattleNet,
        Rockstar,
        GOG
    }

    /// <summary>
    /// Informazioni su un gioco.
    /// </summary>
    public class GameData
    {
        /// <summary>
        /// Informazioni sul gioco.
        /// </summary>
        public IGame Data { get; }

        /// <summary>
        /// Piattaforma del gioco.
        /// </summary>
        public GamePlatform Platform { get; }

        /// <summary>
        /// Inizializza una nuova istanza di <see cref="GameData"/>.
        /// </summary>
        /// <param name="Data">Dati sul gioco.</param>
        /// <param name="Platform">Piattaforma.</param>
        public GameData(IGame Data, GamePlatform Platform)
        {
            this.Data = Data;
            this.Platform = Platform;
        }
    }
}