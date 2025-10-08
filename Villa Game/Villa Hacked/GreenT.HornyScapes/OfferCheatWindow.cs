using System;
using System.Collections.Generic;
using System.Linq;
using GreenT.HornyScapes.Bank;
using GreenT.HornyScapes.Bank.Offer.UI;
using GreenT.HornyScapes.Characters;
using GreenT.HornyScapes.Characters.Skins;
using GreenT.Types;
using GreenT.UI;
using Merge;
using StripClub.Model;
using StripClub.Model.Shop;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Zenject;

namespace GreenT.HornyScapes;

public class OfferCheatWindow : MonoBehaviour
{
	[SerializeField]
	private TMP_InputField _inputField;

	[SerializeField]
	private Button _showOfferWindow;

	private GameStarter _gameStarter;

	private DiContainer _container;

	private OfferSettings.Manager _offerManager;

	private OfferSettings.Manager _eventManager;

	private OfferSettings.Manager _battlepassManager;

	private PushController _offersPushController;

	private SectionController _sectionController;

	private CharacterProvider _characterProvider;

	private SkinDataLoadingController _skinDataLoadingController;

	[Inject]
	public void Init(GameStarter gameStarter, DiContainer container)
	{
		_gameStarter = gameStarter;
		_container = container;
	}

	private void Awake()
	{
		_showOfferWindow.SetActive(active: false);
		_showOfferWindow.onClick.AddListener(delegate
		{
			TryPushOffer(ReadValue());
		});
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<bool>(Observable.Where<bool>((IObservable<bool>)_gameStarter.IsGameActive, (Func<bool, bool>)((bool x) => x)), (Action<bool>)delegate
		{
			IDictionary<ContentType, OfferSettings.Manager> dictionary = _container.Resolve<IDictionary<ContentType, OfferSettings.Manager>>();
			_offerManager = dictionary[ContentType.Main];
			_eventManager = dictionary[ContentType.Event];
			_battlepassManager = dictionary[ContentType.BattlePass];
			_characterProvider = _container.Resolve<CharacterProvider>();
			_skinDataLoadingController = _container.Resolve<SkinDataLoadingController>();
			Scene scene = FindSceneByName("Bank");
			SceneContext sceneContext = GetSceneContext(scene);
			_sectionController = ((Context)sceneContext).Container.Resolve<SectionController>();
			_offersPushController = ((Context)sceneContext).Container.Resolve<PushControllerCluster>().GetController(ContentType.Main);
			_showOfferWindow.SetActive(active: true);
		}), (Component)this);
	}

	private void TryPushOffer(int offerId)
	{
		WindowOpener windowOpener = _offersPushController.GetComponent<WindowOpener>();
		OfferSettings offer = _offerManager.Collection.FirstOrDefault((OfferSettings o) => o.ID == offerId);
		if (offer == null)
		{
			offer = _eventManager.Collection.FirstOrDefault((OfferSettings o) => o.ID == offerId);
			if (offer == null)
			{
				offer = _battlepassManager.Collection.FirstOrDefault((OfferSettings o) => o.ID == offerId);
			}
		}
		IObservable<LootboxLinkedContent> observable = Observable.ToObservable<LootboxLinkedContent>(offer.Bundles.Select((BundleLot _bundle) => _bundle.Content).OfType<LootboxLinkedContent>());
		IObservable<Unit> observable2 = Observable.AsUnitObservable<ICharacter>(Observable.DefaultIfEmpty<ICharacter>(Observable.Concat<ICharacter>(Observable.Select<int, IObservable<ICharacter>>(Observable.SelectMany<LootboxLinkedContent, int>(observable, (Func<LootboxLinkedContent, IEnumerable<int>>)((LootboxLinkedContent content) => content.Lootbox.CharacterIdPossibleDrops)), (Func<int, IObservable<ICharacter>>)((int _girlIds) => _characterProvider.Get(_girlIds))))));
		IObservable<Unit> observable3 = Observable.AsUnitObservable<SkinData>(Observable.DefaultIfEmpty<SkinData>(Observable.Concat<SkinData>(Observable.Select<int, IObservable<SkinData>>(Observable.SelectMany<LootboxLinkedContent, int>(observable, (Func<LootboxLinkedContent, IEnumerable<int>>)((LootboxLinkedContent content) => content.Lootbox.SkinIdPossibleDrops)), (Func<int, IObservable<SkinData>>)((int ids) => _skinDataLoadingController.InsertDataOnLoad(ids))))));
		ObservableExtensions.Subscribe<OfferSettings>(Observable.Select<Unit, OfferSettings>(Observable.WhenAll(new IObservable<Unit>[2] { observable2, observable3 }), (Func<Unit, OfferSettings>)((Unit _) => offer)), (Action<OfferSettings>)delegate
		{
			windowOpener.Click();
			_sectionController.ForceLoadSection(offer);
		});
	}

	private int ReadValue()
	{
		int result;
		int result2 = ((!int.TryParse(_inputField.text, out result)) ? 1 : result);
		_inputField.text = string.Empty;
		return result2;
	}

	private SceneContext GetSceneContext(Scene scene)
	{
		return (from go in scene.GetRootGameObjects()
			select go.GetComponent<SceneContext>()).FirstOrDefault((SceneContext sc) => (UnityEngine.Object)(object)sc != null);
	}

	private Scene FindSceneByName(string sceneName)
	{
		int sceneCount = SceneManager.sceneCount;
		for (int i = 0; i < sceneCount; i++)
		{
			Scene sceneAt = SceneManager.GetSceneAt(i);
			if (sceneAt.name.Equals(sceneName, StringComparison.OrdinalIgnoreCase))
			{
				return sceneAt;
			}
		}
		return default(Scene);
	}
}
