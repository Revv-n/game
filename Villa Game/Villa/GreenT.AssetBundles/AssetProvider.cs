using GreenT.HornyScapes;
using StripClub.Model.Shop.Data;
using UnityEngine;

namespace GreenT.AssetBundles;

public class AssetProvider
{
	private readonly FakeAssetService _fakeAssetService;

	private readonly BundlesProviderBase _bundlesProvider;

	public AssetProvider(BundlesProviderBase bundlesProvider, FakeAssetService fakeAssetService)
	{
		_bundlesProvider = bundlesProvider;
		_fakeAssetService = fakeAssetService;
	}

	public Sprite FindBundleImageOrFake(ContentSource contentSource, string bundleName, AssetResolveType resolveType = (AssetResolveType)0)
	{
		Sprite sprite = _bundlesProvider.TryFindInConcreteBundle<Sprite>(contentSource, bundleName, (resolveType | AssetResolveType.Silent) == resolveType);
		if (sprite == null)
		{
			if ((resolveType | AssetResolveType.Fake) != resolveType)
			{
				return null;
			}
			return _fakeAssetService.HandleFakeBySource(contentSource, bundleName);
		}
		return sprite;
	}

	public TextAsset FindBundleAssetOrFake(ContentSource contentSource, string bundleName, AssetResolveType resolveType = (AssetResolveType)0)
	{
		return _bundlesProvider.TryFindInConcreteBundle<TextAsset>(contentSource, bundleName, (resolveType | AssetResolveType.Silent) == resolveType);
	}
}
