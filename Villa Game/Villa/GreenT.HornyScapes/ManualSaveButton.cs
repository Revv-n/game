using System;
using DG.Tweening;
using GreenT.HornyScapes.Saves;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace GreenT.HornyScapes;

public class ManualSaveButton : MonoBehaviour
{
	private const float FADE_DURATION = 1.5f;

	[Inject]
	private ManualSave _saveEvent;

	[SerializeField]
	private Button _saveBtn;

	[SerializeField]
	private Image _checkMarkImage;

	private IDisposable _saveButtonStream;

	private void OnEnable()
	{
		_saveButtonStream = _saveBtn.OnClickAsObservable().Do(delegate
		{
			FadeCheckMark();
		}).Subscribe(delegate
		{
			_saveEvent.Save();
		});
	}

	private void FadeCheckMark()
	{
		_checkMarkImage.color = Color.white;
		_checkMarkImage.DOFade(0f, 1.5f);
	}

	private void OnDisable()
	{
		_saveButtonStream?.Dispose();
	}
}
