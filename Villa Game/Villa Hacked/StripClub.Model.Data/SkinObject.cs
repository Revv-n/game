using UnityEngine;

namespace StripClub.Model.Data;

[CreateAssetMenu(fileName = "New Skin", menuName = "StripClub/Shop/Skin", order = 5)]
public class SkinObject : ShopObject
{
	[field: SerializeField]
	public int CharacterID { get; private set; }

	[field: SerializeField]
	public int SkinID { get; private set; }

	[field: SerializeField]
	public Cost Cost { get; private set; }
}
