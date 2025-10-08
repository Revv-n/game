using System;
using GreenT.HornyScapes.NetConnection;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace GreenT.HornyScapes.Cheats;

public class CheatNetConnection : MonoBehaviour
{
	public TMP_Text Title;

	public Image Image;

	private NetConnectionSystem system;

	public void Track()
	{
		if (system != null)
		{
			DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<bool>((IObservable<bool>)system.IsPinging, (Action<bool>)SetStatus), (Component)this);
			Title.text = GetType().Name;
		}
	}

	private void SetStatus(bool state)
	{
		Image.color = (state ? Color.green : Color.red);
	}
}
