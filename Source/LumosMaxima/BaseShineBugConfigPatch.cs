using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

using Harmony;

using Newtonsoft.Json;

using UnityEngine;

namespace LumosMaxima
{
  public sealed class Configuration
  {
    public static readonly Configuration Default = new Configuration
    {
      LuminosityBase = 1200,
      LuminosityStep = 300,
      Tiers = new Dictionary<string, int>
      {
        [LightBugBabyConfig.ID]        = 0,
        [LightBugConfig.ID]            = 1,
        [LightBugOrangeBabyConfig.ID]  = 1,
        [LightBugOrangeConfig.ID]      = 2,
        [LightBugPurpleBabyConfig.ID]  = 2,
        [LightBugPurpleConfig.ID]      = 3,
        [LightBugPinkBabyConfig.ID]    = 3,
        [LightBugPinkConfig.ID]        = 4,
        [LightBugBlueBabyConfig.ID]    = 4,
        [LightBugBlueConfig.ID]        = 5,
        [LightBugCrystalBabyConfig.ID] = 6,
        [LightBugCrystalConfig.ID]     = 7,
      }
    };

    public int LuminosityBase
    {
      get;
      set;
    }

    public int LuminosityStep
    {
      get;
      set;
    }

    public Dictionary<string, int> Tiers
    {
      get;
      set;
    }

    public static Configuration LoadConfiguration(Stream stream) => JsonSerializer.CreateDefault().Deserialize<Configuration>(new JsonTextReader(new StreamReader(stream)));
  }

  [HarmonyPatch(typeof(BaseLightBugConfig), nameof(BaseLightBugConfig.BaseLightBug))]
  public static class BaseLightBugConfigPatch
  {
    private static Configuration Configuration;

    [HarmonyPrepare]
    public static void LoadConfiguration()
    {
      var filePath = Path.Combine(Path.GetDirectoryName(Assembly.GetCallingAssembly().Location), "LumosMaxima.json");

      try
      {
        if (File.Exists(filePath))
        {
          using (var fileStream = File.OpenRead(filePath))
          {
            Configuration = Configuration.LoadConfiguration(fileStream);
          }
        }
      }
      catch (Exception exception)
      {
        Console.Out.Write(exception.ToString());
      }

      if (Configuration == null)
      {
        Configuration = Configuration.Default;
      }
    }

    [HarmonyPostfix]
    public static void Modify(string id, ref GameObject __result)
    {
      var light = __result.GetComponent<Light2D>();

      if (light != null && Configuration.Tiers.TryGetValue(id, out var tier))
      {
        light.Lux = Configuration.LuminosityBase + Configuration.LuminosityStep * tier;
      }
    }
  }
}
