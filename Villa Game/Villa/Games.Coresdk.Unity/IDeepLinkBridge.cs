using System;

namespace Games.Coresdk.Unity;

internal interface IDeepLinkBridge
{
	void OpenURL(string url, Action<string> onDeepLinkActivated);
}
