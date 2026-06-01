using ChatCommands;
using ChatCommands.Attributes;
using SpawnRemap;
using UnityEngine;

[CommandCategory("SpawnRemap")]
class SpawnRemapCommands
{
    const CommandFlags _remapFlags = CommandFlags.ExplorationOnly | CommandFlags.IngameOnly | CommandFlags.HostOnly;

    [Command("setspawn", "remap some spawnpoint in the map to your current location")]
    [CommandAliases("ss")]
    static void SetSpawn(int spawnNumber)
    {
        Debug.LogError("test");
        // var player = FirstPersonController.instance;
        // var map =
        // SaveData
    }
}