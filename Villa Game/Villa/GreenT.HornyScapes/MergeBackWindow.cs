using System;
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
		animations.Open().DoOnCancel(animations.InitClosers).Subscribe(delegate
		{
			animations.InitClosers();
		}, delegate(Exception ex)
		{
			ex.LogException();
		})
			.AddTo(disposables);
	}

	public override void Close()
	{
		disposables.Clear();
		animations.Close().DoOnCancel(delegate
		{
			base.Close();
		}).Subscribe(delegate
		{
			base.Close();
		}, delegate(Exception ex)
		{
			ex.LogException();
		})
			.AddTo(disposables);
	}
}
