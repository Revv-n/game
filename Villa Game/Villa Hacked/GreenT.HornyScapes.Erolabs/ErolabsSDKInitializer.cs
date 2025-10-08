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

	private Games.Coresdk.Unity.Language GetLanguageFromSystem()
	{
		return Application.systemLanguage switch
		{
			SystemLanguage.ChineseSimplified => Games.Coresdk.Unity.Language.cn, 
			SystemLanguage.ChineseTraditional => Games.Coresdk.Unity.Language.zh, 
			SystemLanguage.Chinese => Games.Coresdk.Unity.Language.cn, 
			SystemLanguage.Japanese => Games.Coresdk.Unity.Language.jp, 
			SystemLanguage.Vietnamese => Games.Coresdk.Unity.Language.vi, 
			SystemLanguage.Indonesian => Games.Coresdk.Unity.Language.@in, 
			SystemLanguage.French => Games.Coresdk.Unity.Language.fr, 
			SystemLanguage.Russian => Games.Coresdk.Unity.Language.ru, 
			SystemLanguage.Spanish => Games.Coresdk.Unity.Language.es, 
			SystemLanguage.Korean => Games.Coresdk.Unity.Language.ko, 
			_ => Games.Coresdk.Unity.Language.en, 
		};
	}
}
