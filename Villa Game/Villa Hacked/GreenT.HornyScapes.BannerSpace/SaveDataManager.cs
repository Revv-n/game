using System;
using System.Collections.Generic;
using System.Linq;
using GreenT.Data;

namespace GreenT.HornyScapes.BannerSpace;

[MementoHolder]
public class SaveDataManager : ISavableState
{
	[Serializable]
	public class BannerMemento : Memento
	{
		public readonly BannerSaveData[] Data;

		public BannerMemento(SaveDataManager savableState)
			: base(savableState)
		{
			Data = savableState._saves.Values.ToArray();
		}
	}

	private readonly Dictionary<string, BannerSaveData> _saves = new Dictionary<string, BannerSaveData>();

	public SavableStatePriority Priority => SavableStatePriority.Base;

	public void OnBannerBoth(string source, bool isLegendary, bool isMain)
	{
		ValidateSource(source);
		BannerSaveData bannerSaveData = _saves[source];
		bannerSaveData.AddStep();
		if (isLegendary)
		{
			bannerSaveData.ResetLegendaryStep();
			if (isMain)
			{
				bannerSaveData.ResetMainStep();
			}
			else
			{
				bannerSaveData.AddMainStep();
			}
		}
		else
		{
			bannerSaveData.AddLegendaryStep();
		}
	}

	public StepInfo GetStepInfo(string source)
	{
		ValidateSource(source);
		return new StepInfo(_saves[source]);
	}

	private void ValidateSource(string source)
	{
		if (!_saves.ContainsKey(source))
		{
			_saves.Add(source, new BannerSaveData(source));
		}
	}

	public string UniqueKey()
	{
		return "banner_save_data_manager";
	}

	public Memento SaveState()
	{
		return new BannerMemento(this);
	}

	public void LoadState(Memento memento)
	{
		BannerSaveData[] data = ((BannerMemento)memento).Data;
		foreach (BannerSaveData bannerSaveData in data)
		{
			if (bannerSaveData.MainStep < 1)
			{
				bannerSaveData.ResetMainStep();
			}
			if (bannerSaveData.LegendaryStep < 1)
			{
				bannerSaveData.ResetLegendaryStep();
			}
			_saves.Add(bannerSaveData.Source, bannerSaveData);
		}
	}
}
