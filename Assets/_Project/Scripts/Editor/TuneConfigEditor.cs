/*
====================================================================
* TuneConfigEditor - Custom Inspector with audio section preview
====================================================================
* Project: Snake Enchanter
* Course: PIP-3 Theme B - SRH Fachschulen
* Developer: Julian Gomez
* Date: 2026-02-18
* Version: 1.0

* AUTHORSHIP CLASSIFICATION:

* [AI-ASSISTED]
* - CustomEditor for TuneConfig ScriptableObject
* - Audio preview with fade in/out in Editor

* DEPENDENCIES:
* - TuneConfig.cs (SnakeEnchanter.Tunes)
* - EditorApplication.update for preview timing
====================================================================
*/

#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using SnakeEnchanter.Tunes;

namespace SnakeEnchanter.Editor
{
    [CustomEditor(typeof(TuneConfig))]
    public class TuneConfigEditor : UnityEditor.Editor
    {
        private AudioSource _previewSource;
        private bool _isPreviewing = false;
        private TuneConfig _config;

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            _config = (TuneConfig)target;

            EditorGUILayout.Space(10);

            if (_config.melody == null)
            {
                EditorGUILayout.HelpBox("Assign a melody clip to enable audio preview.", MessageType.Info);
                return;
            }

            // Audio Section Info (read-only)
            EditorGUILayout.LabelField("Audio Section Info", EditorStyles.boldLabel);
            float sectionDuration = _config.MelodySectionDuration;
            float effectiveEnd = _config.EffectiveMelodyEndPoint;
            EditorGUILayout.LabelField($"Clip Length: {_config.melody.length:F2}s");
            EditorGUILayout.LabelField($"Section: {_config.melodyStartPoint:F2}s  ->  {effectiveEnd:F2}s  ({sectionDuration:F2}s)");
            EditorGUILayout.LabelField($"Fade In: {_config.FadeInDuration:F2}s  |  Fade Out: {_config.FadeOutDuration:F2}s");

            EditorGUILayout.Space(5);

            // Preview controls
            EditorGUILayout.BeginHorizontal();

            GUI.enabled = !_isPreviewing;
            if (GUILayout.Button("Preview", GUILayout.Height(28)))
            {
                StartPreview();
            }

            GUI.enabled = _isPreviewing;
            if (GUILayout.Button("Stop", GUILayout.Height(28)))
            {
                StopPreview();
            }

            GUI.enabled = true;
            EditorGUILayout.EndHorizontal();

            // Playback position bar
            if (_isPreviewing && _previewSource != null && _previewSource.isPlaying)
            {
                float currentTime = _previewSource.time;
                float progress = Mathf.InverseLerp(_config.melodyStartPoint, effectiveEnd, currentTime);

                EditorGUILayout.Space(5);
                Rect progressRect = EditorGUILayout.GetControlRect(false, 20);
                EditorGUI.ProgressBar(progressRect, progress,
                    $"{currentTime:F1}s / {effectiveEnd:F1}s  (Vol: {_previewSource.volume:F2})");

                Repaint();
            }
        }

        private void StartPreview()
        {
            StopPreview();

            GameObject previewGO = new GameObject("TuneConfig_Preview")
            {
                hideFlags = HideFlags.HideAndDontSave
            };
            _previewSource = previewGO.AddComponent<AudioSource>();
            _previewSource.clip = _config.melody;
            _previewSource.time = _config.melodyStartPoint;
            _previewSource.volume = 0f;
            _previewSource.Play();

            _isPreviewing = true;
            EditorApplication.update += UpdatePreview;
        }

        private void StopPreview()
        {
            _isPreviewing = false;
            EditorApplication.update -= UpdatePreview;

            if (_previewSource != null)
            {
                _previewSource.Stop();
                DestroyImmediate(_previewSource.gameObject);
                _previewSource = null;
            }
        }

        private void UpdatePreview()
        {
            if (!_isPreviewing || _previewSource == null || _config == null || _config.melody == null)
            {
                StopPreview();
                return;
            }

            float currentTime = _previewSource.time;
            float startPoint = _config.melodyStartPoint;
            float endPoint = _config.EffectiveMelodyEndPoint;
            float sectionDuration = _config.MelodySectionDuration;
            float fadeInDuration = _config.FadeInDuration;
            float fadeOutDuration = _config.FadeOutDuration;

            float elapsed = currentTime - startPoint;

            // Compute volume based on fade curves
            float volume = 1f;

            if (fadeInDuration > 0.01f && elapsed < fadeInDuration)
            {
                volume = Mathf.Clamp01(elapsed / fadeInDuration);
            }

            float fadeOutStart = sectionDuration - fadeOutDuration;
            if (fadeOutDuration > 0.01f && elapsed > fadeOutStart)
            {
                volume = Mathf.Clamp01((sectionDuration - elapsed) / fadeOutDuration);
            }

            _previewSource.volume = volume;

            // Stop at end point
            if (currentTime >= endPoint || !_previewSource.isPlaying)
            {
                StopPreview();
            }
        }

        private void OnDisable()
        {
            StopPreview();
        }
    }
}
#endif
