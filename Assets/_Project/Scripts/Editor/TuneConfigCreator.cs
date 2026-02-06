/*
====================================================================
* TuneConfigCreator - Editor utility to generate TuneConfig assets
====================================================================
* Project: Snake Enchanter
* EDITOR ONLY — This script only runs in Unity Editor
*
* USAGE: Unity Menu → SnakeEnchanter → Create Tune Configs
* Creates 4 TuneConfig ScriptableObjects with GDD values.
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

            // Tune 2 — Sleep (GDD: 4s, Zone 35-60%)
            CreateTuneConfig(folder, "Tune2_Sleep", new TuneConfigData
            {
                tuneName = "Sleep",
                keyNumber = 2,
                effect = SnakeEffect.Sleep,
                description = "Lulls the snake into a deep sleep. Disables collision.",
                duration = 4.0f,
                zoneStart = 0.35f,
                zoneEnd = 0.60f,
                simpleModeBonus = 0.10f,
                zoneColor = new Color(0.3f, 0.5f, 0.9f) // Blue
            });

            // Tune 3 — Attack (GDD: 5s, Zone 30-55%)
            CreateTuneConfig(folder, "Tune3_Attack", new TuneConfigData
            {
                tuneName = "Attack",
                keyNumber = 3,
                effect = SnakeEffect.Attack,
                description = "Commands the snake to attack the nearest enemy. High risk, high reward.",
                duration = 5.0f,
                zoneStart = 0.30f,
                zoneEnd = 0.55f,
                simpleModeBonus = 0.10f,
                zoneColor = new Color(0.9f, 0.5f, 0.1f) // Orange
            });

            // Tune 4 — Freeze (GDD: 6s, Zone 25-50%, Advanced only)
            CreateTuneConfig(folder, "Tune4_Freeze", new TuneConfigData
            {
                tuneName = "Freeze",
                keyNumber = 4,
                effect = SnakeEffect.Freeze,
                description = "Freezes all creatures in range. Emergency tool for Advanced mode.",
                duration = 6.0f,
                zoneStart = 0.25f,
                zoneEnd = 0.50f,
                simpleModeBonus = 0.0f, // Advanced only, no simple bonus
                zoneColor = new Color(0.2f, 0.8f, 0.8f) // Cyan
            });

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log("TuneConfigCreator: All 4 TuneConfig assets created in " + folder);
            EditorUtility.DisplayDialog("Tune Configs Created",
                "4 TuneConfig ScriptableObjects created:\n" +
                "• Tune1_Move (3s, 40-65%)\n" +
                "• Tune2_Sleep (4s, 35-60%)\n" +
                "• Tune3_Attack (5s, 30-55%)\n" +
                "• Tune4_Freeze (6s, 25-50%)\n\n" +
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
