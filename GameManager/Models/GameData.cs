using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;

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
    public abstract class GameData : INotifyPropertyChanged
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
        /// Stringa di disinstallazione.
        /// </summary>
        public string? UninstallString { get; protected init; }

        /// <summary>
        /// Eseguibili.
        /// </summary>
        public List<string> Executables { get; protected init; } = new();

        private bool _IsRunning;

        /// <summary>
        /// Indica se il gioco è in esecuzione.
        /// </summary>
        public bool IsRunning 
        { 
            get
            {
                return _IsRunning;
            }
            set
            {
                if (_IsRunning != value)
                {
                    _IsRunning = value;
                    NotifyPropertyChange();
                }
            }
        }

        /// <summary>
        /// Data di installazione.
        /// </summary>
        public DateTime InstallDate { get; protected init; }

        /// <summary>
        /// Piattaforma del gioco.
        /// </summary>
        public GamePlatform Platform { get; protected init; }

        public event PropertyChangedEventHandler? PropertyChanged;

        private void NotifyPropertyChange([CallerMemberName] string PropertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(PropertyName));
        }

        /// <summary>
        /// Avvia il gioco.
        /// </summary>
        public abstract bool StartGame();

        /// <summary>
        /// Disinstalla il gioco.
        /// </summary>
        public abstract bool UninstallGame();

        /// <summary>
        /// Apre la cartella di installazione del gioco.
        /// </summary>
        public bool OpenGameFolder()
        {
            using Process ExplorerProcess = Process.Start("explorer.exe", GamePath);
            if (ExplorerProcess is not null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Crea un collegamento sul desktop al gioco.
        /// </summary>
        public abstract bool CreateShortcut();
    }
}