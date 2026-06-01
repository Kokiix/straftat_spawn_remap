using System.Collections.Generic;
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
            JsonConvert.SerializeObject(SaveData.spawnRemaps)
        );
    }

    [CustomRPC]
    void MoveSpawnPoints(string spawnJSON)
    {
        try
        {
            JsonConvert.DeserializeObject<List<SpawnData>>(spawnJSON);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"SpawnRemapper: Failed to decode JSON from host: {e}");
        }

        SpawnPoint[] spawns = Object.FindObjectsByType<SpawnPoint>(FindObjectsSortMode.None);
    }
}