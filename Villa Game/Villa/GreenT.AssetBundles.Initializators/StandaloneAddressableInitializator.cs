using System.Collections;
using UnityEngine.AddressableAssets;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace GreenT.AssetBundles.Initializators;

public class StandaloneAddressableInitializator : IAddressableInitializator
{
	public IEnumerator Initialize(string currentVersion)
	{
		AsyncOperationHandle<IResourceLocator> handle = Addressables.InitializeAsync(autoReleaseHandle: false);
		while (!handle.Task.IsCompleted)
		{
			yield return null;
		}
		Addressables.Release(handle);
	}
}
