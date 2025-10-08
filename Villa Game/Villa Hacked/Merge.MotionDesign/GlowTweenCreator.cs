using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Merge.MotionDesign;

public class GlowTweenCreator : MonoBehaviour
{
	[SerializeField]
	private Transform target;

	[SerializeField]
	private Transform buttonPlay;

	[SerializeField]
	private SpriteRenderer glow1sr;

	[SerializeField]
	private SpriteRenderer glow2sr;

	[SerializeField]
	private float time;

	[SerializeField]
	private float flyTime;

	[SerializeField]
	private Transform[] startPositions;

	private Transform fakeButtonPlay;

	public Tween DoEffect(IList<GIGhost> toFly)
	{
		CreateFakeButton();
		Sequence sequence = DOTween.Sequence();
		sequence.Join(DOTweenModuleSprite.DOFade(glow1sr, 1f, 0.1f));
		sequence.Append(glow2sr.transform.DOScale(3.8f, time));
		sequence.Join(DOTweenModuleSprite.DOFade(glow2sr, 1f, time));
		sequence.Append(DOTweenModuleSprite.DOFade(glow1sr, 0f, time));
		sequence.Append(DOTweenModuleSprite.DOFade(glow2sr, 0f, time));
		sequence.Join(BuildFlyAllItems());
		sequence.onComplete = (TweenCallback)Delegate.Combine(sequence.onComplete, new TweenCallback(CompleteCallback));
		return sequence;
		Tween BuildFlyAllItems()
		{
			Sequence sequence4 = DOTween.Sequence();
			float num = time + flyTime / 2f;
			for (int i = 0; i < toFly.Count; i++)
			{
				sequence4.Insert(num * (float)i, BuildItemTween(i));
			}
			return sequence4;
		}
		Tween BuildItemTween(int index)
		{
			GIGhost gIGhost = toFly[index];
			gIGhost.IconRenderer.SetOrder(glow1sr, 5);
			Sequence sequence2 = DOTween.Sequence();
			sequence2.Join(EasyBezierTweener.DoBezier(b: new Vector3(target.position.x, target.position.y), fly: gIGhost.transform, a: startPositions[index].position, time: flyTime));
			sequence2.Join(gIGhost.transform.DOScale(1.4f, flyTime / 2f));
			sequence2.Insert(flyTime / 2f, gIGhost.transform.DOScale(1f, flyTime / 2f));
			Sequence sequence3 = DOTween.Sequence();
			sequence3.Append(gIGhost.transform.DOScale(0.8f, time));
			sequence3.Append(sequence2);
			sequence3.Append(DOTweenModuleSprite.DOFade(gIGhost.IconRenderer, 0f, time));
			sequence3.Join(buttonPlay.DOScale(1.4f, time));
			sequence3.Append(buttonPlay.DOScale(1f, time));
			return sequence3;
		}
		void CompleteCallback()
		{
			foreach (GIGhost item in toFly)
			{
				UnityEngine.Object.Destroy(item.gameObject);
			}
			UnityEngine.Object.Destroy(fakeButtonPlay.gameObject);
		}
	}

	private void CreateFakeButton()
	{
		fakeButtonPlay = UnityEngine.Object.Instantiate(buttonPlay.gameObject).transform;
		fakeButtonPlay.SetParent(UIMaster.OverlayUiCanvas.transform);
		fakeButtonPlay.SetScale(1f);
		fakeButtonPlay.position = buttonPlay.transform.position;
		fakeButtonPlay.GetComponent<CanvasGroup>().alpha = 1f;
		fakeButtonPlay.GetComponent<Button>().enabled = false;
	}
}
