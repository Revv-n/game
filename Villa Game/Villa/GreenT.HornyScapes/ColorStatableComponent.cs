using StripClub.UI;
using UnityEngine;

namespace GreenT.HornyScapes;

public abstract class ColorStatableComponent<TEntity> : StatableComponentArray<TEntity, Color>
{
	public override void Set(int stateNumber)
	{
		SetColor(stateNumber);
	}

	protected abstract void SetColor(int stateNumber);
}
