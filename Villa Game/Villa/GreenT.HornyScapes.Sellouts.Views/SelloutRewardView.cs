using System;
using System.Collections.Generic;
using GreenT.HornyScapes.Booster;
using GreenT.HornyScapes.Characters.Skins.Content;
using GreenT.HornyScapes.Content;
using GreenT.HornyScapes.Events;
using GreenT.HornyScapes.Lootboxes;
using GreenT.HornyScapes.Meta.Decorations;
using GreenT.HornyScapes.Presents.Models;
using GreenT.HornyScapes.Sellouts.Models;
using GreenT.Localizations;
using GreenT.UI;
using Merge.Meta.RoomObjects;
using StripClub.Model;
using StripClub.Model.Shop.UI;
using StripClub.UI;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace GreenT.HornyScapes.Sellouts.Views;

public class SelloutRewardView : MonoView<(int Id, SelloutRewardsInfo RewardInfo)>
{
	private const string NotEnoughtPointsKey = "ui.sellout.points_needed";

	[SerializeField]
	private RectTransform _rectTransform;

	[SerializeField]
	private Image _frame;

	[SerializeField]
	private Button _rewardClaimButton;

	[SerializeField]
	private TMP_Text _pointsText;

	[SerializeField]
	private SelloutProgressContainer _progressContainer;

	[SerializeField]
	private FlexibleGridLayoutGroup _standardSmallCardLayoutGroup;

	[SerializeField]
	private GameObject _light;

	[SerializeField]
	private StatableComponentGroup _rewardStateStatable;

	[SerializeField]
	private StatableComponent _firstInTrackStatable;

	[SerializeField]
	private StatableComponent _lineStatable;

	[SerializeField]
	private StatableComponent _frameShadowsStatable;

	private StandardSmallCardsViewManager _standardSmallCardsViewManager;

	private PremiumSmallCardsViewManager _premiumSmallCardsViewManager;

	private LocalizationService _localizationService;

	private int _id;

	private Sellout _sellout;

	private ISmallCardViewStrategy _currentViewStrategy;

	private IDisposable _stateStream;

	private IDisposable _pointsStream;

	private readonly ISmallCardViewStrategy _premiumCardStrategy = new SmallCardWithPremiumViewStrategy();

	private readonly ISmallCardViewStrategy _standardCardStrategy = new SmallCardViewStrategy();

	private readonly Subject<int> _rewardClaimed = new Subject<int>();

	private readonly CompositeDisposable _disposables = new CompositeDisposable();

	public RectTransform RectTransform => _rectTransform;

	public IObservable<SelloutRewardView> ProgressChanged => _progressContainer.ProgressChanged;

	public IObservable<int> RewardClaimed => _rewardClaimed.AsObservable();

	[Inject]
	private void Construct(StandardSmallCardsViewManager standardSmallCardsViewManager, PremiumSmallCardsViewManager premiumSmallCardsViewManager, LocalizationService localizationService)
	{
		_standardSmallCardsViewManager = standardSmallCardsViewManager;
		_premiumSmallCardsViewManager = premiumSmallCardsViewManager;
		_localizationService = localizationService;
	}

	public override void Set((int Id, SelloutRewardsInfo RewardInfo) source)
	{
		base.Set(source);
		_firstInTrackStatable.Set(0);
		(_id, _) = source;
		_standardSmallCardsViewManager.HideAll();
		_premiumSmallCardsViewManager.HideAll();
		IReadOnlyList<RewardWithManyConditions> premiumRewards = base.Source.RewardInfo.PremiumRewards;
		IReadOnlyList<RewardWithManyConditions> rewards = base.Source.RewardInfo.Rewards;
		CheckViewStrategy(0 < premiumRewards.Count);
		DisplayRewards(_premiumSmallCardsViewManager, premiumRewards);
		DisplayRewards(_standardSmallCardsViewManager, rewards);
		CheckRewards(premiumRewards);
		CheckRewards(rewards);
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

	public void SetAsFirstInTrack()
	{
		_firstInTrackStatable.Set(1);
	}

	public void SetTargetPoints(Sellout sellout, int startValue, int endValue)
	{
		_sellout = sellout;
		_progressContainer.Set(sellout, this, startValue, endValue);
		_pointsStream?.Dispose();
		_pointsStream = _sellout.Points.Subscribe(delegate(int points)
		{
			UpdatePoints(points);
			UpdateLine(points, startValue);
		});
		UpdatePoints(_sellout.Points.Value);
		UpdateLine(_sellout.Points.Value, startValue);
	}

	public void SetFrame(int index)
	{
		if (_sellout != null)
		{
			Sprite[] frameRewardSprites = _sellout.BundleData.FrameRewardSprites;
			if (frameRewardSprites != null)
			{
				index--;
				Mathf.Clamp(index, 0, frameRewardSprites.Length - 1);
				_frame.sprite = frameRewardSprites[index];
				_frameShadowsStatable.Set(index);
			}
		}
	}

	public void SetActiveLight(bool isActive)
	{
		_light.SetActive(isActive);
	}

	public EntityStatus GetState()
	{
		return base.Source.RewardInfo.Rewards[0].State.Value;
	}

	private void OnDestroy()
	{
		_stateStream?.Dispose();
		_pointsStream?.Dispose();
	}

	private void Init()
	{
		_rewardClaimButton.onClick.AddListener(TryClaimReward);
		ReactiveProperty<EntityStatus> state = base.Source.RewardInfo.Rewards[0].State;
		UpdateState(state.Value);
		_stateStream?.Dispose();
		_stateStream = state.Subscribe(UpdateState);
	}

	private void Hide()
	{
		_rewardClaimButton.onClick.RemoveListener(TryClaimReward);
		_stateStream?.Dispose();
	}

	private void CheckRewards(IReadOnlyList<RewardWithManyConditions> rewards)
	{
		foreach (RewardWithManyConditions reward in rewards)
		{
			if (reward.State.Value != EntityStatus.Complete)
			{
				reward.TrySetInProgress();
			}
		}
	}

	private void CheckViewStrategy(bool isPremium)
	{
		if (!(_standardSmallCardLayoutGroup == null))
		{
			_currentViewStrategy = (isPremium ? _premiumCardStrategy : _standardCardStrategy);
			_currentViewStrategy.UpdateConstraint(_standardSmallCardLayoutGroup);
		}
	}

	private void DisplayRewards(SmallCardsViewManager smallCardsViewManager, IReadOnlyList<RewardWithManyConditions> rewards)
	{
		foreach (RewardWithManyConditions reward in rewards)
		{
			LinkedContent content = reward.Content;
			if (content is LootboxLinkedContent lootboxLinkedContent)
			{
				smallCardsViewManager.Display(RewType.Lootbox, reward.Selector, (int)lootboxLinkedContent.GetRarity());
				continue;
			}
			if (content is PresentLinkedContent presentLinkedContent)
			{
				smallCardsViewManager.Display(RewType.Resource, reward.Selector, null, presentLinkedContent.Quantity);
				continue;
			}
			if (content is CurrencyLinkedContent currencyLinkedContent)
			{
				smallCardsViewManager.Display(RewType.Resource, reward.Selector, null, currencyLinkedContent.Quantity);
				continue;
			}
			if (content is CardLinkedContent cardLinkedContent)
			{
				smallCardsViewManager.Display(RewType.Characters, reward.Selector, null, cardLinkedContent.Quantity);
				continue;
			}
			if (content is MergeItemLinkedContent mergeItemLinkedContent)
			{
				smallCardsViewManager.Display(RewType.MergeItem, reward.Selector, null, mergeItemLinkedContent.Quantity);
				continue;
			}
			if (content is SkinLinkedContent skinLinkedContent)
			{
				smallCardsViewManager.Display(RewType.Skin, reward.Selector, (int)skinLinkedContent.Skin.Rarity);
				continue;
			}
			if (content is BoosterLinkedContent)
			{
				smallCardsViewManager.Display(RewType.Booster, reward.Selector);
				continue;
			}
			if (content is DecorationLinkedContent)
			{
				smallCardsViewManager.Display(RewType.Decorations, reward.Selector);
				continue;
			}
			throw new Exception($"No behaviour for this content.Type: {content.Type}").LogException();
		}
	}

	private void UpdateState(EntityStatus state)
	{
		switch (state)
		{
		case EntityStatus.Blocked:
			SetInProgressState();
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

	private void SetInProgressState()
	{
		_rewardStateStatable.Set(0);
	}

	private void SetCompleteState()
	{
		_rewardStateStatable.Set(1);
	}

	private void SetRewardedState()
	{
		_rewardStateStatable.Set(2);
	}

	private void TryClaimReward()
	{
		foreach (RewardWithManyConditions premiumReward in base.Source.RewardInfo.PremiumRewards)
		{
			premiumReward.TryCollectReward();
		}
		foreach (RewardWithManyConditions reward in base.Source.RewardInfo.Rewards)
		{
			reward.TryCollectReward();
		}
		_rewardClaimed?.OnNext(_id);
	}

	private void UpdatePoints(int points)
	{
		int num = int.Parse(base.Source.RewardInfo.Rewards[0].Conditions[0].ConditionText);
		int currentPoints = num - points;
		TMP_SpriteAsset spriteAsset = _sellout.BundleData.SpriteAsset;
		string sprite = $"<sprite=\"Sellout\\{spriteAsset.name}\" index=0>";
		_localizationService.ObservableText("ui.sellout.points_needed").Subscribe(delegate(string text)
		{
			_pointsText.text = string.Format(text, currentPoints, sprite);
		}).AddTo(_disposables);
	}

	private void UpdateLine(int points, int startValue)
	{
		EntityStatus value = base.Source.RewardInfo.Rewards[0].State.Value;
		if (startValue <= points && value == EntityStatus.InProgress)
		{
			_lineStatable.Set(1);
		}
	}
}
