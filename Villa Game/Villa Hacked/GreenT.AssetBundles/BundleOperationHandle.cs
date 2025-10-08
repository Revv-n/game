using System.Collections.Generic;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace GreenT.AssetBundles;

public struct BundleOperationHandle
{
	public AsyncOperationHandle<IList<AsyncOperationHandle>> handle;

	public BundleOperationHandle(AsyncOperationHandle<IList<AsyncOperationHandle>> handle)
	{
		this.handle = handle;
	}
}
