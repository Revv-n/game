using System;
using StripClub.UI;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace GreenT.HornyScapes;

public class PromocodeSettings : MonoBehaviour
{
	[SerializeField]
	private TMP_InputField promocodeInputField;

	[SerializeField]
	private Button sendPromocodeButton;

	[SerializeField]
	private LocalizedTextMeshPro messageText;

	private PromocodeService promocodeService;

	[Inject]
	public void Init(PromocodeService promocodeService)
	{
		this.promocodeService = promocodeService;
	}

	private void Start()
	{
		SetSendButton();
		UpdateButtonState(promocodeInputField.text);
		promocodeInputField.onValueChanged.AddListener(UpdateButtonState);
		ObservableExtensions.Subscribe<string>(promocodeService.OnResponseMessage, (Action<string>)ShowMessage);
	}

	private void OnEnable()
	{
		Clear();
		messageText.gameObject.SetActive(value: false);
	}

	private void SetSendButton()
	{
		sendPromocodeButton.onClick.RemoveAllListeners();
		sendPromocodeButton.onClick.AddListener(TryToActivatePromocode);
	}

	private void UpdateButtonState(string text)
	{
		sendPromocodeButton.interactable = !string.IsNullOrWhiteSpace(text);
	}

	private void TryToActivatePromocode()
	{
		string text = promocodeInputField.text;
		promocodeService.TryToActivatePromocode(text);
		Clear();
	}

	private void ShowMessage(string localization_key)
	{
		messageText.Init(localization_key);
		messageText.gameObject.SetActive(value: true);
	}

	private void Clear()
	{
		promocodeInputField.text = "";
	}

	private void OnDestroy()
	{
		sendPromocodeButton.onClick.RemoveAllListeners();
		promocodeInputField.onValueChanged.RemoveAllListeners();
	}
}
