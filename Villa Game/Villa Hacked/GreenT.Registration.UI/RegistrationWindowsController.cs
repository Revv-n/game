using GreenT.HornyScapes;
using GreenT.Net;
using GreenT.Net.User;
using GreenT.UI;
using UnityEngine;
using Zenject;

namespace GreenT.Registration.UI;

public class RegistrationWindowsController : MonoBehaviour
{
	[SerializeField]
	private int order;

	[SerializeField]
	private WindowID nicknameWindowID;

	[SerializeField]
	private WindowID loginWindowID;

	private RegistrationRequestProcessor registrationProcessor;

	private IWindowsManager windowsManager;

	private GameStarter gameStarter;

	[Inject]
	public void Init(RegistrationRequestProcessor registrationController, IWindowsManager windowsManager, GameStarter gameStarter)
	{
		registrationProcessor = registrationController;
		this.windowsManager = windowsManager;
		this.gameStarter = gameStarter;
	}

	private void OnEnable()
	{
		registrationProcessor.AddListener(OpenNicknameWindow, order);
	}

	private void OpenNicknameWindow(Response<UserDataMapper> response)
	{
		if (response.Status == 200 || response.Status == 0)
		{
			windowsManager.GetWindow(loginWindowID)?.Close();
			IWindow window = windowsManager.GetWindow(nicknameWindowID);
			window.Open();
			gameStarter.Play();
		}
	}

	private void OnDisable()
	{
		registrationProcessor.RemoveListener(OpenNicknameWindow);
	}
}
