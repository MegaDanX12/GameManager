using System;
using System.Collections.Generic;

namespace GameManager.Models
{
    /// <summary>
    /// Piattaforma di gioco.
    /// </summary>
    public enum GamePlatform 
    {
        None,
        Steam,
        EA,
        EpicGames,
        Ubisoft
    }

    /// <summary>
    /// Informazioni su un gioco.
    /// </summary>
    public abstract class GameData
    {
        /// <summary>
        /// ID univoco.
        /// </summary>
        public string? AppID { get; protected init; }

        /// <summary>
        /// Titolo del gioco.
        /// </summary>
        public string Title { get; protected init; }

        /// <summary>
        /// Percorso di installazione.
        /// </summary>
        public string GamePath { get; protected init; }

        /// <summary>
        /// Nome dell'eseguibile.
        /// </summary>
        public string? ExecutableName { get; protected init; }

        /// <summary>
        /// Parametri di avvio.
        /// </summary>
        public string? LaunchArguments { get; protected init; }

        /// <summary>
        /// Eseguibili.
        /// </summary>
        public List<string> Executables { get; protected init; } = new();

        /// <summary>
        /// Indica se il gioco è in esecuzione.
        /// </summary>
        public bool IsRunning { get; set; }

        /// <summary>
        /// Data di installazione.
        /// </summary>
        public DateTime InstallDate { get; protected init; }

        /// <summary>
        /// Piattaforma del gioco.
        /// </summary>
        public GamePlatform Platform { get; protected init; }
    }
}