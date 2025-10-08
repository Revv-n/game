using StripClub.UI;
using UnityEngine;

namespace GreenT.HornyScapes;

public abstract class SpriteStatableComponent<TEntity> : StatableComponentArray<TEntity, Sprite>
{
	public override void Set(int stateNumber)
	{
		SetSprite(stateNumber);
	}

	protected abstract void SetSprite(int stateNumber);
}
