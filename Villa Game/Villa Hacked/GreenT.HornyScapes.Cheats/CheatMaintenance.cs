using System;
using GreenT.HornyScapes.Maintenance;
using GreenT.Net;
using GreenT.Settings.Data;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace GreenT.HornyScapes.Cheats;

public class CheatMaintenance : MonoBehaviour
{
	public TMP_Text Title;

	public Image Image;

	private MaintenanceListener _listener;

	private void Constructor(MaintenanceListener system)
	{
		_listener = system;
	}

	public void Track()
	{
		if (_listener != null)
		{
			Title.text = GetType().Name;
			DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<Response<ConfigurationInfo>>(_listener.Ping, (Action<Response<ConfigurationInfo>>)ChangeColor), (Component)this);
		}
	}

	private void ChangeColor(Response<ConfigurationInfo> response)
	{
		Image.color = ((Image.color == Color.green) ? Color.red : Color.green);
	}
}
