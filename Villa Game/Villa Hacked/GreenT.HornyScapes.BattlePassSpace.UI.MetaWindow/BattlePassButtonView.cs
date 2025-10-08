using System;
using System.Collections.Generic;
using GreenT.HornyScapes.Events;
using Merge.Meta.RoomObjects;
using StripClub.Extensions;
using StripClub.UI;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace GreenT.HornyScapes.BattlePassSpace.UI.MetaWindow;

public class BattlePassButtonView : MonoView<CalendarModel>
{
	[SerializeField]
	private TMP_Text _levelField;

	[SerializeField]
	private BattlePassButtonSubscription subscription;

	[SerializeField]
	private MonoTimer timerView;

	[SerializeField]
	private Image icon;

	[SerializeField]
	private Sprite rewardSp;

	[SerializeField]
	private Sprite comingSoonSp;

	[SerializeField]
	private Transform _levelHolder;

	[SerializeField]
	private Image levelHolderBG;

	[SerializeField]
	private GameObject soonObject;

	[SerializeField]
	private GameObject soonLevelObject;

	[SerializeField]
	private GameObject rewardObject;

	[SerializeField]
	private List<StatableComponent> statableComponents = new List<StatableComponent>();

	private PlayerExperience _playerExperience;

	private BattlePassSettingsProvider _battlePassSettingsProvider;

	private TimeHelper _timeHelper;

	private IDisposable _levelStream;

	private IDisposable _calendarStateStream;

	private IDisposable _rewardStateStream;

	[Inject]
	public void Construct(TimeHelper timeHelper, PlayerExperience playerExperience, BattlePassSettingsProvider battlePassSettingsProvider)
	{
		_timeHelper = timeHelper;
		_playerExperience = playerExperience;
		_battlePassSettingsProvider = battlePassSettingsProvider;
	}

	public override void Set(CalendarModel source)
	{
		base.Set(source);
		subscription.Set(source);
		_calendarStateStream?.Dispose();
		_calendarStateStream = ObservableExtensions.Subscribe<EntityStatus>((IObservable<EntityStatus>)source.CalendarState, (Action<EntityStatus>)SetState);
	}

	private void SetState(EntityStatus status)
	{
		if (_battlePassSettingsProvider.TryGetBattlePass(base.Source.BalanceId, out var battlePass))
		{
			switch (status)
			{
			case EntityStatus.Blocked:
				SetPreview();
				break;
			case EntityStatus.InProgress:
				SetStart();
				break;
			case EntityStatus.Complete:
				SetComplete(battlePass);
				break;
			case EntityStatus.Rewarded:
				SetIsSoonState(value: true);
				break;
			default:
				throw new Exception().SendException($"{GetType().Name}: doesn't have behaviour for {status.ToString()} int = {status}");
			}
		}
	}

	public void SetIsSoonState(bool value)
	{
		soonObject.gameObject.SetActive(value);
		if (value)
		{
			SetSoon();
		}
	}

	private void SetSoon()
	{
		base.Set(null);
		subscription.Reset();
		timerView.gameObject.SetActive(value: false);
		levelHolderBG.gameObject.SetActive(value: false);
		rewardObject.SetActive(value: false);
		soonLevelObject.SetActive(value: true);
		icon.sprite = comingSoonSp;
	}

	private void SetPreview()
	{
		levelHolderBG.gameObject.SetActive(value: false);
		timerView.gameObject.SetActive(value: true);
		soonObject.SetActive(value: false);
		soonLevelObject.SetActive(value: true);
		rewardObject.SetActive(value: false);
		_levelField.transform.gameObject.SetActive(value: false);
		ChangeState();
		icon.sprite = comingSoonSp;
		SetTimer(base.Source.ComingSoonTimer);
	}

	private void SetStart()
	{
		levelHolderBG.gameObject.SetActive(value: true);
		timerView.gameObject.SetActive(value: true);
		soonObject.gameObject.SetActive(value: false);
		soonLevelObject.SetActive(value: false);
		rewardObject.SetActive(value: false);
		_levelStream?.Dispose();
		_levelField.transform.gameObject.SetActive(value: true);
		ChangeState();
		SetBankIcon();
		SetTimer(base.Source.Duration);
		_levelStream = ObservableExtensions.Subscribe<int>((IObservable<int>)_playerExperience.Level, (Action<int>)delegate(int level)
		{
			_levelField.text = level.ToString();
		});
	}

	private void SetReward()
	{
		ChangeState();
		icon.sprite = rewardSp;
		_levelField.transform.gameObject.SetActive(value: false);
	}

	private void SetComplete(BattlePass battlePass)
	{
		levelHolderBG.gameObject.SetActive(value: false);
		timerView.gameObject.SetActive(value: false);
		soonObject.gameObject.SetActive(value: false);
		soonLevelObject.SetActive(value: false);
		rewardObject.SetActive(value: true);
		if (battlePass.HasRewards())
		{
			SetReward();
		}
		else
		{
			SetIsSoonState(value: true);
		}
	}

	private void SetBankIcon()
	{
		if (_battlePassSettingsProvider.TryGetBattlePass(base.Source.BalanceId, out var battlePass))
		{
			icon.sprite = battlePass.CurrentViewSettings.LevelButton;
		}
	}

	private void SetTimer(GenericTimer timer)
	{
		timerView.Init(timer, _timeHelper.UseCombineFormat);
	}

	private void ChangeState()
	{
		base.gameObject.SetActive(value: true);
		int stateNumber = base.Source.CalendarState.Value.ConvertToInt();
		foreach (StatableComponent statableComponent in statableComponents)
		{
			statableComponent.Set(stateNumber);
		}
	}

	public Transform GetLevelHolder()
	{
		return _levelHolder;
	}

	private void OnDestroy()
	{
		_rewardStateStream?.Dispose();
		_calendarStateStream?.Dispose();
	}
}
