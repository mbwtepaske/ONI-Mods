using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Common
{
  public sealed class Range : IFormattable
  {
    private const char DivideCharacter = '~';
    private static readonly Regex DivideExpression = new Regex($@"\s*{DivideCharacter}\s*", RegexOptions.Compiled);

    public Range() : this(0, 1)
    {
    }

    public Range(float minimum, float maximum)
    {
      Maximum = maximum;
      Minimum = minimum;
    }

    public float Maximum
    {
      get;
      set;
    }

    public float Minimum
    {
      get;
      set;
    }

    /// <inheritdoc />
    public override string ToString() => ToString(null, null);

    /// <inheritdoc />
    public string ToString(string format, IFormatProvider formatProvider)
     => $"{Minimum.ToString(format, formatProvider)} {DivideCharacter} {Maximum.ToString(format, formatProvider)}";

    public static Range Parse(string value)
    {
      var values = DivideExpression.Split(value);

      if (values.Length != 2)
      {
        throw new FormatException($"Expecting two values separated by '{DivideCharacter}'.");
      }

      if (!Single.TryParse(values[0], NumberStyles.Float, null, out var minimum))
      {
        throw new FormatException($"Unable to parse the minimum value '{values[0]}'.");
      }

      if (!Single.TryParse(values[1], NumberStyles.Float, null, out var maximum))
      {
        throw new FormatException($"Unable to parse the maximum value '{values[1]}'.");
      }

      return new Range(minimum, maximum);
    }
  }
}