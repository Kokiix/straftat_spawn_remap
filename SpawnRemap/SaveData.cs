using System;
using System.Collections.Generic;
using System.IO;
using BepInEx;
using Newtonsoft.Json;
using SpawnRemap;
using UnityEngine;

namespace SpawnRemap;

using SpawnMappings = Dictionary<string, List<SpawnData>>;

[Serializable]
struct SpawnData
{
    internal SerializableVector3 position;
    internal SerializableVector3 angle;
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
                spawnRemaps = JsonConvert.DeserializeObject<SpawnMappings>(File.ReadAllText(_savePath));
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
            File.WriteAllText(_savePath, JsonConvert.SerializeObject(spawnRemaps, Formatting.Indented));
        }
        catch (Exception e)
        {
            Debug.LogError($"SpawnRemapper: Failed to save data {e}");
        }
    }
}

[Serializable]
struct SerializableVector3(float x, float y, float z)
{
    float x = x;
    float y = y;
    float z = z;

    // Allows: Vector3 unityVec = mySerializableVec;
    public static implicit operator Vector3(SerializableVector3 sVec)
    {
        return new Vector3(sVec.x, sVec.y, sVec.z);
    }

    // Allows: SerializableVector3 mySerializableVec = unityVec;
    public static implicit operator SerializableVector3(Vector3 uVec)
    {
        return new SerializableVector3(uVec.x, uVec.y, uVec.z);
    }
}