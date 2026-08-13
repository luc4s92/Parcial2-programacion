#if UNITY_EDITOR
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public static class MetroidvaniaTestSceneBuilder
{
    private const string SceneFolder = "Assets/Scenes/Playtests";
    private const string ScenePath = SceneFolder + "/MetroidvaniaMovementTest.unity";
    private const string BlockoutFolder = "Assets/Art/Blockout";
    private const string BlockoutTexturePath = BlockoutFolder + "/BlockoutGrid.png";

    private const string PlayerPrefabPath = "Assets/Prefabs/Player/Player.prefab";
    private const string SkeletonPrefabPath = "Assets/Prefabs/Enemies/Enemy.prefab";
    private const string DemonPrefabPath = "Assets/Prefabs/Enemies/Demon.prefab";
    private const string HellHoundTriggerPrefabPath =
        "Assets/Prefabs/Enemies/HellHoundSpawnTrigger.prefab";
    private const string ShurikenUnlockPrefabPath =
        "Assets/Prefabs/Items/ShurikenUnlockPickup.prefab";
    private const string ShurikenChargePrefabPath =
        "Assets/Prefabs/Items/ShurikenChargePickup.prefab";
    private const string ShurikenHudPrefabPath =
        "Assets/Prefabs/UI/ShurikenHUD.prefab";

    private static readonly Color MainRouteColor = new(0.33f, 0.43f, 0.46f);
    private static readonly Color UpperRouteColor = new(0.24f, 0.48f, 0.58f);
    private static readonly Color OneWayRouteColor = new(0.29f, 0.62f, 0.43f);
    private static readonly Color LowerRouteColor = new(0.63f, 0.39f, 0.22f);
    private static readonly Color BoundaryColor = new(0.22f, 0.24f, 0.25f);

    [MenuItem("Tools/Level Design/Regenerar escena metroidvania %#m")]
    public static void BuildFromMenu()
    {
        if (File.Exists(ScenePath) &&
            !EditorUtility.DisplayDialog(
                "Regenerar blockout",
                "La escena existente sera reemplazada. Los cambios manuales dentro de ella se perderan.",
                "Regenerar",
                "Cancelar"))
        {
            return;
        }

        BuildScene();
        Selection.activeObject = AssetDatabase.LoadAssetAtPath<SceneAsset>(ScenePath);
    }

    public static void BuildFromCommandLine()
    {
        BuildScene();
    }

    private static void BuildScene()
    {
        EnsureFolder("Assets/Scenes", "Playtests");
        EnsureFolder("Assets", "Art");
        EnsureFolder("Assets/Art", "Blockout");

        Sprite blockoutSprite = EnsureBlockoutSprite();
        ValidateRequiredAssets();

        Scene previousActiveScene = SceneManager.GetActiveScene();
        Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Additive);
        SceneManager.SetActiveScene(scene);

        GameObject root = new("PLAYTEST_Metroidvania");
        Transform geometry = CreateGroup("Geometry", root.transform);
        Transform actors = CreateGroup("Actors", root.transform);
        Transform helpers = CreateGroup("Helpers", root.transform);

        Transform startZone = CreateGroup("00_Start_Run_Attack", geometry);
        CreatePlatform("StartFloor", new Vector2(-17f, -3f), new Vector2(16f, 1f),
            MainRouteColor, blockoutSprite, startZone);
        CreatePlatform("RunAttackFloor", new Vector2(-3f, -3f), new Vector2(12f, 1f),
            MainRouteColor, blockoutSprite, startZone);
        CreatePlatform("LeftBoundary", new Vector2(-25.5f, 1f), new Vector2(1f, 9f),
            BoundaryColor, blockoutSprite, startZone);

        Transform jumpZone = CreateGroup("01_Jump_Metrics", geometry);
        CreatePlatform("Jump_01_Gap_1_5", new Vector2(5.25f, -2.25f), new Vector2(2.5f, 1f),
            UpperRouteColor, blockoutSprite, jumpZone);
        CreatePlatform("Jump_02_Gap_2_5", new Vector2(9.75f, -0.75f), new Vector2(3.5f, 1f),
            UpperRouteColor, blockoutSprite, jumpZone);
        CreatePlatform("Jump_03_Gap_2_5", new Vector2(14.5f, 1f), new Vector2(3f, 1f),
            UpperRouteColor, blockoutSprite, jumpZone);
        CreatePlatform("Jump_04_To_Upper_Route", new Vector2(18.5f, 2.75f), new Vector2(3f, 1f),
            UpperRouteColor, blockoutSprite, jumpZone);

        Transform lowerRoute = CreateGroup("02_Lower_Route_HellHound", geometry);
        CreatePlatform("LowerCombatFloor", new Vector2(9.5f, -9f), new Vector2(15f, 1f),
            LowerRouteColor, blockoutSprite, lowerRoute);
        CreatePlatform("LowerLeftWall", new Vector2(1.5f, -6.5f), new Vector2(1f, 5f),
            BoundaryColor, blockoutSprite, lowerRoute);
        CreatePlatform("LowerRightWall", new Vector2(17.5f, -7.5f), new Vector2(1f, 3f),
            BoundaryColor, blockoutSprite, lowerRoute);
        CreatePlatform("RecoveryStep_01", new Vector2(18.75f, -6.5f), new Vector2(3.5f, 1f),
            LowerRouteColor, blockoutSprite, lowerRoute);
        CreatePlatform("RecoveryStep_02", new Vector2(20.5f, -4f), new Vector2(3f, 1f),
            LowerRouteColor, blockoutSprite, lowerRoute);
        CreatePlatform("RecoveryStep_03", new Vector2(20.5f, -1.5f), new Vector2(3f, 1f),
            LowerRouteColor, blockoutSprite, lowerRoute);
        CreatePlatform("RecoveryStep_04", new Vector2(18.5f, 1f), new Vector2(3f, 1f),
            LowerRouteColor, blockoutSprite, lowerRoute);

        Transform upperRoute = CreateGroup("03_Upper_Route_Demon", geometry);
        CreatePlatform("UpperFloor_Right", new Vector2(17f, 5f), new Vector2(6f, 1f),
            UpperRouteColor, blockoutSprite, upperRoute);
        CreatePlatform("UpperFloor_Center_OneWay", new Vector2(8f, 5f), new Vector2(8f, 1f),
            OneWayRouteColor, blockoutSprite, upperRoute, isOneWay: true);
        CreatePlatform("UpperFloor_Left", new Vector2(0f, 5f), new Vector2(4f, 1f),
            UpperRouteColor, blockoutSprite, upperRoute);
        CreatePlatform("UpperRightBoundary", new Vector2(20.5f, 8f), new Vector2(1f, 7f),
            BoundaryColor, blockoutSprite, upperRoute);

        Transform shortcut = CreateGroup("04_Return_Shortcut", geometry);
        CreatePlatform("ShortcutStep_01", new Vector2(-3f, 2.75f), new Vector2(3f, 1f),
            MainRouteColor, blockoutSprite, shortcut);
        CreatePlatform("ShortcutStep_02", new Vector2(-5.5f, 0.5f), new Vector2(3f, 1f),
            MainRouteColor, blockoutSprite, shortcut);
        CreatePlatform("ShortcutLanding", new Vector2(-8f, -1.25f), new Vector2(3f, 1f),
            MainRouteColor, blockoutSprite, shortcut);

        Vector3 playerSpawnPosition = new(-21f, -0.35f, 0f);
        Transform respawnPoint = CreateRespawnPoint(playerSpawnPosition, helpers);

        GameObject player = InstantiatePrefab(PlayerPrefabPath, "Player_Playtest",
            playerSpawnPosition, actors);
        CreateCamera(player.transform, actors);
        CreateHud();

        InstantiatePrefab(ShurikenUnlockPrefabPath, "ShurikenUnlockPickup",
            playerSpawnPosition + new Vector3(3f, 1f, 0f), actors);
        InstantiatePrefab(ShurikenChargePrefabPath, "ShurikenChargePickup",
            playerSpawnPosition + new Vector3(6f, 1f, 0f), actors);

        InstantiatePrefab(SkeletonPrefabPath, "Skeleton_Runway", new Vector3(-1f, -0.8f, 0f),
            actors);
        InstantiatePrefab(DemonPrefabPath, "Demon_UpperRoute", new Vector3(10f, 5.85f, 0f),
            actors);

        GameObject hellHoundTrigger = InstantiatePrefab(HellHoundTriggerPrefabPath,
            "HellHound_LowerRoute_Trigger", new Vector3(5f, -7.7f, 0f), actors);
        ConfigureHellHoundTrigger(hellHoundTrigger);

        CreateGoalMarker(new Vector2(-1f, 7f), blockoutSprite, helpers);
        CreateRespawnZone(respawnPoint, helpers);

        EditorSceneManager.SaveScene(scene, ScenePath);
        AddSceneToBuildSettings();
        AssetDatabase.SaveAssets();

        if (previousActiveScene.IsValid() && previousActiveScene.isLoaded)
        {
            SceneManager.SetActiveScene(previousActiveScene);
            EditorSceneManager.CloseScene(scene, true);
        }

        Debug.Log($"Escena de prueba generada en {ScenePath}");
    }

    private static Transform CreateGroup(string name, Transform parent)
    {
        GameObject group = new(name);
        group.transform.SetParent(parent, false);
        return group.transform;
    }

    private static GameObject CreatePlatform(
        string name,
        Vector2 position,
        Vector2 size,
        Color color,
        Sprite sprite,
        Transform parent,
        bool isOneWay = false)
    {
        GameObject platform = new(name);
        platform.tag = "Ground";
        platform.layer = LayerMask.NameToLayer("ground");
        platform.transform.SetParent(parent, false);
        platform.transform.position = position;

        SpriteRenderer renderer = platform.AddComponent<SpriteRenderer>();
        renderer.sprite = sprite;
        renderer.color = color;
        renderer.drawMode = SpriteDrawMode.Tiled;
        renderer.tileMode = SpriteTileMode.Continuous;
        renderer.size = size;
        renderer.sortingOrder = -10;

        BoxCollider2D collider = platform.AddComponent<BoxCollider2D>();
        collider.size = size;

        if (isOneWay)
            platform.AddComponent<OneWayPlatform>();

        return platform;
    }

    private static GameObject InstantiatePrefab(
        string prefabPath,
        string instanceName,
        Vector3 position,
        Transform parent)
    {
        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
        GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab, parent);
        instance.name = instanceName;
        instance.transform.position = position;
        return instance;
    }

    private static void CreateCamera(Transform player, Transform parent)
    {
        GameObject cameraObject = new("Main Camera");
        cameraObject.tag = "MainCamera";
        cameraObject.transform.SetParent(parent, false);
        cameraObject.transform.position = player.position + new Vector3(0f, 2f, -10f);

        Camera camera = cameraObject.AddComponent<Camera>();
        camera.orthographic = true;
        camera.orthographicSize = 6f;
        camera.clearFlags = CameraClearFlags.SolidColor;
        camera.backgroundColor = new Color(0.055f, 0.065f, 0.075f);

        cameraObject.AddComponent<AudioListener>();

        CameraController controller = cameraObject.AddComponent<CameraController>();
        controller.Configure(player, new Vector3(0f, 1.25f, -10f));
    }

    private static void CreateHud()
    {
        GameObject canvasObject = new(
            "Canvas",
            typeof(RectTransform),
            typeof(Canvas),
            typeof(CanvasScaler),
            typeof(GraphicRaycaster)
        );

        Canvas canvas = canvasObject.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;

        CanvasScaler scaler = canvasObject.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(800f, 600f);

        GameObject hud = InstantiatePrefab(
            ShurikenHudPrefabPath,
            "ShurikenHUD",
            Vector3.zero,
            canvasObject.transform
        );
        hud.GetComponent<RectTransform>().anchoredPosition = new Vector2(18f, -50f);
    }

    private static Transform CreateRespawnPoint(Vector3 position, Transform parent)
    {
        GameObject point = new("RespawnPoint");
        point.transform.SetParent(parent, false);
        point.transform.position = position;
        return point.transform;
    }

    private static void CreateRespawnZone(Transform respawnPoint, Transform parent)
    {
        GameObject zone = new("FallRecoveryZone");
        zone.transform.SetParent(parent, false);
        zone.transform.position = new Vector3(-2f, -15f, 0f);

        BoxCollider2D collider = zone.AddComponent<BoxCollider2D>();
        collider.size = new Vector2(58f, 4f);
        collider.isTrigger = true;

        PlaytestRespawnZone respawn = zone.AddComponent<PlaytestRespawnZone>();
        respawn.Configure(respawnPoint);
    }

    private static void CreateGoalMarker(Vector2 position, Sprite sprite, Transform parent)
    {
        GameObject marker = new("Goal_Loop_Complete");
        marker.transform.SetParent(parent, false);
        marker.transform.position = position;

        SpriteRenderer renderer = marker.AddComponent<SpriteRenderer>();
        renderer.sprite = sprite;
        renderer.color = new Color(0.92f, 0.77f, 0.24f, 0.75f);
        renderer.drawMode = SpriteDrawMode.Tiled;
        renderer.size = new Vector2(1f, 4f);
        renderer.sortingOrder = -9;
    }

    private static void ConfigureHellHoundTrigger(GameObject triggerObject)
    {
        BoxCollider2D collider = triggerObject.GetComponent<BoxCollider2D>();
        collider.size = new Vector2(3f, 4f);

        HellHoundSpawnTrigger trigger = triggerObject.GetComponent<HellHoundSpawnTrigger>();
        SerializedObject serializedTrigger = new(trigger);
        serializedTrigger.FindProperty("spawnDistance").floatValue = 7f;
        serializedTrigger.ApplyModifiedPropertiesWithoutUndo();
    }

    private static Sprite EnsureBlockoutSprite()
    {
        if (!File.Exists(BlockoutTexturePath))
        {
            Texture2D texture = new(16, 16, TextureFormat.RGBA32, false);
            Color32[] pixels = new Color32[16 * 16];

            for (int y = 0; y < 16; y++)
            {
                for (int x = 0; x < 16; x++)
                {
                    bool isGridLine = x == 0 || y == 0;
                    pixels[y * 16 + x] = isGridLine
                        ? new Color32(150, 150, 150, 255)
                        : new Color32(255, 255, 255, 255);
                }
            }

            texture.SetPixels32(pixels);
            texture.Apply();
            File.WriteAllBytes(BlockoutTexturePath, texture.EncodeToPNG());
            Object.DestroyImmediate(texture);
            AssetDatabase.ImportAsset(BlockoutTexturePath);
        }

        TextureImporter importer = (TextureImporter)AssetImporter.GetAtPath(BlockoutTexturePath);
        importer.textureType = TextureImporterType.Sprite;
        importer.spriteImportMode = SpriteImportMode.Single;
        importer.spritePixelsPerUnit = 16f;
        importer.filterMode = FilterMode.Point;
        importer.wrapMode = TextureWrapMode.Repeat;
        importer.mipmapEnabled = false;
        importer.textureCompression = TextureImporterCompression.Uncompressed;

        TextureImporterSettings settings = new();
        importer.ReadTextureSettings(settings);
        settings.spriteMeshType = SpriteMeshType.FullRect;
        importer.SetTextureSettings(settings);
        importer.SaveAndReimport();

        return AssetDatabase.LoadAssetAtPath<Sprite>(BlockoutTexturePath);
    }

    private static void ValidateRequiredAssets()
    {
        string[] paths =
        {
            PlayerPrefabPath,
            SkeletonPrefabPath,
            DemonPrefabPath,
            HellHoundTriggerPrefabPath,
            ShurikenUnlockPrefabPath,
            ShurikenChargePrefabPath,
            ShurikenHudPrefabPath
        };

        string missingPath = paths.FirstOrDefault(path =>
            AssetDatabase.LoadAssetAtPath<GameObject>(path) == null);

        if (missingPath != null)
            throw new FileNotFoundException($"No se encontro el prefab requerido: {missingPath}");
    }

    private static void AddSceneToBuildSettings()
    {
        EditorBuildSettingsScene[] scenes = EditorBuildSettings.scenes;

        if (scenes.Any(scene => scene.path == ScenePath))
            return;

        EditorBuildSettings.scenes = scenes
            .Append(new EditorBuildSettingsScene(ScenePath, false))
            .ToArray();
    }

    private static void EnsureFolder(string parent, string child)
    {
        string path = $"{parent}/{child}";

        if (!AssetDatabase.IsValidFolder(path))
            AssetDatabase.CreateFolder(parent, child);
    }
}
#endif
