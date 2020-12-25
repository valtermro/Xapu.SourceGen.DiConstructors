using System.Linq;
using System.Xml.Linq;

namespace Xapu.SourceGen.DiConstructors
{
    internal class GenerationConfig
    {
        public string PostConstructorMethodName { get; private set; }

        private static GenerationConfig WithDefaults()
        {
            return new GenerationConfig
            {
                PostConstructorMethodName = "AfterConstructor"
            };
        }

        public static GenerationConfig FromXDocument(XDocument configXDocument)
        {
            var config = WithDefaults();
            var xElements = configXDocument?.DescendantNodes().OfType<XElement>();

            if (TryGetXElementValue(nameof(PostConstructorMethodName), out string postConstructorMethodName))
                config.PostConstructorMethodName = postConstructorMethodName;

            return config;

            bool TryGetXElementValue(string nodeName, out string value)
            {
                value = xElements?.FirstOrDefault(p => p.Name == nodeName)?.Value;
                return !string.IsNullOrEmpty(value);
            }
        }
    }
}
