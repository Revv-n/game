using StripClub.Stories;
using UnityEngine;

namespace StripClub.Model.Character;

[CreateAssetMenu(fileName = "Stories", menuName = "StripClub/Character/Stories")]
public class CharacterStories : ScriptableObject
{
	public static string bundleNamePrefix = "employee/stories/";

	[SerializeField]
	protected int characterID;

	[SerializeField]
	protected Sprite storySprite;

	[SerializeField]
	protected AnimatedSpeaker animationPrefab;

	public Sprite StorySprite => storySprite;

	public AnimatedSpeaker AnimationPrefab => animationPrefab;
}
