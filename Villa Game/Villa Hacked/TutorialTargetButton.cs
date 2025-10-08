using StripClub.UI;
using UnityEngine;
using UnityEngine.UI;

public sealed class TutorialTargetButton : MonoView
{
	[SerializeField]
	private Button _target;

	public Button Button => _target;
}
