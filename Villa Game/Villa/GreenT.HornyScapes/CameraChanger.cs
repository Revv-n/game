using System;
using System.Collections.Generic;
using GreenT.HornyScapes.UI;
using GreenT.UI;
using StripClub.UI.Level;
using UnityEngine;
using Zenject;

namespace GreenT.HornyScapes;

public class CameraChanger : MonoBehaviour, ICameraChanger
{
	[SerializeField]
	private Camera mainCamera;

	private IWindowsManager manager;

	private Dictionary<Type, Camera> cameraWindow = new Dictionary<Type, Camera>();

	public Camera MergeCamera => cameraWindow[typeof(MergeWindow)];

	public Camera MainCamera => mainCamera;

	[Inject]
	private void Construct(IWindowsManager manager)
	{
		this.manager = manager;
		cameraWindow[typeof(LevelWindow)] = mainCamera;
		manager.OnOpenWindow += ChangeCamera;
		manager.OnCloseWindow += ChangeCamera;
	}

	public void ChangeCamera(IWindow window)
	{
		Type type = window.GetType();
		if (!cameraWindow.ContainsKey(type) || type == typeof(LevelWindow))
		{
			return;
		}
		foreach (Type key in cameraWindow.Keys)
		{
			cameraWindow[key].enabled = key == typeof(LevelWindow);
		}
		cameraWindow[type].enabled = window.IsOpened;
	}

	public void AddCameraToWindow(IWindow window, Camera camera)
	{
		Type type = window.GetType();
		cameraWindow[type] = camera;
	}

	public void RemovewCameraByWindow(IWindow window)
	{
		Type type = window.GetType();
		cameraWindow.Remove(type);
	}

	private void OnDestroy()
	{
		if (manager != null)
		{
			manager.OnOpenWindow -= ChangeCamera;
			manager.OnCloseWindow -= ChangeCamera;
		}
	}
}
