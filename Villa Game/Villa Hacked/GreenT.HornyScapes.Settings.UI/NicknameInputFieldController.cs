using System.Collections.Generic;
using StripClub.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GreenT.HornyScapes.Settings.UI;

public class NicknameInputFieldController : MonoBehaviour
{
	[SerializeField]
	private TMP_InputField inputField;

	[SerializeField]
	private Button editButton;

	[SerializeField]
	private List<StatableComponent> statableComponents;

	private void Start()
	{
		inputField.readOnly = true;
		foreach (StatableComponent statableComponent in statableComponents)
		{
			statableComponent.Set(0);
		}
		if (PlatformHelper.IsEpochaMonetization() || PlatformHelper.IsSteamMonetization() || PlatformHelper.IsHaremMonetization() || PlatformHelper.IsErolabsMonetization())
		{
			editButton.onClick.AddListener(EditField);
		}
		else if (PlatformHelper.IsNutakuMonetization())
		{
			editButton.gameObject.SetActive(value: false);
		}
	}

	private void EditField()
	{
		if (!inputField.interactable)
		{
			return;
		}
		inputField.readOnly = false;
		foreach (StatableComponent statableComponent in statableComponents)
		{
			statableComponent.Set(1);
		}
		editButton.onClick.RemoveListener(EditField);
		editButton.onClick.AddListener(SaveNickname);
		inputField.ActivateInputField();
	}

	private void SaveNickname()
	{
		inputField.readOnly = true;
		foreach (StatableComponent statableComponent in statableComponents)
		{
			statableComponent.Set(0);
		}
		editButton.onClick.RemoveListener(SaveNickname);
		editButton.onClick.AddListener(EditField);
	}

	private void Update()
	{
	}
}
