using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using MyceliumNetworking;
using Newtonsoft.Json;
using SpawnRemap;
using UnityEngine;

class RemapSpawnLocal : MonoBehaviour
{
    const int MyceliumID = 239487836;

    void Awake()
    {
        MyceliumNetwork.RegisterNetworkObject(this, MyceliumID);
    }

    internal static void SendSpawnPoints(List<SpawnData> spawns)
    {
        MyceliumNetwork.RPC(
            MyceliumID,
            "MoveSpawnPoints",
            ReliableType.Reliable,
            JsonConvert.SerializeObject(spawns)
        );
        Debug.LogError("sent RPC");
    }

    [CustomRPC]
    void MoveSpawnPoints(string spawnJSON)
    {
        Debug.LogError("received RPC");
        List<SpawnData> spawnRemaps;
        try
        {
            spawnRemaps = JsonConvert.DeserializeObject<List<SpawnData>>(spawnJSON);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"SpawnRemapper: Failed to decode JSON from host: {e}");
            return;
        }

        List<Transform> spawns = Object.FindObjectsByType<SpawnPoint>(FindObjectsSortMode.None).Select(s => s.transform).ToList();
        foreach (var spawnPoint in spawns)
        {
            int spawnIdx;
            try
            {
                spawnIdx = int.Parse(spawnPoint.name[^1].ToString());
            }
            catch (System.Exception)
            {
                Debug.LogError("uhhhh this map has weird spawn names");
                return;
            }

            if (spawnRemaps[spawnIdx].rotation == new Vector3()) continue;
            spawnPoint.position = spawnRemaps[spawnIdx].position;
            spawnPoint.eulerAngles = spawnRemaps[spawnIdx].rotation;
        }
    }
}