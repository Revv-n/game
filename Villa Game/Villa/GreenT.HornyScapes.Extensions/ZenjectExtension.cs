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
		container.ResolveUrlByType(type).WhenInjectedInto<T>();
	}

	public static ScopeConcreteIdArgConditionCopyNonLazyBinder ResolveUrlByType(this DiContainer container, PostRequestType type)
	{
		return container.Bind<string>().FromResolveGetter((IProjectSettings _settings) => _settings.RequestUrlResolver.PostRequestUrl(type));
	}

	public static void BindPostRequest<T>(this DiContainer container, PostRequestType type)
	{
		container.Bind<T>().AsSingle();
		container.BindUrlWhenInjectedToType<T>(type);
	}

	public static void BindViewStructure<T, K>(this DiContainer container, Object prefab, Transform parent) where T : MonoView where K : ViewManager<T>
	{
		container.Bind<K>().FromNewComponentOn(parent.gameObject).AsSingle()
			.WhenInjectedInto<SmallCardsViewManager>();
		container.BindIFactory<T>().FromComponentInNewPrefab(prefab).UnderTransform(parent);
	}

	public static void BindViewStructure<M, T, K>(this DiContainer container, Object prefab, Transform parent, string id) where T : MonoView where K : ViewManager<T>
	{
		container.Bind<K>().WithId(id).FromNewComponentOn(parent.gameObject)
			.AsCached()
			.WhenInjectedInto<M>();
		container.BindIFactory<T>().FromComponentInNewPrefab(prefab).UnderTransform(parent)
			.When((InjectContext _context) => _context.ParentContext.Identifier != null && _context.ParentContext.Identifier.Equals(id));
	}

	public static InstantiateCallbackConditionCopyNonLazyBinder BindViewFactory<TSource, TView>(this DiContainer container, Transform parent, TView prefab) where TView : MonoView<TSource>
	{
		return container.BindInterfacesAndSelfTo<ViewFactory<TSource, TView>>().AsSingle().WithArguments(parent, prefab);
	}

	public static void BindRequestProcessor<TRequest, TProcessor>(this DiContainer container, PostRequestType type, string key = "") where TRequest : IPostRequest<Response<UserDataMapper>> where TProcessor : UserPostRequestProcessor
	{
		if (string.IsNullOrWhiteSpace(key))
		{
			container.BindUrlWhenInjectedToType<TRequest>(type);
			container.Bind<IPostRequest<Response<UserDataMapper>>>().To<TRequest>().WhenInjectedInto<TProcessor>();
		}
		else
		{
			container.ResolveUrlByType(type).WithConcreteId(key);
			container.Bind<IPostRequest<Response<UserDataMapper>>>().WithId(key).To<TRequest>()
				.WhenInjectedInto<TProcessor>();
		}
		container.Bind(typeof(UserPostRequestProcessor), typeof(TProcessor)).To<TProcessor>().AsCached();
	}
}
