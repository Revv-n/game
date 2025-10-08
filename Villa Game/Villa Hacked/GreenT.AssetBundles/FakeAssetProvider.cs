using System.Collections.Generic;
using System.Linq;
using StripClub.Model.Shop.Data;
using UnityEngine;

namespace GreenT.AssetBundles;

public class FakeAssetProvider
{
	private readonly FakeAssetsSO _fakeAssetsSo;

	public FakeAssetProvider(FakeAssetsSO fakeAssetsSo)
	{
		_fakeAssetsSo = fakeAssetsSo;
	}

	public Sprite GetFakeBySource(ContentSource contentSource, string asset)
	{
		List<Sprite> source = contentSource switch
		{
			ContentSource.BattlePass => _fakeAssetsSo.BattlePassIcon, 
			ContentSource.MiniEvent => _fakeAssetsSo.MiniEventIcon, 
			ContentSource.Employee => _fakeAssetsSo.EmployeeIcon, 
			ContentSource.Background => _fakeAssetsSo.BackgroundIcon, 
			ContentSource.EventBundle => _fakeAssetsSo.EventIcon, 
			_ => throw new KeyNotFoundException($"Unknown content source for fake: {contentSource}"), 
		};
		return source.FirstOrDefault((Sprite item) => asset.Contains(item.name)) ?? source.FirstOrDefault();
	}

	public Sprite GetFakeCharacterBankImages(int characterId)
	{
		int index = (int)((float)characterId % 100f % (float)_fakeAssetsSo.CharacterBankImages.Count);
		return _fakeAssetsSo.CharacterBankImages[index];
	}

	public Sprite GetFakeSkinIcon(int skinId)
	{
		int index = (int)((float)skinId % 100f % (float)_fakeAssetsSo.SkinIcons.Count);
		return _fakeAssetsSo.SkinIcons[index];
	}

	public Sprite GetFakeMergeItemIcon(int level)
	{
		if (level >= _fakeAssetsSo.MergeItemIcon.Count)
		{
			return _fakeAssetsSo.MergeItemIcon[0];
		}
		return _fakeAssetsSo.MergeItemIcon[level];
	}
}
