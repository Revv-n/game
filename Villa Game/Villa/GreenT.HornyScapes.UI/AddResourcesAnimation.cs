using GreenT.HornyScapes.Resources.UI;
using GreenT.Types;
using GreenT.UI;
using Merge;
using StripClub.Model;
using StripClub.UI;
using StripClub.UI.Rewards;
using UnityEngine;
using Zenject;

namespace GreenT.HornyScapes.UI;

public class AddResourcesAnimation : Clip
{
	[SerializeField]
	private AnimateCollect animateCollect;

	[SerializeField]
	private float resourcesAddTime = 1f;

	private CurrencyLinkedContent currencyContent;

	private Sprite currencyIcon;

	private CurrencyType currencyType;

	private Transform endPoint;

	private Transform startPoint;

	private GameSettings gameSettings;

	private SoundController soundController;

	private ResourcesWindow resourceWindow;

	private ImagePool pool;

	[Inject]
	public void InnerInit(GameSettings gameSettings, SoundController soundController, IWindowsManager windowsManager, ImagePool imagePool)
	{
		this.gameSettings = gameSettings;
		this.soundController = soundController;
		resourceWindow = windowsManager.Get<ResourcesWindow>();
		pool = imagePool;
	}

	public void Init(CurrencyLinkedContent content)
	{
		currencyContent = content;
		currencyType = currencyContent.Currency;
		currencyIcon = gameSettings.CurrencySettings[currencyContent.Currency, default(CompositeIdentificator)].Sprite;
		endPoint = resourceWindow.GetCurrencyTransform(currencyContent.Currency);
		animateCollect.Init(pool, endPoint, startPoint, currencyIcon, kill: false);
	}

	public override void Play()
	{
		AnimateFlyingCurrency();
		soundController.PlayCurrencySound(currencyType);
	}

	private void AnimateFlyingCurrency()
	{
		animateCollect.OnEnd += Stop;
		animateCollect.Launch();
	}

	public override void Stop()
	{
		animateCollect.OnEnd -= Stop;
		base.Stop();
	}
}
