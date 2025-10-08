using System.Collections;
using UnityEngine;

namespace StripClub.UI;

public abstract class StatableComponentDict<TEntity, TModifiers> : StatableComponent where TModifiers : IDictionary
{
	[SerializeField]
	protected TEntity element;

	[SerializeField]
	protected TModifiers states;

	private void OnValidate()
	{
		if (element == null)
		{
			TryGetComponent<TEntity>(out element);
		}
		if (states.Count == 0)
		{
			Debug.LogError(GetType().Name + ": states can't be empty", this);
		}
	}
}
