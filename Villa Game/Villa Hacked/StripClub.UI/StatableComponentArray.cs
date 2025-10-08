using UnityEngine;

namespace StripClub.UI;

public abstract class StatableComponentArray<TModifiers> : StatableComponent
{
	[SerializeField]
	protected TModifiers[] states;
}
public abstract class StatableComponentArray<TEntity, TModifiers> : StatableComponent
{
	[SerializeField]
	protected TEntity element;

	[SerializeField]
	protected TModifiers[] states;

	private void OnValidate()
	{
		if (element == null)
		{
			TryGetComponent<TEntity>(out element);
		}
		if (states.Length == 0)
		{
			Debug.LogError(GetType().Name + ": states can't be empty", this);
		}
	}
}
