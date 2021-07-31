using UnityEngine;

namespace ConfigurablePipes
{
  internal abstract class DisplayConduitPortInfo
  {
    readonly internal ConduitType type;
    readonly internal CellOffset  offset;
    readonly internal CellOffset  offsetFlipped;
    readonly internal bool        input;
    readonly internal Color       color;

    protected DisplayConduitPortInfo(ConduitType type, CellOffset offset, CellOffset? offsetFlipped, bool input, Color? color)
    {
      this.type   = type;
      this.offset = offset;
      this.input  = input;

      this.offsetFlipped = offsetFlipped ?? offset;

      // assign port colors
      if (color != null)
      {
        this.color = color ?? Color.white;
      }
      else
      {
        // none given. Use defaults
        var resources = BuildingCellVisualizerResources.Instance();
        var ioColors  = type == ConduitType.Gas ? resources.gasIOColours : resources.liquidIOColours;
        var colorSet  = input ? ioColors.input : ioColors.output;

        this.color = colorSet.connected;
      }
    }
  }
}