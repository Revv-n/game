using GreenT.HornyScapes.Stories;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace GreenT.HornyScapes.Cheats;

public class CheatDialogues : MonoBehaviour
{
	[SerializeField]
	private Toggle activate;

	private StoryController storyController;

	[Inject]
	public void Init(StoryController storyController)
	{
		this.storyController = storyController;
	}

	private void Awake()
	{
		activate.onValueChanged.AddListener(SetState);
	}

	private void SetState(bool state)
	{
	}
}
