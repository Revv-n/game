using GreenT.UI;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace GreenT.HornyScapes.Collection;

public class CollectionExitButton : MonoBehaviour
{
	public Button Button;

	public Window Window;

	private WindowOpener currentOpener;

	[Inject]
	private void Constructor(ReturnButtonStrategy ReturnButtonStrategy)
	{
		ReturnButtonStrategy.Add(this);
	}

	private void Awake()
	{
	}

	public void Set(WindowOpener nextOpener)
	{
		currentOpener = nextOpener;
	}

	private void OnClick()
	{
		Window.Close();
		currentOpener.Click();
	}

	private void OnDestroy()
	{
		Button.onClick.RemoveAllListeners();
	}
}
