using UnityEngine;

namespace StripClub.Model.Cards;

[CreateAssetMenu(fileName = "Tab", menuName = "StripClub/Collection Tab Settings")]
public class TabSettings : ScriptableObject
{
	[field: SerializeField]
	public int GroupID { get; private set; } = -1;


	[field: SerializeField]
	public Sprite Icon { get; private set; }
}
