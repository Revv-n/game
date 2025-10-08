using System.Collections.Generic;
using System.Linq;
using GreenT.HornyScapes.Animations;
using GreenT.HornyScapes.Tasks;
using StripClub.Model;
using StripClub.Model.Quest;
using UnityEngine;
using Zenject;

namespace GreenT.HornyScapes.MiniEvents;

public sealed class CompleteTaskState : MiniEventBaseTaskStateView
{
	[SerializeField]
	private MiniEventTaskItemView _taskView;

	[SerializeField]
	private RectTransformAnimation _hideAnimation;

	private MiniEventsAnimationsService _miniEventsAnimationsService;

	private const double LOOTBOX_SHOW_DELAY = 0.25;

	[Inject]
	private void Init(MiniEventsAnimationsService miniEventsAnimationsService)
	{
		_miniEventsAnimationsService = miniEventsAnimationsService;
	}

	private void Awake()
	{
		_hideAnimation.Init();
	}

	public override void Enter()
	{
		_buttonSpriteStates.Set(_buttonState);
		_textMeshProValueStates.Set(_buttonTextState);
		_buttonStrategy.SetInteractable(source.ReadyToComplete);
		_buttonStrategy.SetClaimReward(OnClaimButtonClick);
	}

	public override void Exit()
	{
		_buttonStrategy.Button.onClick.RemoveListener(OnClaimButtonClick);
	}

	private void OnClaimButtonClick()
	{
		_buttonStrategy.SetInteractable(isOn: false);
		if (source.Reward is CurrencyLinkedContent)
		{
			List<CurrencyLinkedContent> allCurrencyRewards = GetAllCurrencyRewards(source.Reward);
			if (allCurrencyRewards.Any())
			{
				_miniEventsAnimationsService.SetupCurrencyViewInAnimationState();
				LinkedContent reward = source.Reward;
				GiveReward(reward);
				_miniEventsAnimationsService.LaunchCurrencyFlyAnimation(_taskView, allCurrencyRewards);
				Task miniEventTask = source;
				_miniEventsAnimationsService.LaunchTaskHideAnimation(_hideAnimation, delegate
				{
					SetFullComplete(miniEventTask);
				});
			}
		}
		else if (source.Reward is LootboxLinkedContent)
		{
			LinkedContent linkedContent = source.Reward;
			Task miniEventTask = source;
			_miniEventsAnimationsService.LaunchTaskHideAnimation(_hideAnimation, delegate
			{
				GiveReward(linkedContent);
				SetFullComplete(miniEventTask);
			}, 0.25);
		}
		else
		{
			Task miniEventTask = source;
			_miniEventsAnimationsService.LaunchTaskHideAnimation(_hideAnimation, delegate
			{
				SetFullComplete(miniEventTask);
			}, 0.25);
			LinkedContent reward2 = source.Reward;
			_miniEventsAnimationsService.LaunchAnyRewardAnimation(reward2);
		}
	}

	private List<CurrencyLinkedContent> GetAllCurrencyRewards(LinkedContent linkedContent)
	{
		List<CurrencyLinkedContent> list = new List<CurrencyLinkedContent>();
		for (CurrencyLinkedContent next = linkedContent.GetNext<CurrencyLinkedContent>(checkThis: true); next != null; next = next.GetNext<CurrencyLinkedContent>())
		{
			list.Add(next);
		}
		return list;
	}

	private void GiveReward(LinkedContent linkedContent)
	{
		linkedContent.AddCurrentToPlayer();
	}

	private void SetFullComplete(Task miniEventTask)
	{
		miniEventTask.SelectState(StateType.Rewarded);
		_buttonStrategy.SetInteractable(isOn: true);
	}
}
