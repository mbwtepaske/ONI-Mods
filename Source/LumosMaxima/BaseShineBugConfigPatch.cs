using System;
using System.IO;
using System.Reflection;

using HarmonyLib;

using UnityEngine;

namespace LumosMaxima
{
  [HarmonyPatch(typeof(BaseLightBugConfig), nameof(BaseLightBugConfig.BaseLightBug))]
  public static class BaseLightBugConfigPatch
  {
    private static Configuration Configuration;

    [HarmonyPrepare]
    public static void LoadConfiguration()
    {
      var filePath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "LumosMaxima.json");
      
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
        Console.Out.WriteLine(exception.ToString());
      }

      if (Configuration == null)
      {
        Debug.Log($"{filePath} did not exist or could not be loaded, using default configuration instead.");

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
