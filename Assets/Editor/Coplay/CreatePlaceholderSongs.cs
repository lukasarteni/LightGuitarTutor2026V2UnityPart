using System;
using UnityEngine;
using UnityEditor;
using Coplay.Controllers.Functions;

public class CreatePlaceholderSongs
{
    public static string Execute()
    {
        // Ensure directory exists
        if (!AssetDatabase.IsValidFolder("Assets/LukasMadeStuff/Songs"))
        {
            AssetDatabase.CreateFolder("Assets/LukasMadeStuff", "Songs");
        }

        string[] names = { "Song 1", "Song 2", "Song 3", "Song 4", "Song 5" };
        string[] artists = { "Artist A", "Artist B", "Artist C", "Artist D", "Artist E" };
        string[] durations = { "3:24", "4:10", "2:58", "5:02", "3:45" };
        Difficulty[] difficulties = { Difficulty.Easy, Difficulty.Easy, Difficulty.Medium, Difficulty.Medium, Difficulty.Hard };

        SongData[] songAssets = new SongData[5];

        for (int i = 0; i < 5; i++)
        {
            string assetPath = $"Assets/LukasMadeStuff/Songs/Song{i + 1}.asset";

            // Delete if exists already
            var existing = AssetDatabase.LoadAssetAtPath<SongData>(assetPath);
            if (existing != null)
            {
                AssetDatabase.DeleteAsset(assetPath);
            }

            SongData song = ScriptableObject.CreateInstance<SongData>();
            song.songName = names[i];
            song.artist = artists[i];
            song.duration = durations[i];
            song.difficulty = difficulties[i];
            song.clip = null; // placeholder — no audio clip

            AssetDatabase.CreateAsset(song, assetPath);
            songAssets[i] = song;
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        // Reload all assets to be safe
        for (int i = 0; i < 5; i++)
        {
            string assetPath = $"Assets/LukasMadeStuff/Songs/Song{i + 1}.asset";
            songAssets[i] = AssetDatabase.LoadAssetAtPath<SongData>(assetPath);
        }

        // Find the MenuManager in the scene and assign the songs array
        var menuManager = GameObject.FindFirstObjectByType<MenuManager>();
        if (menuManager == null)
        {
            return "ERROR: Could not find MenuManager in the scene.";
        }

        menuManager.songs = songAssets;

        // Mark the scene and object as dirty so Unity saves the change
        EditorUtility.SetDirty(menuManager);
        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(
            UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());
        UnityEditor.SceneManagement.EditorSceneManager.SaveOpenScenes();

        return $"Created 5 placeholder SongData assets in Assets/LukasMadeStuff/Songs/ and assigned them to MenuManager.songs:\n" +
               $"  1. {songAssets[0].songName} - {songAssets[0].artist} ({songAssets[0].duration}) [{songAssets[0].difficulty}]\n" +
               $"  2. {songAssets[1].songName} - {songAssets[1].artist} ({songAssets[1].duration}) [{songAssets[1].difficulty}]\n" +
               $"  3. {songAssets[2].songName} - {songAssets[2].artist} ({songAssets[2].duration}) [{songAssets[2].difficulty}]\n" +
               $"  4. {songAssets[3].songName} - {songAssets[3].artist} ({songAssets[3].duration}) [{songAssets[3].difficulty}]\n" +
               $"  5. {songAssets[4].songName} - {songAssets[4].artist} ({songAssets[4].duration}) [{songAssets[4].difficulty}]";
    }
}
