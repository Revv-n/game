using UnityEngine;

namespace Merge.Meta;

public class RoomObjectBonusImage : MonoBehaviour
{
	[SerializeField]
	private SpriteRenderer sr;

	[SerializeField]
	private Animator anim;

	public SpriteRenderer View => sr;

	public Animator Anim => anim;

	public void SetValues(int order, Sprite sprite, Vector3 pos)
	{
		sr.SetOrder(order);
		sr.SetSprite(sprite);
		base.transform.localPosition = pos;
	}
}
