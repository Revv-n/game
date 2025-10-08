using System;
using TMPro;
using UniRx;
using UnityEngine;
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

	public IObservable<int> OnClick => _onClick;

	public void Set(int value)
	{
		_disposable?.Clear();
		SubscribeToButtonClick(value);
		SetText(value);
	}

	private void SubscribeToButtonClick(int value)
	{
		_button.onClick.AsObservable().Subscribe(delegate
		{
			_onClick.OnNext(value);
		}).AddTo(_disposable);
	}

	private void SetText(int value)
	{
		string text = ((value >= 0) ? "+" : "-");
		string text2 = ((Math.Abs(value) > 1) ? Math.Abs(value).ToString() : "");
		_text.text = text + text2;
	}
}
