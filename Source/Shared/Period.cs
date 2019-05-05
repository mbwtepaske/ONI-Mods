using System;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace Common
{
  [DebuggerDisplay("{Length.Minimum} ~ {Length.Maximum} Cycles {DivideCharacter} {Percentage.Minimum:P0} ~ {Percentage.Maximum:P0}")]
  public sealed class Period : IFormattable
  {
    private const char DivideCharacter = '|';
    private static readonly Regex DivideExpression = new Regex($@"\s*{DivideCharacter}\s*", RegexOptions.Compiled);

    public Period() : this(null, null)
    {
    }

    public Period(Range length, Range percentage)
    {
      Length = length;
      Percentage = percentage;
    }

    public Range Length
    {
      get;
      set;
    }

    public Range Percentage
    {
      get;
      set;
    }

    public override string ToString() => ToString(null, null);

    /// <inheritdoc />
    public string ToString(string format, IFormatProvider formatProvider) => $"{Length.ToString(format, formatProvider)} {DivideCharacter} {Percentage.Minimum:P0}-{Percentage.Maximum:P0}";

    public static Period Parse(string value)
    {
      var values = DivideExpression.Split(value);

      return values.Length != 2 ? throw new FormatException($"Expecting two range-values separated by '{DivideCharacter}'.") : new Period(Range.Parse(values[0]), Range.Parse(values[1]));
    }
  }
}