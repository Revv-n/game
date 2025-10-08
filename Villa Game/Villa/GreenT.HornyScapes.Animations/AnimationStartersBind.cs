using System;
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
		stream?.Clear();
		BindStarters(fromStarter, toStarter);
	}

	private void BindStarters(AnimationStarter fromStarter, AnimationStarter toStarter)
	{
		fromStarter.OnEnd.First().Subscribe(delegate
		{
			LaunchStarter(toStarter);
		}, delegate(Exception ex)
		{
			ex.LogException();
		}).AddTo(stream);
	}

	private void LaunchStarter(AnimationStarter starter)
	{
		starter.PlayAnimation();
		starter.OnEnd.First().DoOnCancel(delegate
		{
			starter.ResetAnimation();
		}).Subscribe(delegate
		{
			starter.ResetAnimation();
		}, delegate(Exception ex)
		{
			ex.LogException();
		})
			.AddTo(stream);
	}

	private void OnDestroy()
	{
		stream.Dispose();
	}
}
