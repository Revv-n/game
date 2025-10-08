using System;
using System.Collections.Generic;
using UnityEngine;

namespace Merge.Meta;

[CreateAssetMenu(fileName = "CharacterConfig", menuName = "DL/Configs/Meta/Character")]
public class CharacterConfig : ScriptableObject
{
	[Serializable]
	public struct Emotion
	{
		[SerializeField]
		private int id;

		[SerializeField]
		private string name;

		[SerializeField]
		private string spritePath;

		[SerializeField]
		private Sprite sprite;

		public string Name => name;

		public string SpritePath => spritePath;

		public Sprite Sprite => sprite;

		public int ID => id;
	}

	[SerializeField]
	private string characterName;

	[SerializeField]
	private string characterNameKey;

	[SerializeField]
	private List<Emotion> emotions;
}
