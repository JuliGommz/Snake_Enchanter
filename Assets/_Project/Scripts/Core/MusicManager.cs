/*
====================================================================
* MusicManager - Background Music Controller
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
* - Singleton + DontDestroyOnLoad pattern
* - Scene-based music switching via SceneManager.sceneLoaded
* - Coroutine-based track alternation
* Human reviewed and approved.
*
* DESIGN:
* Singleton das über Szenen hinweg bestehen bleibt.
* Reagiert automatisch auf Szenen-Wechsel (SceneManager.sceneLoaded).
* MainMenu: Eine Spur loopt.
* GameLevel: Zwei Spuren wechseln sich automatisch ab.
*
* SETUP (Unity Editor):
* 1. GameObject "MusicManager" in MainMenu Scene erstellen
* 2. Dieses Script hinzufügen
* 3. AudioSource Component hinzufügen (Play On Awake = false, Loop = false)
* 4. Die 3 AudioClips im Inspector zuweisen:
*    - Menu Music
*    - Gameplay Track 1
*    - Gameplay Track 2
*
* VERSION HISTORY:
* - v1.0: Initial — scene detection, menu loop, gameplay alternation
====================================================================
*/

using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SnakeEnchanter.Core
{
    /// <summary>
    /// Singleton that controls background music across all scenes.
    /// Persists via DontDestroyOnLoad — place only in MainMenu scene.
    /// </summary>
    public class MusicManager : MonoBehaviour
    {
        #region Singleton

        public static MusicManager Instance { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);

            _audioSource = GetComponent<AudioSource>();
            if (_audioSource == null)
            {
                _audioSource = gameObject.AddComponent<AudioSource>();
            }

            _audioSource.loop        = false;
            _audioSource.playOnAwake = false;
            _audioSource.volume      = _volume;
        }

        #endregion

        #region Config

        [Header("Music Tracks")]
        [Tooltip("Läuft im Hauptmenü (loopt)")]
        [SerializeField] private AudioClip _menuMusic;

        [Tooltip("Erster Gameplay-Track")]
        [SerializeField] private AudioClip _gameplayTrack1;

        [Tooltip("Zweiter Gameplay-Track (wechselt nach Track 1)")]
        [SerializeField] private AudioClip _gameplayTrack2;

        [Header("Settings")]
        [Range(0f, 1f)]
        [SerializeField] private float _volume = 0.4f;

        #endregion

        #region Private Fields

        private AudioSource _audioSource;
        private Coroutine   _playlistCoroutine;
        private bool        _playingTrack1     = true;
        private bool        _isPaused          = false;
        private bool        _audioRestartPending = false;

        // Scene names — must match exactly (Build Settings)
        private const string SCENE_MENU  = "MainMenu";
        private const string SCENE_GAME  = "GameLevel";

        #endregion

        #region Unity Lifecycle

        private void Start()
        {
            // Play music for the scene that was already loaded at startup
            HandleSceneChange(SceneManager.GetActiveScene().name);
        }

        private void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
            AudioSettings.OnAudioConfigurationChanged += OnAudioConfigChanged;
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            AudioSettings.OnAudioConfigurationChanged -= OnAudioConfigChanged;
        }

        // [AI-ASSISTED] Unity Recorder fires OnAudioConfigurationChanged multiple times rapidly
        // when starting/stopping audio capture. The debounce flag (_audioRestartPending) prevents
        // a restart loop where each event call would kill the previous restart coroutine.
        private void OnAudioConfigChanged(bool deviceWasChanged)
        {
            if (_audioRestartPending) return;
            _audioRestartPending = true;
            StartCoroutine(RestartAfterAudioReset());
        }

        private IEnumerator RestartAfterAudioReset()
        {
            // 0.5s gives Recorder time to finish all its audio-init events before we restart
            yield return new WaitForSeconds(0.5f);
            _audioRestartPending = false;

            if (!_audioSource.isPlaying && !_isPaused)
            {
                HandleSceneChange(SceneManager.GetActiveScene().name);
                Debug.Log("[MusicManager] Audio restarted after Recorder config change.");
            }
        }

        #endregion

        #region Scene Handling

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            HandleSceneChange(scene.name);
        }

        private void HandleSceneChange(string sceneName)
        {
            // Stop any running playlist coroutine
            if (_playlistCoroutine != null)
            {
                StopCoroutine(_playlistCoroutine);
                _playlistCoroutine = null;
            }

            if (sceneName == SCENE_MENU)
            {
                PlayLooping(_menuMusic);
            }
            else if (sceneName == SCENE_GAME)
            {
                _playingTrack1     = true;
                _playlistCoroutine = StartCoroutine(GameplayPlaylist());
            }
        }

        #endregion

        #region Playback

        /// <summary>Plays a single clip on loop (used for MainMenu).</summary>
        private void PlayLooping(AudioClip clip)
        {
            if (clip == null)
            {
                Debug.LogWarning("[MusicManager] Menu music clip is null — assign in Inspector.");
                return;
            }

            _audioSource.loop = true;
            _audioSource.clip = clip;
            _audioSource.Play();

            Debug.Log($"[MusicManager] Playing (loop): {clip.name}");
        }

        /// <summary>
        /// Alternates between GameplayTrack1 and GameplayTrack2.
        /// Waits for each clip to finish, then switches to the other.
        /// </summary>
        private IEnumerator GameplayPlaylist()
        {
            _audioSource.loop = false;

            while (true)
            {
                AudioClip current = _playingTrack1 ? _gameplayTrack1 : _gameplayTrack2;

                if (current != null)
                {
                    _audioSource.clip = current;
                    _audioSource.Play();

                    Debug.Log($"[MusicManager] Playing track {(_playingTrack1 ? "1" : "2")}: {current.name}");

                    yield return new WaitForSeconds(current.length);
                }
                else
                {
                    // Clip not assigned — wait briefly before retrying
                    Debug.LogWarning($"[MusicManager] Gameplay track {(_playingTrack1 ? "1" : "2")} not assigned.");
                    yield return new WaitForSeconds(2f);
                }

                // Alternate
                _playingTrack1 = !_playingTrack1;
            }
        }

        #endregion

        #region Public API

        /// <summary>Sets master volume (0–1). Called from Settings if needed.</summary>
        public void SetVolume(float volume)
        {
            _volume                = Mathf.Clamp01(volume);
            _audioSource.volume    = _volume;
        }

        /// <summary>Pause/Resume music (e.g. for Pause screen).</summary>
        public void SetPaused(bool paused)
        {
            _isPaused = paused;
            if (paused) _audioSource.Pause();
            else        _audioSource.UnPause();
        }

        #endregion
    }
}
