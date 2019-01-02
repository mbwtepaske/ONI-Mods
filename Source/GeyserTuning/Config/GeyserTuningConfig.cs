using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

using Newtonsoft;
using Newtonsoft.Json;

using Common;

namespace GeyserTuning.Config
{
  public sealed class GeyserTuningConfig
  {
    public static GeyserTuningConfig Create()
    {
      var result = new GeyserTuningConfig();
      var configurationFilePath = Path.Combine(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Config"), "Geysers.json");

      if (File.Exists(configurationFilePath))
      {
        using (var reader = new JsonTextReader(File.OpenText(configurationFilePath)))
        {
          var serializer = JsonSerializer.CreateDefault();

          while (reader.Read())
          {
            switch (reader.TokenType)
            {
              case JsonToken.PropertyName when reader.Value.Equals(nameof(Types)) && reader.Read() && reader.TokenType == JsonToken.StartArray:
              {
                var depth = reader.Depth;

                while (reader.Read() && reader.Depth > depth)
                {
                  var geyserTypeConfig = serializer.Deserialize<GeyserTypeConfig>(reader);
                  
                  result.Types[geyserTypeConfig.Identity] = geyserTypeConfig;
                }

                break;
              }
            }
          }
        }
      }

      return result;
    }

    /// <summary>
    /// Gets the <see cref="GeyserTypeConfig"/> by id.
    /// </summary>
    public Dictionary<string, GeyserTypeConfig> Types
    {
      get;
    } = new Dictionary<string, GeyserTypeConfig>();
  }
}
