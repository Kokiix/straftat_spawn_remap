using System;
using BepInEx;
using BepInEx.Configuration;
using ComputerysModdingUtilities;
using HarmonyLib;
using UnityEngine;
using SpawnRemap;
using UnityEngine.SceneManagement;

[assembly: StraftatMod(isVanillaCompatible: false)]

[BepInDependency(ChatCommands.PluginInfo.PLUGIN_GUID, BepInDependency.DependencyFlags.SoftDependency)]
[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class SRPlugin : BaseUnityPlugin
{
    Harmony _harmony = new(MyPluginInfo.PLUGIN_GUID);

    void Awake()
    {
        this.gameObject.hideFlags = HideFlags.HideAndDontSave;

        InitConfig();
        SaveData.Init();

        // patchAll doesn't work because harmony explodes on custom attribute if chatcommands isnt loaded
        // _harmony.CreateClassProcessor(typeof())

        var chatCommandsLoaded = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey(ChatCommands.PluginInfo.PLUGIN_GUID);
        if (chatCommandsLoaded)
            RegisterCommands();
    }

    void InitConfig()
    {
    }

    void OnDestroy()
    {
        _harmony.UnpatchSelf();
    }

    void RegisterCommands()
    {
        ChatCommands.CommandRegistry.RegisterCommandsFromAssembly();
    }

    // Debug
    // void Update()
    // {
    //     if (Input.GetKeyDown(KeyCode.V))
    //     {
    //         Debug.LogError(SceneManager.GetActiveScene().name);
    //     }
    // }
}