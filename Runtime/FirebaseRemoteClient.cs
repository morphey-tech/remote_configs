using System;
using System.Collections.Generic;
using System.Threading.Tasks;

#if REMOTE_FIREBASE
using Firebase;
using Firebase.RemoteConfig;
using Firebase.Extensions;
#endif

namespace Morphey.Remote
{
  public sealed class FirebaseRemoteClient : BaseRemoteClient
  {
    public override event Action OnFetchComplete;

    private bool _dependencyIsFixed;

#if REMOTE_FIREBASE
    private static FirebaseRemoteConfig Instance => FirebaseRemoteConfig.GetInstance(FirebaseApp.DefaultInstance);
#endif

    private const ulong CACHE_EXPIRATION_IN_MILLISECONDS = 30000;

    public FirebaseRemoteClient(Dictionary<string, object> defaults) : base(defaults)
    {
#if REMOTE_FIREBASE
      FirebaseApp.CheckAndFixDependenciesAsync()
        .ContinueWith(task => _dependencyIsFixed = task.Result == DependencyStatus.Available);
#endif
    }

    public override async void LoadAsync()
    {
#if REMOTE_FIREBASE
      while (_dependencyIsFixed == false)
        await Task.Delay(100);

      await Task.Run(() =>
      {
        SetDefaultData();
        FetchData();
      });
#endif
    }

    protected override void SetDefaultData()
    {
#if REMOTE_FIREBASE
      Instance.SetDefaultsAsync(Default)
        .ContinueWithOnMainThread(_ =>
        {
          
        });
#endif
    }

    protected override void FetchData()
    {
#if REMOTE_FIREBASE
      Instance.FetchAsync(TimeSpan.FromMilliseconds(CACHE_EXPIRATION_IN_MILLISECONDS)).
        ContinueWith(_ => Instance.ActivateAsync().ContinueWith(TryCompleteFetch));
#endif
    }

    protected override bool TryCompleteFetch(Task fetch)
    {
      if (fetch.IsCanceled)
        Console.WriteLine("Fetch remote data is canceled.");

      if (fetch.IsCompleted)
        Console.WriteLine("Fetch remote data is successfully");

      if (fetch.IsFaulted)
      {
        Console.WriteLine("Fetch remote data is failed");

        if (fetch.Exception != null)
          foreach (var exception in fetch.Exception.InnerExceptions)
            Console.WriteLine(exception);
      }

#if REMOTE_FIREBASE
      ConfigInfo info = Instance.Info;

      switch (info.LastFetchStatus)
      {
        case LastFetchStatus.Success:
          Instance.ActivateAsync()
            .ContinueWithOnMainThread(_ =>
            {
              FillData();
              OnFetchComplete?.Invoke();
              Console.WriteLine($"Remote data loaded and ready (last fetch time {info.FetchTime.ToUniversalTime()}).");
            });
          break;
        case LastFetchStatus.Failure:
          switch (info.LastFetchFailureReason)
          {
            case FetchFailureReason.Error:
              Console.WriteLine("Fetch remote data failed for unknown reason");
              break;
            case FetchFailureReason.Throttled:
              Console.WriteLine($"Fetch remote data throttled until {info.ThrottledEndTime}");
              break;
            case FetchFailureReason.Invalid:
              break;
          }
          break;
        case LastFetchStatus.Pending:
          Console.WriteLine("Latest fetch remote data call still pending.");
          break;
      }
#endif

      return fetch.IsCompleted;
    }

    protected override void FillData()
    {
#if REMOTE_FIREBASE
      Configs.Clear();

      foreach (KeyValuePair<string, ConfigValue> value in Instance.AllValues)
        Configs.Add(value.Key, new FirebaseConfigValue(value.Value));
#endif
    }
  }
}
