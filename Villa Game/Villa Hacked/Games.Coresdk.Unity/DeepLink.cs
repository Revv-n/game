using System;
using UnityEngine;

namespace Games.Coresdk.Unity;

public class DeepLink
{
	private IDeepLinkBridge m_bridge;

	private Action<string> m_callback;

	private MainThreadDispatcher m_mainThreadDispatcher;

	public static DeepLink Initialize(MainThreadDispatcher mainThreadDispatcher)
	{
		return new DeepLink
		{
			m_mainThreadDispatcher = mainThreadDispatcher
		};
	}

	public void OpenURL(string url, Action<string> callback)
	{
		m_callback = callback;
		m_bridge.OpenURL(url, OnDeepLinkActivated);
		if (Application.isEditor || Debug.isDebugBuild)
		{
			Debug.Log(url);
		}
	}

	private void OnDeepLinkActivated(string url)
	{
		m_mainThreadDispatcher.Add(delegate
		{
			m_callback(url);
			if (Application.isEditor || Debug.isDebugBuild)
			{
				Debug.Log(url);
			}
		});
	}
}
