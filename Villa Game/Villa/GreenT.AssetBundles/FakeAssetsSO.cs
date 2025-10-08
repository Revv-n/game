using System.Collections.Generic;
using UnityEngine;

namespace GreenT.AssetBundles;

[CreateAssetMenu(menuName = "GreenT/Assets/Fakes/CharacterBankImages", order = 1)]
public class FakeAssetsSO : ScriptableObject
{
	public List<Sprite> CharacterBankImages = new List<Sprite>();

	public List<Sprite> SkinIcons = new List<Sprite>();

	public List<Sprite> MergeItemIcon = new List<Sprite>();

	public List<Sprite> BattlePassIcon = new List<Sprite>();

	public List<Sprite> MiniEventIcon = new List<Sprite>();

	public List<Sprite> EmployeeIcon = new List<Sprite>();

	public List<Sprite> BackgroundIcon = new List<Sprite>();

	public List<Sprite> EventIcon = new List<Sprite>();
}
