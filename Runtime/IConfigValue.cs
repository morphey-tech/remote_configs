using System.Collections.Generic;

namespace Morphey.Remote
{
  public interface IConfigValue
  {
    bool BooleanValue { get; }
    string StringValue { get; }
    IEnumerable<byte> ByteArrayValue { get; }
    double DoubleValue { get; }
    long LongValue { get; }
  }
}
