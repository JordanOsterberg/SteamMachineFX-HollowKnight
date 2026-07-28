using HarmonyLib;
using SteamMachineFX_HollowKnight.LED;

namespace SteamMachineFX_HollowKnight.Patches;

public static class PlayerDataPatchesHelper
{
    private static bool _shouldChangeLEDs;
    
    public static void Run(PlayerData playerData)
    {
        SteamMachineFX.Logger.LogInfo($"Health is {playerData.health} / {playerData.CurrentMaxHealth}");

        if (!_shouldChangeLEDs) return;
        LEDManager.Instance.WriteHealthToLEDs(playerData);
    }

    public static void OnSaveStarted(PlayerData playerData)
    {
        _shouldChangeLEDs = true;
        Run(playerData);
    }

    public static void CleanUp()
    {
        SteamMachineFX.Logger.LogInfo("PlayerDataPatchesHelper.CleanUp called");

        LEDManager.Instance.RestoreInitialLEDState();
        _shouldChangeLEDs = false;
    }
}

[HarmonyPatch(typeof(PlayerData), nameof(PlayerData.AddHealth))]
public class PlayerAddHealthPatch
{
    [HarmonyPostfix]
    public static void Postfix(ref PlayerData __instance)
    {
        SteamMachineFX.Logger.LogInfo("PlayerData.AddHealth called.");
        PlayerDataPatchesHelper.Run(__instance);
    }
}
    
[HarmonyPatch(typeof(PlayerData), nameof(PlayerData.TakeHealth))]
public class PlayerTakeHealthPatch
{
    [HarmonyPostfix]
    public static void Postfix(ref PlayerData __instance)
    {
        SteamMachineFX.Logger.LogInfo("PlayerData.TakeHealth called.");
        PlayerDataPatchesHelper.Run(__instance);
    }
}

[HarmonyPatch(typeof(PlayerData), nameof(PlayerData.UpdateBlueHealth))]
public class PlayerUpdateBlueHealthPatch
{
    [HarmonyPostfix]
    public static void Postfix(ref PlayerData __instance)
    {
        SteamMachineFX.Logger.LogInfo("PlayerData.UpdateBlueHealth called.");
        PlayerDataPatchesHelper.Run(__instance);
    }
}

[HarmonyPatch(typeof(PlayerData), nameof(PlayerData.AddToMaxHealth))]
public class PlayerAddToMaxHealthPatch
{
    [HarmonyPostfix]
    public static void Postfix(ref PlayerData __instance)
    {
        SteamMachineFX.Logger.LogInfo("PlayerData.AddToMaxHealth called.");
        PlayerDataPatchesHelper.Run(__instance);
    }
}

[HarmonyPatch(typeof(PlayerData), nameof(PlayerData.EquipCharm))]
public class PlayerEquipCharmPatch
{
    [HarmonyPostfix]
    public static void Postfix(ref PlayerData __instance)
    {
        SteamMachineFX.Logger.LogInfo("PlayerData.EquipCharm called.");
        PlayerDataPatchesHelper.Run(__instance);
    }
}

[HarmonyPatch(typeof(PlayerData), nameof(PlayerData.UnequipCharm))]
public class PlayerUnequipCharmPatch
{
    [HarmonyPostfix]
    public static void Postfix(ref PlayerData __instance)
    {
        SteamMachineFX.Logger.LogInfo("PlayerData.UnequipCharm called.");
        PlayerDataPatchesHelper.Run(__instance);
    }
}