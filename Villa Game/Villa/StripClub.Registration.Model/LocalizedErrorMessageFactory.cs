using GreenT.Localizations;
using StripClub.Model;
using Zenject;

namespace StripClub.Registration.Model;

public class LocalizedErrorMessageFactory : IFactory<string, long, IErrorMessage>, IFactory
{
	private LocalizationService _localizationService;

	public LocalizedErrorMessageFactory(LocalizationService localizationService)
	{
		_localizationService = localizationService;
	}

	public IErrorMessage Create(string prefix, long code)
	{
		return new LocalizedErrorMessage(_localizationService.Text(prefix + code));
	}
}
