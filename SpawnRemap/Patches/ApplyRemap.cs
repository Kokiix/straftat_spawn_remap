using SpawnRemap;
using UnityEngine.SceneManagement;

static class ApplyRemap
{
    internal static void Init()
    {
        // uhh its a patch in spirit
        SceneManager.sceneLoaded += DetermineRemapping;
    }

    static void DetermineRemapping(Scene s, LoadSceneMode mode)
    {
        if (SaveData.spawnRemaps.ContainsKey(s.name))
            RemapSpawnLocal.SendSpawnPoints(SaveData.spawnRemaps[s.name]);
    }
}