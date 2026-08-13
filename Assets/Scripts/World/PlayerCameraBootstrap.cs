using UnityEngine;
using UnityEngine.SceneManagement;

internal static class PlayerCameraBootstrap
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Register()
    {
        SceneManager.sceneLoaded -= ConfigureLoadedScene;
        SceneManager.sceneLoaded += ConfigureLoadedScene;
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void ConfigureInitialScenes()
    {
        for (int index = 0; index < SceneManager.sceneCount; index++)
            ConfigureScene(SceneManager.GetSceneAt(index));
    }

    private static void ConfigureLoadedScene(Scene scene, LoadSceneMode loadMode)
    {
        ConfigureScene(scene);
    }

    private static void ConfigureScene(Scene scene)
    {
        if (!scene.IsValid() || !scene.isLoaded)
            return;

        Transform player = FindPlayer(scene);
        Camera mainCamera = FindMainCamera(scene);
        if (player == null || mainCamera == null)
            return;

        CameraController controller = mainCamera.GetComponent<CameraController>();
        if (controller == null)
            controller = mainCamera.gameObject.AddComponent<CameraController>();

        if (!controller.HasTarget)
            controller.SetTarget(player);
    }

    private static Transform FindPlayer(Scene scene)
    {
        foreach (GameObject root in scene.GetRootGameObjects())
        {
            Transform[] transforms = root.GetComponentsInChildren<Transform>(true);
            foreach (Transform candidate in transforms)
            {
                if (candidate.CompareTag("Player"))
                    return candidate;
            }
        }

        return null;
    }

    private static Camera FindMainCamera(Scene scene)
    {
        foreach (GameObject root in scene.GetRootGameObjects())
        {
            Camera[] cameras = root.GetComponentsInChildren<Camera>(true);
            foreach (Camera candidate in cameras)
            {
                if (candidate.CompareTag("MainCamera"))
                    return candidate;
            }
        }

        return null;
    }
}
