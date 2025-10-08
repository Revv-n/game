using System;
using System.Collections.Generic;
using System.Linq;
using GreenT.Data;
using GreenT.HornyScapes.MergeStore;

namespace GreenT.HornyScapes.Events;

public class EventMergeShopCleaner : IDataCleaner
{
	private readonly SectionCluster _sectionCluster;

	private readonly ISaver _saver;

	public EventMergeShopCleaner(ISaver saver, SectionCluster sectionCluster)
	{
		_sectionCluster = sectionCluster;
		_saver = saver ?? throw new ArgumentNullException("saver");
	}

	public void ClearData()
	{
		List<StoreSection> sections = _sectionCluster.GetEventSectionsToReset().ToList();
		ClearSections(sections);
		_sectionCluster.ResetEventSection();
	}

	private void ClearSections(List<StoreSection> sections)
	{
		if (sections.Any())
		{
			ApplySavedStates(sections);
		}
	}

	private void ApplySavedStates(List<StoreSection> mergeStoreSections)
	{
		List<Memento> state = SaveSectionStates(mergeStoreSections);
		List<ISavableState> savableObjets = mergeStoreSections.Cast<ISavableState>().ToList();
		if (_saver is Saver saver)
		{
			saver.LoadState(state, savableObjets);
		}
	}

	private List<Memento> SaveSectionStates(IEnumerable<StoreSection> sections)
	{
		return sections.Select((StoreSection section) => section.Clear().SaveState()).ToList();
	}
}
