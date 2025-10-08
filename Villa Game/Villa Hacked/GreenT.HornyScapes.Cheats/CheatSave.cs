using GreenT.HornyScapes.Saves;
using UnityEngine;
using Zenject;

namespace GreenT.HornyScapes.Cheats;

public class CheatSave : MonoBehaviour
{
	private SaveModeSelector selector;

	[Inject]
	private void Constructor(SaveModeSelector selector)
	{
		this.selector = selector;
	}

	[EditorButton]
	public void SetMain()
	{
		selector.Select(SaveMode.Main);
	}

	[EditorButton]
	public void SetTutorial()
	{
		selector.Select(SaveMode.Tutorial);
	}
}
