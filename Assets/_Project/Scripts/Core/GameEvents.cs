/*
====================================================================
* GameEvents - Central event system for loose coupling
====================================================================
* Project: Snake Enchanter
* Course: PIP-3 Theme B - SRH Fachschulen
* Developer: Julian Gomez
* Date: 2026-02-03
* Version: 1.0
*
* AUTHORSHIP CLASSIFICATION:
*
* [AI-ASSISTED]
* - Event system architecture based on ADR-003
* - Static event pattern for decoupled communication
*
* NOTES:
* - All game systems subscribe/publish through this class
* - Prevents tight coupling between systems
====================================================================
*/

using System;
using UnityEngine;

namespace SnakeEnchanter.Core
{
    /// <summary>
    /// Central event hub for game-wide communication.
    /// Systems subscribe to events they care about without direct references.
    /// </summary>
    public static class GameEvents
    {
        #region Health Events
        /// <summary>
        /// Fired when player health changes.
        /// int = new health value
        /// </summary>
        public static event Action<int> OnHealthChanged;

        /// <summary>
        /// Fired when player takes damage.
        /// int = damage amount
        /// </summary>
        public static event Action<int> OnPlayerDamaged;

        /// <summary>
        /// Fired when player heals.
        /// int = heal amount
        /// </summary>
        public static event Action<int> OnPlayerHealed;
        #endregion

        #region Tune Events
        /// <summary>
        /// Fired when a tune is successfully completed.
        /// </summary>
        public static event Action OnTuneSuccess;

        /// <summary>
        /// Fired when a tune fails.
        /// bool = true if snake attacks (too late), false if safe fail (too early)
        /// </summary>
        public static event Action<bool> OnTuneFailed;

        /// <summary>
        /// Fired when player starts holding a tune key.
        /// int = tune number (1-4)
        /// </summary>
        public static event Action<int> OnTuneStarted;

        /// <summary>
        /// Fired when player releases a tune key.
        /// </summary>
        public static event Action OnTuneReleased;
        #endregion

        #region Game State Events
        /// <summary>
        /// Fired when game is won (reached exit).
        /// </summary>
        public static event Action OnGameWin;

        /// <summary>
        /// Fired when game is lost (HP = 0).
        /// </summary>
        public static event Action OnGameOver;

        /// <summary>
        /// Fired when game is paused/unpaused.
        /// bool = isPaused
        /// </summary>
        public static event Action<bool> OnGamePaused;
        #endregion

        #region Invokers
        // Health
        public static void HealthChanged(int newHealth) => OnHealthChanged?.Invoke(newHealth);
        public static void PlayerDamaged(int damage) => OnPlayerDamaged?.Invoke(damage);
        public static void PlayerHealed(int amount) => OnPlayerHealed?.Invoke(amount);

        // Tune
        public static void TuneSuccess() => OnTuneSuccess?.Invoke();
        public static void TuneFailed(bool snakeAttacks) => OnTuneFailed?.Invoke(snakeAttacks);
        public static void TuneStarted(int tuneNumber) => OnTuneStarted?.Invoke(tuneNumber);
        public static void TuneReleased() => OnTuneReleased?.Invoke();

        // Game State
        public static void GameWin() => OnGameWin?.Invoke();
        public static void GameOver() => OnGameOver?.Invoke();
        public static void GamePaused(bool isPaused) => OnGamePaused?.Invoke(isPaused);
        #endregion

        #region Cleanup
        /// <summary>
        /// Clears all event subscriptions. Call on scene unload to prevent memory leaks.
        /// </summary>
        public static void ClearAllEvents()
        {
            OnHealthChanged = null;
            OnPlayerDamaged = null;
            OnPlayerHealed = null;
            OnTuneSuccess = null;
            OnTuneFailed = null;
            OnTuneStarted = null;
            OnTuneReleased = null;
            OnGameWin = null;
            OnGameOver = null;
            OnGamePaused = null;
        }
        #endregion
    }
}
