using StripClub.UI;
using UnityEngine;

namespace GreenT.HornyScapes.Relationships.Views;

public sealed class StatusView : MonoView<int>
{
	[SerializeField]
	private SpriteStates _spriteStates;

	public override void Set(int source)
	{
		base.Set(source);
		_spriteStates.Set(base.Source);
	}
}
