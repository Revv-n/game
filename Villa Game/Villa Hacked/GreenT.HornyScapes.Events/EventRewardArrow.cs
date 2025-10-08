using System.Collections.Generic;
using StripClub.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GreenT.HornyScapes.Events;

public class EventRewardArrow : StatableComponent
{
	[SerializeField]
	private Image iconTarget;

	[SerializeField]
	private List<TMP_Text> targetCount = new List<TMP_Text>();

	[SerializeField]
	private List<Transform> targets = new List<Transform>();

	public override void Set(int stateNumber)
	{
		foreach (Transform target in targets)
		{
			target.gameObject.SetActive(value: false);
		}
		targets[stateNumber].gameObject.SetActive(value: true);
	}

	public void Set(BaseReward source, Sprite sourceBundleTarget)
	{
		foreach (TMP_Text item in targetCount)
		{
			item.text = source.Target.ToString();
		}
		if (iconTarget != null)
		{
			iconTarget.sprite = sourceBundleTarget;
		}
	}
}
