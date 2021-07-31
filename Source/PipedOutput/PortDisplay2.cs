﻿using UnityEngine;

namespace ConfigurablePipes
{
  [SkipSaveFileSerialization]
  internal class PortDisplay2 : KMonoBehaviour
  {
    #region Fields

    [SerializeField]
    internal Color32 color;

    [SerializeField]
    internal bool input;

    [SerializeField]
    internal CellOffset offset;

    [SerializeField]
    internal CellOffset offsetFlipped;

    [SerializeField]
    internal Sprite sprite;

    [SerializeField]
    internal ConduitType type;

    [SerializeField]
    private Color lastColor = Color.black;

    // The cache for last location/color.
    // The default values doesn't matter and will be overwritten on first call.
    // However there is a theoredical risk that no default value can cause a crash, hence setting them to something.
    [SerializeField]
    private int lastUtilityCell = -1;

    private GameObject portObject;

    #endregion

    #region Methods

    internal void AssignPort(DisplayConduitPortInfo port)
    {
      this.type          = port.type;
      this.offset        = port.offset;
      this.offsetFlipped = port.offsetFlipped;
      this.input         = port.input;
      this.color         = port.color;
      this.sprite        = GetSprite();
    }

    internal void DisableIcons()
    {
      if (this.portObject != null)
      {
        if (this.portObject != null && this.portObject.activeInHierarchy)
        {
          this.portObject.SetActive(false);
        }
      }
    }

    internal void Draw(GameObject obj, BuildingCellVisualizer visualizer, bool force)
    {
      var building    = visualizer.GetBuilding();
      var      utilityCell = building.GetCellWithOffset(building.Orientation == Orientation.Neutral ? this.offset : this.offsetFlipped);

      // redraw if anything changed
      if (force || utilityCell != this.lastUtilityCell || color != this.lastColor)
      {
        this.lastColor       = color;
        this.lastUtilityCell = utilityCell;
        visualizer.DrawUtilityIcon(utilityCell, this.sprite, ref portObject, color, Color.white);
      }
    }

    protected override void OnCleanUp()
    {
      base.OnCleanUp();
      if (this.portObject != null)
      {
        Destroy(this.portObject);
      }
    }

    private Sprite GetSprite()
    {
      var resources = BuildingCellVisualizerResources.Instance();
      if (input)
      {
        if (this.type == ConduitType.Gas)
        {
          return resources.gasInputIcon;
        }
        else if (this.type == ConduitType.Liquid || this.type == ConduitType.Solid)
        {
          return resources.liquidInputIcon;
        }
      }
      else
      {
        if (this.type == ConduitType.Gas)
        {
          return resources.gasOutputIcon;
        }
        else if (this.type == ConduitType.Liquid || this.type == ConduitType.Solid)
        {
          return resources.liquidOutputIcon;
        }
      }

      return null;
    }

    #endregion
  }
}