using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using MyceliumNetworking;
using Newtonsoft.Json;
using SpawnRemap;
using UnityEngine;
using UnityEngine.SceneManagement;

class RemapSpawnLocal : MonoBehaviour
{
    const int MyceliumID = 239487836;

    void Awake()
    {
        MyceliumNetwork.RegisterNetworkObject(this, MyceliumID);
        originalSpawnLocations = [new SpawnData(), new SpawnData(), new SpawnData(), new SpawnData(), new SpawnData(), new SpawnData()];
    }

    internal static void SendSpawnPoints(List<SpawnData> spawns)
    {
        MyceliumNetwork.RPC(
            MyceliumID,
            "MoveSpawnPoints",
            ReliableType.Reliable,
            JsonConvert.SerializeObject(spawns)
        );
    }

    internal static List<SpawnData> originalSpawnLocations; // Used for seeing live changes when removing spawns

    [CustomRPC]
    void MoveSpawnPoints(string spawnJSON)
    {
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

        List<Transform> spawns = Object.FindObjectsByType<SpawnPoint>(FindObjectsSortMode.None)
        .Select(s => s.transform)
        .OrderBy(t => t.name)
        .ToList();

        var newList = new List<Transform>();
        newList.AddRange(spawns.Where(t => t.parent.tag != "Spawnpoints4Player"));
        newList.AddRange(spawns.Where(t => t.parent.tag == "Spawnpoints4Player"));
        spawns = newList;

        for (int i = 0; i < spawnRemaps.Count; i++)
        {
            var spawnPoint = spawns[i];
            if (spawnRemaps[i].position == new Vector3())
            {
                if (originalSpawnLocations[i].position != new Vector3())
                {
                    spawnPoint.position = originalSpawnLocations[i].position;
                    spawnPoint.eulerAngles = originalSpawnLocations[i].rotation;
                }
            }
            else
            {
                if (originalSpawnLocations[i].position == new Vector3())
                {
                    var origPos = originalSpawnLocations[i];
                    origPos.position = spawnPoint.position;
                    origPos.rotation = new Vector3(spawnPoint.rotation.eulerAngles.x, spawnPoint.rotation.eulerAngles.y, spawnPoint.rotation.eulerAngles.z);
                    originalSpawnLocations[i] = origPos;
                }

                spawnPoint.position = spawnRemaps[i].position;
                spawnPoint.eulerAngles = spawnRemaps[i].rotation;
            }
        }
    }
}