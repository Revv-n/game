using System.Collections.Generic;
using UnityEngine;

namespace GreenT.HornyScapes.Tutorial;

[CreateAssetMenu(fileName = "TutorialGroup", menuName = "GreenT/Tutorial/Group")]
public class TutorialGroupSO : ScriptableObject
{
	public int GroupID;

	public List<TutorialStepSO> Steps;

	public List<LockerListItem> LockerList = new List<LockerListItem>();
}
