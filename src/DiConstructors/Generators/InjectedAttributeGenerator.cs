using Xapu.SourceGen.DiConstructors.TextBuffers;

namespace Xapu.SourceGen.DiConstructors.Generators
{
    internal class InjectedAttributeGenerator
    {
        private readonly GenerationConfig _config;

        public InjectedAttributeGenerator(GenerationConfig config)
        {
            _config = config;
        }

        public void Execute(ISourceTextBuffer buffer)
        {
            buffer.WriteLine("using System;");
            buffer.WriteLine();
            buffer.WriteLine($"namespace {_config.InjectedAttributeNamespace}");
            buffer.BeginBlock();

            buffer.WriteLine("[AttributeUsage(AttributeTargets.Field)]");
            buffer.WriteLine($"public class {_config.InjectedAttributeName}Attribute : Attribute");
            buffer.BeginBlock();
            buffer.EndBlock();

            buffer.EndBlock();
        }
    }
}
