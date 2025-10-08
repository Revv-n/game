using System;
using GreenT.HornyScapes.Lootboxes;
using UnityEngine;

namespace GreenT.HornyScapes;

[Serializable]
public class Reward
{
	[SerializeField]
	private RewType type;

	[SerializeField]
	private string selectorString;

	private Selector selector;

	public RewType Type => type;

	public Selector GetSelector()
	{
		if (selector == null)
		{
			return selector = SelectorTools.CreateSelector(selectorString);
		}
		return selector;
	}
}
