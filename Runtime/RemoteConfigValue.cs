using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Morphey.Remote
{
  public readonly struct RemoteConfigValue : IConfigValue

  {
  private readonly object _data;
  private static readonly Regex _booleanTruePattern = new Regex("^(1|true|t|yes|y|on)$", RegexOptions.IgnoreCase);
  private static readonly Regex _booleanFalsePattern = new Regex("^(0|false|f|no|n|off|)$", RegexOptions.IgnoreCase);

  public RemoteConfigValue([NotNull] object data) : this()
  {
    _data = data ??
            throw new AggregateException($"Argument {nameof(data)} is NULL! Pass it in constructor.");
  }

  public bool BooleanValue
  {
    get
    {
      string stringValue = StringValue;

      if (_booleanTruePattern.IsMatch(stringValue) == false && _booleanFalsePattern.IsMatch(stringValue) == false)
        throw new FormatException($"RemoteDataValue \"{stringValue}\" is not a boolean value.");

      return _booleanTruePattern.IsMatch(stringValue);
    }
  }

  public string StringValue => Convert.ToString(_data);
  public IEnumerable<byte> ByteArrayValue => (IEnumerable<byte>)_data;
  public double DoubleValue => Convert.ToDouble(StringValue, CultureInfo.InvariantCulture);
  public long LongValue => Convert.ToInt64(StringValue, CultureInfo.InvariantCulture);
  }
}