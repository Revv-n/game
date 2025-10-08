using UnityEngine;
using UnityEngine.UI;

namespace GreenT.HornyScapes.Cheats.UI;

public abstract class CheatButton : MonoBehaviour, IValidatedCheat, ICheat
{
	[SerializeField]
	protected Button rewindButton;

	protected virtual void OnEnable()
	{
		rewindButton.onClick.AddListener(Apply);
	}

	protected virtual void OnDisable()
	{
		rewindButton.onClick.RemoveListener(Apply);
	}

	public abstract bool IsValid();

	public virtual void Validate()
	{
		rewindButton.interactable = IsValid();
	}

	public abstract void Apply();
}
