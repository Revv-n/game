using System;
using System.Collections.Generic;
using Gpm.WebView;
using UniRx;
using UnityEngine;

namespace GreenT.HornyScapes;

public class WebViewHarem : IDisposable
{
	private readonly Subject<Unit> _onCloseWebViewSubject = new Subject<Unit>();

	private float WidthBrowserInPercent = 0.4f;

	public IObservable<Unit> OnCloseWebViewObservable => Observable.AsObservable<Unit>((IObservable<Unit>)_onCloseWebViewSubject);

	public void OpenWebView(string url)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Expected O, but got Unknown
		GpmWebView.ShowUrl(url, GetConfiguration(), new GpmWebViewDelegate(OnWebViewCallback), (List<string>)null);
	}

	public void CloseWebView()
	{
		GpmWebView.Close();
	}

	private Configuration GetConfiguration()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Expected O, but got Unknown
		return new Configuration
		{
			style = 0,
			isNavigationBarVisible = true,
			isCloseButtonVisible = true,
			size = GetConfigurationSize(),
			position = GetConfigurationPosition(),
			orientation = ((Screen.orientation == ScreenOrientation.LandscapeLeft) ? 4 : 8),
			navigationBarColor = "#2B3375"
		};
	}

	private Size GetConfigurationSize()
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		bool hasValue = true;
		int width = (int)((float)Screen.width * WidthBrowserInPercent);
		int height = (int)((float)Screen.height * 0.95f);
		Size result = default(Size);
		result.hasValue = hasValue;
		result.width = width;
		result.height = height;
		return result;
	}

	private Position GetConfigurationPosition()
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		bool hasValue = true;
		int x = (int)(((float)Screen.width - (float)Screen.width * WidthBrowserInPercent) / 1.5f);
		int y = 50;
		Position result = default(Position);
		result.hasValue = hasValue;
		result.x = x;
		result.y = y;
		return result;
	}

	private void OnWebViewCallback(CallbackType callbackType, string data, GpmWebViewError error)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Invalid comparison between Unknown and I4
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		if ((int)callbackType == 1)
		{
			_onCloseWebViewSubject.OnNext(Unit.Default);
		}
	}

	public void Dispose()
	{
		_onCloseWebViewSubject?.Dispose();
	}
}
