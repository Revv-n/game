using ModestTree;
using UnityEngine;
using UnityEngine.UI;

namespace GreenT.HornyScapes.Animations;

public abstract class GraphicAnimation : Animation
{
	[SerializeField]
	protected Graphic graphic;

	public Graphic Graphic => graphic;

	private void Awake()
	{
		Assert.IsNotNull((object)graphic);
	}
}
