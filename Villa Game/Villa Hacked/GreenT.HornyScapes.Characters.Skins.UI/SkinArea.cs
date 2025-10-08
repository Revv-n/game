using System;
using System.Collections.Generic;
using System.Linq;
using GreenT.HornyScapes.Lockers;
using GreenT.HornyScapes.UI;
using StripClub.UI;
using UniRx;
using UnityEngine;
using Zenject;

namespace GreenT.HornyScapes.Characters.Skins.UI;

public class SkinArea : CharacterView
{
	[Tooltip("Минимальное количество скинов, которое будет отображаться. Если в конфигах скинов не хватает, то будут созданы болванки для того, чтобы заполнить пустые места")]
	[SerializeField]
	private int minSkinsCountToDisplay = 6;

	private IViewManager<PromoteSkinView.Settings, PromoteSkinView> viewManager;

	private SkinManager skinManager;

	private IDisposable skinSelectStream;

	private DefaultSkinFactory skinFactory = new DefaultSkinFactory();

	[Inject]
	public void Init(IViewManager<PromoteSkinView.Settings, PromoteSkinView> viewManager, SkinManager skinManager)
	{
		this.viewManager = viewManager;
		this.skinManager = skinManager;
	}

	public override void Set(CharacterSettings source)
	{
		base.Set(source);
		skinSelectStream?.Dispose();
		viewManager.HideAll();
		List<Skin> visibleSkins = GetVisibleSkins(source);
		IObservable<Skin> observable = Observable.Empty<Skin>();
		foreach (Skin item in visibleSkins)
		{
			try
			{
				PromoteSkinView.Settings source2 = new PromoteSkinView.Settings(item, base.Source);
				PromoteSkinView promoteSkinView = viewManager.Display(source2);
				observable = Observable.Merge<Skin>(observable, new IObservable<Skin>[1] { promoteSkinView.OnSelect });
			}
			catch (Exception innerException)
			{
				innerException.SendException("Can't display: " + item);
			}
		}
		skinSelectStream = ObservableExtensions.Subscribe<Skin>(observable, (Action<Skin>)TrySelectSkin);
	}

	private List<Skin> GetVisibleSkins(CharacterSettings source)
	{
		Skin item = skinFactory.Create(source.Public);
		List<Skin> list = new List<Skin> { item };
		list.AddRange(skinManager.GetSkinByCharacter(base.Source.Public.ID));
		IEnumerable<Skin> dummies = GetDummies(list);
		list.AddRange(dummies);
		Comparison<Skin> comparison = delegate(Skin _skin1, Skin _skin2)
		{
			if (_skin1.IsOwned && _skin2.IsOwned)
			{
				return _skin1.OrderNumber - _skin2.OrderNumber;
			}
			return (!_skin1.IsOwned) ? 1 : (-1);
		};
		list.Sort(comparison);
		return list;
	}

	private IEnumerable<Skin> GetDummies(List<Skin> skins)
	{
		int num = minSkinsCountToDisplay - skins.Count();
		Skin[] array = new Skin[(num >= 0) ? num : 0];
		if (num > 0)
		{
			int num2 = skins.Last().ID + 1;
			for (int i = 0; i != array.Length; i++)
			{
				int dummyID = num2 + i;
				Skin skin = CreateDummy(base.Source.Public.ID, dummyID);
				array[i] = skin;
			}
		}
		return array;
	}

	private Skin CreateDummy(int girlID, int dummyID)
	{
		return new Skin(new SkinMapper
		{
			id = dummyID,
			girl_id = girlID,
			order_number = int.MaxValue,
			unlock_message = string.Empty
		}, new PermanentLocker(isOpen: false));
	}

	public void TrySelectSkin(Skin skin)
	{
		int skin2 = ((base.Source.SkinID != skin.ID) ? skin.ID : 0);
		base.Source.SetSkin(skin2);
	}

	protected virtual void OnDisable()
	{
		skinSelectStream?.Dispose();
	}
}
