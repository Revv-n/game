using System;
using GreenT.UI;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace GreenT.HornyScapes.Tasks.UI;

public abstract class BaseButtonStrategy : MonoBehaviour
{
	public Button Button;

	public WindowOpener WindowOpener;

	protected Action _claimReward;

	protected bool _isClaimRewardState;

	protected ButtonStrategyProvider buttonStrategyProvider;

	protected Task source;

	protected bool hasStrategy;

	private bool isCompleteState;

	[Inject]
	private void Constructor(ButtonStrategyProvider buttonStrategyProvider)
	{
		this.buttonStrategyProvider = buttonStrategyProvider;
	}

	private void Awake()
	{
		Button.onClick.AddListener(OnClick);
	}

	public void Set(Task task)
	{
		source = task;
		_claimReward = null;
		_isClaimRewardState = false;
		HasStrategy(source.Goal.ActionButtonType);
		CheckInteractable();
	}

	public void SetState(bool completeState)
	{
		isCompleteState = completeState;
	}

	public void SetInteractable(bool isOn)
	{
		Button.interactable = isOn;
	}

	public void SetActive(bool isActive)
	{
		Button.gameObject.SetActive(isActive);
	}

	public void CheckInteractable()
	{
		SetInteractable(hasStrategy || isCompleteState);
	}

	private void OnClick()
	{
		if (_isClaimRewardState)
		{
			if (_claimReward != null)
			{
				_claimReward();
			}
		}
		else if (isCompleteState)
		{
			Complete();
		}
		else if (hasStrategy)
		{
			TransitionToWindow();
		}
	}

	private void Complete()
	{
		SetInteractable(isOn: false);
		SetRewardState();
	}

	protected abstract void SetRewardState();

	protected virtual void HasStrategy(ActionButtonType actionButtonType)
	{
		hasStrategy = buttonStrategyProvider.TryGetStrategy(source.Goal.ActionButtonType, out WindowOpener);
	}

	protected virtual void TransitionToWindow()
	{
		WindowOpener.ClickSelector();
	}

	private void OnDestroy()
	{
		Button.onClick.RemoveListener(OnClick);
	}
}
