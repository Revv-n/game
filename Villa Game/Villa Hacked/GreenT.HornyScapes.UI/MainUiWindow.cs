using System;
using System.Collections.Generic;
using GreenT.HornyScapes.Animations;
using GreenT.HornyScapes.Tasks.UI;
using GreenT.UI;
using UniRx;
using UnityEngine;

namespace GreenT.HornyScapes.UI;

public class MainUiWindow : Window
{
	[SerializeField]
	private AnimationSetOpenCloseController generalStarters;

	private CompositeDisposable disposables = new CompositeDisposable();

	public TaskBookNotify TaskNotify;

	protected override void Awake()
	{
		base.Awake();
		generalStarters.InitOpeners();
	}

	public override void Open()
	{
		disposables.Clear();
		base.Open();
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<AnimationSetOpenCloseController>(Observable.DoOnCancel<AnimationSetOpenCloseController>(generalStarters.Open(), (Action)generalStarters.InitClosers), (Action<AnimationSetOpenCloseController>)delegate
		{
			generalStarters.InitClosers();
		}), (ICollection<IDisposable>)disposables);
	}

	public override void Close()
	{
		disposables.Clear();
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<AnimationSetOpenCloseController>(Observable.DoOnCancel<AnimationSetOpenCloseController>(generalStarters.Close(), (Action)base.Close), (Action<AnimationSetOpenCloseController>)delegate
		{
			base.Close();
		}), (ICollection<IDisposable>)disposables);
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		disposables.Dispose();
	}
}
