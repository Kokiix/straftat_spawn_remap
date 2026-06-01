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
    static string PlaceSpawn(int spawnPointIndex, bool twovtwo = false)
    {
        if (!twovtwo && (spawnPointIndex != 0 && spawnPointIndex != 1)) throw new CommandException("Input a spawn # from 0-1!");
        if (twovtwo && (spawnPointIndex < 0 || spawnPointIndex > 3)) throw new CommandException("Input a spawn # from 0-3!");

        var player = Settings.Instance.localPlayer;
        var map = SceneManager.GetActiveScene().name;
        if (!SaveData.spawnRemaps.TryGetValue(map, out List<SpawnData> spawns))
        {
            spawns = new List<SpawnData>(6);
            spawns.AddRange([new SpawnData(), new SpawnData(), new SpawnData(), new SpawnData(), new SpawnData(), new SpawnData()]);
        }

        if (twovtwo) spawnPointIndex += 2;
        spawns[spawnPointIndex] = new SpawnData()
        {
            position = player.transform.position,
            rotation = new Vector3(player.rotationX, player.transform.rotation.eulerAngles.y, player.rotationZ)
        };

        SaveData.spawnRemaps[map] = spawns;
        SaveData.Save();

        RemapSpawnLocal.SendSpawnPoints(spawns);

        if (twovtwo)
            return $"Set spawnpoint {spawnPointIndex - 2} for 2v2";
        else
            return $"Set spawnpoint {spawnPointIndex} for 1v1";
    }

    [Command("resetspawn", "remove a spawnpoint mapping (or all of them) for this map")]
    [CommandAliases("srrs")]
    static string ResetSpawn(string spawnPointIndex, bool twovtwo = false)
    {
        var map = SceneManager.GetActiveScene().name;
        if (!SaveData.spawnRemaps.TryGetValue(map, out List<SpawnData> spawns))
        {
            throw new CommandException("No spawn remaps have been set for this map.");
        }
        if (spawnPointIndex == "all")
        {
            if (twovtwo)
            {
                spawns.RemoveRange(2, 4);
                spawns.AddRange([new SpawnData(), new SpawnData(), new SpawnData(), new SpawnData()]);
            }
            else
            {
                spawns[0] = new SpawnData();
                spawns[1] = new SpawnData();
            }
        }
        else if (int.TryParse(spawnPointIndex, out int spawnIdx))
        {
            if (!twovtwo && (spawnIdx != 0 && spawnIdx != 1)) throw new CommandException("Input a spawn # from 0-1!");
            if (twovtwo && (spawnIdx < 0 || spawnIdx > 3)) throw new CommandException("Input a spawn # from 0-3!");
            var twovtwoIdxMod = twovtwo ? 2 : 0;
            if (spawns[spawnIdx + twovtwoIdxMod].rotation == new Vector3())
                throw new CommandException($"Spawn #{spawnIdx} has no remap!");
            else
                spawns[spawnIdx + twovtwoIdxMod] = new SpawnData();
        }
        else
        {
            throw new CommandException("Input a spawn # from 0-3, or the word \"all\"!");
        }

        SaveData.spawnRemaps[map] = spawns;
        SaveData.Save();

        RemapSpawnLocal.SendSpawnPoints(spawns);

        return $"Removed spawnpoint {spawnPointIndex}";
    }
}