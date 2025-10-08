using System;
using System.Collections;
using Erolabs.Sdk.Unity;
using Games.Coresdk.Unity;
using UnityEngine;

namespace GreenT.HornyScapes.Erolabs;

public class ErolabsSDKInitializer : MonoBehaviour
{
	public IEnumerator Init(Action callback)
	{
		yield return ErolabsSDK.Initialize(delegate
		{
		}, GetLanguageFromSystem());
		callback();
	}

	private Language GetLanguageFromSystem()
	{
		return Application.systemLanguage switch
		{
			SystemLanguage.ChineseSimplified => Language.cn, 
			SystemLanguage.ChineseTraditional => Language.zh, 
			SystemLanguage.Chinese => Language.cn, 
			SystemLanguage.Japanese => Language.jp, 
			SystemLanguage.Vietnamese => Language.vi, 
			SystemLanguage.Indonesian => Language.@in, 
			SystemLanguage.French => Language.fr, 
			SystemLanguage.Russian => Language.ru, 
			SystemLanguage.Spanish => Language.es, 
			SystemLanguage.Korean => Language.ko, 
			_ => Language.en, 
		};
	}
}
