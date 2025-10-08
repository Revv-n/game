using System.Collections.Generic;
using GreenT.AssetBundles;

namespace GreenT.HornyScapes.MiniEvents;

public sealed class MiniEventsBundlesProvider : IRemovableMiniEventsBundlesProvider
{
	private Dictionary<int, int> _questKeys;

	private Dictionary<int, int> _shopKeys;

	private Dictionary<int, IAssetBundle> _miniEventBundles;

	public MiniEventsBundlesProvider()
	{
		_questKeys = new Dictionary<int, int>();
		_shopKeys = new Dictionary<int, int>();
		_miniEventBundles = new Dictionary<int, IAssetBundle>();
	}

	public void TryAdd(int miniEventId, IAssetBundle assetBundle)
	{
		_miniEventBundles.TryAdd(miniEventId, assetBundle);
	}

	public void TryRemove(int miniEventId)
	{
		if (_miniEventBundles.ContainsKey(miniEventId))
		{
			_miniEventBundles.Remove(miniEventId);
		}
	}

	public IAssetBundle TryGet(int miniEventId)
	{
		if (_miniEventBundles.TryGetValue(miniEventId, out var value))
		{
			return value;
		}
		return null;
	}

	public void TryAdd(int miniEventId, int tabId, TabType tabType)
	{
		switch (tabType)
		{
		case TabType.Shop:
			_shopKeys.TryAdd(tabId, miniEventId);
			break;
		case TabType.Task:
			_questKeys.TryAdd(tabId, miniEventId);
			break;
		}
	}

	public void TryRemove(int tabId, TabType tabType)
	{
		switch (tabType)
		{
		case TabType.Task:
			if (_questKeys.ContainsKey(tabId))
			{
				_questKeys.Remove(tabId);
			}
			break;
		case TabType.Shop:
			if (_shopKeys.ContainsKey(tabId))
			{
				_shopKeys.Remove(tabId);
			}
			break;
		}
	}

	public int GetKeyById(int tabId, TabType tabType)
	{
		int value = 0;
		switch (tabType)
		{
		case TabType.Shop:
			_shopKeys.TryGetValue(tabId, out value);
			break;
		case TabType.Task:
			_questKeys.TryGetValue(tabId, out value);
			break;
		}
		return value;
	}
}
