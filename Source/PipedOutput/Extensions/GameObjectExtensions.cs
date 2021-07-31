namespace UnityEngine
{
  internal static class GameObjectExtensions
  {
    internal static bool IsPreview(this GameObject gameObject)
    {
      var name = gameObject.PrefabID().Name;

      return name.Substring(name.Length - 7) == "Preview";
    }
  }
}