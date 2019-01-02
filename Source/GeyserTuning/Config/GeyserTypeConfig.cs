using Common;

namespace GeyserTuning.Config
{
  public sealed class GeyserTypeConfig
  {
    public string Identity
    {
      get;
      set;
    }

    public SimHashes Element
    {
      get;
      set;
    }

    public Range ActivePeriod
    {
      get;
      set;
    }

    public Range ActivePeriodPercentage
    {
      get;
      set;
    }

    public Range EruptionPeriod
    {
      get;
      set;
    }

    public Range EruptionPeriodPercentage
    {
      get;
      set;
    }

    public float Pressure
    {
      get;
      set;
    }

    public float RateMultiplier
    {
      get;
      set;
    }

    public Range RatePerCycle
    {
      get;
      set;
    }

    public float Temperature
    {
      get;
      set;
    }

    public override string ToString() => $"{Identity}: Element: {Element}, Temperature: {Temperature - 273.15F} °C, Rate: {RatePerCycle} / Cycle, Active: {ActivePeriod}, Eruption: {EruptionPeriod}";
  }
}