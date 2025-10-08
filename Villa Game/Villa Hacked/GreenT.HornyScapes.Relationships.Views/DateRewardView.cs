using System.Collections.Generic;
using GreenT.HornyScapes.Dates.Models;
using GreenT.HornyScapes.Dates.Services;
using GreenT.HornyScapes.Events;
using GreenT.HornyScapes.Relationships.Analytics;
using GreenT.Localizations;
using Merge.Meta.RoomObjects;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace GreenT.HornyScapes.Relationships.Views;

public class DateRewardView : BaseRewardView
{
	private const string TitleLocKey = "content.dates.name.{0}";

	[Header("Date")]
	[SerializeField]
	private Image _dateIcon;

	[SerializeField]
	private TMP_Text _dateTitle;

	[SerializeField]
	private OnlyOneStatable _collectButtonStates;

	[SerializeField]
	private Image _notificationIcon;

	private DateController _dateController;

	private LocalizationService _localizationService;

	private RelationshipAnalytic _relationshipAnalytic;

	private Date _date;

	[Inject]
	private void Init(DateController dateController, LocalizationService localizationService, RelationshipAnalytic relationshipAnalytic)
	{
		_dateController = dateController;
		_localizationService = localizationService;
		_relationshipAnalytic = relationshipAnalytic;
	}

	public override void Set((int Id, IReadOnlyList<RewardWithManyConditions> Rewards) source)
	{
		base.Set(source);
		_date = ((DateLinkedContent)source.Rewards[0].Content).Date;
		_dateIcon.sprite = _date.IconData.Icon;
		SetTitle();
	}

	protected override void TryClaimReward()
	{
		switch (base.Source.Rewards[0].State.Value)
		{
		case EntityStatus.Complete:
			base.Source.Rewards[0].TryCollectReward(isFast: true);
			_dateController.StartDate(_date);
			_rewardClaimed?.OnNext(_id);
			_relationship.RemoveComingSoonDate(base.Source.Rewards[0]);
			_notificationIcon.gameObject.SetActive(value: false);
			_relationshipAnalytic.SendRewardReceivedEvent(_id);
			break;
		case EntityStatus.Rewarded:
			_dateController.StartDate(_date);
			break;
		}
	}

	protected override void SetBlockedState()
	{
		base.SetBlockedState();
		_collectButtonStates.Set(0);
	}

	protected override void SetInProgressState()
	{
		base.SetInProgressState();
		_collectButtonStates.Set(0);
	}

	protected override void SetCompleteState()
	{
		base.SetCompleteState();
		_collectButtonStates.Set(0);
	}

	protected override void SetRewardedState()
	{
		base.SetRewardedState();
		_collectButtonStates.Set(1);
	}

	protected override void CheckNotification()
	{
		base.CheckNotification();
		bool active = _relationship.WasComingSoonDate(base.Source.Rewards[0]);
		_notificationIcon.gameObject.SetActive(active);
	}

	private void SetTitle()
	{
		_dateTitle.text = _localizationService.Text($"content.dates.name.{_date.ID}");
	}
}
