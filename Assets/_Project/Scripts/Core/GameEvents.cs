/*
====================================================================
* GameEvents - Central event system for loose coupling
====================================================================
* Project: Snake Enchanter
* Course: PIP-3 Theme B - SRH Fachschulen
* Developer: Julian Gomez
* Date: 2026-02-03
* Version: 1.2 - Added Phase 7 Spell System events
*
* AUTHORSHIP CLASSIFICATION:
*
* [AI-ASSISTED]
* - Event system architecture based on ADR-003
* - Static event pattern for decoupled communication
* - OnTuneSuccessWithId for snake-tune targeting (v1.1)
* - Spell system events: scroll collection, shield, cooldown, range (v1.2)
*
* NOTES:
* - All game systems subscribe/publish through this class
* - Prevents tight coupling between systems
*
* VERSION HISTORY:
* - v1.0: Initial event hub (health, tune, game state)
* - v1.1: Added OnTuneSuccessWithId for per-snake tune targeting
* - v1.2: Added Phase 7 spell system events (scroll, shield, cooldown, range)
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
        /// Fired when a tune is successfully completed (with tune number).
        /// int = tune number (1-4) that was successfully cast
        /// </summary>
        public static event Action<int> OnTuneSuccessWithId;

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

        #region Spell System Events
        /// <summary>
        /// Fired when a spell scroll is collected.
        /// int = tuneNumber (1-3), string = scroll name, string = description
        /// </summary>
        public static event Action<int, string, string> OnScrollCollected;

        /// <summary>
        /// Fired when a tune is unlocked via scroll collection.
        /// int = tuneNumber (1-3)
        /// </summary>
        public static event Action<int> OnTuneUnlocked;

        /// <summary>
        /// Fired when the Shield spell is activated.
        /// </summary>
        public static event Action OnShieldActivated;

        /// <summary>
        /// Fired when the Shield spell deactivates.
        /// bool = true if shield absorbed an attack, false if it expired normally
        /// </summary>
        public static event Action<bool> OnShieldDeactivated;

        /// <summary>
        /// Fired when the Shield absorbs an incoming snake attack.
        /// </summary>
        public static event Action OnShieldAbsorbedAttack;

        /// <summary>
        /// Fired when a snake is successfully charmed (replaces heal-on-TuneSuccess).
        /// int = tuneNumber that was used
        /// </summary>
        public static event Action<int> OnSnakeCharmed;

        /// <summary>
        /// Fired when a tune cooldown begins.
        /// int = tuneNumber, float = cooldown duration in seconds
        /// </summary>
        public static event Action<int, float> OnTuneCooldownStarted;

        /// <summary>
        /// Fired when a tune cooldown expires and the tune is ready again.
        /// int = tuneNumber
        /// </summary>
        public static event Action<int> OnTuneCooldownExpired;

        /// <summary>
        /// Fired when a snake enters or leaves the player's charming range.
        /// bool = true if snake is now in range, false if out of range
        /// </summary>
        public static event Action<bool> OnSnakeInRangeChanged;
        #endregion

        #region Invokers
        // Health
        public static void HealthChanged(int newHealth) => OnHealthChanged?.Invoke(newHealth);
        public static void PlayerDamaged(int damage) => OnPlayerDamaged?.Invoke(damage);
        public static void PlayerHealed(int amount) => OnPlayerHealed?.Invoke(amount);

        // Tune
        public static void TuneSuccess() => OnTuneSuccess?.Invoke();
        public static void TuneSuccessWithId(int tuneNumber) => OnTuneSuccessWithId?.Invoke(tuneNumber);
        public static void TuneFailed(bool snakeAttacks) => OnTuneFailed?.Invoke(snakeAttacks);
        public static void TuneStarted(int tuneNumber) => OnTuneStarted?.Invoke(tuneNumber);
        public static void TuneReleased() => OnTuneReleased?.Invoke();

        // Game State
        public static void GameWin() => OnGameWin?.Invoke();
        public static void GameOver() => OnGameOver?.Invoke();
        public static void GamePaused(bool isPaused) => OnGamePaused?.Invoke(isPaused);

        // Spell System
        public static void ScrollCollected(int tuneNumber, string scrollName, string description) => OnScrollCollected?.Invoke(tuneNumber, scrollName, description);
        public static void TuneUnlocked(int tuneNumber) => OnTuneUnlocked?.Invoke(tuneNumber);
        public static void ShieldActivated() => OnShieldActivated?.Invoke();
        public static void ShieldDeactivated(bool absorbed) => OnShieldDeactivated?.Invoke(absorbed);
        public static void ShieldAbsorbedAttack() => OnShieldAbsorbedAttack?.Invoke();
        public static void SnakeCharmed(int tuneNumber) => OnSnakeCharmed?.Invoke(tuneNumber);
        public static void TuneCooldownStarted(int tuneNumber, float duration) => OnTuneCooldownStarted?.Invoke(tuneNumber, duration);
        public static void TuneCooldownExpired(int tuneNumber) => OnTuneCooldownExpired?.Invoke(tuneNumber);
        public static void SnakeInRangeChanged(bool inRange) => OnSnakeInRangeChanged?.Invoke(inRange);
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
            OnTuneSuccessWithId = null;
            OnTuneFailed = null;
            OnTuneStarted = null;
            OnTuneReleased = null;
            OnGameWin = null;
            OnGameOver = null;
            OnGamePaused = null;
            // Spell System
            OnScrollCollected = null;
            OnTuneUnlocked = null;
            OnShieldActivated = null;
            OnShieldDeactivated = null;
            OnShieldAbsorbedAttack = null;
            OnSnakeCharmed = null;
            OnTuneCooldownStarted = null;
            OnTuneCooldownExpired = null;
            OnSnakeInRangeChanged = null;
        }
        #endregion
    }
}
