using System;
using GreenT.HornyScapes.MergeCore;
using GreenT.UI;
using Merge;
using StripClub.UI;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace GreenT.HornyScapes;

public class InfoButton : MonoView<GIKey>
{
	[SerializeField]
	private Button infoButton;

	[SerializeField]
	private WindowOpener windowOpenerOpener;

	private IDisposable clickStream;

	protected virtual void OnEnable()
	{
		clickStream = ObservableExtensions.Subscribe<Unit>(UnityUIComponentExtensions.OnClickAsObservable(infoButton), (Action<Unit>)delegate
		{
			OpenInfoWindow();
		});
	}

	public void OpenInfoWindow()
	{
		if (IsActive())
		{
			Controller<CollectionController>.Instance.ShowWindow(base.Source);
			windowOpenerOpener.Click();
		}
	}

	protected virtual void OnDisable()
	{
		clickStream?.Dispose();
	}
}
