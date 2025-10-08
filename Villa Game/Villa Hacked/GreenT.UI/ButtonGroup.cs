using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace GreenT.UI;

public class ButtonGroup : MonoBehaviour
{
	[SerializeField]
	private List<Button> buttons = new List<Button>();

	private Dictionary<Button, UnityAction> actions = new Dictionary<Button, UnityAction>();

	public void Add(Button button)
	{
		if (!buttons.Contains(button))
		{
			buttons.Add(button);
			UnityAction call = CreateClickListener(button);
			button.onClick.AddListener(call);
		}
	}

	private UnityAction CreateClickListener(Button button)
	{
		UnityAction unityAction = delegate
		{
			OnClick(button);
		};
		actions[button] = unityAction;
		return unityAction;
	}

	public void Remove(Button button)
	{
		if (buttons.Contains(button))
		{
			buttons.Remove(button);
			button.onClick.RemoveListener(actions[button]);
			actions.Remove(button);
			button.interactable = true;
		}
	}

	public void OnClick(Button button)
	{
		foreach (Button item in buttons.Where((Button _button) => !_button.IsInteractable()))
		{
			item.interactable = true;
		}
		button.interactable = false;
	}

	public void SetActive(Button button)
	{
		if (buttons.Contains(button))
		{
			OnClick(button);
		}
	}
}
