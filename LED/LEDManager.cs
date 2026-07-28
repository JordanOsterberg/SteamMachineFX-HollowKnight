using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

#nullable enable

namespace SteamMachineFX_HollowKnight.LED;

public sealed class LEDManager
{
    private List<LEDState> _initialLedStates = new List<LEDState>();
    
    private static LEDManager? _instance;
    public static LEDManager Instance => _instance ??= new LEDManager();
    
    public void StoreInitialLEDState()
    {
        SteamMachineFX.Logger.LogInfo("Storing initial LED state...");
        _initialLedStates = LEDState.SaveState();
    }

    public void RestoreInitialLEDState()
    {
        SteamMachineFX.Logger.LogInfo("Restoring initial LED state...");
        
        LEDState.WriteStates(_initialLedStates);

        Task.Run(async () =>
        {
            await Task.Delay(300); // Wait 300ms so we'll have waited at least long enough for the last update to have been written
            LEDState.DisableLEDMonitor();
        });
    }

    public void WriteHealthToLEDs(PlayerData playerData)
    {
        var percentage = (float)playerData.health / playerData.CurrentMaxHealth;
        
        WritePercentageToLEDs(percentage, playerData.healthBlue);
    }
    
    private void WritePercentageToLEDs(float percentage, int totalBlueHealth)
    {
        var totalLEDs = _initialLedStates.Count - totalBlueHealth; // blue health removes LEDs from the pool
        var litLEDs = (int)Math.Ceiling(percentage * totalLEDs);
    
        SteamMachineFX.Logger.LogInfo($"Writing percentage {percentage} across {totalLEDs} LEDs...");
    
        var states = new List<LEDState>();
        
        for (var i = 0; i < totalLEDs; i++)
        {
            var led = _initialLedStates[i];
    
            var shouldBeLit = i < litLEDs;
            var color = shouldBeLit ? "255 0 0" : "0 0 0";
            var brightness = shouldBeLit ? "255" : "0";
            
            SteamMachineFX.Logger.LogInfo($"{led.Path} / {led.Number} will be {(shouldBeLit ? "255" : "1")}");
            
            var newState = new LEDState
            {
                Number = led.Number,
                Path = led.Path,
                Brightness = brightness,
                Effect = "manual",
                MultiIntensity = color
            };
            
            states.Add(newState);
        }

        for (var i = 0; i < totalBlueHealth; i++)
        {
            var led = _initialLedStates.FirstOrDefault(state => state.Number == i.ToString());
            if (led == null) continue;
            
            var newState = new LEDState
            {
                Number = led.Number,
                Path = led.Path,
                Brightness = "255",
                Effect = "manual",
                MultiIntensity = "0 255 255" // cyan
            };
            
            states.Add(newState);
        }
    
        LEDState.WriteStates(states);
    }
}