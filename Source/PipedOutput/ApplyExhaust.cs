﻿using UnityEngine;

namespace ConfigurablePipes
{
  public static class ApplyExhaust
  {
    internal static PortDisplayOutput AddOutput(GameObject go, CellOffset offset, SimHashes elementHash)
    {
      var element = ElementLoader.GetElement(elementHash.CreateTag());
      var conduitType = element.IsGas ? ConduitType.Gas : element.IsLiquid ? ConduitType.Liquid : ConduitType.Solid;

      // port color
      var color = element.substance.conduitColour;
      color.a = 255; // for some reason the alpha channel is set to invisible for some elements (hydrogen only?)

      if (color.r == 0 && color.g == 0 && color.b == 0)
      {
        // avoid completely black icons since the background is black
        color.r = 25;
        color.g = 25;
        color.b = 25;
      }

      var outputPort = new PortDisplayOutput(conduitType, offset, null, color);

      var controller = go.AddOrGet<PortDisplayController>();
      controller.Init(go);

      controller.AssignPort(go, outputPort);

      var converter = go.GetComponent<ElementConverter>();
      if (converter != null)
      {
        for (var i = 0; i < converter.outputElements.Length; ++i)
        {
          var item = converter.outputElements[i];
          if (item.elementHash == elementHash)
          {
            converter.outputElements[i].storeOutput = true;

            PipedDispenser dispenser = go.AddComponent<PipedDispenser>();
            dispenser.elementFilter = new SimHashes[] { elementHash };
            dispenser.AssignPort(outputPort);
            dispenser.alwaysDispense = true;
            dispenser.SkipSetOperational = true;

            PipedOptionalExhaust exhaust = go.AddComponent<PipedOptionalExhaust>();
            exhaust.dispenser = dispenser;
            exhaust.elementHash = elementHash;
            exhaust.elementTag = elementHash.CreateTag();
            exhaust.capacity = item.massGenerationRate * converter.OutputMultiplier * 5;

            break;
          }
        }
      }
      else
      {
        var energyGenerator = go.GetComponent<EnergyGenerator>();
        if (energyGenerator != null)
        {
          for (var i = 0; i < energyGenerator.formula.outputs.Length; ++i)
          {
            var item = energyGenerator.formula.outputs[i];
            if (item.element == elementHash)
            {
              energyGenerator.formula.outputs[i].store = true;

              PipedDispenser dispenser = go.AddComponent<PipedDispenser>();
              dispenser.elementFilter = new SimHashes[] { elementHash };
              dispenser.AssignPort(outputPort);
              dispenser.alwaysDispense = true;
              dispenser.SkipSetOperational = true;

              PipedOptionalExhaust exhaust = go.AddComponent<PipedOptionalExhaust>();
              exhaust.dispenser = dispenser;
              exhaust.elementHash = elementHash;
              exhaust.elementTag = elementHash.CreateTag();
              exhaust.capacity = item.creationRate * 5;

              break;
            }
          }
        }
      }
      return outputPort;
    }
  }
}