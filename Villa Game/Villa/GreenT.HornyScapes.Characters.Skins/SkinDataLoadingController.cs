using System;
using System.Linq;
using UniRx;

namespace GreenT.HornyScapes.Characters.Skins;

public class SkinDataLoadingController
{
	private readonly SkinManager _skinManager;

	private readonly SkinLoader _skinLoader;

	private readonly GameStarter _gameStarter;

	public SkinDataLoadingController(SkinManager skinManager, SkinLoader skinLoader, GameStarter gameStarter)
	{
		_skinManager = skinManager;
		_skinLoader = skinLoader;
		_gameStarter = gameStarter;
	}

	public void Initialize()
	{
		(from _skin in _skinManager.Collection.ToObservable().Merge(_skinManager.OnNew)
			where _skin.IsDataEmpty
			select _skin).SelectMany((Skin _skin) => from _ in _skin.Locker.IsOpen.FirstOrDefault((bool x) => x)
			select _skin);
	}

	public IObservable<SkinData> InsertDataOnLoad(int id)
	{
		Skin skin2 = _skinManager.Collection.FirstOrDefault((Skin skin) => skin.ID == id);
		return InsertDataOnLoad(skin2);
	}

	public IObservable<SkinData> InsertDataOnLoad(Skin skin)
	{
		return _skinLoader.Load(skin.ID).Do(skin.Insert);
	}

	public IObservable<SkinData> LoadOwnedSkins()
	{
		return (from skin in _skinManager.Collection.ToObservable()
			where skin.IsOwned && skin.Locker.IsOpen.Value && skin.IsDataEmpty
			select skin).Select(InsertDataOnLoad).Concat().DefaultIfEmpty();
	}
}
