using System;
using GreenT.HornyScapes.BattlePassSpace;
using GreenT.HornyScapes.BattlePassSpace.RewardCards;
using Merge.Meta.RoomObjects;
using StripClub.Model.Shop;
using StripClub.UI;
using UniRx;
using UnityEngine;
using Zenject;

namespace GreenT.HornyScapes.Events.BattlePassRewardCards;

public class BattlePassRewardLevelView : MonoView<BattlePassRewardPairData>
{
	[SerializeField]
	private Transform _holder;

	[SerializeField]
	private BattlePassProgressSlider slider;

	[SerializeField]
	private LevelViewObjectCase levelView;

	private ICurrencyProcessor _currencyProcessor;

	private BattlePassRewardHolderFactory _rewardHolderFactory;

	private BaseBattlePassRewardHolder _rewardHolder;

	private BattlePass _battlePass;

	private int _startProgressValue;

	private IDisposable _pointStream;

	[Inject]
	private void Construct(ICurrencyProcessor currencyProcessor, BattlePassRewardHolderFactory holderFactory)
	{
		_currencyProcessor = currencyProcessor;
		_rewardHolderFactory = holderFactory;
	}

	public override void Set(BattlePassRewardPairData source)
	{
		base.Set(source);
		_startProgressValue = source.StartProgressValue;
		slider.Initialization(source.TargetProgressValue - source.StartProgressValue);
		ResetSlider();
		levelView.Initialization(source.TargetLevel, source.StartLevel, source.IsLast);
		SetGameObjectNames(source);
	}

	public void UpdateRewardHolder(BattlePass battlePass)
	{
		_battlePass = battlePass;
		if (_rewardHolder == null)
		{
			SetupRewardHolder(base.Source, battlePass);
		}
		else
		{
			_rewardHolder.Set(base.Source);
		}
		Unsubscribe();
		_pointStream = ObservableExtensions.Subscribe<int>((IObservable<int>)_battlePass.Data.LevelInfo.Points, (Action<int>)delegate
		{
			SetProgress();
		});
		SetProgress();
	}

	private void SetupRewardHolder(BattlePassRewardPairData source, BattlePass battlePass)
	{
		_rewardHolder = _rewardHolderFactory.Create(source, battlePass);
		RectTransform obj = (RectTransform)_rewardHolder.transform;
		obj.SetParent(_holder);
		obj.localScale = Vector3.one;
		obj.anchoredPosition = Vector2.zero;
		levelView.SetBattlePassSprites(battlePass);
	}

	private void SetProgress()
	{
		if (base.Source.Status == EntityStatus.Blocked)
		{
			ResetSlider();
			return;
		}
		int value = _battlePass.Data.LevelInfo.Points.Value;
		if (value >= _startProgressValue)
		{
			int progress = value - _startProgressValue;
			slider.SetProgress(progress);
			if (value >= base.Source.StartProgressValue)
			{
				levelView.SetStartProgress();
			}
			if (value >= base.Source.TargetProgressValue)
			{
				levelView.SetCompleted();
			}
		}
	}

	private void ResetSlider()
	{
		slider.SetProgress(0);
	}

	public override void Display(bool display)
	{
		if (!display)
		{
			Unsubscribe();
		}
		_rewardHolder.Display(display);
		base.Display(display);
	}

	private void Unsubscribe()
	{
		_pointStream?.Dispose();
	}

	private void OnDestroy()
	{
		Unsubscribe();
	}

	private void SetGameObjectNames(BattlePassRewardPairData source)
	{
	}
}
