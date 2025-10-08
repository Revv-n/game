using System.Collections.Generic;
using UnityEngine;

namespace Games.Coresdk.Unity;

public class PaymentClient
{
	private AuthClient m_authClient;

	private Config m_config;

	private MainThreadDispatcher m_mainThreadDispatcher;

	private Coresdk m_sdk;

	public PaymentClient(Coresdk sdk, AuthClient authClient, Config config, MainThreadDispatcher mainThreadDispatcher)
	{
		m_sdk = sdk;
		m_authClient = authClient;
		m_config = config;
		m_mainThreadDispatcher = mainThreadDispatcher;
	}

	public void Purchase()
	{
		Dictionary<string, string> query = new Dictionary<string, string>
		{
			{ "jwt", m_authClient.Token },
			{ "lang", m_sdk.language }
		};
		Application.OpenURL(string.Format("{0}/{1}?{2}", m_config.PaymentDomain, "/payment", APIUtil.GetQueryString(query)));
	}
}
