using UnityEngine;

namespace StripClub.UI;

public class SiblingIndexState : StatableComponent
{
	[SerializeField]
	private Transform target;

	[SerializeField]
	private IntTransformDictionary states;

	public override void Set(int stateNumber)
	{
		int siblingIndex = states[stateNumber].GetSiblingIndex();
		target.SetSiblingIndex(siblingIndex);
	}

	private void OnValidate()
	{
		if (target == null)
		{
			target = base.transform;
		}
	}
}
