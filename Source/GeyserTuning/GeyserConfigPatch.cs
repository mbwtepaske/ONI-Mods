using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;

using static System.IO.Path;

using Common;

using Harmony;

namespace GeyserTuning
{
  [HarmonyPatch(typeof(GeyserGenericConfig), "GenerateConfigs")]
  public static class GeyserConfigPatch
  {
    private const string Separator = ",";

    public static void Postfix(List<GeyserGenericConfig.GeyserPrefabParams> __result)
    {
      try
      {
        var configurationFilePath = Combine(GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Geysers.csv");

        if (File.Exists(configurationFilePath))
        {
          var lines = File.ReadAllLines(configurationFilePath);
          var types = new Dictionary<string, GeyserTypeConfig>();

          for (var index = 1; index < lines.Length; index++)
          {
            var values = lines[index].Split(new[] { Separator }, StringSplitOptions.RemoveEmptyEntries);
            var identity = values[0];

            types[identity] = new GeyserTypeConfig
            {
              Identity                  = identity,
              Element                   = (SimHashes) Enum.Parse(typeof(SimHashes), values[1]),
              Temperature               = Single.Parse(values[2]),
              Pressure                  = Single.Parse(values[3]),
              RateMultiplier            = Single.Parse(values[4]),
              RatePerCycle              = new Range(Single.Parse(values[05]), Single.Parse(values[06])),
              ActivePeriod              = new Range(Single.Parse(values[07]), Single.Parse(values[08])),
              ActivePeriodPercentage    = new Range(Single.Parse(values[09]), Single.Parse(values[10])),
              EruptionPeriod            = new Range(Single.Parse(values[11]), Single.Parse(values[12])),
              EruptionPeriodPercentage  = new Range(Single.Parse(values[13]), Single.Parse(values[13]))
            };
          }

          foreach (var parameter in __result)
          {
            var geyserType = parameter.geyserType;

            if (types.TryGetValue(geyserType.id, out var geyserTypeConfig))
            {
              geyserType.element = geyserTypeConfig.Element;
              geyserType.temperature = geyserTypeConfig.Temperature;
              geyserType.maxPressure = geyserTypeConfig.Pressure;

              geyserType.minRatePerCycle = geyserTypeConfig.RatePerCycle.Minimum;
              geyserType.maxRatePerCycle = geyserTypeConfig.RatePerCycle.Maximum;

              geyserType.minRatePerCycle *= geyserTypeConfig.RateMultiplier;
              geyserType.maxRatePerCycle *= geyserTypeConfig.RateMultiplier;

              geyserType.minIterationLength = geyserTypeConfig.EruptionPeriod.Minimum * Constants.SECONDS_PER_CYCLE;
              geyserType.maxIterationLength = geyserTypeConfig.EruptionPeriod.Maximum * Constants.SECONDS_PER_CYCLE;
              geyserType.minIterationPercent = geyserTypeConfig.EruptionPeriodPercentage.Minimum / 100F;
              geyserType.maxIterationPercent = geyserTypeConfig.EruptionPeriodPercentage.Maximum / 100F;

              geyserType.minYearLength = geyserTypeConfig.ActivePeriod.Minimum * Constants.SECONDS_PER_CYCLE;
              geyserType.maxYearLength = geyserTypeConfig.ActivePeriod.Maximum * Constants.SECONDS_PER_CYCLE;

              geyserType.minYearPercent = geyserTypeConfig.ActivePeriodPercentage.Minimum / 100F;
              geyserType.maxYearPercent = geyserTypeConfig.ActivePeriodPercentage.Maximum / 100F;
            }
          }
        }
        else
        {
          using (var writer = new StreamWriter(File.Open(configurationFilePath, FileMode.Create, FileAccess.ReadWrite, FileShare.Read)))
          {
            writer.WriteLine(String.Join(Separator
              , nameof(GeyserTypeConfig.Identity)
              , nameof(GeyserTypeConfig.Element)
              , nameof(GeyserTypeConfig.Temperature)
              , nameof(GeyserTypeConfig.Pressure)
              , nameof(GeyserTypeConfig.RateMultiplier)
              , nameof(GeyserTypeConfig.RatePerCycle) + " (Min)"
              , nameof(GeyserTypeConfig.RatePerCycle) + " (Max)"
              , nameof(GeyserTypeConfig.ActivePeriod) + " (Min)"
              , nameof(GeyserTypeConfig.ActivePeriod) + " (Max)"
              , nameof(GeyserTypeConfig.ActivePeriodPercentage) + " (Min %)"
              , nameof(GeyserTypeConfig.ActivePeriodPercentage) + " (Max %)"
              , nameof(GeyserTypeConfig.EruptionPeriod) + " (Min)"
              , nameof(GeyserTypeConfig.EruptionPeriod) + " (Max)"
              , nameof(GeyserTypeConfig.EruptionPeriodPercentage) + " (Min %)"
              , nameof(GeyserTypeConfig.EruptionPeriodPercentage) + " (Max %)"
              ));

            foreach (var parameter in __result)
            {
              var geyserType = parameter.geyserType;

              writer.WriteLine(String.Join(Separator
                , geyserType.id
                , geyserType.element.ToString()
                , geyserType.temperature.ToString(CultureInfo.CurrentCulture)
                , geyserType.maxPressure.ToString(CultureInfo.CurrentCulture)
                , 1F.ToString(CultureInfo.CurrentCulture)
                , geyserType.minRatePerCycle.ToString(null, CultureInfo.CurrentCulture)
                , geyserType.maxRatePerCycle.ToString(null, CultureInfo.CurrentCulture)
                , (geyserType.minYearLength / Constants.SECONDS_PER_CYCLE).ToString(null, CultureInfo.CurrentCulture)
                , (geyserType.maxYearLength / Constants.SECONDS_PER_CYCLE).ToString(null, CultureInfo.CurrentCulture)
                , (geyserType.minYearPercent * 100).ToString(null, CultureInfo.CurrentCulture)
                , (geyserType.maxYearPercent * 100).ToString(null, CultureInfo.CurrentCulture)
                , (geyserType.minIterationLength / Constants.SECONDS_PER_CYCLE).ToString(null, CultureInfo.CurrentCulture)
                , (geyserType.maxIterationLength / Constants.SECONDS_PER_CYCLE).ToString(null, CultureInfo.CurrentCulture)
                , (geyserType.minIterationPercent * 100).ToString(null, CultureInfo.CurrentCulture)
                , (geyserType.maxIterationPercent * 100).ToString(null, CultureInfo.CurrentCulture)
              ));
            }
          }
        }
      }
      catch (Exception exception)
      {
        using (var logger = new StreamWriter(Combine(GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Exception.log")))
        {
          logger.WriteLine(exception.ToString());
        }
      }
    }
  }
}