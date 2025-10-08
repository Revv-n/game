using System;
using GreenT.HornyScapes.Animations;
using Merge;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace GreenT.HornyScapes.MergeCore;

public class SellConfirmationWindow : PopupWindow
{
	[SerializeField]
	private Image _icon;

	[SerializeField]
	private TMP_Text _price;

	[SerializeField]
	private Button _confirmButton;

	private IDisposable _buttonClickStream;

	public void Setup(GIBox.Sell sellBox, Action sellAction)
	{
		_icon.sprite = sellBox.Parent.Icon;
		_price.text = sellBox.Config.Price.ToString();
		_buttonClickStream = ObservableExtensions.Subscribe<Unit>(UnityUIComponentExtensions.OnClickAsObservable(_confirmButton), (Action<Unit>)delegate
		{
			sellAction?.Invoke();
			Close();
		});
	}

	public override void Close()
	{
		_buttonClickStream?.Dispose();
		Controller<SelectionController>.Instance.ClearSelection();
		base.Close();
	}
}
