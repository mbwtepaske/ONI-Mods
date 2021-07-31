﻿using System;
using System.Reflection;

namespace ConfigurablePipes
{
  internal static class ReadPrivate
  {
    /// <summary>
    /// Uses reflection to get the field value from an object.
    /// </summary>
    ///
    /// <param name="type">The instance type.</param>
    /// <param name="instance">The instance object.</param>
    /// <param name="fieldName">The field's name which is to be fetched.</param>
    ///
    /// <returns>The field value from the object.</returns>
    internal static object Get(Type type, object instance, string fieldName)
    {
      var bindFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static;
      var field = type.GetField(fieldName, bindFlags);

      return field.GetValue(instance);
    }

    /// usage:
    /// Make an extension function to act as a get function for the data in question.
    /// Example
    ///         public static Building GetBuilding(this BuildingCellVisualizer __instance)
    ///         {
    ///             return NightLib.ReadPrivate.GetInstanceField(typeof(BuildingCellVisualizer), __instance, "building") as Building;
    ///         }
    /// This will allow an instance of BuildingCellVisualizer to use .GetBuilding() to get the contents of the private member variable building
    internal static void Set(Type type, object instance, string fieldName, object value)
    {
      var bindFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static;
      var field = type.GetField(fieldName, bindFlags);

      field.SetValue(instance, value);
    }

    internal static object Call(object o, string methodName, params object[] args) => o.GetType().GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Instance)?.Invoke(o, args);

    //
    // Below this line are specific examples of ref arguments
    // Ref arguments can't be added generic and they have to have a specific function for each.
    // If added as generic as possible, maybe they can be reused at some point.
    //

    internal static object Call<T>(object o, string methodName, object a, object b, object c, ref T d) where T : class
    {
      var mi = o.GetType().GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Instance);
      if (mi != null)
      {
        var    varlist = new object[] { a, b, c, d };
        var result  = mi.Invoke(o, varlist);
        d = varlist[3] as T;
        return result;
      }
      return null;
    }

    private static FieldInfo GetFieldInfo(Type type, string fieldName)
    {
      FieldInfo fieldInfo;

      do
      {
        fieldInfo = type.GetField(fieldName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        type = type.BaseType;
      }
      while (fieldInfo == null && type != null);

      return fieldInfo;
    }

    public static void SetFieldValue(this object obj, string fieldName, object val)
    {
      if (obj != null)
      {
        var objType = obj.GetType();
        var fieldInfo = GetFieldInfo(objType, fieldName);
        if (fieldInfo != null)
        {
          fieldInfo.SetValue(obj, val);
        }
        else
        {
          throw new ArgumentOutOfRangeException("fieldName", string.Format("Couldn't find field {0} in type {1}", fieldName, objType.FullName));
        }
      }
      else
      {
        throw new ArgumentNullException("obj");
      }
    }
  }
}