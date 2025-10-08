using System.Collections.Generic;
using UnityEngine;

namespace GreenT.HornyScapes.Tutorial;

[CreateAssetMenu(fileName = "LockerTutorialStep", menuName = "GreenT/Tutorial/Step/LockerStep")]
public class TutorialLockerStepSO : TutorialStepSO
{
	public List<LockerListItem> LockerList = new List<LockerListItem>();
}
