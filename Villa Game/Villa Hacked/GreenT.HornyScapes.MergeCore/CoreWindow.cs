using System;
using System.Collections.Generic;
using GreenT.HornyScapes.Animations;
using GreenT.UI;
using TMPro;
using UniRx;
using UnityEngine;
using Zenject;

namespace GreenT.HornyScapes.MergeCore;

public class CoreWindow : Window
{
	public const string counter_key = "{0}/{1}";

	public ScaleButton uiButton;

	public TMP_Text inventoryCounter;

	[SerializeField]
	private RewardContainer rewardContainer;

	[SerializeField]
	private AnimationSetOpenCloseController starters;

	private CompositeDisposable disposables = new CompositeDisposable();

	[Inject]
	public override void Init(IWindowsManager windowsOpener)
	{
		base.Init(windowsOpener);
		starters.InitOpeners();
	}

	public override void Open()
	{
		disposables.Clear();
		base.Open();
		rewardContainer.Display(display: true);
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<AnimationSetOpenCloseController>(Observable.DoOnCancel<AnimationSetOpenCloseController>(starters.Open(), (Action)starters.InitClosers), (Action<AnimationSetOpenCloseController>)delegate
		{
			starters.InitClosers();
		}), (ICollection<IDisposable>)disposables);
	}

	public override void Close()
	{
		disposables.Clear();
		rewardContainer.Display(display: false);
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<AnimationSetOpenCloseController>(Observable.DoOnCancel<AnimationSetOpenCloseController>(starters.Close(), (Action)base.Close), (Action<AnimationSetOpenCloseController>)delegate
		{
			base.Close();
		}), (ICollection<IDisposable>)disposables);
	}

	private void OnDisable()
	{
		disposables.Clear();
	}

	public void UpdateInventoryCounter(int storedItems, int maxPlace)
	{
		inventoryCounter.text = $"{storedItems}/{maxPlace}";
	}
}
