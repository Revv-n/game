using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace GreenT.HornyScapes.Cheats;

public class CheatConsoleButton : MonoBehaviour
{
	[SerializeField]
	private Button consoleButton;

	[SerializeField]
	private Image consoleButtonImage;

	[SerializeField]
	private Sprite close;

	[SerializeField]
	private Sprite open;

	private CheatConsole console;

	[Inject]
	private void Init(ConsoleCanvas console)
	{
		this.console = console.CheatConsole;
	}

	private void Start()
	{
		consoleButton.onClick.AddListener(delegate
		{
			CallPanel(!console.gameObject.activeSelf);
		});
	}

	public void SetSprite(bool show)
	{
		consoleButtonImage.sprite = (show ? close : open);
	}

	private void CallPanel(bool state)
	{
		SetSprite(state);
		console.ChangePanelView(state);
	}

	private void OnDestroy()
	{
		consoleButton.onClick.RemoveAllListeners();
	}
}
