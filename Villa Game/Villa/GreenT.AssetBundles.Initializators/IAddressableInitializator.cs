using System.Collections;

namespace GreenT.AssetBundles.Initializators;

public interface IAddressableInitializator
{
	IEnumerator Initialize(string version);
}
