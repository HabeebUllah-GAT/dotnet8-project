using System.Reflection;
using Microsoft.Extensions.Localization;

namespace GAT_Integrations.Resources
{
    public class CommonLocalization
    {
        private readonly IStringLocalizer _localizer1;
        public CommonLocalization(IStringLocalizerFactory factory)
        {
            var assemblyName = new AssemblyName(typeof(Resource).GetTypeInfo().Assembly.FullName);
            _localizer1 = factory.Create(nameof(Resource), assemblyName.Name);
        }

        public string Get(string key)
        {
            return _localizer1[key].Value;
        }

        public LocalizedString Getstr(string key)
        {
            return _localizer1[key];
        }
    }
}