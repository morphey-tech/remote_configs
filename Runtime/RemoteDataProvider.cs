using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Morphey.Remote
{
  public sealed class RemoteDataProvider
  {
    private readonly BaseRemoteClient _client;
    private readonly List<IRemoteDataListener> _listeners;
    private readonly List<string> _cachedKeys;

    public RemoteDataProvider(BaseRemoteClient client)
    {
      _client = client;
      _client.OnFetchComplete += () => TrySolveRequestsByConfigs();
      _listeners = new List<IRemoteDataListener>(25);
      _cachedKeys = new List<string>(10);
    }

    public async void LoadClientAsync()
    {
      await Task.Run(_client.LoadAsync);
    }

    public bool TrySubscribe(IRemoteDataListener listener)
    {
      if (_listeners.Contains(listener))
        return false;

      _listeners.Add(listener);

      return _client.IsLoaded
        ? TrySolveRequestsByConfigs()
        : TrySolveRequestsByDefault();
    }

    private bool TrySolveRequestsByConfigs()
    {
      IReadOnlyDictionary<string, IConfigValue> configs = _client.GetConfigs();

      int listenersCount = _listeners.Count;
      for (int i = listenersCount - 1; i >= 0; i--)
      {
        IRemoteDataListener listener = _listeners[i];

        _cachedKeys.Clear();
        _cachedKeys.AddRange(configs.Keys.Intersect(listener.Keys));

        var request = new RemoteDataResult
        {
          Content = new Dictionary<string, IConfigValue>(_cachedKeys.Count)
        };

        foreach (string key in _cachedKeys)
        {
          request.Content.Add(key, configs[key]);
          Console.WriteLine($"Trying resolve request with Key - {key}, Value - {configs[key].StringValue}");
        }

        listener.ApplyRequestResult(request);
        _listeners.RemoveAt(i);
      }

      return true;
    }

    private bool TrySolveRequestsByDefault()
    {
      IReadOnlyDictionary<string, object> defaultValues = _client.GetDefault();

      int listenersCount = _listeners.Count;
      for (int i = listenersCount - 1; i >= 0; i--)
      {
        IRemoteDataListener listener = _listeners[i];

        _cachedKeys.Clear();
        _cachedKeys.AddRange(defaultValues.Keys.Intersect(listener.Keys));

        var request = new RemoteDataResult
        {
          Content = new Dictionary<string, IConfigValue>(_cachedKeys.Count)
        };

        foreach (string key in _cachedKeys)
        {
          request.Content.Add(key, new RemoteConfigValue(defaultValues[key]));
          Console.WriteLine($"Trying resolve request with Key - {key}, Value - {defaultValues[key]}");
        }

        listener.ApplyRequestResult(request);
      }

      return true;
    }
  }
}
