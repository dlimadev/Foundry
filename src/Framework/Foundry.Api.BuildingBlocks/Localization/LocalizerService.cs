using Microsoft.Extensions.Localization;

namespace Foundry.Api.BuildingBlocks.Localization
{
    /// <summary>
    /// Concrete implementation of ILocalizerService using ASP.NET Core's IStringLocalizer.
    /// </summary>
    /// <typeparam name="TResource">A marker type used to identify the .resx resource file.</typeparam>
    public class LocalizerService<TResource> : ILocalizerService where TResource : class
    {
        private readonly IStringLocalizer<TResource> _localizer;

        public LocalizerService(IStringLocalizer<TResource> localizer)
        {
            _localizer = localizer;
        }

        public string GetLocalizedString(string key, params object[] args)
        {
            var localizedString = _localizer[key, args];
            return localizedString.Value;
        }
    }
}