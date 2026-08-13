using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

internal static class TilemapCollisionConfigurator
{
    private const float ExtrusionFactor = 0.02f;
    private const string NoFrictionMaterialPath = "Assets/Physics/PlayerNoFriction.physicsMaterial2D";

    [MenuItem("Tools/Level Design/Configurar colisiones de Tilemaps")]
    private static void ConfigureActiveScene()
    {
        Scene scene = SceneManager.GetActiveScene();
        int configuredTilemaps = ConfigureScene(scene);

        if (configuredTilemaps == 0)
        {
            Debug.LogWarning("No se encontraron TilemapCollider2D en la escena activa.");
            return;
        }

        EditorSceneManager.MarkSceneDirty(scene);
        Debug.Log($"Se configuraron {configuredTilemaps} Tilemaps con colision compuesta.");
    }

    private static int ConfigureScene(Scene scene)
    {
        PhysicsMaterial2D noFrictionMaterial =
            AssetDatabase.LoadAssetAtPath<PhysicsMaterial2D>(NoFrictionMaterialPath);
        int configuredTilemaps = 0;

        foreach (GameObject root in scene.GetRootGameObjects())
        {
            TilemapCollider2D[] tilemapColliders =
                root.GetComponentsInChildren<TilemapCollider2D>(includeInactive: true);

            foreach (TilemapCollider2D tilemapCollider in tilemapColliders)
            {
                ConfigureTilemap(tilemapCollider, noFrictionMaterial);
                configuredTilemaps++;
            }
        }

        return configuredTilemaps;
    }

    private static void ConfigureTilemap(
        TilemapCollider2D tilemapCollider,
        PhysicsMaterial2D noFrictionMaterial)
    {
        GameObject tilemapObject = tilemapCollider.gameObject;
        Rigidbody2D rigidbody = GetOrAddComponent<Rigidbody2D>(tilemapObject);
        CompositeCollider2D compositeCollider = GetOrAddComponent<CompositeCollider2D>(tilemapObject);

        rigidbody.bodyType = RigidbodyType2D.Static;
        rigidbody.simulated = true;

        compositeCollider.geometryType = CompositeCollider2D.GeometryType.Polygons;
        compositeCollider.generationType = CompositeCollider2D.GenerationType.Synchronous;
        compositeCollider.sharedMaterial = noFrictionMaterial;

        tilemapCollider.compositeOperation = Collider2D.CompositeOperation.Merge;
        tilemapCollider.extrusionFactor = ExtrusionFactor;
        tilemapCollider.sharedMaterial = noFrictionMaterial;
        tilemapCollider.ProcessTilemapChanges();

        EditorUtility.SetDirty(rigidbody);
        EditorUtility.SetDirty(compositeCollider);
        EditorUtility.SetDirty(tilemapCollider);
    }

    private static T GetOrAddComponent<T>(GameObject gameObject) where T : Component
    {
        T component = gameObject.GetComponent<T>();
        return component != null ? component : gameObject.AddComponent<T>();
    }

}
