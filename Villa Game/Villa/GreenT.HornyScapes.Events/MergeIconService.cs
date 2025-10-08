using System.Collections.Generic;
using System.Linq;
using GreenT.AssetBundles;
using GreenT.HornyScapes.Events.Content;
using GreenT.HornyScapes.MergeCore;
using GreenT.HornyScapes.MiniEvents;
using GreenT.Types;
using Merge;
using UnityEngine;
using Zenject;

namespace GreenT.HornyScapes.Events;

public class MergeIconService : IInitializable, IContentSelector, ISelector<ContentType>, IMergeIconProvider
{
	private readonly LegacyMergeIconProvider _legacyMergeIconProvider;

	private readonly BattlePassMergeIconProvider _battlePassMergeIconProvider;

	private readonly MiniEventMergeIconsProvider _miniEventMergeIconsProvider;

	private readonly EventMergeIconProvider _baseMergeIconProvider;

	private readonly MainMergeIconProvider _mainMergeIconProvider;

	private readonly Dictionary<ContentType, HashSet<IMergeIconProvider>> _dictionary = new Dictionary<ContentType, HashSet<IMergeIconProvider>>();

	private readonly FakeAssetProvider _fakeAssetProvider;

	private ContentType _current;

	public MergeIconService(LegacyMergeIconProvider legacyMergeIconProvider, BattlePassMergeIconProvider battlePassMergeIconProvider, MiniEventMergeIconsProvider miniEventMergeIconsProvider, EventMergeIconProvider baseMergeIconProvider, FakeAssetProvider fakeAssetProvider, MainMergeIconProvider mainMergeIconProvider)
	{
		_legacyMergeIconProvider = legacyMergeIconProvider;
		_battlePassMergeIconProvider = battlePassMergeIconProvider;
		_miniEventMergeIconsProvider = miniEventMergeIconsProvider;
		_baseMergeIconProvider = baseMergeIconProvider;
		_fakeAssetProvider = fakeAssetProvider;
		_mainMergeIconProvider = mainMergeIconProvider;
	}

	public void Select(ContentType selector)
	{
		_current = selector;
	}

	public void Initialize()
	{
		Add(ContentType.Main, _battlePassMergeIconProvider);
		Add(ContentType.Main, _miniEventMergeIconsProvider);
		Add(ContentType.Main, _legacyMergeIconProvider);
		Add(ContentType.Main, _mainMergeIconProvider);
		Add(ContentType.Event, _baseMergeIconProvider);
	}

	public Sprite GetSprite(GIKey key)
	{
		Sprite sprite = GetSprite(key, _current);
		if (sprite == null)
		{
			sprite = GetSprite(key, ContentType.Main);
		}
		if (sprite == null)
		{
			sprite = GetSprite(key, ContentType.Event);
		}
		if (sprite == null)
		{
			sprite = _fakeAssetProvider.GetFakeMergeItemIcon(key.ID);
		}
		return sprite;
	}

	private Sprite GetSprite(GIKey key, ContentType contentType)
	{
		return _dictionary[contentType].Select((IMergeIconProvider iconManager) => iconManager.GetSprite(key)).FirstOrDefault((Sprite icon) => icon != null);
	}

	private void Add(ContentType type, IMergeIconProvider iconProvider)
	{
		if (!_dictionary.ContainsKey(type))
		{
			_dictionary.Add(type, new HashSet<IMergeIconProvider>());
		}
		_dictionary[type].Add(iconProvider);
	}
}
