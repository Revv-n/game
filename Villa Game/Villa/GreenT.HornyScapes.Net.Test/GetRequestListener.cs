using GreenT.Net;
using GreenT.Net.User;
using UnityEngine;
using Zenject;

namespace GreenT.HornyScapes.Net.Test;

public class GetRequestListener : MonoBehaviour
{
	private GetDataProcessor getDataProcessor;

	[Inject]
	public void Init(GetDataProcessor getDataProcessor)
	{
		this.getDataProcessor = getDataProcessor;
	}

	private void Start()
	{
		getDataProcessor.AddListener(Process);
	}

	private void Process(Response<UserDataMapper> response)
	{
		if (response.Status != 0)
		{
			Debug.Log("Ошибка Get запроса. Код ошибки: " + response.Status);
		}
		else
		{
			Debug.Log("Ответ Get запроса: " + response);
		}
	}

	private void OnDestroy()
	{
		getDataProcessor.RemoveListener(Process);
	}
}
