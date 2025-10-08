using System;
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
		_starters.Open().DoOnCancel(_starters.InitClosers).Subscribe(delegate
		{
			_starters.InitClosers();
		})
			.AddTo(_disposables);
	}

	public override void Close()
	{
		_disposables.Clear();
		_starters.Close().DoOnCancel(base.Close).Subscribe(delegate
		{
			base.Close();
		})
			.AddTo(_disposables);
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		_calendarStream?.Dispose();
	}
}
