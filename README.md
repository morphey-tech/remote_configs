# Remote

## Example

```
BaseRemoteClient client = new FirebaseRemoteClient(new Dictionary<string, object>
{
  { "FirstParam", 60 },
  { "SecondParam", true }
});

RemoteDataProvider provider = new RemoteDataProvider(client);
provider.LoadClientAsync().Forget();

or...

await provider.LoadClientAsync();
```
