using System;
using System.Linq;
using StripClub.Model;
using UnityEngine;

namespace GreenT.HornyScapes.MergeCore;

public class CollectFlyTargetsContainer : MonoBehaviour
{
	[Serializable]
	public class FlyTargetNode
	{
		[HideInInspector]
		public string Name;

		public CurrencyType CurrencyType;

		public Transform Target;
	}

	[SerializeField]
	private Transform _defaultFlyTarget;

	[SerializeField]
	private FlyTargetNode[] _targets;

	public Transform DefaultFlyTarget => _defaultFlyTarget;

	public Transform GetPosition(CurrencyType currencyType)
	{
		FlyTargetNode flyTargetNode = _targets.FirstOrDefault((FlyTargetNode x) => x.CurrencyType == currencyType);
		if (flyTargetNode != null)
		{
			return flyTargetNode.Target;
		}
		return _defaultFlyTarget;
	}

	private void OnValidate()
	{
		FlyTargetNode[] targets = _targets;
		foreach (FlyTargetNode flyTargetNode in targets)
		{
			if (!flyTargetNode.Name.Equals(flyTargetNode.CurrencyType.ToString()))
			{
				flyTargetNode.Name = flyTargetNode.CurrencyType.ToString();
			}
		}
	}
}
