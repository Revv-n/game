using System;
using System.Collections;
using StripClub.UI;
using UniRx;
using UnityEngine;

namespace GreenT.HornyScapes;

public class TypingTextSystem : LocalizedTextMeshPro
{
	[SerializeField]
	private float delay;

	private IDisposable typing;

	public void TestTypingText()
	{
		text.maxVisibleCharacters = 0;
		typing?.Dispose();
		typing = Observable.FromCoroutine(() => ShowSymbol()).Subscribe(delegate
		{
		});
	}

	public void TypingText(string key, params object[] arguments)
	{
		Init(key, arguments);
		text.maxVisibleCharacters = 0;
		typing?.Dispose();
		typing = Observable.FromCoroutine(() => ShowSymbol()).Subscribe(delegate
		{
		});
	}

	private IEnumerator ShowSymbol()
	{
		int lenght = text.text.Length;
		while (lenght > text.maxVisibleCharacters)
		{
			yield return new WaitForSeconds(delay);
			text.maxVisibleCharacters++;
		}
	}
}
