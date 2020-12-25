using System.Linq;
using System.Xml.Linq;

namespace Xapu.SourceGen.DiConstructors
{
    internal class GenerationConfig
    {
        public string InjectedAttributeName { get; private set; }
        public string InjectedAttributeNamespace { get; private set; }

        private static GenerationConfig WithDefaults()
        {
            return new GenerationConfig
            {
                InjectedAttributeName = "Injected",
                InjectedAttributeNamespace = typeof(Generator).Namespace.ToString()
            };
        }

        public static GenerationConfig FromXDocument(XDocument configXDocument)
        {
            var config = WithDefaults();
            var xElements = configXDocument?.DescendantNodes().OfType<XElement>();

            if (TryGetXElementValue(nameof(InjectedAttributeName), out string injectedAttrName))
                config.InjectedAttributeName = injectedAttrName;

            if (TryGetXElementValue(nameof(InjectedAttributeNamespace), out string injectedAttrNamespace))
                config.InjectedAttributeNamespace = injectedAttrNamespace;

            return config;

            bool TryGetXElementValue(string nodeName, out string value)
            {
                value = xElements?.FirstOrDefault(p => p.Name == nodeName)?.Value;
                return !string.IsNullOrEmpty(value);
            }
        }
    }
}
