using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GreenT.HornyScapes.Cheats;

public class CheatLineSwitcher : MonoBehaviour
{
	[Serializable]
	public class Info
	{
		public string Name;

		public GameObject Item;
	}

	[SerializeField]
	private InputSettingCheats inputSettingCheats;

	private Action<int> OnSwitchLine;

	[SerializeField]
	private Button switcherButton;

	[SerializeField]
	private TMP_Text currentLineText;

	[SerializeField]
	private List<GameObject> Lines;

	[SerializeField]
	private List<Info> Sections;

	[SerializeField]
	public TMP_Dropdown ContentTypeDropdown;

	[SerializeField]
	private KeyCode mainKey = KeyCode.LeftControl;

	[SerializeField]
	private KeyCode addKey = KeyCode.Alpha1;

	private int currentLineNumber;

	private void Awake()
	{
		SetNumber(currentLineNumber);
		OpenLine(currentLineNumber);
		ContentTypeDropdown.ClearOptions();
		ContentTypeDropdown.AddOptions(Sections.Select((Info x) => x.Name).ToList());
		ContentTypeDropdown.value = 0;
		ContentTypeDropdown.onValueChanged.AddListener(OpenLine);
		switcherButton.onClick.AddListener(NextLine);
		OnSwitchLine = (Action<int>)Delegate.Combine(OnSwitchLine, new Action<int>(SetNumber));
		OnSwitchLine = (Action<int>)Delegate.Combine(OnSwitchLine, new Action<int>(OpenLine));
	}

	private void Update()
	{
		if (inputSettingCheats.NextLine.IsPressedKeys)
		{
			NextLine();
		}
	}

	private void SetNumber(int line)
	{
		currentLineText.text = line.ToString();
	}

	private void OpenLine(int lineNumber)
	{
		foreach (GameObject item in Sections.Select((Info x) => x.Item))
		{
			item.gameObject.SetActive(value: false);
		}
		Lines[lineNumber].SetActive(value: true);
	}

	public void NextLine()
	{
		currentLineNumber++;
		if (currentLineNumber == Lines.Count)
		{
			currentLineNumber = 0;
		}
		OnSwitchLine?.Invoke(currentLineNumber);
	}

	private void OnDestroy()
	{
		switcherButton.onClick.RemoveAllListeners();
		OnSwitchLine = (Action<int>)Delegate.Remove(OnSwitchLine, new Action<int>(SetNumber));
		OnSwitchLine = (Action<int>)Delegate.Remove(OnSwitchLine, new Action<int>(OpenLine));
	}
}
