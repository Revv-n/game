using DG.Tweening;
using UnityEngine;

namespace Merge.MotionDesign;

public class BubbleEffect : MonoBehaviour
{
	[SerializeField]
	private SpriteRenderer bubble;

	public Tween BubbleTween { get; set; }

	public SpriteRenderer Bubble => bubble;
}
