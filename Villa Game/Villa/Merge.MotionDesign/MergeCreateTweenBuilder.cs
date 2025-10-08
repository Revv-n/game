using DG.Tweening;
using UnityEngine;

namespace Merge.MotionDesign;

public class MergeCreateTweenBuilder : MonoBehaviour
{
	[SerializeField]
	private float downScaleTime = 0.3f;

	[SerializeField]
	private float becomesVisibleTime = 0.2f;

	[SerializeField]
	private float downScaleSize = 0.3f;

	[SerializeField]
	private float downFade = 0.05f;

	[SerializeField]
	private Ease downEase;

	public Tween BuildTweeen(SpriteRenderer drag, SpriteRenderer placed, GameItem created)
	{
		created.gameObject.SetActive(value: false);
		created.transform.localScale = Vector3.one * downScaleSize;
		Sequence sequence = DOTween.Sequence();
		sequence.Append(drag.transform.DOMove(placed.transform.position, downScaleTime).SetEase(downEase)).Join(drag.transform.DOScale(downScaleSize, downScaleTime).SetEase(downEase)).Join(drag.DOFade(downFade, downScaleTime).SetEase(downEase))
			.Join(placed.transform.DOScale(downScaleSize, downScaleTime).SetEase(downEase))
			.Join(placed.DOFade(downFade, downScaleTime).SetEase(downEase));
		sequence.InsertCallback(becomesVisibleTime, delegate
		{
			created.gameObject.SetActive(value: true);
			drag.sortingOrder++;
			placed.sortingOrder++;
		});
		sequence.Insert(becomesVisibleTime, created.DoCreate()).InsertCallback(downScaleTime, delegate
		{
			drag.gameObject.SetActive(value: false);
			placed.gameObject.SetActive(value: false);
		});
		return sequence;
	}
}
