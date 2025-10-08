using System;
using System.Collections.Generic;
using GreenT.HornyScapes.Events;
using GreenT.HornyScapes.Events.BattlePassRewardCards;
using StripClub.UI;
using TMPro;
using UniRx;
using UnityEngine;

namespace GreenT.HornyScapes.BattlePassSpace.RewardCards;

public class SliderUpBattlePassProgress : MonoView<BattlePass>
{
	[SerializeField]
	private BattlePassProgressSlider _battlePassProgressSlider;

	[SerializeField]
	private LevelViewObject _levelViewObject;

	[SerializeField]
	private TMP_Text _currentTextPointsCount;

	[SerializeField]
	private TMP_Text _targetTextPointsCount;

	private int _index;

	private bool _isEnded;

	private BattlePassRewardPairData _currentPairData;

	private IDisposable _pointStream;

	public override void Set(BattlePass newBattlePass)
	{
		_index = 0;
		DisposeStreams();
		base.Set(newBattlePass);
		_levelViewObject.SetBattlePassSprites(newBattlePass);
		_pointStream?.Dispose();
		_pointStream = ObservableExtensions.Subscribe<int>((IObservable<int>)base.Source.Data.LevelInfo.Points, (Action<int>)OnProgressPointsChanged);
		OnProgressPointsChanged(base.Source.Data.LevelInfo.Points.Value);
	}

	private bool TrySetNewPairCase(int startStep = 0)
	{
		int currentLevel = GetCurrentLevel();
		if (_currentPairData != null && _index != 0 && _currentPairData.TargetLevel > currentLevel)
		{
			return false;
		}
		for (int i = startStep; i < base.Source.Data.RewardPairQueue.Cases.Count; i++)
		{
			if (base.Source.Data.RewardPairQueue.Cases[i].TargetLevel > currentLevel)
			{
				_index = i;
				_currentPairData = base.Source.Data.RewardPairQueue.Cases[_index];
				return true;
			}
		}
		IReadOnlyList<BattlePassRewardPairData> cases = base.Source.Data.RewardPairQueue.Cases;
		_currentPairData = cases[cases.Count - 1];
		_isEnded = true;
		return false;
	}

	private int GetCurrentLevel()
	{
		return base.Source.GetLevelForPoints(base.Source.Data.LevelInfo.Points.Value);
	}

	private void OnProgressPointsChanged(int progressPoints)
	{
		if (TrySetNewPairCase(_index))
		{
			InitializeSlider();
			UpdateLevelText();
		}
		else if (_isEnded)
		{
			DisposeStreams();
			UpdateLevelText();
		}
		UpdateHasRewardVisual();
		UpdateSlider(progressPoints);
		UpdatePointsText(progressPoints);
	}

	private void InitializeSlider()
	{
		_battlePassProgressSlider.Initialization(AdjustToStartProgress(_currentPairData.TargetProgressValue));
	}

	private void UpdateLevelText()
	{
		_levelViewObject.Text.text = _currentPairData.TargetLevel.ToString();
	}

	private void UpdateHasRewardVisual()
	{
		if (base.Source.HasRewards())
		{
			_levelViewObject.SetCompleted();
		}
		else
		{
			_levelViewObject.Reset();
		}
	}

	private void UpdateSlider(int progressPoints)
	{
		_battlePassProgressSlider.SetProgress(AdjustToStartProgress(progressPoints));
	}

	private void UpdatePointsText(int points)
	{
		int targetProgressValue = _currentPairData.TargetProgressValue;
		_currentTextPointsCount.text = Math.Clamp(AdjustToStartProgress(points), 0, AdjustToStartProgress(targetProgressValue)).ToString();
		_targetTextPointsCount.text = AdjustToStartProgress(targetProgressValue).ToString();
	}

	private int AdjustToStartProgress(int value)
	{
		return value - _currentPairData.StartProgressValue;
	}

	private void DisposeStreams()
	{
		_pointStream?.Dispose();
	}

	public void Reset()
	{
		_levelViewObject.Text.text = "0";
		_levelViewObject.Reset();
		_battlePassProgressSlider.Initialization(1);
		_battlePassProgressSlider.SetProgress(0);
		_index = 0;
		DisposeStreams();
		base.Set(null);
	}

	private void OnDestroy()
	{
		Reset();
	}
}
