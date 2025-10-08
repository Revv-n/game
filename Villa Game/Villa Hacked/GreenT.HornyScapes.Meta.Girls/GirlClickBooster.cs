using System;
using System.Collections.Generic;
using GreenT.HornyScapes.Constants;
using Spine.Unity;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

namespace GreenT.HornyScapes.Meta.Girls;

public class GirlClickBooster : MonoBehaviour, IPointerDownHandler, IEventSystemHandler
{
	private SkeletonAnimation skeletonAnimation;

	private float scaleAddPower;

	private float scaleAddTime;

	private float initTimeScale;

	private CompositeDisposable changeTimeScaleStream = new CompositeDisposable();

	[Inject]
	public void Init(IConstants<float> floatConstants)
	{
		scaleAddPower = floatConstants["girl_click_scale_animation"];
		scaleAddTime = floatConstants["girl_click_scale_time"];
	}

	public void Init(SkeletonAnimation skeletonAnimation)
	{
		this.skeletonAnimation = skeletonAnimation;
		initTimeScale = skeletonAnimation.timeScale;
	}

	public void OnPointerDown(PointerEventData eventData)
	{
		if ((bool)skeletonAnimation)
		{
			ChangeTimeScale(scaleAddPower);
			DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<long>(Observable.Timer(TimeSpan.FromSeconds(scaleAddTime)), (Action<long>)delegate
			{
				ChangeTimeScale(0f - scaleAddPower);
			}), (ICollection<IDisposable>)changeTimeScaleStream);
		}
	}

	private void ChangeTimeScale(float power)
	{
		skeletonAnimation.timeScale += power;
	}

	private void OnDisable()
	{
		CompositeDisposable obj = changeTimeScaleStream;
		if (obj != null)
		{
			obj.Dispose();
		}
		if (skeletonAnimation != null)
		{
			skeletonAnimation.timeScale = initTimeScale;
		}
	}

	private void OnDestroy()
	{
		if (skeletonAnimation != null)
		{
			skeletonAnimation.timeScale = initTimeScale;
		}
		CompositeDisposable obj = changeTimeScaleStream;
		if (obj != null)
		{
			obj.Dispose();
		}
	}
}
