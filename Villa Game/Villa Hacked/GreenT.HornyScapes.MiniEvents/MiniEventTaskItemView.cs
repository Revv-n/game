using System;
using System.Collections.Generic;
using System.Linq;
using GreenT.HornyScapes.DebugInfo;
using GreenT.HornyScapes.Tasks;
using GreenT.HornyScapes.Tasks.UI;
using GreenT.Types;
using StripClub.Model;
using StripClub.UI;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace GreenT.HornyScapes.MiniEvents;

public class MiniEventTaskItemView : MonoView<MiniEventTask>, IDisposable
{
	[SerializeField]
	private LocalizedTextMeshPro _description;

	[SerializeField]
	private LocalizedTextMeshPro _progress;

	[SerializeField]
	private Slider _progressBar;

	[SerializeField]
	private BaseButtonStrategy _buttonStrategy;

	[SerializeField]
	private DebugInfoContainer _debugInfo;

	[SerializeField]
	private MiniEventTaskItemViewStateMachine _miniEventTaskItemViewStateMachine;

	private MiniEventTaskItemRewardViewManager _taskRewardViewManager;

	private CompositeDisposable _taskUpdateStream = new CompositeDisposable();

	private MiniEventTasksDescriptionLocalizationResolver _miniEventTasksDescriptionLocalizationResolver;

	private MiniEventTasksProgressLocalizationResolver _miniEventTasksProgressLocalizationResolver;

	private List<MiniEventTaskCurrencyRewardItemView> _currencyRewardItems;

	[Inject]
	private void Init(MiniEventTaskItemRewardViewManager taskRewardViewManager, MiniEventTasksDescriptionLocalizationResolver miniEventTasksDescriptionLocalizationResolver, MiniEventTasksProgressLocalizationResolver miniEventTasksProgressLocalizationResolver)
	{
		_taskRewardViewManager = taskRewardViewManager;
		_miniEventTasksDescriptionLocalizationResolver = miniEventTasksDescriptionLocalizationResolver;
		_miniEventTasksProgressLocalizationResolver = miniEventTasksProgressLocalizationResolver;
		_currencyRewardItems = new List<MiniEventTaskCurrencyRewardItemView>();
	}

	private void OnDestroy()
	{
		_taskUpdateStream.Dispose();
	}

	public void Dispose()
	{
		_taskUpdateStream.Clear();
	}

	public override void Set(MiniEventTask source)
	{
		base.Set(source);
		_miniEventTaskItemViewStateMachine.Init(base.Source);
		_buttonStrategy.Set(base.Source);
		SetView();
		TrackUpdate();
		_miniEventTaskItemViewStateMachine.ForceSetFirstShowState();
		_miniEventTaskItemViewStateMachine.SetState(source.State);
		TryShoveDebugInfo(source);
	}

	public void SetRewardState()
	{
		_miniEventTaskItemViewStateMachine.ForceSetRewardState();
	}

	public MiniEventTaskCurrencyRewardItemView TryGetCurrencyRewardItem(CurrencyType currencyType, CompositeIdentificator compositeIdentificator)
	{
		return _currencyRewardItems.FirstOrDefault((MiniEventTaskCurrencyRewardItemView rewardView) => rewardView.CurrencyType == currencyType && rewardView.CompositeIdentificator == compositeIdentificator);
	}

	private void SetView()
	{
		IObjective objective = base.Source.Goal.Objectives.First();
		int num = Mathf.Min(objective.GetProgress(), objective.GetTarget());
		int target = objective.GetTarget();
		string keyByObjective = _miniEventTasksDescriptionLocalizationResolver.GetKeyByObjective(objective);
		string key = _miniEventTasksProgressLocalizationResolver.GetKey(num >= target);
		SetDescription(keyByObjective);
		SetProgress(key, num, target);
		SetRewards(base.Source.Reward);
	}

	private void SetProgress(string key, int current, int target)
	{
		_progress.Init(key, current, target);
		_progressBar.value = (float)current / (float)target;
	}

	private void SetDescription(string longKey)
	{
		string[] array = longKey.Split(':', StringSplitOptions.None);
		string key = array[0];
		if (array.Length > 1)
		{
			int num = 1;
			int num2 = array.Length - num;
			object[] array2 = new object[num2];
			Array.Copy(array, num, array2, 0, num2);
			_description.Init(key, array2);
		}
		else
		{
			_description.Init(key);
		}
	}

	private void SetRewards(LinkedContent linkedContent)
	{
		_taskRewardViewManager.HideAll();
		for (LinkedContent next = linkedContent.GetNext<LinkedContent>(checkThis: true); next != null; next = next.GetNext<LinkedContent>())
		{
			MiniEventTaskRewardItemView miniEventTaskRewardItemView = _taskRewardViewManager.Display(next);
			if (next is CurrencyLinkedContent)
			{
				_currencyRewardItems.Add(miniEventTaskRewardItemView as MiniEventTaskCurrencyRewardItemView);
			}
		}
	}

	private void TrackUpdate()
	{
		_taskUpdateStream.Clear();
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<Task>(base.Source.OnUpdate, (Action<Task>)delegate(Task task)
		{
			_miniEventTaskItemViewStateMachine.SetState(task.State);
			SetView();
		}), (ICollection<IDisposable>)_taskUpdateStream);
	}

	private void TryShoveDebugInfo(MiniEventTask source)
	{
	}
}
