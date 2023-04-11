using System.Collections.Generic;

namespace Morphey.Remote
{
#if REMOTE_FIREBASE
  public sealed class FirebaseConfigValue : IConfigValue
  {
    private readonly Firebase.RemoteConfig.ConfigValue _value;

    public FirebaseConfigValue(Firebase.RemoteConfig.ConfigValue value)
    {
      _value = value;
    }

    public bool BooleanValue => _value.BooleanValue;
    public string StringValue => _value.StringValue;
    public IEnumerable<byte> ByteArrayValue => _value.ByteArrayValue;
    public double DoubleValue => _value.DoubleValue;
    public long LongValue => _value.LongValue;
  }
#endif
}
