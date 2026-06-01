using System;
using BepInEx;
using BepInEx.Configuration;
using ComputerysModdingUtilities;
using HarmonyLib;
using UnityEngine;
using SpawnRemap;

[assembly: StraftatMod(isVanillaCompatible: false)]

[BepInDependency(ChatCommands.PluginInfo.PLUGIN_GUID, BepInDependency.DependencyFlags.HardDependency)]
[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class SRPlugin : BaseUnityPlugin
{
    Harmony _harmony = new(MyPluginInfo.PLUGIN_GUID);

    void Awake()
    {
        _harmony.PatchAll();
        this.gameObject.hideFlags = HideFlags.HideAndDontSave;

        InitConfig();
    }

    void InitConfig()
    {
    }

    void OnDestroy()
    {
        _harmony.UnpatchSelf();
    }

    // Debug
    // void Update()
    // {
    //     if (Input.GetKeyDown(KeyCode.V))
    //     {
    //         if (gameObject.TryGetComponent(out TestBehav test))
    //             UnityEngine.Object.Destroy(test);
    //         else
    //             gameObject.AddComponent<TestBehav>();
    //     }
    // }
}