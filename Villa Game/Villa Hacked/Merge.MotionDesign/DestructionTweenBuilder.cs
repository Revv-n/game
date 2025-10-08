using DG.Tweening;
using UnityEngine;

namespace Merge.MotionDesign;

public class DestructionTweenBuilder : MonoBehaviour
{
	[SerializeField]
	private float time = 0.14f;

	[SerializeField]
	private float fadeTimeMul = 1f;

	[SerializeField]
	private Ease fadeEase = Ease.InSine;

	[SerializeField]
	private GameObject animationPrefab;

	[SerializeField]
	private Vector3[] jamks;

	public Tween BuildTween(GIGhost ghost)
	{
		GameObject fogAnimation = null;
		Sequence sequence = DOTween.Sequence();
		sequence.AppendCallback(CreateAnimation);
		for (int i = 0; i < jamks.Length; i++)
		{
			sequence.Append(ghost.transform.DOScale(jamks[i], time));
		}
		sequence.Insert(0f, DOTweenModuleSprite.DOFade(ghost.IconRenderer, 0f, time * (float)jamks.Length * fadeTimeMul).SetEase(fadeEase));
		return sequence;
		void CreateAnimation()
		{
			fogAnimation = Object.Instantiate(animationPrefab);
			fogAnimation.transform.SetParent(ghost.transform.parent);
			fogAnimation.transform.position = ghost.transform.position;
		}
	}
}
