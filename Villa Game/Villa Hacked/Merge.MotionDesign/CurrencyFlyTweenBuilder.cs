using DG.Tweening;
using GreenT.HornyScapes;
using GreenT.HornyScapes.Animations;
using GreenT.HornyScapes.Resources.UI;
using GreenT.Types;
using GreenT.UI;
using StripClub.Model;
using UnityEngine;
using Zenject;

namespace Merge.MotionDesign;

public class CurrencyFlyTweenBuilder : MonoBehaviour
{
	[Inject]
	private GameSettings gameSettings;

	[Header("Animation Settings:")]
	[SerializeField]
	private CurrencyFlySettingsSO flySettings;

	[Space]
	public FlyingCurrency FlyingCurrency;

	public SpriteBezierAnimate BezierAnimate;

	[Header("Old Animation settings")]
	[SerializeField]
	private float timeJamk;

	[SerializeField]
	private float timeFly;

	[SerializeField]
	private float timeScale;

	[SerializeField]
	private float scaleFly;

	[SerializeField]
	private float scaleOut;

	[SerializeField]
	private Vector3[] jamkScales;

	[Inject]
	private IWindowsManager windowsManager;

	private ResourcesWindow resourcesWindow;

	private void Awake()
	{
		resourcesWindow = windowsManager.Get<ResourcesWindow>();
	}

	public void FlyManyCurrency(GameItem gameItem, CurrencyType type, int count = 3, CompositeIdentificator compositeIdentificator = default(CompositeIdentificator), Sprite sprite = null)
	{
		Transform iconTransform = GetIconTransform(type);
		if (!(iconTransform == null))
		{
			GIGhost ghost = CopyObject(gameItem);
			((Component)(object)this.CreateObjectFromInstance<ParticleSystem>(flySettings.DustCloud, ghost.transform)).gameObject.SetActive(value: true);
			int num = Mathf.Clamp(count, flySettings.MinCount, flySettings.MaxCount);
			Sequence sequence = DOTween.Sequence();
			for (int i = 0; i < num; i++)
			{
				Sequence t = CreateFlySequence(ghost, type, iconTransform.position, sprite, compositeIdentificator);
				sequence = sequence.Insert(flySettings.DelayBetweenCurrency * (float)i, t);
			}
			if (type != CurrencyType.EventXP)
			{
				sequence = AddPunchToCurrencyIcon(sequence, iconTransform);
			}
			sequence.OnComplete(delegate
			{
				Object.Destroy(ghost.gameObject);
			});
		}
		static GIGhost CopyObject(GameItem _gameItem)
		{
			GIGhost gIGhost = GIGhost.CreateGhost(_gameItem);
			gIGhost.transform.position = _gameItem.transform.position;
			return gIGhost;
		}
	}

	private Sequence AddPunchToCurrencyIcon(Sequence sequence, Transform target)
	{
		float atPosition = BezierAnimate.Duration + flySettings.TimeShift;
		Vector3 endValue = Vector3.one * flySettings.PunchIconScale;
		return sequence.Insert(atPosition, target.DOScale(endValue, flySettings.PunchIconDuration)).Append(target.DOScale(Vector3.one, flySettings.PunchIconDuration / 2f)).OnKill(delegate
		{
			target.localScale = Vector3.one;
		});
	}

	private Sequence CreateFlySequence(GIGhost ghost, CurrencyType type, Vector3 targetPosition, Sprite sprite, CompositeIdentificator compositeIdentificator = default(CompositeIdentificator))
	{
		FlyingCurrency flyCurrency = CreateFlyingCurrency(ghost.transform, type, sprite, compositeIdentificator);
		return BuildFlyTween(flyCurrency.transform, ghost.transform.position, targetPosition).Join(DOTweenModuleSprite.DOFade(flyCurrency.Icon, flySettings.EndAlpha.Value, flySettings.EndAlpha.Duration)).Join(ghost.IconRenderer.transform.DOScale(Vector3.zero, flySettings.ScaleToZeroDuration).SetEase(flySettings.GhostScaleEase)).OnComplete(delegate
		{
			Object.Destroy(flyCurrency.gameObject);
		});
	}

	private Transform GetIconTransform(CurrencyType type)
	{
		if (resourcesWindow == null)
		{
			resourcesWindow = windowsManager.Get<ResourcesWindow>();
		}
		return resourcesWindow.GetCurrencyTransform(type, rect: false);
	}

	private FlyingCurrency CreateFlyingCurrency(Transform parent, CurrencyType type, Sprite sprite, CompositeIdentificator compositeIdentificator = default(CompositeIdentificator))
	{
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		FlyingCurrency flyingCurrency = CreateObjectFromInstance(FlyingCurrency, parent);
		flyingCurrency.Icon.sprite = sprite ?? gameSettings.CurrencySettings[type, compositeIdentificator].Sprite;
		Color color = flyingCurrency.Icon.color;
		flyingCurrency.Icon.color = new Color(color.r, color.g, color.b, flySettings.StartAlpha);
		float num = Random.Range(0f - flySettings.AnimationScaleDispersion, 0f);
		flyingCurrency.transform.localScale *= 1f + num;
		MainModule main = flyingCurrency.Stars.main;
		CurrencyFlySettings currencyFlySettings = flySettings.CurrencySettings[type];
		((MainModule)(ref main)).startColor = MinMaxGradient.op_Implicit(currencyFlySettings.StartColor);
		TrailModule trails = flyingCurrency.Trail.trails;
		((TrailModule)(ref trails)).colorOverTrail = MinMaxGradient.op_Implicit(currencyFlySettings.OverTrail);
		((TrailModule)(ref trails)).colorOverLifetime = MinMaxGradient.op_Implicit(currencyFlySettings.OverLifeTime);
		return flyingCurrency;
	}

	public T CreateObjectFromInstance<T>(T gameObject, Transform parent) where T : Object
	{
		return Object.Instantiate(gameObject, parent.position, Quaternion.identity, parent);
	}

	public Sequence BuildFlyTween(Transform flyingObject, Vector3 from, Vector3 to)
	{
		return BezierAnimate.Launch(from, to, flyingObject);
	}
}
