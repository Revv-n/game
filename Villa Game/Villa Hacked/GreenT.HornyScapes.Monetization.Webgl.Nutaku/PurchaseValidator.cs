using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

namespace GreenT.HornyScapes.Monetization.Webgl.Nutaku;

public class PurchaseValidator
{
	internal class ValidationResponse
	{
		public string status;
	}

	private readonly string m_Url;

	public PurchaseValidator(string url)
	{
		m_Url = url;
	}

	public IEnumerator Validate(Transaction transaction, Action<Transaction> validationCallback)
	{
		string url = string.Format(m_Url, transaction.Id);
		UnityWebRequest request = BuildWebRequest(url);
		try
		{
			yield return request.SendWebRequest();
			if (string.IsNullOrEmpty(request.error))
			{
				ValidationResponse validationResponse = GetValidationResponse(request.downloadHandler.text);
				if (validationResponse != null)
				{
					transaction.IsValidated = IsValidStatus(validationResponse.status);
				}
			}
		}
		finally
		{
			((IDisposable)request)?.Dispose();
		}
		validationCallback(transaction);
	}

	private UnityWebRequest BuildWebRequest(string url)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Expected O, but got Unknown
		//IL_0052: Expected O, but got Unknown
		UnityWebRequest val = new UnityWebRequest(url);
		val.SetRequestHeader("Access-Control-Allow-Credentials", "true");
		val.SetRequestHeader("Access-Control-Allow-Headers", "Access-Control-Allow-Headers, Origin, Accept, X-Access-Token, X-Application-Name, X-Request-Sent-Time, X-Requested-With, Content-Type, Access-Control-Request-Method, Access-Control-Request-Headers, Access-Control-Allow-Credentials");
		val.SetRequestHeader("Access-Control-Allow-Methods", "POST, GET, OPTIONS, PUT, DELETE");
		val.SetRequestHeader("Access-Control-Allow-Origin", "*");
		val.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
		return val;
	}

	private bool IsValidStatus(string status)
	{
		if (!(status == "0"))
		{
			return status == "200";
		}
		return true;
	}

	private ValidationResponse GetValidationResponse(string json)
	{
		return JsonUtility.FromJson<ValidationResponse>(json);
	}
}
