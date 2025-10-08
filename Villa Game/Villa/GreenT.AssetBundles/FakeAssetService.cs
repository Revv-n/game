using System;
using GreenT.HornyScapes.Characters;
using GreenT.HornyScapes.Characters.Skins;
using StripClub.Model.Cards;
using StripClub.Model.Shop.Data;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace GreenT.AssetBundles;

public class FakeAssetService
{
	private readonly FakeAssetProvider _fakeAssetProvider;

	private readonly CharacterProvider _characterProvider;

	private readonly SkinDataLoadingController _skinDataLoading;

	private readonly CompositeDisposable streams = new CompositeDisposable();

	public FakeAssetService(FakeAssetProvider fakeAssetProvider, CharacterProvider characterProvider, SkinDataLoadingController skinDataLoading)
	{
		_fakeAssetProvider = fakeAssetProvider;
		_characterProvider = characterProvider;
		_skinDataLoading = skinDataLoading;
	}

	public Sprite HandleFakeBySource(ContentSource contentSource, string asset)
	{
		return _fakeAssetProvider.GetFakeBySource(contentSource, asset);
	}

	public void SetFakeSkinIcon(Skin skin, Image image, Func<Skin, Sprite> getSprite)
	{
		if (!skin.IsDataEmpty)
		{
			image.sprite = getSprite(skin);
			return;
		}
		image.sprite = _fakeAssetProvider.GetFakeSkinIcon(skin.ID);
		_skinDataLoading.InsertDataOnLoad(skin).Take(1).Subscribe(delegate
		{
			image.sprite = getSprite(skin);
		})
			.AddTo(streams);
	}

	public void SetFakeCharacterBankImages(ICard card, Image image, Func<ICharacter, Sprite> getSprite)
	{
		ICharacter character = card as ICharacter;
		SetFakeCharacterBankImages(character, image, getSprite);
	}

	public void SetFakeCharacterBankImages(ICharacter character, Image image, Func<ICharacter, Sprite> getSprite)
	{
		image.sprite = _fakeAssetProvider.GetFakeCharacterBankImages(character.ID);
		_characterProvider.Get(character).Take(1).Subscribe(delegate
		{
			image.sprite = getSprite(character);
		})
			.AddTo(streams);
	}
}
