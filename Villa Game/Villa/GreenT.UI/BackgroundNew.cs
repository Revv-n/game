using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace GreenT.UI;

public class BackgroundNew : MonoBehaviour
{
	[SerializeField]
	private Canvas _canvas;

	[SerializeField]
	private Image _backGround;

	[SerializeField]
	private float _animationDuration;

	[SerializeField]
	private float _targetRadius;

	[SerializeField]
	private Color _targetColor;

	[SerializeField]
	private Color _androidtargetColor;

	private Sequence _animation;

	private const string RADIUSPROPRTY = "_Radius";

	private const string COLORPROPERTY = "_Color";

	private void Awake()
	{
	}

	public void UpdateBackGround(IWindow window)
	{
		if ((window.Settings & WindowSettings.UseBackground) != 0)
		{
			if (_canvas != null && window.Canvas != null)
			{
				_canvas.sortingOrder = window.Canvas.sortingOrder - 1;
			}
			if (_animation.IsActive())
			{
				_animation.Kill(complete: true);
			}
			if ((bool)_backGround && !_backGround.gameObject.activeSelf)
			{
				Show();
			}
		}
	}

	private void Show()
	{
		_backGround.gameObject.SetActive(value: true);
		if (_animation.IsActive())
		{
			_animation.Kill();
		}
		_animation = DOTween.Sequence().OnStart(delegate
		{
			Init(Color.white, 0f);
		}).Append(_backGround.material.DOColor(_targetColor, "_Color", _animationDuration))
			.Join(_backGround.material.DOFloat(_targetRadius, "_Radius", _animationDuration));
	}

	public void Hide()
	{
		if (_animation.IsActive())
		{
			_animation.Kill();
		}
		_animation = DOTween.Sequence().Append(_backGround.material.DOColor(Color.white, "_Color", _animationDuration)).Join(_backGround.material.DOFloat(0f, "_Radius", _animationDuration))
			.OnComplete(delegate
			{
				_backGround.gameObject.SetActive(value: false);
			});
	}

	private void Init(Color color, float radius)
	{
		_backGround.material.SetColor("_Color", color);
		_backGround.material.SetFloat("_Radius", radius);
	}
}
