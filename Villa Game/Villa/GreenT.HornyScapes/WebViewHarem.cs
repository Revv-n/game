using System;
using Gpm.WebView;
using UniRx;
using UnityEngine;

namespace GreenT.HornyScapes;

public class WebViewHarem : IDisposable
{
	private readonly Subject<Unit> _onCloseWebViewSubject = new Subject<Unit>();

	private float WidthBrowserInPercent = 0.4f;

	public IObservable<Unit> OnCloseWebViewObservable => _onCloseWebViewSubject.AsObservable();

	public void OpenWebView(string url)
	{
		GpmWebView.ShowUrl(url, GetConfiguration(), OnWebViewCallback, null);
	}

	public void CloseWebView()
	{
		GpmWebView.Close();
	}

	private GpmWebViewRequest.Configuration GetConfiguration()
	{
		return new GpmWebViewRequest.Configuration
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

	private GpmWebViewRequest.Size GetConfigurationSize()
	{
		bool hasValue = true;
		int width = (int)((float)Screen.width * WidthBrowserInPercent);
		int height = (int)((float)Screen.height * 0.95f);
		GpmWebViewRequest.Size result = default(GpmWebViewRequest.Size);
		result.hasValue = hasValue;
		result.width = width;
		result.height = height;
		return result;
	}

	private GpmWebViewRequest.Position GetConfigurationPosition()
	{
		bool hasValue = true;
		int x = (int)(((float)Screen.width - (float)Screen.width * WidthBrowserInPercent) / 1.5f);
		int y = 50;
		GpmWebViewRequest.Position result = default(GpmWebViewRequest.Position);
		result.hasValue = hasValue;
		result.x = x;
		result.y = y;
		return result;
	}

	private void OnWebViewCallback(GpmWebViewCallback.CallbackType callbackType, string data, GpmWebViewError error)
	{
		if (callbackType == GpmWebViewCallback.CallbackType.Close)
		{
			_onCloseWebViewSubject.OnNext(Unit.Default);
		}
	}

	public void Dispose()
	{
		_onCloseWebViewSubject?.Dispose();
	}
}
