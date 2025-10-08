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
		using (UnityWebRequest request = BuildWebRequest(url))
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
		validationCallback(transaction);
	}

	private UnityWebRequest BuildWebRequest(string url)
	{
		UnityWebRequest unityWebRequest = new UnityWebRequest(url);
		unityWebRequest.SetRequestHeader("Access-Control-Allow-Credentials", "true");
		unityWebRequest.SetRequestHeader("Access-Control-Allow-Headers", "Access-Control-Allow-Headers, Origin, Accept, X-Access-Token, X-Application-Name, X-Request-Sent-Time, X-Requested-With, Content-Type, Access-Control-Request-Method, Access-Control-Request-Headers, Access-Control-Allow-Credentials");
		unityWebRequest.SetRequestHeader("Access-Control-Allow-Methods", "POST, GET, OPTIONS, PUT, DELETE");
		unityWebRequest.SetRequestHeader("Access-Control-Allow-Origin", "*");
		unityWebRequest.downloadHandler = new DownloadHandlerBuffer();
		return unityWebRequest;
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
