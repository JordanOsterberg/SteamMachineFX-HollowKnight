using System;
using GlobalEnums;
using HarmonyLib;

namespace SteamMachineFX_HollowKnight.Patches;

[HarmonyPatch(typeof(GameManager), nameof(GameManager.SetState))]
public class GameManagerSetStatePatch
{
    [HarmonyPostfix]
    public static void Postfix(ref GameManager __instance, GameState newState)
    {
        SteamMachineFX.Logger.LogInfo($"Got new game state, {newState}");

        try
        {
            switch (newState)
            {
                case GameState.PLAYING:
                    PlayerDataPatchesHelper.OnSaveStarted(__instance.playerData);
                    break;
                case GameState.MAIN_MENU:
                    PlayerDataPatchesHelper.CleanUp();
                    break;
            }
        }
        catch (Exception e)
        {
            SteamMachineFX.Logger.LogError("Failed to handle game state change with exception");
            SteamMachineFX.Logger.LogError(e.ToString());
        }
    }
}