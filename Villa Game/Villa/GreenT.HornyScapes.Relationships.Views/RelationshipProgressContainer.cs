using System;
using GreenT.Types;
using StripClub.UI;
using UnityEngine;

namespace GreenT.HornyScapes.Relationships.Views;

public class RelationshipProgressContainer : MonoView
{
	[SerializeField]
	private RelationshipProgressView _lovePointsProgressView;

	public IObservable<bool> IsPressStarted => _lovePointsProgressView.IsPressStarted;

	public IObservable<BaseRewardView> ProgressChanged => _lovePointsProgressView.ProgressChanged;

	public void Set(BaseRewardView rewardView, CompositeIdentificator identificator, int startValue, int endValue)
	{
		_lovePointsProgressView.Set(rewardView, identificator, startValue, endValue);
	}

	public int GetStartLovePoints()
	{
		return _lovePointsProgressView.GetStartPoints();
	}

	public int GetTargetLovePoints()
	{
		return _lovePointsProgressView.GetTargetPoints();
	}

	public RectTransform GetHandleRectTransform()
	{
		return _lovePointsProgressView.GetHandleRectTransform();
	}
}
