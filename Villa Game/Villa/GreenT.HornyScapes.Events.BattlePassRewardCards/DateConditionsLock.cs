using System;
using StripClub.UI;
using UniRx;
using UnityEngine;

namespace GreenT.HornyScapes.Events.BattlePassRewardCards;

public class DateConditionsLock : MonoBehaviour, IConditionsLock, IDisposable
{
	[SerializeField]
	private GameObject textObject;

	[SerializeField]
	private GameObject root;

	[SerializeField]
	private LocalizedTextMeshPro text;

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
		root.SetActive(value: false);
		Dispose();
	}

	public void Reset()
	{
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
