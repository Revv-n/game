using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace Merge.MotionDesign;

public class BubbleTweenCreator : MonoBehaviour
{
	[Serializable]
	public struct BubbleIdleInfo
	{
		public float giSize;

		public float time;

		public float bubbleSize;

		public float bubbleAlpha;

		public float bubbleRotation;

		public float itemSize;

		public float itemAlpha;
	}

	[Serializable]
	public struct PopBubbleInfo
	{
		public float bubbleScale;

		public float popTime;

		public float backTime;

		public float bigScale;

		public float destroyGhostTime;
	}

	[SerializeField]
	private BubbleEffect prefab;

	[SerializeField]
	private BubbleIdleInfo idleInfo;

	[SerializeField]
	private PopBubbleInfo popItemInfo;

	[SerializeField]
	private GameObject particlesPrefab;

	public BubbleEffect CreateBubbleEffect(GameItem gi)
	{
		BubbleEffect bubbleEffect = UnityEngine.Object.Instantiate(prefab, gi.transform);
		bubbleEffect.transform.SetParent(gi.IconRenderer.transform.parent);
		bubbleEffect.transform.localScale = Vector3.one;
		bubbleEffect.transform.localPosition = Vector3.zero;
		gi.IconRenderer.transform.localScale = Vector3.one * idleInfo.giSize;
		Sequence sequence = DOTween.Sequence();
		sequence.Join(bubbleEffect.Bubble.transform.DOScale(idleInfo.bubbleSize, idleInfo.time));
		sequence.Join(DOTweenModuleSprite.DOFade(bubbleEffect.Bubble, idleInfo.bubbleAlpha, idleInfo.time));
		sequence.Join(bubbleEffect.Bubble.transform.DORotate(new Vector3(0f, 0f, idleInfo.bubbleRotation), 1f));
		sequence.Join(gi.IconRenderer.transform.DOScale(idleInfo.itemSize, idleInfo.time));
		sequence.Join(DOTweenModuleSprite.DOFade(gi.IconRenderer, idleInfo.itemAlpha, idleInfo.time));
		sequence.SetLoops(-1, LoopType.Yoyo);
		sequence.SetId(gi);
		bubbleEffect.BubbleTween = sequence;
		return bubbleEffect;
	}

	public Tween PopFromBubble(GameItem poped, GIGhost bubbled, BubbleEffect effect)
	{
		poped.transform.SetScale(0f);
		poped.IconRenderer.SetAlpha(0f);
		effect.BubbleTween?.Kill();
		DOTween.Sequence();
		Sequence sequence = DOTween.Sequence();
		List<GameObject> toDestroy = new List<GameObject> { bubbled.gameObject, effect.gameObject };
		CreateSoap(poped.transform);
		sequence.Join(bubbled.transform.DOScale(0f, popItemInfo.destroyGhostTime));
		sequence.Join(effect.transform.DOScale(effect.transform.localScale.x * popItemInfo.bubbleScale, popItemInfo.destroyGhostTime));
		sequence.Join(DOTweenModuleSprite.DOFade(effect.Bubble, 0f, popItemInfo.destroyGhostTime));
		effect.BubbleTween = sequence;
		sequence.SetId(effect.gameObject);
		sequence.onComplete = (TweenCallback)Delegate.Combine(sequence.onComplete, new TweenCallback(DestroyTrash));
		sequence.onKill = (TweenCallback)Delegate.Combine(sequence.onKill, new TweenCallback(DestroyTrash));
		return poped.DoCreate();
		void DestroyTrash()
		{
			toDestroy.ForEach(delegate(GameObject x)
			{
				UnityEngine.Object.Destroy(x);
			});
		}
	}

	public Tween UnlockBubble(GameItem gi, BubbleEffect effect)
	{
		effect.transform.SetParent(gi.transform.parent);
		effect.BubbleTween?.Kill();
		Sequence sequence = DOTween.Sequence();
		Sequence sequence2 = DOTween.Sequence();
		sequence2.Append(effect.transform.DOScale(effect.transform.localScale.x * popItemInfo.bubbleScale, popItemInfo.destroyGhostTime));
		sequence2.Join(DOTweenModuleSprite.DOFade(effect.Bubble, 0f, popItemInfo.destroyGhostTime));
		effect.BubbleTween = sequence2;
		sequence2.onComplete = (TweenCallback)Delegate.Combine(sequence2.onComplete, new TweenCallback(DestroyTrash));
		sequence.AppendCallback(delegate
		{
			CreateSoap(gi.transform);
		});
		sequence.Join(gi.IconRenderer.transform.DOScale(1f, popItemInfo.destroyGhostTime));
		sequence.Join(DOTweenModuleSprite.DOFade(gi.IconRenderer, 1f, popItemInfo.destroyGhostTime));
		gi.AppendOuterTween(sequence);
		return sequence;
		void DestroyTrash()
		{
			UnityEngine.Object.Destroy(effect.gameObject);
		}
	}

	private void CreateSoap(Transform parent)
	{
		UnityEngine.Object.Instantiate(particlesPrefab, parent);
	}
}
