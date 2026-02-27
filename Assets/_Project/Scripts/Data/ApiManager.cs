/*
====================================================================
* ApiManager - HTTP Communication with Snake Enchanter Backend
====================================================================
* Project: Snake Enchanter
* Course: PIP-3 Theme B - SRH Fachschulen
* Developer: Julian Gomez
* Date: 2026-02-27
* Version: 1.0
*
* ⚠️ WICHTIG: KOMMENTIERUNG NICHT LÖSCHEN! ⚠️
* Diese detaillierte Authorship-Dokumentation ist für die akademische
* Bewertung erforderlich und darf nicht entfernt werden!
*
* AUTHORSHIP CLASSIFICATION:
* [AI-ASSISTED]
* - HTTP request structure (UnityWebRequest)
* - JSON serialization pattern
* - Coroutine-based async design
* Human reviewed and approved.
*
* DEPENDENCIES:
* - UnityEngine.Networking (UnityWebRequest)
* - Node.js backend running on localhost:3000
*
* USAGE:
* ApiManager.Instance.PostSession(data);
* ApiManager.Instance.GetLeaderboard("simple", OnResult);
* ApiManager.Instance.GetPlayerStats(OnResult);
*
* DESIGN:
* Singleton — one instance per game session.
* All requests run as Coroutines (non-blocking).
* Errors are logged but never crash the game (fail-silent).
*
* VERSION HISTORY:
* - v1.0: Initial implementation (POST session, GET leaderboard, GET stats)
====================================================================
*/

using System;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace SnakeEnchanter.Data
{
    /// <summary>
    /// Singleton that handles all HTTP communication with the backend REST API.
    /// Runs requests as Coroutines — never blocks the main thread.
    /// </summary>
    public class ApiManager : MonoBehaviour
    {
        #region Singleton

        public static ApiManager Instance { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        #endregion

        #region Config

        [Header("API Config")]
        [Tooltip("Backend base URL (no trailing slash)")]
        [SerializeField] private string _baseUrl = "http://localhost:3000";

        [Tooltip("Timeout in seconds for each request")]
        [SerializeField] private int _timeoutSeconds = 5;

        #endregion

        #region Data Models

        /// <summary>
        /// Session data sent to POST /api/game-session after each run.
        /// Field names match the backend JSON schema exactly.
        /// </summary>
        [Serializable]
        public class SessionData
        {
            public string sessionId;
            public string modeType;         // "simple" or "advanced"
            public bool   success;
            public int    completionTime;   // seconds
            public float  startingHp;
            public float  endingHp;
            public float  totalDamageTaken;
            public float  totalHpRestored;
            public int    successfulTuneCasts;
            public int    failedTuneCasts;
            public int    tooEarlyCount;
            public int    tooLateCount;
            public int    snakeBiteCount;
            public bool   fourthTuneUnlocked;
            public int    heartsRemaining;
        }

        #endregion

        #region Public API

        /// <summary>
        /// POST /api/game-session — sends session stats to backend.
        /// Called after Win or Lose screen appears.
        /// Fail-silent: if backend is unavailable, game continues normally.
        /// </summary>
        public void PostSession(SessionData data)
        {
            StartCoroutine(PostSessionCoroutine(data));
        }

        /// <summary>
        /// GET /api/leaderboard?mode=simple|advanced
        /// Calls onComplete with raw JSON string (or null on error).
        /// </summary>
        public void GetLeaderboard(string mode, Action<string> onComplete)
        {
            string url = $"{_baseUrl}/api/leaderboard?mode={mode}";
            StartCoroutine(GetCoroutine(url, onComplete));
        }

        /// <summary>
        /// GET /api/player-stats — aggregate stats across all sessions.
        /// Calls onComplete with raw JSON string (or null on error).
        /// </summary>
        public void GetPlayerStats(Action<string> onComplete)
        {
            StartCoroutine(GetCoroutine($"{_baseUrl}/api/player-stats", onComplete));
        }

        #endregion

        #region Coroutines

        private IEnumerator PostSessionCoroutine(SessionData data)
        {
            string json = JsonUtility.ToJson(data);
            byte[] bodyBytes = Encoding.UTF8.GetBytes(json);

            string url = $"{_baseUrl}/api/game-session";

            using UnityWebRequest request = new UnityWebRequest(url, "POST");
            request.uploadHandler   = new UploadHandlerRaw(bodyBytes);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            request.timeout = _timeoutSeconds;

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log($"[ApiManager] Session posted: {request.downloadHandler.text}");
            }
            else
            {
                // Fail-silent: log but don't crash the game
                Debug.LogWarning($"[ApiManager] POST failed: {request.error} — backend offline?");
            }
        }

        private IEnumerator GetCoroutine(string url, Action<string> onComplete)
        {
            using UnityWebRequest request = UnityWebRequest.Get(url);
            request.timeout = _timeoutSeconds;

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log($"[ApiManager] GET {url} OK");
                onComplete?.Invoke(request.downloadHandler.text);
            }
            else
            {
                Debug.LogWarning($"[ApiManager] GET failed: {request.error}");
                onComplete?.Invoke(null);
            }
        }

        #endregion
    }
}
