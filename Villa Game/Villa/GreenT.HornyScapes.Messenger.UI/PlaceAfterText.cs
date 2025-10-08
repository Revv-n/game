using TMPro;
using UnityEngine;

namespace GreenT.HornyScapes.Messenger.UI;

public sealed class PlaceAfterText : MonoBehaviour
{
	[Header("Источник текста")]
	[SerializeField]
	private TMP_Text _sourceText;

	[Header("Перемещаемый контейнер")]
	[SerializeField]
	private RectTransform _targetContainer;

	[Header("Отступ сверху для нового ряда (в пикселях)")]
	[SerializeField]
	private float _lineHeightOffset = 30f;

	[Header("Отступ слева от текста (в пикселях)")]
	[SerializeField]
	private float _leftOffset = 5f;

	[Header("Отступ слева от текста для новой строки (в пикселях)")]
	[SerializeField]
	private float _leftNewLineOffset = 20f;

	[Header("Текст-заглушка для создания переноса в тексте")]
	[SerializeField]
	private string _dummyText = "<br><color=#00000000> </color>";

	private void LateUpdate()
	{
		if (_sourceText == null || _targetContainer == null || !_targetContainer.gameObject.activeSelf)
		{
			return;
		}
		_sourceText.ForceMeshUpdate();
		TMP_TextInfo textInfo = _sourceText.textInfo;
		int characterCount = textInfo.characterCount;
		if (characterCount == 0)
		{
			_targetContainer.anchoredPosition = new Vector2(0f, 0f);
			return;
		}
		TMP_CharacterInfo tMP_CharacterInfo = textInfo.characterInfo[characterCount - 1];
		float x = tMP_CharacterInfo.topRight.x;
		float baseLine = tMP_CharacterInfo.baseLine;
		float num = _sourceText.rectTransform.rect.width * 0.5f;
		float num2 = _targetContainer.rect.width * 0.5f;
		float num3 = _targetContainer.rect.height * 0.25f;
		float num4 = x + num2;
		Vector2 anchoredPosition;
		if (!(num < num4 + _leftOffset))
		{
			anchoredPosition = ((tMP_CharacterInfo.character != ' ') ? new Vector2(num4 + _leftOffset, baseLine + num3) : new Vector2(num4, baseLine + num3));
		}
		else
		{
			_sourceText.text += _dummyText;
			float num5 = ((_lineHeightOffset != 0f) ? _lineHeightOffset : (_sourceText.preferredHeight / (float)_sourceText.textInfo.lineCount));
			anchoredPosition = new Vector2(0f, baseLine - num5);
		}
		_targetContainer.anchoredPosition = anchoredPosition;
	}
}
