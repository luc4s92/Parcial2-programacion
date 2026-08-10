#if UNITY_EDITOR
using System;
using System.Linq;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

internal static class PlayerShurikenAssetBuilder
{
    private const string ThrowTexturePath =
        "Assets/Sprites/Player/Animation/throw/THROW.png";
    private const string ShurikenTexturePath =
        "Assets/Sprites/Player/Projectiles/shuriken.png";
    private const string ThrowClipPath =
        "Assets/Sprites/Player/Animation/animations/throw.anim";
    private const string PlayerControllerPath =
        "Assets/Sprites/Player/Animation/animations/Player.controller";
    private const string ShurikenPrefabPath =
        "Assets/Prefabs/Projectiles/PlayerShuriken.prefab";
    private const string PlayerPrefabPath =
        "Assets/Prefabs/Player/Player.prefab";

    private const int ThrowFrameCount = 7;
    private const int ThrowFrameSize = 96;
    private const float PixelsPerUnit = 16f;
    private const float ThrowFrameRate = 15f;

    [MenuItem("Tools/Player/Configurar ataque shuriken %#k")]
    public static void ConfigureFromMenu()
    {
        try
        {
            ConfigureThrowTexture();
            ConfigureShurikenTexture();

            AnimationClip throwClip = CreateOrUpdateThrowClip();
            AddOrUpdateThrowState(throwClip);

            PlayerShurikenProjectile projectilePrefab =
                CreateOrUpdateShurikenPrefab();
            ConfigurePlayerPrefab(projectilePrefab);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("[PlayerShurikenAssetBuilder] Ataque shuriken configurado.");
        }
        catch (Exception exception)
        {
            Debug.LogException(exception);
        }
    }

    private static void ConfigureThrowTexture()
    {
        TextureImporter importer = GetTextureImporter(ThrowTexturePath);
        ConfigurePixelArt(importer, SpriteImportMode.Multiple);

        SpriteMetaData[] frames = new SpriteMetaData[ThrowFrameCount];
        for (int index = 0; index < ThrowFrameCount; index++)
        {
            frames[index] = new SpriteMetaData
            {
                name = $"THROW_{index}",
                rect = new Rect(
                    index * ThrowFrameSize,
                    0f,
                    ThrowFrameSize,
                    ThrowFrameSize
                ),
                alignment = (int)SpriteAlignment.Center,
                pivot = new Vector2(0.5f, 0.5f)
            };
        }

#pragma warning disable CS0618
        importer.spritesheet = frames;
#pragma warning restore CS0618
        importer.SaveAndReimport();
    }

    private static void ConfigureShurikenTexture()
    {
        TextureImporter importer = GetTextureImporter(ShurikenTexturePath);
        ConfigurePixelArt(importer, SpriteImportMode.Single);
        importer.spritePivot = new Vector2(0.5f, 0.5f);
        importer.SaveAndReimport();
    }

    private static TextureImporter GetTextureImporter(string assetPath)
    {
        TextureImporter importer = AssetImporter.GetAtPath(assetPath) as TextureImporter;
        if (importer == null)
            throw new InvalidOperationException($"No se encontro la textura {assetPath}.");

        return importer;
    }

    private static void ConfigurePixelArt(
        TextureImporter importer,
        SpriteImportMode importMode)
    {
        importer.textureType = TextureImporterType.Sprite;
        importer.spriteImportMode = importMode;
        importer.spritePixelsPerUnit = PixelsPerUnit;
        importer.mipmapEnabled = false;
        importer.filterMode = FilterMode.Point;
        importer.textureCompression = TextureImporterCompression.Uncompressed;
    }

    private static AnimationClip CreateOrUpdateThrowClip()
    {
        Sprite[] frames = AssetDatabase.LoadAllAssetsAtPath(ThrowTexturePath)
            .OfType<Sprite>()
            .OrderBy(sprite => sprite.name)
            .ToArray();

        if (frames.Length != ThrowFrameCount)
        {
            throw new InvalidOperationException(
                $"Se esperaban {ThrowFrameCount} frames de lanzamiento y se encontraron {frames.Length}."
            );
        }

        AnimationClip clip = AssetDatabase.LoadAssetAtPath<AnimationClip>(ThrowClipPath);
        if (clip == null)
        {
            clip = new AnimationClip();
            AssetDatabase.CreateAsset(clip, ThrowClipPath);
        }

        clip.name = "throw";
        clip.frameRate = ThrowFrameRate;
        clip.wrapMode = WrapMode.Once;

        EditorCurveBinding spriteBinding = new EditorCurveBinding
        {
            path = string.Empty,
            type = typeof(SpriteRenderer),
            propertyName = "m_Sprite"
        };

        ObjectReferenceKeyframe[] keyframes = new ObjectReferenceKeyframe[frames.Length + 1];
        for (int index = 0; index < frames.Length; index++)
        {
            keyframes[index] = new ObjectReferenceKeyframe
            {
                time = index / ThrowFrameRate,
                value = frames[index]
            };
        }

        keyframes[^1] = new ObjectReferenceKeyframe
        {
            time = frames.Length / ThrowFrameRate,
            value = frames[^1]
        };

        AnimationUtility.SetObjectReferenceCurve(clip, spriteBinding, keyframes);
        AnimationUtility.SetAnimationEvents(
            clip,
            new[]
            {
                new AnimationEvent
                {
                    time = 3f / ThrowFrameRate,
                    functionName = "OnRangedAttackRelease"
                },
                new AnimationEvent
                {
                    time = frames.Length / ThrowFrameRate - 0.001f,
                    functionName = "OnRangedAttackAnimationFinished"
                }
            }
        );

        AnimationClipSettings settings = AnimationUtility.GetAnimationClipSettings(clip);
        settings.loopTime = false;
        AnimationUtility.SetAnimationClipSettings(clip, settings);
        EditorUtility.SetDirty(clip);
        return clip;
    }

    private static void AddOrUpdateThrowState(AnimationClip throwClip)
    {
        AnimatorController controller =
            AssetDatabase.LoadAssetAtPath<AnimatorController>(PlayerControllerPath);
        if (controller == null)
            throw new InvalidOperationException("No se encontro el Animator Controller del jugador.");

        AnimatorStateMachine stateMachine = controller.layers[0].stateMachine;
        AnimatorState throwState = stateMachine.states
            .Select(childState => childState.state)
            .FirstOrDefault(state => state.name == "throw");

        if (throwState == null)
            throwState = stateMachine.AddState("throw", new Vector3(680f, 430f, 0f));

        throwState.motion = throwClip;
        throwState.writeDefaultValues = true;
        EditorUtility.SetDirty(throwState);
        EditorUtility.SetDirty(controller);
    }

    private static PlayerShurikenProjectile CreateOrUpdateShurikenPrefab()
    {
        Sprite shurikenSprite = AssetDatabase.LoadAllAssetsAtPath(ShurikenTexturePath)
            .OfType<Sprite>()
            .FirstOrDefault();
        if (shurikenSprite == null)
            throw new InvalidOperationException("No se pudo cargar el sprite del shuriken.");

        GameObject existingPrefab =
            AssetDatabase.LoadAssetAtPath<GameObject>(ShurikenPrefabPath);
        GameObject root = existingPrefab != null
            ? PrefabUtility.LoadPrefabContents(ShurikenPrefabPath)
            : new GameObject("PlayerShuriken");

        try
        {
            SpriteRenderer spriteRenderer = GetOrAddComponent<SpriteRenderer>(root);
            spriteRenderer.sprite = shurikenSprite;
            spriteRenderer.sortingOrder = 10;

            Rigidbody2D rigidBody = GetOrAddComponent<Rigidbody2D>(root);
            rigidBody.bodyType = RigidbodyType2D.Dynamic;
            rigidBody.gravityScale = 0f;
            rigidBody.linearDamping = 0f;
            rigidBody.angularDamping = 0f;
            rigidBody.interpolation = RigidbodyInterpolation2D.Interpolate;
            rigidBody.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            rigidBody.constraints = RigidbodyConstraints2D.None;

            CircleCollider2D collider = GetOrAddComponent<CircleCollider2D>(root);
            collider.isTrigger = true;
            collider.radius = 0.24f;

            GetOrAddComponent<PlayerShurikenProjectile>(root);
            PrefabUtility.SaveAsPrefabAsset(root, ShurikenPrefabPath);
        }
        finally
        {
            if (existingPrefab != null)
                PrefabUtility.UnloadPrefabContents(root);
            else
                UnityEngine.Object.DestroyImmediate(root);
        }

        GameObject savedPrefab =
            AssetDatabase.LoadAssetAtPath<GameObject>(ShurikenPrefabPath);
        return savedPrefab.GetComponent<PlayerShurikenProjectile>();
    }

    private static void ConfigurePlayerPrefab(
        PlayerShurikenProjectile projectilePrefab)
    {
        GameObject playerRoot = PrefabUtility.LoadPrefabContents(PlayerPrefabPath);
        try
        {
            Transform firePoint = playerRoot.transform.Find("ShurikenFirePoint");
            if (firePoint == null)
            {
                firePoint = new GameObject("ShurikenFirePoint").transform;
                firePoint.SetParent(playerRoot.transform, false);
            }

            firePoint.localPosition = new Vector3(1.43f, -1f, 0f);
            firePoint.localRotation = Quaternion.identity;
            firePoint.localScale = Vector3.one;

            PlayerMovement movement = playerRoot.GetComponent<PlayerMovement>();
            if (movement == null)
                throw new InvalidOperationException("El prefab Player no tiene PlayerMovement.");

            SerializedObject serializedMovement = new SerializedObject(movement);
            serializedMovement.FindProperty("shurikenProjectilePrefab").objectReferenceValue =
                projectilePrefab;
            serializedMovement.FindProperty("shurikenFirePoint").objectReferenceValue = firePoint;
            serializedMovement.ApplyModifiedPropertiesWithoutUndo();

            PrefabUtility.SaveAsPrefabAsset(playerRoot, PlayerPrefabPath);
        }
        finally
        {
            PrefabUtility.UnloadPrefabContents(playerRoot);
        }
    }

    private static T GetOrAddComponent<T>(GameObject target) where T : Component
    {
        T component = target.GetComponent<T>();
        return component != null ? component : target.AddComponent<T>();
    }
}
#endif
