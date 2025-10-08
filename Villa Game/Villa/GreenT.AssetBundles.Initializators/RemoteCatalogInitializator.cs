using System.Collections;
using UnityEngine.AddressableAssets;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace GreenT.AssetBundles.Initializators;

public class RemoteCatalogInitializator : IAddressableInitializator
{
	public static string CatalogAdditionName = "_horny_villa";

	public virtual IEnumerator Initialize(string currentVersion)
	{
		string catalogPath = AddressablesService.DownloadURL + "/catalog" + CatalogAdditionName + ".json?ver=" + currentVersion;
		AsyncOperationHandle<IResourceLocator> handle = Addressables.LoadContentCatalogAsync(catalogPath);
		while (!handle.Task.IsCompleted)
		{
			yield return null;
		}
		Addressables.Release(handle);
	}
}
