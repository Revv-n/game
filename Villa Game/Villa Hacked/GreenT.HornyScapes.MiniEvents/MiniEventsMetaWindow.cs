using System;
using System.Collections.Generic;
using GreenT.HornyScapes.Animations;
using GreenT.UI;
using UniRx;
using UnityEngine;

namespace GreenT.HornyScapes.MiniEvents;

public class MiniEventsMetaWindow : Window
{
	[SerializeField]
	private MiniEventsButtonView _battlePassButtonView;

	[SerializeField]
	private AnimationSetOpenCloseController _starters;

	private IDisposable _calendarStream;

	private readonly CompositeDisposable _disposables = new CompositeDisposable();

	public override void Open()
	{
		_disposables.Clear();
		base.Open();
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<AnimationSetOpenCloseController>(Observable.DoOnCancel<AnimationSetOpenCloseController>(_starters.Open(), (Action)_starters.InitClosers), (Action<AnimationSetOpenCloseController>)delegate
		{
			_starters.InitClosers();
		}), (ICollection<IDisposable>)_disposables);
	}

	public override void Close()
	{
		_disposables.Clear();
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<AnimationSetOpenCloseController>(Observable.DoOnCancel<AnimationSetOpenCloseController>(_starters.Close(), (Action)base.Close), (Action<AnimationSetOpenCloseController>)delegate
		{
			base.Close();
		}), (ICollection<IDisposable>)_disposables);
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		_calendarStream?.Dispose();
	}
}
