using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using StripClub.UI;
using UnityEngine;
using UnityEngine.UI;

namespace GreenT.HornyScapes.Relationships.Views.Aminations;

public class LovePointsAnimateCollect : MonoBehaviour
{
	private const float DurationSpread = 0.2f;

	private const float FromHiddenToVisibleTimeInSeconds = 0.1f;

	[SerializeField]
	private int _imagesCount = 4;

	[SerializeField]
	private float _duration;

	[SerializeField]
	private float _delay;

	[SerializeField]
	private Vector3 _startOffset = Vector3.zero;

	[SerializeField]
	private Vector3 _startScale = Vector3.one;

	private ImagePool _pool;

	private Transform _startPoint;

	private Transform _endPoint;

	private Sprite _rewardIcon;

	private bool _isInited;

	private readonly List<Image> _items = new List<Image>(32);

	private readonly List<Coroutine> _activeCoroutines = new List<Coroutine>(4);

	public void Init(ImagePool pool, Transform startPoint, Transform endPoint, Sprite rewardIcon)
	{
		_pool = pool;
		_endPoint = endPoint;
		_startPoint = startPoint;
		_rewardIcon = rewardIcon;
		_isInited = true;
	}

	public void Launch()
	{
		if (_isInited)
		{
			for (int i = 0; i < _imagesCount; i++)
			{
				Image instance = _pool.GetInstance();
				instance.sprite = _rewardIcon;
				instance.transform.localScale = _startScale;
				instance.transform.position = _startPoint.position + _startOffset;
				instance.color = new Color(instance.color.r, instance.color.g, instance.color.b, 0f);
				instance.gameObject.SetActive(value: true);
				_items.Add(instance);
				float delay = _delay * (float)i;
				instance.DOColor(new Color(instance.color.r, instance.color.g, instance.color.b, 1f), 0.1f).SetDelay(delay);
				Coroutine item = StartCoroutine(MoveItemTowardsTarget(instance, delay));
				_activeCoroutines.Add(item);
			}
		}
	}

	private IEnumerator MoveItemTowardsTarget(Image item, float delay)
	{
		yield return new WaitForSeconds(delay);
		Transform itemTransform = item.transform;
		Vector3 startPos = itemTransform.position;
		float actualDuration = _duration + Random.Range(0f, 0.2f);
		float elapsed = 0f;
		while (elapsed < actualDuration)
		{
			elapsed += Time.deltaTime;
			float t = Mathf.Clamp01(elapsed / actualDuration);
			Vector3 b = ((_endPoint != null) ? _endPoint.position : startPos);
			Vector3 position = Vector3.Lerp(startPos, b, t);
			itemTransform.position = position;
			yield return null;
		}
		ReleaseItem(item);
	}

	private void ReleaseItem(Image item)
	{
		if (!(item == null))
		{
			_pool.Return(item);
			_items.Remove(item);
		}
	}

	private void OnDisable()
	{
		if (!_isInited)
		{
			return;
		}
		foreach (Coroutine activeCoroutine in _activeCoroutines)
		{
			if (activeCoroutine != null)
			{
				StopCoroutine(activeCoroutine);
			}
		}
		_activeCoroutines.Clear();
		foreach (Image item in _items)
		{
			if (item != null)
			{
				_pool.Return(item);
			}
		}
		_items.Clear();
	}
}
