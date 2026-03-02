/*
====================================================================
* ApiManager - HTTP Communication with Snake Enchanter Backend
====================================================================
* Project: Snake Enchanter
* Course: PIP-3 Theme B - SRH Fachschulen
* Developer: Julian Gomez
* Date: 2026-02-27
* Version: 1.1
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
* ApiManager.Instance.PostSession(data);        // game start — creates pending session
* ApiManager.Instance.PutSession(data);         // game end   — updates with final stats
* ApiManager.Instance.DeleteSession(onComplete);// result screen — removes last session
* ApiManager.Instance.GetLeaderboard("simple", OnResult);
* ApiManager.Instance.GetPlayerStats(OnResult);
* ApiManager.Instance.LastSessionDbId;          // DB id of the last POSTed session
*
* DESIGN:
* Singleton — one instance per game session.
* All requests run as Coroutines (non-blocking).
* Errors are logged but never crash the game (fail-silent).
*
* VERSION HISTORY:
* - v1.0: Initial implementation (POST session, GET leaderboard, GET stats)
* - v1.1: Two-phase session lifecycle — POST on start, PUT on end, DELETE from result screen
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

        #region State

        /// <summary>
        /// DB id returned by the last successful POST.
        /// Used by PutSession and DeleteSession to target the correct record.
        /// -1 means no session has been posted yet this run.
        /// </summary>
        public int LastSessionDbId { get; private set; } = -1;

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
        /// Session data sent to POST (pending) and PUT (final stats).
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

        // Internal: parse DB id from POST response
        [Serializable]
        private class PostResponse { public int id; }

        #endregion

        #region Public API

        /// <summary>
        /// POST /api/game-session — creates a pending session at game start.
        /// Stores the returned DB id in LastSessionDbId for PUT/DELETE.
        /// Fail-silent: if backend is unavailable, game continues normally.
        /// </summary>
        public void PostSession(SessionData data)
        {
            StartCoroutine(PostSessionCoroutine(data));
        }

        /// <summary>
        /// PUT /api/game-session/{LastSessionDbId} — updates the pending session
        /// with final stats at game end. Requires PostSession to have been called first.
        /// Fail-silent: if backend is unavailable, game continues normally.
        /// </summary>
        public void PutSession(SessionData data)
        {
            if (LastSessionDbId < 0)
            {
                Debug.LogWarning("[ApiManager] PutSession called but no session id — POST may have failed.");
                return;
            }
            StartCoroutine(PutSessionCoroutine(LastSessionDbId, data));
        }

        /// <summary>
        /// DELETE /api/game-session/{LastSessionDbId} — deletes the last session.
        /// Called from result screen "don't save this run".
        /// onComplete(true) on success, onComplete(false) on failure or no id.
        /// </summary>
        public void DeleteSession(Action<bool> onComplete)
        {
            if (LastSessionDbId < 0)
            {
                Debug.LogWarning("[ApiManager] DeleteSession called but no session id — nothing to delete.");
                onComplete?.Invoke(false);
                return;
            }
            StartCoroutine(DeleteSessionCoroutine(LastSessionDbId, onComplete));
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
            string json      = JsonUtility.ToJson(data);
            byte[] bodyBytes = Encoding.UTF8.GetBytes(json);
            string url       = $"{_baseUrl}/api/game-session";

            using UnityWebRequest request = new UnityWebRequest(url, "POST");
            request.uploadHandler   = new UploadHandlerRaw(bodyBytes);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            request.timeout = _timeoutSeconds;

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                // Capture returned DB id so PutSession / DeleteSession can target this record
                var response = JsonUtility.FromJson<PostResponse>(request.downloadHandler.text);
                LastSessionDbId = response.id;
                Debug.Log($"[ApiManager] Session created: id={LastSessionDbId}");
            }
            else
            {
                Debug.LogWarning($"[ApiManager] POST failed: {request.error} — backend offline?");
            }
        }

        private IEnumerator PutSessionCoroutine(int dbId, SessionData data)
        {
            string json      = JsonUtility.ToJson(data);
            byte[] bodyBytes = Encoding.UTF8.GetBytes(json);
            string url       = $"{_baseUrl}/api/game-session/{dbId}";

            using UnityWebRequest request = new UnityWebRequest(url, "PUT");
            request.uploadHandler   = new UploadHandlerRaw(bodyBytes);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            request.timeout = _timeoutSeconds;

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log($"[ApiManager] Session updated: id={dbId}");
            }
            else
            {
                Debug.LogWarning($"[ApiManager] PUT failed: {request.error} — backend offline?");
            }
        }

        private IEnumerator DeleteSessionCoroutine(int dbId, Action<bool> onComplete)
        {
            string url = $"{_baseUrl}/api/game-session/{dbId}";

            using UnityWebRequest request = UnityWebRequest.Delete(url);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.timeout = _timeoutSeconds;

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log($"[ApiManager] Session deleted: id={dbId}");
                LastSessionDbId = -1;
                onComplete?.Invoke(true);
            }
            else
            {
                Debug.LogWarning($"[ApiManager] DELETE failed: {request.error}");
                onComplete?.Invoke(false);
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
