﻿using UnityEngine;
using System.Collections;
using System.IO;
using System.Diagnostics;

public class Util : MonoBehaviour {
    public static string TEMP_SONG_PATH = ".temp-song.wav";
    public static string CONFIG_PATH = "playmusic3d.cfg";
    private static bool configInitialized = false;
    private static Hashtable configs = new Hashtable();

    public static void InitializeConfig () {
        if (File.Exists(CONFIG_PATH)) {
            string[] configContents = File.ReadAllLines(CONFIG_PATH);
            foreach (string entry in configContents) {
                string[] pair = entry.Split('=');
                configs.Add(pair[0], pair[1]);
            }
            configInitialized = true;
        }
    }

    public static void CreateConfig (string config) {
        StreamWriter writer = new StreamWriter(File.Create(CONFIG_PATH));
        writer.WriteLine(config);
        writer.Close();
    }

    public static bool IsConfigInitialized () {
        return configInitialized;
    }

    public static string GetConfigValue (string key) {
        if (!IsConfigInitialized()) {
            InitializeConfig();
        }
        return (string)configs[key];
    }

    public static Texture2D LoadTexture(RecordInfo info) {
        if (info.imageData.Length > 0) {
            Texture2D result = new Texture2D(1, 1);
            result.LoadImage(info.imageData);
            return result;
        } else {
            return null;
        }
    }

    public static void LoadTempSong(string path) {
        Process process = new Process();
        process.StartInfo.FileName = "ffmpeg";
        process.StartInfo.Arguments = "-i \"" + path + "\" -y -nostdin " + TEMP_SONG_PATH;
        process.StartInfo.CreateNoWindow = true;
        process.StartInfo.UseShellExecute = false;
        process.StartInfo.RedirectStandardOutput = true;

        process.Start();
        process.WaitForExit();
        process.Close();

        File.SetAttributes(TEMP_SONG_PATH, FileAttributes.Hidden);
    }

    public static void CleanupTempSong () {
        while (File.Exists(TEMP_SONG_PATH)) {
            try {
                File.Delete(TEMP_SONG_PATH);
            } catch (IOException) { }
        }
    }

    public static bool IsFileUnusable (string path) {
        try {
            File.Open(path, FileMode.Open, FileAccess.Read, FileShare.None).Close();
        } catch (IOException) {
            UnityEngine.Debug.Log("Unacceptable!");
            return true;
        }
        return false;
    }

    public static Color GetAverageColorFromTexture (Texture2D texture, float samplesX = 8, float samplesY = 8) {
        float totalR = 0;
        float totalG = 0;
        float totalB = 0;
        float count = samplesX * samplesY;
        for (float y = 0; y < samplesY; y++) {
            for (float x = 0; x < samplesX; x++) {
                Color pixel = texture.GetPixelBilinear(x / samplesX, y / samplesY);
                totalR += pixel.r;
                totalG += pixel.g;
                totalB += pixel.b;
            }
        }
        return new Color(totalR / count, totalG / count, totalB / count);
    }

    public static float GetBrightnessFromColor (Color color) {
        return 0.299f * color.r + 0.587f * color.g + 0.114f * color.b;
    }

    public static Color GetOverlayColor (Color color) {
        return GetBrightnessFromColor(color) < 0.5 ? new Color(1, 1, 1) : new Color(0, 0, 0);
    }

    public static GameObject GetHoveredGameObject () {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        Physics.Raycast(ray, out hit, 10);
        GameObject result = null;
        if (hit.collider != null) {
            result = hit.collider.gameObject;
        }
        return result;
    }
}
