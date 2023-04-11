using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Morphey.Remote
{
  public sealed class GameAnalyticsRemoteClient : BaseRemoteClient
  {
    public override event Action OnFetchComplete;

    public GameAnalyticsRemoteClient(Dictionary<string, object> defaultValues) : base(defaultValues)
    {
    }

    public override void LoadAsync()
    {
      base.LoadAsync();
    }

    protected override void SetDefaultData()
    {
      throw new NotImplementedException();
    }

    protected override void FetchData()
    {
      throw new NotImplementedException();
    }

    protected override bool TryCompleteFetch(Task fetch)
    {
      return base.TryCompleteFetch(fetch);
    }

    protected override void FillData()
    {
      throw new NotImplementedException();
    }
  }
}
