using System;
using System.Collections.Generic;
using System.IO;
using BepInEx;
using SpawnRemap;
using UnityEngine;

namespace SpawnRemap;

using SpawnMappings = Dictionary<string, List<SpawnData>>;

struct SpawnData
{
    internal Vector3 position;
    internal Vector3 angle;
}

static class SaveData
{
    const string _saveFilename = MyPluginInfo.PLUGIN_GUID + "_spawnpoint_data.json";
    static string _savePath;

    static internal SpawnMappings spawnRemaps;

    static internal void Init()
    {
        _savePath = Path.Combine(Paths.ConfigPath, _saveFilename);

        try
        {
            if (File.Exists(_savePath))
                spawnRemaps = JsonUtility.FromJson<SpawnMappings>(File.ReadAllText(_savePath));
            else
            {
                spawnRemaps = [];
                Save();
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"SpawnRemapper: Failed to read save data: {e}");
        }
    }

    internal static void Save()
    {
        try
        {
            File.WriteAllText(_savePath, JsonUtility.ToJson(spawnRemaps, prettyPrint: true));
        }
        catch (Exception e)
        {
            Debug.LogError($"SpawnRemapper: Failed to save data {e}");
        }
    }
}