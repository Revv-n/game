using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GreenT.HornyScapes.Cheats.UI;

public abstract class CheatButtonWithInputField : MonoBehaviour, IValidatedCheat<string>, ICheat
{
	[SerializeField]
	protected TMP_InputField inputField;

	[SerializeField]
	protected Button rewindButton;

	protected virtual void OnEnable()
	{
		inputField.onValueChanged.AddListener(Validate);
		rewindButton.onClick.AddListener(Apply);
	}

	protected virtual void OnDisable()
	{
		inputField.onValueChanged.RemoveListener(Validate);
		rewindButton.onClick.RemoveListener(Apply);
	}

	public abstract bool IsValid(string param);

	public virtual void Validate(string param)
	{
		rewindButton.interactable = IsValid(param);
	}

	public abstract void Apply();
}
