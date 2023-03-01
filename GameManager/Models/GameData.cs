using System;
using System.Collections.Generic;
using System.Drawing;
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
        Ubisoft
    }

    /// <summary>
    /// Informazioni su un gioco.
    /// </summary>
    public class GameData
    {

        public string Title { get; }


        public string GamePath { get; }


        public string ExecutableName { get; }


        public bool IsRunning { get; }


        public DateOnly InstallDate { get; }

        /// <summary>
        /// Piattaforma del gioco.
        /// </summary>
        public GamePlatform Platform { get; }

        /// <summary>
        /// Inizializza una nuova istanza di <see cref="GameData"/>.
        /// </summary>
        public GameData()
        {
            
        }
    }
}