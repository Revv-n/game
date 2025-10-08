using System;
using System.Collections.Generic;
using StripClub.Model.Data;
using UniRx;

namespace StripClub.Model.Cards;

public class TabLoader : ILoader<IEnumerable<Tab>>
{
	private readonly ILoader<IEnumerable<TabSettings>> settingsLoader;

	private readonly ILoader<IEnumerable<TabMapper>> mapperLoader;

	public TabLoader(ILoader<IEnumerable<TabSettings>> settingsLoader, ILoader<IEnumerable<TabMapper>> mapperLoader)
	{
		this.settingsLoader = settingsLoader;
		this.mapperLoader = mapperLoader;
	}

	public IObservable<IEnumerable<Tab>> Load()
	{
		return from _loaded in mapperLoader.Load().Zip(settingsLoader.Load(), (IEnumerable<TabMapper> x, IEnumerable<TabSettings> y) => (mappers: x, settings: y))
			select CreateTabs(_loaded.mappers, _loaded.settings);
	}

	private IEnumerable<Tab> CreateTabs(IEnumerable<TabMapper> mappers, IEnumerable<TabSettings> settings)
	{
		List<Tab> list = new List<Tab>();
		foreach (TabMapper mapper in mappers)
		{
			foreach (TabSettings setting in settings)
			{
				if (mapper.id == setting.GroupID)
				{
					UnlockSettings unlockSettings = new UnlockSettings(mapper.unlock_type, mapper.unlock_value);
					Tab item = new Tab(mapper.id, mapper.position, setting.Icon, unlockSettings);
					list.Add(item);
				}
			}
		}
		return list;
	}
}
