using System;
using StripClub.UI;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace GreenT.HornyScapes.Events.BattlePassRewardCards;

public class PremiumConditionsLock : MonoBehaviour, IConditionsLock, IDisposable
{
	[SerializeField]
	private Image lockObject;

	[SerializeField]
	private Sprite unlockSprite;

	[SerializeField]
	private Sprite lockSprite;

	[SerializeField]
	private LocalizedTextMeshPro text;

	[SerializeField]
	private GameObject textObject;

	[SerializeField]
	private GameObject root;

	private IDisposable subscribe;

	public void Initialization(IConditionReceivingReward conditionReceiving)
	{
		root.SetActive(value: true);
		text.Init(conditionReceiving.ConditionText);
		if (!IsDisabled(conditionReceiving.State.Value))
		{
			subscribe = conditionReceiving.State.Where(IsDisabled).Subscribe(delegate
			{
				OnCompleted();
			});
		}
		else
		{
			OnCompleted();
		}
	}

	private static bool IsDisabled(ConditionState p)
	{
		if (p != ConditionState.Completed)
		{
			return p == ConditionState.Disabled;
		}
		return true;
	}

	private void OnCompleted()
	{
		textObject.SetActive(value: false);
		root.SetActive(value: false);
		lockObject.sprite = unlockSprite;
		Dispose();
	}

	public void Reset()
	{
		textObject.SetActive(value: true);
		lockObject.sprite = lockSprite;
		root.SetActive(value: false);
		Dispose();
	}

	public void Dispose()
	{
		subscribe?.Dispose();
		subscribe = null;
	}

	private void OnDestroy()
	{
		Dispose();
	}
}
