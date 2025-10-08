using System;
using System.Collections.Generic;
using DG.Tweening;
using GreenT.HornyScapes.Animations;
using GreenT.HornyScapes.Events;
using GreenT.HornyScapes.Relationships.Models;
using GreenT.Localizations;
using GreenT.Types;
using Merge;
using Merge.Meta.RoomObjects;
using StripClub.Model.Cards;
using StripClub.UI;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace GreenT.HornyScapes.Relationships.Views;

public abstract class BaseRewardView : MonoView<(int Id, IReadOnlyList<RewardWithManyConditions> Rewards)>
{
	[SerializeField]
	private RectTransform _rectTransform;

	[SerializeField]
	private StatableComponent _firstInTrackStatable;

	[SerializeField]
	private StatableComponent _lastInTrackStatable;

	[SerializeField]
	private Button _rewardClaimButton;

	[SerializeField]
	private RelationshipProgressContainer _progressContainer;

	[SerializeField]
	private RelationshipLevelView _levelView;

	[SerializeField]
	private GameObject _blockedLevelView;

	[SerializeField]
	private CanvasGroup _blockedDescription;

	[SerializeField]
	private TMP_Text _blockedLevelText;

	[SerializeField]
	private string _blockedLevelKey;

	[SerializeField]
	private TMP_Text _pointsText;

	[SerializeField]
	private StatusViewController _startStatusViewController;

	[SerializeField]
	private StatusViewControllerChecker _startStatusViewControllerChecker;

	[SerializeField]
	private StatusViewController _endStatusViewController;

	[SerializeField]
	private StatusViewControllerChecker _endStatusViewControllerChecker;

	[SerializeField]
	private RelationshipsToolTipOpener _toolTipOpener;

	[SerializeField]
	private RectTransform _dropTarget;

	[Header("States")]
	[SerializeField]
	private SpriteStates _sliderSpriteStates;

	[SerializeField]
	private SpriteStates _gradientSliderSpriteStates;

	[SerializeField]
	private StatableComponentGroup _rewardStateStatable;

	[SerializeField]
	private StatableComponentGroup _rewardTypeStatable;

	[Header("Animations")]
	[SerializeField]
	private ChainAnimationGroup _rewardClaimButtonAnimator;

	[SerializeField]
	private BlockedLevelAnimator _blockedLevelAnimator;

	[SerializeField]
	private UnblockedIconAnimator _unblockedIconAnimator;

	[SerializeField]
	private UnblockedRewardAnimator _unblockedRewardAnimator;

	private LocalizationService _localizationService;

	private IPromote _promote;

	private int _promoteLevel;

	private bool _wasBlocked;

	private bool _isLast;

	private EntityStatus _previousStatus;

	private IDisposable _promoteStream;

	private IDisposable _stateStream;

	private IDisposable _progressStream;

	protected int _id;

	protected Relationship _relationship;

	protected readonly Subject<int> _rewardClaimed = new Subject<int>();

	public RectTransform RectTransform => _rectTransform;

	public int PromoteLevel => _promoteLevel;

	public IObservable<RelationshipsToolTipOpener> TooltipRequested => _toolTipOpener.Requested;

	public IObservable<BaseRewardView> ProgressChanged => _progressContainer.ProgressChanged;

	public IObservable<int> RewardClaimed => Observable.AsObservable<int>((IObservable<int>)_rewardClaimed);

	[Inject]
	public void Init(LocalizationService localizationService)
	{
		_localizationService = localizationService;
	}

	public override void Set((int Id, IReadOnlyList<RewardWithManyConditions> Rewards) source)
	{
		base.Set(source);
		_firstInTrackStatable.Set(0);
		_lastInTrackStatable.Set(0);
		(_id, _) = source;
		_pointsText.text = base.Source.Rewards[0].Conditions[0].ConditionText;
		_previousStatus = base.Source.Rewards[0].State.Value;
		_wasBlocked = false;
	}

	public override void Display(bool display)
	{
		base.Display(display);
		if (display)
		{
			Init();
		}
		else
		{
			Hide();
		}
	}

	public void SetLevel(int level, bool isNewStatus)
	{
		_rewardTypeStatable.Set(level);
		_levelView.Set(level);
		if (isNewStatus)
		{
			_gradientSliderSpriteStates.Set(level);
		}
		else
		{
			_sliderSpriteStates.Set(level);
		}
	}

	public void ShowLevel(bool isShow)
	{
		_levelView.SetActive(isShow);
	}

	public void SetAsFirstInTrack()
	{
		_firstInTrackStatable.Set(1);
	}

	public void SetAsLastInTrack()
	{
		_isLast = true;
		_lastInTrackStatable.Set(1);
	}

	public void SetStatus(int statusStart, int statusEnd, int countStart, int countEnd, IReadOnlyList<RewardWithManyConditions> rewardStart, IReadOnlyList<RewardWithManyConditions> rewardEnd)
	{
		_startStatusViewControllerChecker.Set(rewardStart);
		_startStatusViewController.SetStatus(statusStart, countStart);
		_endStatusViewControllerChecker.Set(rewardEnd);
		_endStatusViewController.SetStatus(statusEnd, countEnd);
	}

	public void SetTargetPoints(CompositeIdentificator identificator, int startValue, int endValue)
	{
		_progressContainer.Set(this, identificator, startValue, endValue);
	}

	public void SetPromote(IPromote promote, int promoteLevel, Relationship relationship)
	{
		_promoteStream?.Dispose();
		_promote = promote;
		_promoteLevel = promoteLevel;
		_relationship = relationship;
		_promoteStream = ObservableExtensions.Subscribe<int>((IObservable<int>)_promote.Level, (Action<int>)CheckLevel);
		_blockedLevelText.text = string.Format(_localizationService.Text(_blockedLevelKey), _promoteLevel);
		CheckNotification();
	}

	public int GetStartLovePoints()
	{
		return _progressContainer.GetStartLovePoints();
	}

	public int GetTargetLovePoints()
	{
		return _progressContainer.GetTargetLovePoints();
	}

	public RectTransform GetHandleRectTransform()
	{
		return _progressContainer.GetHandleRectTransform();
	}

	public void SetActiveBlockedLevel(bool isActive)
	{
		_blockedLevelView.SetActive(isActive);
		_blockedDescription.gameObject.SetActive(isActive);
		if (isActive)
		{
			_blockedLevelAnimator.Prepare();
			_blockedDescription.alpha = 1f;
		}
	}

	public void AnimateBlockedLevel()
	{
		if (_blockedLevelView.activeSelf)
		{
			_blockedLevelAnimator.Play();
		}
	}

	public void PrepareAnimateUnblockedRewards()
	{
		SetBlockedState();
	}

	public void AnimateUnblockedLevel(int index, BaseRewardView nextBlockedRewardView)
	{
		if (index == 0)
		{
			_blockedLevelAnimator.Stop();
			_unblockedIconAnimator.Play();
			_unblockedRewardAnimator.Play(0, SetTargetState).OnComplete(SetInProgressState);
		}
		else
		{
			_unblockedRewardAnimator.Play(index, SetTargetState).OnComplete(SetInProgressState);
		}
		void SetTargetState()
		{
			_wasBlocked = false;
			SetInProgressState();
			ForceCheckStatusState();
			if (_isLast)
			{
				SetAsLastInTrack();
			}
			nextBlockedRewardView.ForceCheckStatusState();
		}
	}

	public void ForceCheckStatusState()
	{
		_startStatusViewControllerChecker.ForceUpdateState();
		_startStatusViewControllerChecker.ForceUpdateState();
	}

	public RectTransform GetDropTarget()
	{
		return _dropTarget;
	}

	protected virtual void SetBlockedState()
	{
		_wasBlocked = true;
		_blockedLevelAnimator.Stop();
		_rewardStateStatable.Set(0);
	}

	protected virtual void SetInProgressState()
	{
		if (!_wasBlocked)
		{
			_rewardStateStatable.Set(1);
		}
	}

	protected virtual void SetCompleteState()
	{
		_rewardStateStatable.Set(2);
		if (_previousStatus != EntityStatus.Complete)
		{
			_rewardClaimButtonAnimator.Play();
		}
	}

	protected virtual void SetRewardedState()
	{
		_rewardStateStatable.Set(3);
	}

	protected virtual void CheckNotification()
	{
	}

	protected abstract void TryClaimReward();

	private void Awake()
	{
		_unblockedIconAnimator.Init();
		_rewardClaimButtonAnimator.Init();
	}

	private void OnDestroy()
	{
		StopAnimations();
	}

	private void Init()
	{
		_rewardClaimButton.onClick.AddListener(TryClaimReward);
		ReactiveProperty<EntityStatus> state = base.Source.Rewards[0].State;
		UpdateState(state.Value);
		_stateStream?.Dispose();
		_stateStream = ObservableExtensions.Subscribe<EntityStatus>((IObservable<EntityStatus>)state, (Action<EntityStatus>)UpdateState);
		_progressStream?.Dispose();
		_progressStream = ObservableExtensions.Subscribe<bool>(_progressContainer.IsPressStarted, (Action<bool>)CheckToolTip);
	}

	private void Hide()
	{
		_rewardClaimButton.onClick.RemoveListener(TryClaimReward);
		_stateStream?.Dispose();
		_progressStream?.Dispose();
	}

	private void CheckToolTip(bool isPressStarted)
	{
		if (isPressStarted)
		{
			_toolTipOpener.HideToolTip();
		}
		else
		{
			_toolTipOpener.Request();
		}
	}

	private void CheckLevel(int level)
	{
		int value = _promote.Level.Value;
		bool flag = _promoteLevel <= value;
		foreach (RewardWithManyConditions item in base.Source.Rewards)
		{
			EntityStatus value2 = item.State.Value;
			if (flag)
			{
				if (value2 != EntityStatus.Complete)
				{
					item.TrySetInProgress();
				}
			}
			else if (value2 != EntityStatus.Blocked)
			{
				item.ForceSetState(EntityStatus.Blocked);
			}
		}
	}

	private void UpdateState(EntityStatus state)
	{
		switch (state)
		{
		case EntityStatus.Blocked:
			SetBlockedState();
			break;
		case EntityStatus.InProgress:
			SetInProgressState();
			break;
		case EntityStatus.Complete:
			SetCompleteState();
			break;
		case EntityStatus.Rewarded:
			SetRewardedState();
			break;
		}
	}

	private void StopAnimations()
	{
		_blockedLevelAnimator.Stop();
		_unblockedIconAnimator.Stop();
		_unblockedRewardAnimator.Stop();
		_rewardClaimButtonAnimator.Stop();
	}
}
