using System.Collections.Generic;
using System.Linq;
using GreenT.Types;

namespace GreenT.HornyScapes.BannerSpace;

public class BannerCluster : IBannerCluster
{
	private readonly Dictionary<ContentType, Dictionary<int, Banner>> _banners = new Dictionary<ContentType, Dictionary<int, Banner>>();

	private readonly Dictionary<int, int> _tabsForId = new Dictionary<int, int>();

	public void AddBanner(Banner banner)
	{
		if (!_banners.ContainsKey(banner.ContentType))
		{
			_banners.Add(banner.ContentType, new Dictionary<int, Banner>());
		}
		_banners[banner.ContentType].TryAdd(banner.Id, banner);
		_tabsForId.TryAdd(banner.BankTabId, banner.Id);
	}

	public bool HaveReadyBanner(int id)
	{
		return _banners.Any((System.Collections.Generic.KeyValuePair<ContentType, Dictionary<int, Banner>> contentType) => contentType.Value.Any((System.Collections.Generic.KeyValuePair<int, Banner> pair) => pair.Key == id));
	}

	public void RemoveBanner(Banner banner)
	{
		if (_banners.TryGetValue(banner.ContentType, out var value))
		{
			if (value.ContainsKey(banner.Id))
			{
				value.Remove(banner.Id);
			}
			if (_tabsForId.ContainsKey(banner.BankTabId))
			{
				_tabsForId.Remove(banner.BankTabId);
			}
			if (value.Count == 0)
			{
				_banners.Remove(banner.ContentType);
			}
		}
	}

	public Banner Get(ContentType contentType, int bannerID)
	{
		if (!_banners.TryGetValue(contentType, out var value))
		{
			return null;
		}
		if (value.TryGetValue(bannerID, out var value2))
		{
			return value2;
		}
		return null;
	}

	public Banner GetForTab(ContentType contentType, int tabID)
	{
		if (_tabsForId.TryGetValue(tabID, out var value))
		{
			return Get(contentType, value);
		}
		return null;
	}
}
