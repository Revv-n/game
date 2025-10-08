using System;
using GreenT.HornyScapes.Events;
using GreenT.UI;
using Merge.Meta.RoomObjects;
using StripClub.UI;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace GreenT.HornyScapes.BattlePassSpace.UI.MetaWindow;

public class BattlePassButtonSubscription : MonoView<CalendarModel>
{
	[SerializeField]
	private Button button;

	[SerializeField]
	private WindowID comingSoonWindow;

	[SerializeField]
	private WindowOpener openComingSoon;

	[SerializeField]
	private WindowOpener openStartWindow;

	[SerializeField]
	private WindowOpener openProgressWindow;

	private ComingSoonWindow _comingSoonWindow;

	private BattlePass _battlePass;

	private IDisposable _buttonClickStream;

	private BattlePassSettingsProvider _battlePassSettingsProvider;

	private IWindowsManager _windowsManager;

	[Inject]
	private void InnerInit(IWindowsManager windowsManager, BattlePassSettingsProvider battlePassSettingsProvider)
	{
		_battlePassSettingsProvider = battlePassSettingsProvider;
		_windowsManager = windowsManager;
	}

	private void OnDestroy()
	{
		_buttonClickStream?.Dispose();
	}

	public void Reset()
	{
		base.Set(null);
		_battlePass = null;
		_buttonClickStream?.Dispose();
	}

	public override void Set(CalendarModel source)
	{
		base.Set(source);
		_buttonClickStream?.Dispose();
		if (_battlePassSettingsProvider.TryGetBattlePass(source.BalanceId, out var battlePass))
		{
			_battlePass = battlePass;
			_buttonClickStream = button.OnClickAsObservable().Subscribe(delegate
			{
				OpenWindow();
			});
		}
		else
		{
			Debug.LogError("В [BattlePassButtonSubscription] попал не [BattlePass]");
		}
	}

	private void OpenWindow()
	{
		if (_comingSoonWindow == null)
		{
			_comingSoonWindow = _windowsManager.GetWindow(comingSoonWindow) as ComingSoonWindow;
		}
		if (_battlePass == null || _comingSoonWindow == null)
		{
			Debug.LogError("В [BattlePassButtonSubscription] не указан [BattlePass] на момент [OpenWindow]");
			return;
		}
		switch (base.Source.CalendarState.Value)
		{
		case EntityStatus.Blocked:
			ShowComingSoon();
			break;
		case EntityStatus.InProgress:
			ShowProgress();
			break;
		case EntityStatus.Complete:
			ShowRewards();
			break;
		default:
			throw new Exception().SendException($"{GetType().Name}: no behaviour for {base.Source.CalendarState.Value}");
		case EntityStatus.Rewarded:
			break;
		}
	}

	private void ShowRewards()
	{
		openProgressWindow.Click();
	}

	private void ShowComingSoon()
	{
		_comingSoonWindow.Set(base.Source);
		openComingSoon.Click();
	}

	private void ShowProgress()
	{
		if (!_battlePass.Data.StartData.StartWindowShown)
		{
			openStartWindow.Click();
			_battlePass.Data.StartData.SetStartedBattlePassProgress();
		}
		else
		{
			openProgressWindow.Click();
			_battlePass.Data.StartData.SetFirstTimeStarted();
		}
	}
}
