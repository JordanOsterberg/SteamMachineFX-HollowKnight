using System.IO;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using SteamMachineFX_HollowKnight.LED;

namespace SteamMachineFX_HollowKnight;

[BepInPlugin("com.jordanosterberg.steammachinefx-hk", "SteamMachineFX", "1.0.0")]
public class SteamMachineFX : BaseUnityPlugin
{
    public new static ManualLogSource Logger;
    public static SteamMachineFX Instance { get; private set; } = null!;

    public static bool CanControlLEDs { get; private set; }

    private void Awake()
    {
        Instance = this;
        Logger = base.Logger;
        
        if (!Directory.Exists("/sys/class/leds"))
        {
            LogWrongLEDsWarning();
            return;
        }
		
        var directories = Directory.GetDirectories("/sys/class/leds", "valve-leds*");
        if (directories.Length <= 0)
        {
            LogWrongLEDsWarning();
            return;
        }

        if (!Directory.Exists("/home/deck/steam-machine-fx-broker/"))
        {
            Logger.LogError("Please install SteamMachineFX before using this mod -- https://github.com/JordanOsterberg/SteamMachineFX-Installer");
            return;
        }

        Logger.LogInfo($"Found appropriate LED directories (total {directories.Length}), storing current LED state & patching.");
        CanControlLEDs = true;

        var harmony = new Harmony("com.jordanosterberg.steammachinefx-hk");
        harmony.PatchAll();
        
        Logger.LogInfo("About to store initial LED state");
        LEDManager.Instance.StoreInitialLEDState();
    }
        
    private static void LogWrongLEDsWarning()
    {
        Logger.LogWarning("Did not find appropriate LED directories, skipping initialization. If you are using a Steam Machine, please report this as a bug.");
    }
}
