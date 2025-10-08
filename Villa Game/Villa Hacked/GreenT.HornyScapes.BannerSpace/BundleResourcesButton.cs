using System;
using System.Collections.Generic;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace GreenT.HornyScapes.BannerSpace;

public class BundleResourcesButton : MonoBehaviour
{
	[SerializeField]
	private Button _button;

	[SerializeField]
	private TMP_Text _text;

	private readonly Subject<int> _onClick = new Subject<int>();

	private readonly CompositeDisposable _disposable = new CompositeDisposable();

	public IObservable<int> OnClick => (IObservable<int>)_onClick;

	public void Set(int value)
	{
		CompositeDisposable disposable = _disposable;
		if (disposable != null)
		{
			disposable.Clear();
		}
		SubscribeToButtonClick(value);
		SetText(value);
	}

	private void SubscribeToButtonClick(int value)
	{
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<Unit>(UnityEventExtensions.AsObservable((UnityEvent)_button.onClick), (Action<Unit>)delegate
		{
			_onClick.OnNext(value);
		}), (ICollection<IDisposable>)_disposable);
	}

	private void SetText(int value)
	{
		string text = ((value >= 0) ? "+" : "-");
		string text2 = ((Math.Abs(value) > 1) ? Math.Abs(value).ToString() : "");
		_text.text = text + text2;
	}
}
