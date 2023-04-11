using System.Collections.Generic;

namespace Morphey.Remote
{
  public interface IRemoteDataListener
  {
    IEnumerable<string> Keys { get; }
    void Subscribe();
    void ApplyRequestResult(RemoteDataResult result);
  }
}
