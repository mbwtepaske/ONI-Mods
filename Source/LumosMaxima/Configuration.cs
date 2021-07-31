using System.Collections.Generic;
using System.IO;

using Newtonsoft.Json;

namespace LumosMaxima
{
  public sealed class Configuration
  {
    public static readonly Configuration Default = new Configuration
    {
      LuminosityBase = 900,
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
}