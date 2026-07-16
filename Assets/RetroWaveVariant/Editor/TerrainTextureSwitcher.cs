using UnityEditor;
using UnityEngine;

public static class TerrainTextureSwitcher
{
    static readonly string[] SegmentPrefabPaths =
    {
        "Assets/RetroWaveVariant/Prefabs/Mountains_Transit_Narrow.prefab",
        "Assets/RetroWaveVariant/Prefabs/Mountains_1_Narrow.prefab",
        "Assets/RetroWaveVariant/Prefabs/Mountains_2_Narrow.prefab",
        "Assets/RetroWaveVariant/Prefabs/Mountains_3_Narrow.prefab",
        "Assets/RetroWaveVariant/Prefabs/Mountains_4_Narrow.prefab",
    };

    const string OriginalMaterialPath = "Assets/Dreamteck/Forever/Examples/Retro Wave/Materials/Terrain.mat";
    const string SewerMaterialPath = "Assets/RetroWaveVariant/Textures/SewerFloor.mat";

    [MenuItem("Tools/Retro Wave/Terrain Texture/Original", false, 1)]
    static void UseOriginal() => Apply(OriginalMaterialPath);

    [MenuItem("Tools/Retro Wave/Terrain Texture/Sewer Floor", false, 2)]
    static void UseSewerFloor() => Apply(SewerMaterialPath);

    [MenuItem("Tools/Retro Wave/Terrain Texture/Original", true)]
    [MenuItem("Tools/Retro Wave/Terrain Texture/Sewer Floor", true)]
    static bool ValidateNotPlaying() => !Application.isPlaying;

    static void Apply(string materialPath)
    {
        if (Application.isPlaying)
        {
            Debug.LogWarning("TerrainTextureSwitcher: cannot switch terrain textures while in Play Mode.");
            return;
        }

        var mat = AssetDatabase.LoadAssetAtPath<Material>(materialPath);
        if (mat == null)
        {
            Debug.LogError("TerrainTextureSwitcher: material not found at " + materialPath);
            return;
        }

        foreach (var prefabPath in SegmentPrefabPaths)
        {
            var root = PrefabUtility.LoadPrefabContents(prefabPath);
            var renderer = root.GetComponent<MeshRenderer>();
            renderer.sharedMaterial = mat;
            PrefabUtility.SaveAsPrefabAsset(root, prefabPath);
            PrefabUtility.UnloadPrefabContents(root);
        }

        AssetDatabase.SaveAssets();
        Debug.Log("Terrain texture switched to: " + mat.name);
    }
}
