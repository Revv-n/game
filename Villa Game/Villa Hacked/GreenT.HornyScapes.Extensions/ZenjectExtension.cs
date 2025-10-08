using System;
using GreenT.Net;
using GreenT.Net.User;
using GreenT.Settings;
using GreenT.Settings.Data;
using StripClub.Model.Shop.UI;
using StripClub.UI;
using UnityEngine;
using Zenject;

namespace GreenT.HornyScapes.Extensions;

public static class ZenjectExtension
{
	public static void BindUrlWhenInjectedToType<T>(this DiContainer container, PostRequestType type)
	{
		((ConditionCopyNonLazyBinder)container.ResolveUrlByType(type)).WhenInjectedInto<T>();
	}

	public static ScopeConcreteIdArgConditionCopyNonLazyBinder ResolveUrlByType(this DiContainer container, PostRequestType type)
	{
		return ((FromBinderGeneric<string>)(object)container.Bind<string>()).FromResolveGetter<IProjectSettings>((Func<IProjectSettings, string>)((IProjectSettings _settings) => _settings.RequestUrlResolver.PostRequestUrl(type)));
	}

	public static void BindPostRequest<T>(this DiContainer container, PostRequestType type)
	{
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)container.Bind<T>()).AsSingle();
		container.BindUrlWhenInjectedToType<T>(type);
	}

	public static void BindViewStructure<T, K>(this DiContainer container, UnityEngine.Object prefab, Transform parent) where T : MonoView where K : ViewManager<T>
	{
		((ConditionCopyNonLazyBinder)((FromBinder)container.Bind<K>()).FromNewComponentOn(parent.gameObject).AsSingle()).WhenInjectedInto<SmallCardsViewManager>();
		((TransformScopeConcreteIdArgConditionCopyNonLazyBinder)((FactoryFromBinderBase)container.BindIFactory<T>()).FromComponentInNewPrefab(prefab)).UnderTransform(parent);
	}

	public static void BindViewStructure<M, T, K>(this DiContainer container, UnityEngine.Object prefab, Transform parent, string id) where T : MonoView where K : ViewManager<T>
	{
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Expected O, but got Unknown
		((ConditionCopyNonLazyBinder)((FromBinder)container.Bind<K>().WithId((object)id)).FromNewComponentOn(parent.gameObject).AsCached()).WhenInjectedInto<M>();
		((ConditionCopyNonLazyBinder)((TransformScopeConcreteIdArgConditionCopyNonLazyBinder)((FactoryFromBinderBase)container.BindIFactory<T>()).FromComponentInNewPrefab(prefab)).UnderTransform(parent)).When((BindingCondition)((InjectContext _context) => _context.ParentContext.Identifier != null && _context.ParentContext.Identifier.Equals(id)));
	}

	public static InstantiateCallbackConditionCopyNonLazyBinder BindViewFactory<TSource, TView>(this DiContainer container, Transform parent, TView prefab) where TView : MonoView<TSource>
	{
		return ((ArgConditionCopyNonLazyBinder)((ScopeConcreteIdArgConditionCopyNonLazyBinder)container.BindInterfacesAndSelfTo<ViewFactory<TSource, TView>>()).AsSingle()).WithArguments<Transform, TView>(parent, prefab);
	}

	public static void BindRequestProcessor<TRequest, TProcessor>(this DiContainer container, PostRequestType type, string key = "") where TRequest : IPostRequest<Response<UserDataMapper>> where TProcessor : UserPostRequestProcessor
	{
		if (string.IsNullOrWhiteSpace(key))
		{
			container.BindUrlWhenInjectedToType<TRequest>(type);
			((ConditionCopyNonLazyBinder)((ConcreteBinderGeneric<IPostRequest<Response<UserDataMapper>>>)(object)container.Bind<IPostRequest<Response<UserDataMapper>>>()).To<TRequest>()).WhenInjectedInto<TProcessor>();
		}
		else
		{
			((ConcreteIdArgConditionCopyNonLazyBinder)container.ResolveUrlByType(type)).WithConcreteId((object)key);
			((ConditionCopyNonLazyBinder)container.Bind<IPostRequest<Response<UserDataMapper>>>().WithId((object)key).To<TRequest>()).WhenInjectedInto<TProcessor>();
		}
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((ConcreteBinderNonGeneric)container.Bind(new Type[2]
		{
			typeof(UserPostRequestProcessor),
			typeof(TProcessor)
		})).To<TProcessor>()).AsCached();
	}
}
