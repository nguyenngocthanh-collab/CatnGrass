using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;

public class DevTools : MonoBehaviour
{
    public bool showMenu = false;

    private bool platformEnabled = true;

    void Update()
    {
        // F1 bật/tắt menu dev
        if (Input.GetKeyDown(KeyCode.F1))
        {
            showMenu = !showMenu;
        }
    }

    void OnGUI()
    {
        if (!showMenu) return;

        GUI.Box(new Rect(10, 10, 280, 700), "DEV MENU");

        // ===== PLATFORM SYSTEM =====

        if (GUI.Button(
            new Rect(20, 50, 240, 40),
            platformEnabled ?
            "Disable Platforms" :
            "Enable Platforms"))
        {
            platformEnabled = !platformEnabled;

            DynamicPlatform[] platforms =
                FindObjectsOfType<DynamicPlatform>();

            foreach (DynamicPlatform p in platforms)
            {
                p.SetMovementState(platformEnabled);
            }
        }

        // ===== RESET PLATFORM =====

        if (GUI.Button(
            new Rect(20, 100, 240, 40),
            "Reset Platforms"))
        {
            DynamicPlatform[] platforms =
                FindObjectsOfType<DynamicPlatform>();

            foreach (DynamicPlatform p in platforms)
            {
                p.ResetPlatform();
            }
        }

        // ===== RESTART SCENE =====

        if (GUI.Button(
            new Rect(20, 150, 240, 40),
            "Restart Current Scene"))
        {
            SceneManager.LoadScene(
                SceneManager.GetActiveScene().buildIndex
            );
        }

        // ===== SCENE LIST =====

        GUI.Label(
            new Rect(20, 200, 240, 30),
            "SCENES"
        );

        int y = 240;

        for (int i = 0;
            i < SceneManager.sceneCountInBuildSettings;
            i++)
        {
            string scenePath =
                SceneUtility.GetScenePathByBuildIndex(i);

            string sceneName =
                Path.GetFileNameWithoutExtension(scenePath);

            if (GUI.Button(
                new Rect(20, y, 240, 35),
                i + " - " + sceneName))
            {
                SceneManager.LoadScene(i);
            }

            y += 40;
        }
    }
}