using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace Morphey.Remote
{
  public abstract class BaseRemoteClient
  {
    public abstract event Action OnFetchComplete;

    public bool IsLoaded => Configs is { Count: > 0 };

    protected readonly Dictionary<string, IConfigValue> Configs = new();
    protected readonly Dictionary<string, object> Default;

    protected BaseRemoteClient([NotNull] Dictionary<string, object> defaultValues)
    {
      Default = defaultValues ??
                throw new ArgumentException(
                  $"Argument {nameof(defaultValues)} is NULL! Pass it in constructor.");
    }

    public IReadOnlyDictionary<string, IConfigValue> GetConfigs()
    {
      return Configs;
    }

    public IReadOnlyDictionary<string, object> GetDefault()
    {
      return Default;
    }

    public virtual async void LoadAsync()
    {
    }

    protected abstract void SetDefaultData();
    protected abstract void FetchData();
    protected abstract void FillData();

    protected virtual bool TryCompleteFetch(Task fetch)
    {
      if (fetch.IsCanceled)
        return false;

      if (fetch.IsCompleted)
        return fetch.IsCompleted;

      if (fetch.Exception != null)
        foreach (var exception in fetch.Exception.InnerExceptions)
          Console.WriteLine(exception);

      return fetch.IsFaulted;
    }
  }
}
