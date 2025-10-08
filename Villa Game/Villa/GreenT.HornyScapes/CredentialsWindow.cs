using System;
using GreenT.HornyScapes.Animations;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace GreenT.HornyScapes;

public class CredentialsWindow : PopupWindow
{
	[SerializeField]
	private Button closeBtn;

	private IDisposable closeButtonStream;

	private void OnEnable()
	{
		closeButtonStream = closeBtn.OnClickAsObservable().Subscribe(delegate
		{
			Close();
		});
	}

	protected override void OnDisable()
	{
		base.OnDisable();
		closeButtonStream?.Dispose();
	}
}
