#if UNITY_EDITOR
using System;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

internal static class ShurikenProgressionAssetBuilder
{
    private const string ShurikenTexturePath =
        "Assets/Sprites/Player/Projectiles/shuriken.png";
    private const string PickupAudioTemplatePath =
        "Assets/Prefabs/Items/pocion.prefab";
    private const string UnlockPrefabPath =
        "Assets/Prefabs/Items/ShurikenUnlockPickup.prefab";
    private const string ChargePrefabPath =
        "Assets/Prefabs/Items/ShurikenChargePickup.prefab";
    private const string HudPrefabPath =
        "Assets/Prefabs/UI/ShurikenHUD.prefab";

    private static readonly string[] GameplayScenePaths =
    {
        "Assets/Scenes/Level1.unity",
        "Assets/Scenes/Level2.unity",
        "Assets/Scenes/Playtests/MetroidvaniaMovementTest.unity"
    };

    [MenuItem("Tools/Player/Configurar progresion shuriken %#g")]
    public static void ConfigureFromMenu()
    {
        try
        {
            ConfigureAssetsAndScenes();
            Debug.Log("[ShurikenProgressionAssetBuilder] Progresion y HUD configurados.");
        }
        catch (Exception exception)
        {
            Debug.LogException(exception);
        }
    }

    public static void ConfigureFromCommandLine()
    {
        ConfigureAssetsAndScenes();
    }

    private static void ConfigureAssetsAndScenes()
    {
        EnsureFolder("Assets/Prefabs", "UI");

        Sprite shurikenSprite = AssetDatabase.LoadAllAssetsAtPath(ShurikenTexturePath)
            .OfType<Sprite>()
            .FirstOrDefault();
        if (shurikenSprite == null)
            throw new InvalidOperationException("No se pudo cargar el sprite del shuriken.");

        GameObject unlockPrefab = CreatePickupPrefab<ShurikenUnlockPickup>(
            "ShurikenUnlockPickup",
            UnlockPrefabPath,
            shurikenSprite,
            new Color(0.3f, 0.9f, 1f),
            2f
        );
        GameObject chargePrefab = CreatePickupPrefab<ShurikenChargePickup>(
            "ShurikenChargePickup",
            ChargePrefabPath,
            shurikenSprite,
            new Color(1f, 0.9f, 0.35f),
            1.5f
        );
        GameObject hudPrefab = CreateHudPrefab(shurikenSprite);

        foreach (string scenePath in GameplayScenePaths)
        {
            bool placePickups = scenePath.EndsWith("Level1.unity") ||
                scenePath.Contains("Playtests");
            ConfigureScene(scenePath, hudPrefab, unlockPrefab, chargePrefab, placePickups);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    private static GameObject CreatePickupPrefab<T>(
        string name,
        string prefabPath,
        Sprite sprite,
        Color color,
        float visualScale) where T : Item
    {
        GameObject root = new(name);

        Rigidbody2D rigidBody = root.AddComponent<Rigidbody2D>();
        rigidBody.bodyType = RigidbodyType2D.Kinematic;
        rigidBody.gravityScale = 0f;
        rigidBody.constraints = RigidbodyConstraints2D.FreezeRotation;

        CircleCollider2D collider = root.AddComponent<CircleCollider2D>();
        collider.isTrigger = true;
        collider.radius = 0.65f;

        T pickup = root.AddComponent<T>();
        AudioSource audioSource = root.AddComponent<AudioSource>();
        CopyPickupAudioSettings(audioSource);

        ItemAudio itemAudio = root.AddComponent<ItemAudio>();
        SetObjectReference(itemAudio, "hitSFX", audioSource);
        SetObjectReference(pickup, "itemAudio", itemAudio);

        GameObject visualObject = new("Visual");
        visualObject.transform.SetParent(root.transform, false);
        visualObject.transform.localScale = Vector3.one * visualScale;

        SpriteRenderer renderer = visualObject.AddComponent<SpriteRenderer>();
        renderer.sprite = sprite;
        renderer.color = color;
        renderer.sortingOrder = 10;

        FloatingPickupVisual floatingVisual = root.AddComponent<FloatingPickupVisual>();
        SetObjectReference(floatingVisual, "visual", visualObject.transform);

        GameObject savedPrefab = PrefabUtility.SaveAsPrefabAsset(root, prefabPath);
        UnityEngine.Object.DestroyImmediate(root);
        return savedPrefab;
    }

    private static void CopyPickupAudioSettings(AudioSource target)
    {
        GameObject template = AssetDatabase.LoadAssetAtPath<GameObject>(PickupAudioTemplatePath);
        AudioSource source = template != null ? template.GetComponent<AudioSource>() : null;

        target.playOnAwake = false;
        target.spatialBlend = 0f;
        if (source == null)
            return;

        target.clip = source.clip;
        target.outputAudioMixerGroup = source.outputAudioMixerGroup;
        target.volume = source.volume;
        target.pitch = source.pitch;
        target.priority = source.priority;
    }

    private static GameObject CreateHudPrefab(Sprite shurikenSprite)
    {
        GameObject root = new("ShurikenHUD", typeof(RectTransform));
        root.layer = LayerMask.NameToLayer("UI");

        RectTransform rootRect = root.GetComponent<RectTransform>();
        rootRect.anchorMin = new Vector2(0f, 1f);
        rootRect.anchorMax = new Vector2(0f, 1f);
        rootRect.pivot = new Vector2(0f, 1f);
        rootRect.anchoredPosition = new Vector2(18f, -50f);
        rootRect.sizeDelta = new Vector2(120f, 28f);

        GameObject content = CreateUiObject("Content", root.transform);
        RectTransform contentRect = content.GetComponent<RectTransform>();
        Stretch(contentRect);

        Image icon = CreateUiObject("ShurikenIcon", content.transform)
            .AddComponent<Image>();
        icon.sprite = shurikenSprite;
        icon.preserveAspect = true;
        icon.raycastTarget = false;
        SetRect(icon.rectTransform, new Vector2(0f, 0.5f), new Vector2(12f, 0f),
            new Vector2(24f, 24f), new Vector2(0.5f, 0.5f));

        GameObject chargesObject = CreateUiObject("Charges", content.transform);
        RectTransform chargesRect = chargesObject.GetComponent<RectTransform>();
        SetRect(chargesRect, new Vector2(0f, 0.5f), new Vector2(36f, 0f),
            new Vector2(74f, 20f), new Vector2(0f, 0.5f));

        HorizontalLayoutGroup layout = chargesObject.AddComponent<HorizontalLayoutGroup>();
        layout.childAlignment = TextAnchor.MiddleLeft;
        layout.spacing = 10f;
        layout.childControlWidth = false;
        layout.childControlHeight = false;
        layout.childForceExpandWidth = false;
        layout.childForceExpandHeight = false;

        Image chargeTemplate = CreateUiObject("Charge_1", chargesObject.transform)
            .AddComponent<Image>();
        chargeTemplate.raycastTarget = false;
        chargeTemplate.color = new Color(0.25f, 0.82f, 0.95f, 1f);
        chargeTemplate.rectTransform.sizeDelta = new Vector2(11f, 11f);
        chargeTemplate.rectTransform.localRotation = Quaternion.Euler(0f, 0f, 45f);

        LayoutElement layoutElement = chargeTemplate.gameObject.AddComponent<LayoutElement>();
        layoutElement.preferredWidth = 11f;
        layoutElement.preferredHeight = 11f;

        Outline outline = chargeTemplate.gameObject.AddComponent<Outline>();
        outline.effectColor = new Color(0.02f, 0.04f, 0.06f, 1f);
        outline.effectDistance = new Vector2(1f, -1f);

        ShurikenHud hud = root.AddComponent<ShurikenHud>();
        SetObjectReference(hud, "contentRoot", content);
        SetObjectReference(hud, "chargeContainer", chargesRect);
        SetObjectReference(hud, "chargeTemplate", chargeTemplate);

        GameObject savedPrefab = PrefabUtility.SaveAsPrefabAsset(root, HudPrefabPath);
        UnityEngine.Object.DestroyImmediate(root);
        return savedPrefab;
    }

    private static void ConfigureScene(
        string scenePath,
        GameObject hudPrefab,
        GameObject unlockPrefab,
        GameObject chargePrefab,
        bool placePickups)
    {
        Scene previousActiveScene = SceneManager.GetActiveScene();
        Scene scene = SceneManager.GetSceneByPath(scenePath);
        bool wasAlreadyLoaded = scene.IsValid() && scene.isLoaded;

        if (!wasAlreadyLoaded)
            scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Additive);

        Canvas canvas = FindComponentInScene<Canvas>(scene);
        if (canvas == null)
            canvas = CreateCanvas(scene);

        ReplaceNamedPrefab(scene, "ShurikenHUD", hudPrefab, canvas.transform, Vector3.zero);

        if (placePickups)
        {
            Transform player = FindPlayer(scene);
            Vector3 playerPosition = player != null ? player.position : Vector3.zero;

            ReplaceNamedPrefab(scene, "ShurikenUnlockPickup", unlockPrefab, null,
                playerPosition + new Vector3(3f, 1f, 0f));
            ReplaceNamedPrefab(scene, "ShurikenChargePickup", chargePrefab, null,
                playerPosition + new Vector3(6f, 1f, 0f));
        }

        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);

        if (!wasAlreadyLoaded)
            EditorSceneManager.CloseScene(scene, true);

        if (previousActiveScene.IsValid() && previousActiveScene.isLoaded)
            SceneManager.SetActiveScene(previousActiveScene);
    }

    private static void ReplaceNamedPrefab(
        Scene scene,
        string instanceName,
        GameObject prefab,
        Transform parent,
        Vector3 position)
    {
        GameObject existing = scene.GetRootGameObjects()
            .SelectMany(root => root.GetComponentsInChildren<Transform>(true))
            .Select(transform => transform.gameObject)
            .FirstOrDefault(gameObject => gameObject.name == instanceName);
        if (existing != null)
        {
            GameObject source = PrefabUtility.GetCorrespondingObjectFromSource(existing);
            if (source == prefab)
            {
                if (parent != null && existing.transform.parent != parent)
                    existing.transform.SetParent(parent, false);
                else if (parent == null)
                    existing.transform.position = position;

                return;
            }

            UnityEngine.Object.DestroyImmediate(existing);
        }

        GameObject instance = parent != null
            ? (GameObject)PrefabUtility.InstantiatePrefab(prefab, parent)
            : (GameObject)PrefabUtility.InstantiatePrefab(prefab, scene);
        instance.name = instanceName;

        if (parent == null)
            instance.transform.position = position;
    }

    private static T FindComponentInScene<T>(Scene scene) where T : Component
    {
        return scene.GetRootGameObjects()
            .SelectMany(root => root.GetComponentsInChildren<T>(true))
            .FirstOrDefault();
    }

    private static Transform FindPlayer(Scene scene)
    {
        return scene.GetRootGameObjects()
            .SelectMany(root => root.GetComponentsInChildren<Transform>(true))
            .FirstOrDefault(transform => transform.CompareTag("Player"));
    }

    private static Canvas CreateCanvas(Scene scene)
    {
        GameObject canvasObject = new(
            "Canvas",
            typeof(RectTransform),
            typeof(Canvas),
            typeof(CanvasScaler),
            typeof(GraphicRaycaster)
        );
        SceneManager.MoveGameObjectToScene(canvasObject, scene);

        Canvas canvas = canvasObject.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;

        CanvasScaler scaler = canvasObject.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(800f, 600f);
        return canvas;
    }

    private static GameObject CreateUiObject(string name, Transform parent)
    {
        GameObject gameObject = new(name, typeof(RectTransform));
        gameObject.layer = LayerMask.NameToLayer("UI");
        gameObject.transform.SetParent(parent, false);
        return gameObject;
    }

    private static void Stretch(RectTransform rectTransform)
    {
        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = Vector2.one;
        rectTransform.offsetMin = Vector2.zero;
        rectTransform.offsetMax = Vector2.zero;
    }

    private static void SetRect(
        RectTransform rectTransform,
        Vector2 anchor,
        Vector2 position,
        Vector2 size,
        Vector2 pivot)
    {
        rectTransform.anchorMin = anchor;
        rectTransform.anchorMax = anchor;
        rectTransform.pivot = pivot;
        rectTransform.anchoredPosition = position;
        rectTransform.sizeDelta = size;
    }

    private static void SetObjectReference(
        UnityEngine.Object target,
        string propertyName,
        UnityEngine.Object value)
    {
        SerializedObject serializedObject = new(target);
        SerializedProperty property = serializedObject.FindProperty(propertyName);
        if (property == null)
            throw new InvalidOperationException(
                $"No se encontro {propertyName} en {target.GetType().Name}."
            );

        property.objectReferenceValue = value;
        serializedObject.ApplyModifiedPropertiesWithoutUndo();
    }

    private static void EnsureFolder(string parent, string child)
    {
        string path = $"{parent}/{child}";
        if (!AssetDatabase.IsValidFolder(path))
            AssetDatabase.CreateFolder(parent, child);
    }
}
#endif
