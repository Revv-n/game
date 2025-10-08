using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;

namespace GreenT.Net.User;

public class PostRequestProcessor<T> : IDisposable where T : Response
{
	private IPostRequest<T> postRequest;

	private IDisposable requestStream;

	private Dictionary<Action<T>, int> processorDictionary;

	private List<Action<Exception>> exceptionProcessorList;

	private IEnumerable<Action<T>> sortedProcessors;

	public PostRequestProcessor(IPostRequest<T> postRequest)
	{
		this.postRequest = postRequest;
		processorDictionary = new Dictionary<Action<T>, int>();
		sortedProcessors = from _pair in processorDictionary
			orderby _pair.Value
			select _pair.Key;
		exceptionProcessorList = new List<Action<Exception>>();
	}

	protected void PostRequest(IDictionary<string, string> requestParameters)
	{
		requestStream?.Dispose();
		requestStream = ObservableExtensions.Subscribe<T>(Observable.DoOnCancel<T>(postRequest.Post(requestParameters), (Action)OnCancel), (Action<T>)ProcessResponse, (Action<Exception>)OnError, (Action)OnCompleted);
	}

	protected virtual void OnCancel()
	{
	}

	protected virtual void OnCompleted()
	{
	}

	protected virtual void OnError(Exception exception)
	{
		exception.LogException();
		foreach (Action<Exception> exceptionProcessor in exceptionProcessorList)
		{
			exceptionProcessor(exception);
		}
	}

	protected virtual void ProcessResponse(T response)
	{
		foreach (Action<T> sortedProcessor in sortedProcessors)
		{
			sortedProcessor(response);
		}
	}

	public void AddListener(Action<T> responseProcessor, int order = 0)
	{
		processorDictionary[responseProcessor] = order;
	}

	public void AddExceptionListener(Action<Exception> exceptionProcessor)
	{
		exceptionProcessorList.Add(exceptionProcessor);
	}

	public void RemoveListener(Action<T> responseProcessor)
	{
		processorDictionary.Remove(responseProcessor);
	}

	public void RemoveExceptionListener(Action<Exception> exceptionProcessor)
	{
		exceptionProcessorList.Remove(exceptionProcessor);
	}

	public void AddListener(Action<T> responseProcessor, Action<Exception> exceptionProcessor, int order = 0)
	{
		AddListener(responseProcessor, order);
		AddExceptionListener(exceptionProcessor);
	}

	public bool Contains(Action<T> responseProcessor)
	{
		return processorDictionary.ContainsKey(responseProcessor);
	}

	public virtual void Dispose()
	{
		requestStream?.Dispose();
	}
}
