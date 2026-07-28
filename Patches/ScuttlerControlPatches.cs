using System.Collections;
using HarmonyLib;
using UnityEngine;

namespace SteamMachineFX_HollowKnight.Patches;

[HarmonyPatch(typeof(ScuttlerControl), "Heal")]
public class ScuttlerControlHealPatch
{
    [HarmonyPostfix]
    public static void Postfix()
    {
        SteamMachineFX.Logger.LogInfo("ScuttlerControl.Heal called, queueing delayed LED refresh.");
        SteamMachineFX.Instance.StartCoroutine(RunDelayedRefresh());
    }

    private static IEnumerator RunDelayedRefresh()
    {
        yield return new WaitForSeconds(1.25f); // Wait 0.05s more than what is in the original method so we catch the change

        if (PlayerData.instance == null)
        {
            SteamMachineFX.Logger.LogWarning("Skipping scuttler LED refresh because PlayerData.instance was null.");
            yield break;
        }

        PlayerDataPatchesHelper.Run(PlayerData.instance);
    }
}
