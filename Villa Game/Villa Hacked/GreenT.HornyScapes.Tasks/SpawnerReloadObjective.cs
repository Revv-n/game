using System.Collections.Generic;
using System.Linq;
using GreenT.HornyScapes.MergeCore;
using GreenT.Types;
using Merge;
using UnityEngine;

namespace GreenT.HornyScapes.Tasks;

[Objective]
public class SpawnerReloadObjective : SavableObjective
{
	private readonly MergeFieldProvider _fieldProvider;

	private List<GIData> _datas;

	public override bool IsComplete => Data.Progress >= Data.Required;

	public SpawnerReloadObjective(MergeFieldProvider fieldProvider, SavableObjectiveData data)
		: base(data)
	{
		_fieldProvider = fieldProvider;
		_datas = new List<GIData>();
	}

	public override Sprite GetIcon()
	{
		return null;
	}

	public override int GetProgress()
	{
		return Data.Progress;
	}

	public override int GetTarget()
	{
		return Data.Required;
	}

	public override void Track()
	{
		GameItemController instance = Controller<GameItemController>.Instance;
		instance.OnItemCreated += OnItemCreated;
		instance.OnItemRemoved += OnItemRemoved;
		instance.OnItemTakenFromSomethere += OnItemRemoved;
		Controller<BubbleController>.Instance.OnBubbleUnlock += OnItemCreated;
		Controller<LockedController>.Instance.OnItemActionUnlock += OnItemCreated;
		SubscribeSpawners(ContentType.Main);
		SubscribeSpawners(ContentType.Event);
		SetProgress(ContentType.Main);
		SetProgress(ContentType.Event);
		onUpdate.OnNext((IObjective)this);
	}

	public override void OnRewardTask()
	{
		GameItemController instance = Controller<GameItemController>.Instance;
		instance.OnItemCreated -= OnItemCreated;
		instance.OnItemRemoved -= OnItemRemoved;
		instance.OnItemTakenFromSomethere -= OnItemRemoved;
		Controller<BubbleController>.Instance.OnBubbleUnlock -= OnItemCreated;
		Controller<LockedController>.Instance.OnItemActionUnlock -= OnItemCreated;
		UnsubscribeSpawners();
	}

	private void OnItemCreated(GameItem item)
	{
		if (item.Data.HasModule(GIModuleType.ClickSpawn) && !_datas.Contains(item.Data))
		{
			ModuleDatas.ClickSpawn module = item.Data.GetModule<ModuleDatas.ClickSpawn>();
			if (module != null)
			{
				module.OnTimerComplete += IncreaseProgress;
				_datas.Add(item.Data);
			}
		}
	}

	private void OnItemRemoved(GameItem item)
	{
		if (item.Data.HasModule(GIModuleType.ClickSpawn) && _datas.Contains(item.Data))
		{
			ModuleDatas.ClickSpawn module = item.Data.GetModule<ModuleDatas.ClickSpawn>();
			if (module != null)
			{
				module.OnTimerComplete -= IncreaseProgress;
				_datas.Remove(item.Data);
			}
		}
	}

	private void IncreaseProgress()
	{
		if (!IsComplete)
		{
			Data.Progress++;
		}
		onUpdate.OnNext((IObjective)this);
	}

	private void SubscribeSpawners(ContentType contentType)
	{
		if (!_fieldProvider.TryGetData(contentType, out var field))
		{
			return;
		}
		foreach (GIData item in field.Data.GameItems.Where((GIData _object) => _object.HasModule(GIModuleType.ClickSpawn)))
		{
			ModuleDatas.ClickSpawn module = item.GetModule<ModuleDatas.ClickSpawn>();
			if (module != null)
			{
				module.OnTimerComplete += IncreaseProgress;
				_datas.Add(item);
			}
		}
	}

	private void UnsubscribeSpawners()
	{
		foreach (GIData data in _datas)
		{
			ModuleDatas.ClickSpawn module = data.GetModule<ModuleDatas.ClickSpawn>();
			if (module != null)
			{
				module.OnTimerComplete -= IncreaseProgress;
			}
		}
	}

	private void SetProgress(ContentType contentType)
	{
		if (!_fieldProvider.TryGetData(contentType, out var field))
		{
			return;
		}
		foreach (GIData item in field.Data.GameItems.Where((GIData _object) => _object.HasModule(GIModuleType.ClickSpawn)))
		{
			ModuleDatas.ClickSpawn module = item.GetModule<ModuleDatas.ClickSpawn>();
			if (module != null && module.WasRefreshedOffline)
			{
				IncreaseProgress();
			}
		}
	}
}
