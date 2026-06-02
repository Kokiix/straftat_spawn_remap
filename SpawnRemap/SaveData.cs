using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;
using BepInEx;
using Newtonsoft.Json;
using SpawnRemap;
using UnityEngine;

namespace SpawnRemap;

using SpawnMappings = Dictionary<string, List<SpawnData>>;

[Serializable]
struct SpawnData
{
    public SerializableVector3 position;
    public SerializableVector3 rotation;
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

            if (spawnRemaps == null)
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

    // COMP/DECOMP ARE VIBE CODED
    internal static string Compress(string text)
    {
        if (string.IsNullOrEmpty(text))
            return text;

        // Convert string to bytes
        byte[] buffer = Encoding.UTF8.GetBytes(text);

        using (var memoryStream = new MemoryStream())
        {
            // DeflateStream is slightly faster and has less header overhead than GZipStream.
            // If you specifically need gzip headers (for web APIs, etc.), replace DeflateStream with GZipStream.
            using (var compressor = new DeflateStream(memoryStream, CompressionMode.Compress, true))
            {
                compressor.Write(buffer, 0, buffer.Length);
            }

            memoryStream.Position = 0;
            byte[] compressedData = memoryStream.ToArray();

            // Convert binary compressed data to a safe Base64 string
            return Convert.ToBase64String(compressedData);
        }
    }

    internal static string Decompress(string compressedText)
    {
        if (string.IsNullOrEmpty(compressedText))
            return compressedText;

        // Convert Base64 string back to compressed bytes
        byte[] compressedData = Convert.FromBase64String(compressedText);

        using (var memoryStream = new MemoryStream(compressedData))
        {
            using (var decompressor = new DeflateStream(memoryStream, CompressionMode.Decompress))
            {
                using (var resultStream = new MemoryStream())
                {
                    decompressor.CopyTo(resultStream);
                    return Encoding.UTF8.GetString(resultStream.ToArray());
                }
            }
        }
    }
}

[Serializable]
struct SerializableVector3(float x, float y, float z)
{
    public float x = x;
    public float y = y;
    public float z = z;

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