using System.Collections.Generic;
using ChatCommands;
using ChatCommands.Attributes;
using SpawnRemap;
using UnityEngine;
using UnityEngine.SceneManagement;

[CommandCategory("SpawnRemap")]
class SpawnRemapCommands
{
    const CommandFlags _remapFlags = CommandFlags.ExplorationOnly | CommandFlags.IngameOnly | CommandFlags.HostOnly;

    [Command("placespawn", "remap some spawnpoint in the map to your current location")]
    [CommandAliases("srps")]
    static void PlaceSpawn(int spawnIdx)
    {
        if (spawnIdx < 0 || spawnIdx > 3) throw new CommandException("Input a spawn # from 0-3!");

        var player = Settings.Instance.localPlayer;
        var map = SceneManager.GetActiveScene().name;
        if (!SaveData.spawnRemaps.TryGetValue(map, out List<SpawnData> spawns))
            spawns = new List<SpawnData>(4);

        spawns[spawnIdx] = new SpawnData()
        {
            position = player.transform.position,
            angle = new Vector3(player.rotationX, player.transform.rotation.eulerAngles.y, player.rotationZ)
        };

        SaveData.Save();
    }

    [Command("resetspawn", "remove a spawnpoint mapping (or all of them) for this map")]
    [CommandAliases("srrs")]
    static void ResetSpawn(string spawnIdxStr)
    {
        var map = SceneManager.GetActiveScene().name;
        if (!SaveData.spawnRemaps.TryGetValue(map, out List<SpawnData> spawns))
            spawns = new List<SpawnData>(4);
        if (spawnIdxStr == "all")
        {
            spawns.Clear();
        }
        else if (int.TryParse(spawnIdxStr, out int spawnIdx))
        {
            spawns.RemoveAt(spawnIdx);
        }
        else
        {
            throw new CommandException("Input a spawn # from 0-3, or the word \"all\"!");
        }
        SaveData.Save();
    }
}