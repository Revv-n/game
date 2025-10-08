using System;
using Steamworks;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace GreenT.HornyScapes._HornyScapes._Scripts.UI;

public class SteamDeckKeyboardCaller : MonoBehaviour, IDisposable
{
	[SerializeField]
	private TMP_InputField inputField;

	private UnityAction<string> onSelectedCallback;

	private Callback<GamepadTextInputDismissed_t>.DispatchDelegate onTextEnteredCallback;

	private Callback<GamepadTextInputDismissed_t> m_GamepadTextInputDismissed;

	private void Awake()
	{
		if (!SteamUtils.IsSteamRunningOnSteamDeck())
		{
			UnityEngine.Object.Destroy(this);
			return;
		}
		onSelectedCallback = OnSelected;
		onTextEnteredCallback = OnTextEntered;
	}

	private void OnEnable()
	{
		inputField.onSelect.AddListener(onSelectedCallback);
	}

	private void OnSelected(string text)
	{
		m_GamepadTextInputDismissed?.Dispose();
		SteamUtils.ShowGamepadTextInput(EGamepadTextInputMode.k_EGamepadTextInputModeNormal, EGamepadTextInputLineMode.k_EGamepadTextInputLineModeSingleLine, string.Empty, 32u, string.Empty);
		m_GamepadTextInputDismissed = Callback<GamepadTextInputDismissed_t>.Create(onTextEnteredCallback);
	}

	private void OnTextEntered(GamepadTextInputDismissed_t pCallback)
	{
		m_GamepadTextInputDismissed.Dispose();
		if (pCallback.m_bSubmitted)
		{
			uint unSubmittedText = pCallback.m_unSubmittedText;
			if (SteamUtils.GetEnteredGamepadTextInput(out var pchText, unSubmittedText))
			{
				inputField.SetTextWithoutNotify(pchText);
			}
		}
	}

	private void OnDisable()
	{
		if (onSelectedCallback != null)
		{
			inputField.onSelect.RemoveListener(onSelectedCallback);
		}
	}

	public void Dispose()
	{
		m_GamepadTextInputDismissed?.Dispose();
	}
}
