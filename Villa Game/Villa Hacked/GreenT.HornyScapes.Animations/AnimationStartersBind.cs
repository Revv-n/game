using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace GreenT.HornyScapes.Animations;

public class AnimationStartersBind : MonoBehaviour
{
	[Header("EditorOnly. Назначение.")]
	[SerializeField]
	private string description;

	[SerializeField]
	private AnimationStarter fromStarter;

	[SerializeField]
	private AnimationStarter toStarter;

	private CompositeDisposable stream = new CompositeDisposable();

	public void Set(AnimationStarter fromStarter, AnimationStarter toStarter)
	{
		this.fromStarter = fromStarter;
		this.toStarter = toStarter;
	}

	public void InitStarters()
	{
		fromStarter.Init();
		toStarter.Init();
	}

	public void Bind()
	{
		CompositeDisposable obj = stream;
		if (obj != null)
		{
			obj.Clear();
		}
		BindStarters(fromStarter, toStarter);
	}

	private void BindStarters(AnimationStarter fromStarter, AnimationStarter toStarter)
	{
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<AnimationDependedStarter>(Observable.First<AnimationDependedStarter>(fromStarter.OnEnd), (Action<AnimationDependedStarter>)delegate
		{
			LaunchStarter(toStarter);
		}, (Action<Exception>)delegate(Exception ex)
		{
			ex.LogException();
		}), (ICollection<IDisposable>)stream);
	}

	private void LaunchStarter(AnimationStarter starter)
	{
		starter.PlayAnimation();
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<AnimationDependedStarter>(Observable.DoOnCancel<AnimationDependedStarter>(Observable.First<AnimationDependedStarter>(starter.OnEnd), (Action)delegate
		{
			starter.ResetAnimation();
		}), (Action<AnimationDependedStarter>)delegate
		{
			starter.ResetAnimation();
		}, (Action<Exception>)delegate(Exception ex)
		{
			ex.LogException();
		}), (ICollection<IDisposable>)stream);
	}

	private void OnDestroy()
	{
		stream.Dispose();
	}
}
