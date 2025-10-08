using System;
using System.Collections.Generic;
using GreenT.HornyScapes.Animations;
using GreenT.HornyScapes.Events.Content;
using GreenT.Types;
using GreenT.UI;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace GreenT.HornyScapes;

public class MergeBackWindow : Window
{
	[SerializeField]
	private AnimationSetOpenCloseController animations;

	[SerializeField]
	private Image bg;

	[SerializeField]
	private Sprite bgSp;

	private Sprite eventBgSp;

	private CompositeDisposable disposables = new CompositeDisposable();

	[Inject]
	private ContentSelectorGroup contentSelectorGroup;

	[Inject]
	public override void Init(IWindowsManager windowsOpener)
	{
		base.Init(windowsOpener);
		animations.InitOpeners();
	}

	public void Init()
	{
		Image image = bg;
		ContentSelectorGroup obj = contentSelectorGroup;
		image.sprite = ((obj != null && obj.Current == ContentType.Main) ? bgSp : eventBgSp);
	}

	public void Set(Sprite eventBgSp)
	{
		this.eventBgSp = eventBgSp;
	}

	public override void Open()
	{
		Init();
		disposables.Clear();
		base.Open();
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<AnimationSetOpenCloseController>(Observable.DoOnCancel<AnimationSetOpenCloseController>(animations.Open(), (Action)animations.InitClosers), (Action<AnimationSetOpenCloseController>)delegate
		{
			animations.InitClosers();
		}, (Action<Exception>)delegate(Exception ex)
		{
			ex.LogException();
		}), (ICollection<IDisposable>)disposables);
	}

	public override void Close()
	{
		disposables.Clear();
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<AnimationSetOpenCloseController>(Observable.DoOnCancel<AnimationSetOpenCloseController>(animations.Close(), (Action)delegate
		{
			base.Close();
		}), (Action<AnimationSetOpenCloseController>)delegate
		{
			base.Close();
		}, (Action<Exception>)delegate(Exception ex)
		{
			ex.LogException();
		}), (ICollection<IDisposable>)disposables);
	}
}
