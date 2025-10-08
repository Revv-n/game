using System;
using GreenT.AssetBundles;
using GreenT.HornyScapes.Animations;
using GreenT.HornyScapes.Card.UI;
using StripClub.UI;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Zenject;

namespace GreenT.HornyScapes.Characters.Skins.UI;

public class PromoteSkinView : NodeMonoView<PromoteSkinView.Settings>, IPointerClickHandler, IEventSystemHandler
{
	public class Settings
	{
		public Skin Skin { get; }

		public bool IsSelected { get; private set; }

		public bool IsOwned { get; private set; }

		public CharacterSettings CharacterSettings { get; }

		public Settings(Skin skin, CharacterSettings characterSettings)
		{
			IsSelected = characterSettings.SkinID == skin.ID;
			IsOwned = skin.IsOwned;
			Skin = skin;
			CharacterSettings = characterSettings;
		}

		public void SetOwned(bool isOwned)
		{
			IsOwned = isOwned;
		}

		public void SetSelected(bool isSelected)
		{
			IsSelected = isSelected;
		}
	}

	public class Manager : ViewManager<Settings, PromoteSkinView>
	{
	}

	public class Factory : ViewFactory<Settings, PromoteSkinView>
	{
		public Factory(DiContainer diContainer, Transform objectContainer, PromoteSkinView prefab)
			: base(diContainer, objectContainer, prefab)
		{
		}
	}

	private const string LOCALIZATION_KEY_GIRL_NAME = "content.character.{0}.name";

	[Tooltip("Переключение между заблокированным и стандартным видом. Стандартный - 0, заблокированный - 1")]
	[SerializeField]
	private StatableComponent viewState;

	[SerializeField]
	private StatableComponent selectedState;

	[SerializeField]
	private StatableComponent rarityState;

	[SerializeField]
	private Image image;

	[SerializeField]
	private Image lockedImage;

	[SerializeField]
	private LocalizedTextMeshPro skinName;

	[SerializeField]
	private LocalizedTextMeshPro unlockTips;

	[SerializeField]
	private GreenT.HornyScapes.Animations.Animation selectAnimation;

	[SerializeField]
	private GreenT.HornyScapes.Animations.Animation deselectAnimation;

	[SerializeField]
	private CharacterPlaceholderSpriteCollection placeholderCollection;

	public Subject<Skin> onSelect = new Subject<Skin>();

	private IDisposable characterUpdateStream;

	[Inject]
	private FakeAssetService _fakeAssetService;

	public IObservable<Skin> OnSelect => onSelect.AsObservable();

	public override void Set(Settings source)
	{
		characterUpdateStream?.Dispose();
		base.Set(source);
		viewState.Set((!source.IsOwned) ? 1 : 0);
		if (source.IsOwned)
		{
			selectedState.Set(source.IsSelected ? 1 : 0);
			SetupOwnedView(source);
		}
		else
		{
			unlockTips.Init(source.Skin.UnlockMessageKey);
			lockedImage.sprite = placeholderCollection.Get(source.Skin.ID);
		}
		characterUpdateStream = source.CharacterSettings.OnUpdate.Subscribe(UpdateView);
		SetSkinCardName(source);
	}

	private void SetupOwnedView(Settings source)
	{
		if (source.Skin.ID != 0)
		{
			_fakeAssetService.SetFakeSkinIcon(source.Skin, image, (Skin _) => source.Skin.Data.CardImage);
		}
		else
		{
			image.sprite = source.Skin.Data.CardImage;
		}
		rarityState.Set((int)source.Skin.Rarity);
	}

	private void SetSkinCardName(Settings source)
	{
		if (!(skinName == null))
		{
			Skin skin = source.Skin;
			string key = skin.NameKey;
			if (skin.ID == 0)
			{
				key = $"content.character.{skin.GirlID}.name";
			}
			skinName.Init(key);
		}
	}

	private void UpdateView(CharacterSettings characterSettings)
	{
		if (base.Source.IsOwned != base.Source.Skin.IsOwned)
		{
			base.Source.SetOwned(base.Source.Skin.IsOwned);
			SetupOwnedView(base.Source);
		}
		if (!base.Source.IsSelected && characterSettings.SkinID == base.Source.Skin.ID)
		{
			SetSelected(selected: true);
		}
		if (base.Source.IsSelected && characterSettings.SkinID != base.Source.Skin.ID)
		{
			SetSelected(selected: false);
		}
	}

	protected virtual void OnDisable()
	{
		characterUpdateStream?.Dispose();
	}

	protected virtual void OnDestroy()
	{
		onSelect.OnCompleted();
		onSelect.Dispose();
	}

	public void SetSelected(bool selected)
	{
		base.Source.SetSelected(selected);
		selectedState.Set(selected ? 1 : 0);
		if (selected)
		{
			deselectAnimation.Stop();
			selectAnimation.Play();
		}
		else
		{
			selectAnimation.Play();
			deselectAnimation.Play();
		}
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		if (base.Source.Skin.IsOwned)
		{
			onSelect.OnNext(base.Source.Skin);
		}
	}
}
