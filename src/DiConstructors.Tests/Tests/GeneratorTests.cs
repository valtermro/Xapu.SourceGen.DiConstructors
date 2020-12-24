using Xunit;

namespace Xapu.SourceGen.DiConstructors.Tests.Tests
{
    public class GeneratorTests
    {
        [Fact]
        public void Passes()
        {
            var generator = new Generator();

            Assert.True(generator.Passes());
        }
    }
}
