using UnityEngine;
using UnityEngine.UI;

public class ButtonTextCorrector : MonoBehaviour
{
	[SerializeField]
	private ButtonTextPreset preset;

	private Outline outline;

	private Text text;

	private Button button;

	private bool isNormalState;

	private void Awake()
	{
		outline = GetComponent<Outline>();
		text = GetComponent<Text>();
		button = GetComponentInParent<Button>();
		Correct();
	}

	private void Update()
	{
		if (button.interactable != isNormalState)
		{
			Correct();
		}
	}

	private void Correct()
	{
		ButtonTextPreset.State state = (button.interactable ? preset.NormalState : preset.LockedState);
		text.color = state.textColor;
		outline.effectColor = state.outlineColor;
		outline.effectDistance = state.outlineStrange;
		isNormalState = button.interactable;
	}
}
