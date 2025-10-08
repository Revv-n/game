using System.Collections.Generic;
using UnityEngine;

namespace GreenT.HornyScapes.Tutorial;

[CreateAssetMenu(fileName = "TutorialConfig", menuName = "GreenT/Tutorial/Config")]
public class TutorialConfigSO : ScriptableObject
{
	public List<TutorialGroupSO> Groups;
}
