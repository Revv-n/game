using System;
using System.Collections.Generic;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;
using UnityEngine.UI;

namespace StripClub.UI.Rewards;

public class AnimateCollect : MonoBehaviour
{
	public ImagePool pool;

	public Sprite RewardIcon;

	[SerializeField]
	private int multiplyCoefitient = 1;

	private const float duratonSpread = 0.2f;

	private const float fromHiddenToVisibleTimeInSeconds = 0.1f;

	[Header("Настройки кривой")]
	public float ArcDegree = 95f;

	public float ArcPower = 50f;

	public float StartAngle = -66f;

	[Header("Смещение промежуточных точек")]
	public float Randomness = 10f;

	public int PointsCount = 9;

	public float powerY = 10f;

	public float powerX = 5f;

	public Transform startPoint;

	public Vector3 startOffset = Vector3.zero;

	public Vector3 startScale;

	[Header("Настройки таргета")]
	public Transform target;

	public float TargetScaleDuration = 0.1f;

	public float TargetScalePower = 0.3f;

	public int TargetScaleLoopCount;

	public Ease TargetScaleEase = Ease.InOutCubic;

	private List<Sequence> sequences = new List<Sequence>();

	private bool killOnHide = true;

	private bool isInited;

	private TweenerCore<Vector3, Vector3, VectorOptions> revertTargetScale;

	public int ImagesCount { get; private set; }

	[field: SerializeField]
	public int DefaultImagesCount { get; set; } = 4;


	public int MultiplyCoefitient
	{
		get
		{
			return multiplyCoefitient;
		}
		set
		{
			multiplyCoefitient = ((value <= 0) ? 1 : value);
			ImagesCount = multiplyCoefitient * DefaultImagesCount;
		}
	}

	[field: SerializeField]
	public float Duration { get; set; }

	[field: SerializeField]
	public float Delay { get; set; }

	public event Action OnEnd;

	private void Awake()
	{
		ImagesCount = multiplyCoefitient * DefaultImagesCount;
	}

	public void Init(ImagePool pool, Transform target = null, Transform startPoint = null, Sprite rewardIcon = null, bool kill = true)
	{
		this.pool = pool;
		if (target != null)
		{
			this.target = target;
			startScale = target.localScale;
		}
		if (startPoint != null)
		{
			this.startPoint = startPoint;
		}
		if ((bool)rewardIcon)
		{
			RewardIcon = rewardIcon;
		}
		killOnHide = kill;
		isInited = true;
	}

	public void Launch()
	{
		if (isInited)
		{
			List<Image> list = new List<Image>();
			Sequence sequence = DOTween.Sequence();
			sequences.Add(sequence);
			for (int i = 0; i < ImagesCount; i++)
			{
				Image instance = pool.GetInstance();
				list.Add(instance);
				instance.sprite = RewardIcon;
				instance.transform.localScale = startScale;
				instance.transform.position = startPoint.position + startOffset;
				instance.gameObject.SetActive(value: true);
				sequence.Insert(Delay * (float)i, instance.transform.DOPath(SetPath(startPoint, target), Duration + UnityEngine.Random.Range(0f, 0.2f)));
				instance.color = new Color(instance.color.r, instance.color.g, instance.color.b, 0f);
				sequence.Insert(Delay * (float)i, DOTweenModuleUI.DOColor(instance, new Color(instance.color.r, instance.color.g, instance.color.b, 1f), 0.1f));
			}
			Vector3 endValue = (revertTargetScale.IsActive() ? revertTargetScale.endValue : target.transform.localScale);
			TweenerCore<Vector3, Vector3, VectorOptions> t = target.DOScale(TargetScalePower, TargetScaleDuration).SetEase(TargetScaleEase);
			revertTargetScale = target.DOScale(endValue, TargetScaleDuration);
			sequence.Prepend(t).Append(revertTargetScale).OnKill(delegate
			{
				EndAnimation(list);
			});
		}
	}

	private Vector3[] SetPath(Transform parent, Transform target)
	{
		float num = ((UnityEngine.Random.Range(0, 2) == 0) ? powerX : (0f - powerX));
		List<Vector3> list = new List<Vector3>();
		list.Add(parent.position);
		Vector3 position = parent.transform.position;
		for (int i = 0; i < PointsCount; i++)
		{
			float t = (float)i / (float)PointsCount;
			float num2 = Mathf.Lerp(StartAngle, ArcDegree, t);
			Vector3 vector = Vector3.Lerp(position, target.transform.position, t);
			Vector3 vector2 = new Vector3(Mathf.Cos((float)Math.PI / 180f * num2) * num, Mathf.Cos((float)Math.PI / 180f * num2) * powerY);
			Vector3 vector3 = UnityEngine.Random.insideUnitSphere * Randomness;
			vector3.z = 0f;
			list.Add(vector + vector2 * ArcPower + vector3);
		}
		list.Add(target.transform.position);
		return list.ToArray();
	}

	private void EndAnimation(List<Image> list)
	{
		ReleaseItems(list);
		this.OnEnd?.Invoke();
	}

	private void ReleaseItems(List<Image> list)
	{
		foreach (Image item in list)
		{
			pool.Return(item);
		}
		list.Clear();
	}

	private void OnValidate()
	{
		if (startPoint == null)
		{
			startPoint = base.transform;
		}
		ImagesCount = multiplyCoefitient * DefaultImagesCount;
	}

	private void OnDisable()
	{
		if (!isInited)
		{
			return;
		}
		if (killOnHide)
		{
			foreach (Sequence sequence in sequences)
			{
				if (sequence.IsActive())
				{
					sequence.Kill(complete: true);
				}
			}
		}
		sequences.Clear();
	}
}
