namespace ConfigurablePipes
{
  [SkipSaveFileSerialization]
  internal class KAminControllerResize : KMonoBehaviour
  {
    public float width  = 1f;
    public float height = 1f;

    [MyCmpGet]
    private readonly KBatchedAnimController _controller;

    protected override void OnSpawn()
    {
      base.OnSpawn();

      if (_controller != null)
      {
        if (this.width != 1f)
        {
          _controller.animWidth = this.width;
        }
        if (this.height != 1f)
        {
          _controller.animHeight = this.height;
        }
      }
    }
  }
}