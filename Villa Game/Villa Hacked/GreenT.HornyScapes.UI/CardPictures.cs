using System;
using System.Collections.Generic;
using System.Linq;
using GreenT.AssetBundles;
using GreenT.HornyScapes.Characters;
using GreenT.HornyScapes.Characters.Skins;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace GreenT.HornyScapes.UI;

public class CardPictures : MonoBehaviour
{
	[SerializeField]
	private Image cardImage;

	private IDisposable updatesStream;

	private SkinManager skinManager;

	private FakeAssetService _fakeAssetService;

	public CharacterSettings CharacterSettings { get; set; }

	[Inject]
	public void Init(SkinManager skinManager, FakeAssetService fakeAssetService)
	{
		this.skinManager = skinManager;
		_fakeAssetService = fakeAssetService;
	}

	public void Init(CharacterSettings characterSettings)
	{
		updatesStream?.Dispose();
		CharacterSettings = characterSettings;
		updatesStream = ObservableExtensions.Subscribe<CharacterSettings>(Observable.StartWith<CharacterSettings>(characterSettings.OnUpdate, CharacterSettings), (Action<CharacterSettings>)SetupAvatar);
	}

	private void SetupAvatar(CharacterSettings character)
	{
		try
		{
			if (character.SkinID != 0)
			{
				Skin skin = skinManager.Get(character.SkinID);
				_fakeAssetService.SetFakeSkinIcon(skin, cardImage, (Skin _) => skin.Data.CardImage);
				return;
			}
			ICollection<Sprite> values = character.Public.GetBundleData().Avatars.Values;
			Sprite sprite = values.ElementAtOrDefault(character.AvatarNumber);
			if (sprite == null && values.Any())
			{
				character.SetAvatar(0);
			}
			cardImage.sprite = sprite;
		}
		catch (Exception exception)
		{
			exception.LogException();
		}
	}

	private void OnDestroy()
	{
		updatesStream?.Dispose();
	}
}
