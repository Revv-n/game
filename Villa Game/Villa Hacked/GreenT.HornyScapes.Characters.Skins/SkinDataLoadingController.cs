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
		Observable.SelectMany<Skin, Skin>(Observable.Where<Skin>(Observable.Merge<Skin>(Observable.ToObservable<Skin>(_skinManager.Collection), new IObservable<Skin>[1] { _skinManager.OnNew }), (Func<Skin, bool>)((Skin _skin) => _skin.IsDataEmpty)), (Func<Skin, IObservable<Skin>>)((Skin _skin) => Observable.Select<bool, Skin>(Observable.FirstOrDefault<bool>((IObservable<bool>)_skin.Locker.IsOpen, (Func<bool, bool>)((bool x) => x)), (Func<bool, Skin>)((bool _) => _skin))));
	}

	public IObservable<SkinData> InsertDataOnLoad(int id)
	{
		Skin skin2 = _skinManager.Collection.FirstOrDefault((Skin skin) => skin.ID == id);
		return InsertDataOnLoad(skin2);
	}

	public IObservable<SkinData> InsertDataOnLoad(Skin skin)
	{
		return Observable.Do<SkinData>(_skinLoader.Load(skin.ID), (Action<SkinData>)skin.Insert);
	}

	public IObservable<SkinData> LoadOwnedSkins()
	{
		return Observable.DefaultIfEmpty<SkinData>(Observable.Concat<SkinData>(Observable.Select<Skin, IObservable<SkinData>>(Observable.Where<Skin>(Observable.ToObservable<Skin>(_skinManager.Collection), (Func<Skin, bool>)((Skin skin) => skin.IsOwned && skin.Locker.IsOpen.Value && skin.IsDataEmpty)), (Func<Skin, IObservable<SkinData>>)InsertDataOnLoad)));
	}
}
