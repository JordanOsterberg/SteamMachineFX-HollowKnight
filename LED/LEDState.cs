using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Serialization;

namespace SteamMachineFX_HollowKnight.LED;

#nullable enable

public sealed class LEDState
{
    public string Number { get; set; } = "";
    public string Path { get; set; } = "";
    public string Brightness { get; set; } = "";
    public string MultiIntensity { get; set; } = "";
    public string Effect { get; set; } = "";
        
    // MARK: - State Reading
        
    public static List<LEDState> SaveState()
    {
        if (!SteamMachineFX.CanControlLEDs) return new(); // do nothing if we can't control.
        
        return Directory.GetDirectories("/sys/class/leds", "valve-leds*")
            .Select(StateFromPath)
            .Where(state => state != null)
            .Select(state => state!)
            .OrderBy(state => int.Parse(state.Number))
            .Reverse()
            .ToList();
    }
    
    private static readonly Regex LEDNumberingRegex =
        new Regex(@"valve-leds\[(\d+)\]$");
    
    private static LEDState? StateFromPath(string path)
    {
        var name = System.IO.Path.GetFileName(path);
        var match = LEDNumberingRegex.Match(name);

        if (!match.Success)
        {
            SteamMachineFX.Logger.LogWarning($"Could not parse LED number from {name}");
            return null;
        }

        var number = int.Parse(match.Groups[1].Value);
                    
        return new LEDState
        {
            Number = number.ToString(),
            Path = path,
            Brightness = File.ReadAllText(System.IO.Path.Combine(path, "brightness")),
            MultiIntensity = File.ReadAllText(System.IO.Path.Combine(path, "multi_intensity")),
            Effect = File.ReadAllText(System.IO.Path.Combine(path, "effect"))
        };
    }
        
    // MARK: - State Writing
        
    public static void WriteStates(IEnumerable<LEDState> states)
    {
        if (!SteamMachineFX.CanControlLEDs) return; // do nothing if we can't control.
        
        var settings = new Dictionary<string, LEDSettings>();
            
        foreach (var led in states)
        {
            settings.Add(led.Number, new LEDSettings
            {
                Brightness = int.Parse(led.Brightness),
                Color = led.MultiIntensity.Split(' ').Select(int.Parse).ToArray(),
                Effect = led.Effect,
            });
        }
        
        WriteConfigToFile(new LEDConfig
        {
            Enabled = true,
            Leds = settings
        });
    }
    
    // MARK: - LED Serialization
    
    private static readonly JsonSerializerSettings _options = new()
    {
        Formatting = Formatting.Indented,
        ContractResolver = new CamelCasePropertyNamesContractResolver()
    };

    public static void DisableLEDMonitor()
    {
        if (!SteamMachineFX.CanControlLEDs) return; // do nothing if we can't control.
        
        WriteConfigToFile(new LEDConfig
        {
            Enabled = false,
            Leds = new Dictionary<string, LEDSettings>()
        });
    }

    private static void WriteConfigToFile(LEDConfig config)
    {
        try
        {
            const string filePath = "/home/deck/steam-machine-fx-broker/leds.json";

            var json = JsonConvert.SerializeObject(config, _options);
            File.WriteAllText(filePath, json);
        }
        catch (Exception e)
        {
            SteamMachineFX.Logger.LogError("Failed to write updated leds.json with exception");
            SteamMachineFX.Logger.LogError(e.ToString());
        }
    }
}