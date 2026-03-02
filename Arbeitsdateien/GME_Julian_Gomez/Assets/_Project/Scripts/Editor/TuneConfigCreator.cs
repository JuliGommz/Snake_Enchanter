/*
====================================================================
* TuneConfigCreator - Editor utility to generate TuneConfig assets
====================================================================
* Project: Snake Enchanter
* EDITOR ONLY — This script only runs in Unity Editor
*
* USAGE: Unity Menu → SnakeEnchanter → Create Tune Configs
* Creates 3 TuneConfig ScriptableObjects (Move, Daze, Shield).
====================================================================
*/

#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using SnakeEnchanter.Tunes;

namespace SnakeEnchanter.Editor
{
    public static class TuneConfigCreator
    {
        [MenuItem("SnakeEnchanter/Create Tune Configs")]
        public static void CreateAllTuneConfigs()
        {
            string folder = "Assets/_Project/ScriptableObjects/TuneConfigs";

            // Ensure folder exists
            if (!AssetDatabase.IsValidFolder(folder))
            {
                AssetDatabase.CreateFolder("Assets/_Project/ScriptableObjects", "TuneConfigs");
            }

            // Tune 1 — Move (GDD: 3s, Zone 40-65%)
            CreateTuneConfig(folder, "Tune1_Move", new TuneConfigData
            {
                tuneName = "Move",
                keyNumber = 1,
                effect = SnakeEffect.Move,
                description = "Charms the snake to move away from your path. Quick and forgiving.",
                duration = 3.0f,
                zoneStart = 0.40f,
                zoneEnd = 0.65f,
                simpleModeBonus = 0.10f,
                zoneColor = new Color(0.2f, 0.8f, 0.2f) // Green
            });

            // Tune 2 — Daze (GDD: 4s, Zone 35-60%)
            CreateTuneConfig(folder, "Tune2_Daze", new TuneConfigData
            {
                tuneName = "Daze",
                keyNumber = 2,
                effect = SnakeEffect.Daze,
                description = "Dazes the snake into unconsciousness. Disables collision.",
                duration = 4.0f,
                zoneStart = 0.35f,
                zoneEnd = 0.60f,
                simpleModeBonus = 0.10f,
                zoneColor = new Color(0.3f, 0.5f, 0.9f) // Blue
            });

            // Tune 3 — Shield (Phase 7: 5s slider, Zone 30-55%)
            CreateTuneConfig(folder, "Tune3_Shield", new TuneConfigData
            {
                tuneName = "Shield",
                keyNumber = 3,
                effect = SnakeEffect.Shield,
                description = "Activates a protective shield that blocks the next snake attack.",
                duration = 5.0f,
                zoneStart = 0.30f,
                zoneEnd = 0.55f,
                simpleModeBonus = 0.10f,
                zoneColor = new Color(1f, 0.85f, 0f) // Gold
            });

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log("TuneConfigCreator: All 3 TuneConfig assets created in " + folder);
            EditorUtility.DisplayDialog("Tune Configs Created",
                "3 TuneConfig ScriptableObjects created:\n" +
                "• Tune1_Move (3s, 40-65%)\n" +
                "• Tune2_Daze (4s, 35-60%)\n" +
                "• Tune3_Shield (5s, 30-55%)\n\n" +
                "Location: " + folder +
                "\n\nAssign them to TuneController in Inspector!",
                "OK");
        }

        private static void CreateTuneConfig(string folder, string fileName, TuneConfigData data)
        {
            string path = $"{folder}/{fileName}.asset";

            // Check if already exists
            TuneConfig existing = AssetDatabase.LoadAssetAtPath<TuneConfig>(path);
            if (existing != null)
            {
                Debug.Log($"TuneConfigCreator: {fileName} already exists, skipping.");
                return;
            }

            TuneConfig config = ScriptableObject.CreateInstance<TuneConfig>();
            config.tuneName = data.tuneName;
            config.keyNumber = data.keyNumber;
            config.resultEffect = data.effect;
            config.description = data.description;
            config.duration = data.duration;
            config.triggerZoneStart = data.zoneStart;
            config.triggerZoneEnd = data.zoneEnd;
            config.simpleModeZoneBonus = data.simpleModeBonus;
            config.zoneColor = data.zoneColor;

            AssetDatabase.CreateAsset(config, path);
            Debug.Log($"TuneConfigCreator: Created {fileName} at {path}");
        }

        private struct TuneConfigData
        {
            public string tuneName;
            public int keyNumber;
            public SnakeEffect effect;
            public string description;
            public float duration;
            public float zoneStart;
            public float zoneEnd;
            public float simpleModeBonus;
            public Color zoneColor;
        }
    }
}
#endif
