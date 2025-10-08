using System;
using GreenT.HornyScapes.Characters;
using Merge;
using StripClub.Model.Character;
using UnityEngine;
using Zenject;

namespace StripClub.Stories;

public class SpeakerView : MonoBehaviour
{
	[SerializeField]
	private ImageSpeaker storySprite;

	[SerializeField]
	private Transform animatedPlaceholder;

	[SerializeField]
	private Canvas sortingOrderCanvas;

	private AnimatedSpeaker animatedSpeaker;

	private CharacterStories characterStories;

	protected CharacterManager characterManager;

	public int CharacterID { get; set; }

	[Inject]
	private void Init(CharacterManager characterManager)
	{
		this.characterManager = characterManager;
	}

	public void Fade()
	{
		if (animatedSpeaker != null)
		{
			animatedSpeaker.Fade();
			return;
		}
		if (storySprite != null)
		{
			storySprite.Fade();
			return;
		}
		throw new NullReferenceException("CharacterStories bundle is empty for character with ID:" + CharacterID);
	}

	public void UnFade()
	{
		if (animatedSpeaker != null)
		{
			animatedSpeaker.UnFade();
			return;
		}
		if (storySprite != null)
		{
			storySprite.UnFade();
			return;
		}
		throw new NullReferenceException("CharacterStories bundle is empty for character with ID:" + CharacterID);
	}

	public void SetSpeaker(int characterID)
	{
		base.gameObject.SetActive(value: true);
		characterStories = GetStory(characterID);
		if (characterStories == null)
		{
			throw new NullReferenceException("CharacterStories bundle is empty for character with ID:" + characterID);
		}
		CharacterID = characterID;
		if (animatedSpeaker != null)
		{
			animatedSpeaker.Despawn();
			animatedSpeaker = null;
		}
		if (characterStories.AnimationPrefab != null)
		{
			SetAnimatedStory(characterStories.AnimationPrefab);
		}
		else if (characterStories.StorySprite != null)
		{
			SetSpriteStory(characterStories.StorySprite);
		}
		Fade();
	}

	protected virtual CharacterStories GetStory(int characterId)
	{
		return characterManager.Get(characterId).GetStory();
	}

	private void SetAnimatedStory(AnimatedSpeaker animatedPrefab)
	{
		storySprite.SetActive(active: false);
		animatedSpeaker = UnityEngine.Object.Instantiate(animatedPrefab, animatedPlaceholder);
		animatedSpeaker.Init(sortingOrderCanvas);
	}

	private void SetSpriteStory(Sprite sprite)
	{
		storySprite.SetActive(active: true);
		storySprite.Image = sprite;
	}
}
