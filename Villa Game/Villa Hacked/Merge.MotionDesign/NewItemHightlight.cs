using System;
using DG.Tweening;
using UnityEngine;

namespace Merge.MotionDesign;

public class NewItemHightlight : MonoBehaviour
{
	[SerializeField]
	private float showTime = 0.4f;

	[SerializeField]
	private float lifeTime = 5f;

	private Tween tween;

	private Tween lifeTween;

	private GameItem target;

	private void Start()
	{
		base.transform.SetScale(0f);
		tween = base.transform.DOScale(1f, showTime);
		target = GetComponentInParent<GameItem>();
		if (target != null)
		{
			target.OnBecomeDrag += Remove;
			target.OnRemoving += ForceFemove;
			lifeTween = DOVirtual.Float(0f, lifeTime, lifeTime, delegate
			{
			}).OnComplete(delegate
			{
				Remove();
			});
		}
	}

	public void Remove(GameItem item = null)
	{
		tween?.Kill();
		lifeTween?.Kill();
		tween = base.transform.DOScale(0f, showTime);
		if (target != null)
		{
			target.OnBecomeDrag -= Remove;
			Tween obj = tween;
			obj.onComplete = (TweenCallback)Delegate.Combine(obj.onComplete, (TweenCallback)delegate
			{
				target.OnRemoving -= ForceFemove;
				UnityEngine.Object.Destroy(base.gameObject);
			});
		}
	}

	public static NewItemHightlight Create(Transform parent)
	{
		NewItemHightlight newItemHightlight = UnityEngine.Object.Instantiate(Resources.Load<NewItemHightlight>("Effects/NewItemHighlight"));
		newItemHightlight.transform.SetParent(parent);
		newItemHightlight.transform.SetDefault();
		return newItemHightlight;
	}

	private void ForceFemove(GameItem sender)
	{
		Debug.Log("ForceRemove!");
		tween?.Kill();
		lifeTween?.Kill();
		sender.OnBecomeDrag -= Remove;
		sender.OnRemoving -= ForceFemove;
		UnityEngine.Object.Destroy(base.gameObject);
	}

	private void OnDestroy()
	{
		tween?.Kill();
		lifeTween?.Kill();
	}
}
