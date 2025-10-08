using System;
using System.Collections.Generic;
using System.Linq;
using GreenT.AssetBundles;
using GreenT.Settings;
using GreenT.Settings.Data;
using UniRx;
using UnityEngine;

namespace GreenT.HornyScapes.Meta.Data;

public class BackgroundSpritesLoader : AssetsLoaderFromAssetBundle<Sprite>
{
	public BackgroundSpritesLoader(IAssetBundlesLoader assetBundlesLoader, IProjectSettings projectSettings, BundleType bundleType)
		: base(assetBundlesLoader, projectSettings, bundleType)
	{
	}

	public override IObservable<IEnumerable<Sprite>> Load()
	{
		return base.Load().Select(SortArray);
	}

	private IEnumerable<Sprite> SortArray(IEnumerable<Sprite> sprites)
	{
		return sprites.OrderBy(delegate(Sprite _sprite)
		{
			IEnumerable<char> enumerable = _sprite.name.Where(char.IsDigit);
			int num = 0;
			foreach (char item in enumerable)
			{
				int num2 = item - 48;
				num = num * 10 + num2;
			}
			return num;
		});
	}
}
