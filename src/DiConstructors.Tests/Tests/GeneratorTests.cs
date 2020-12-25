using Xapu.SourceGen.DiConstructors.Tests.FixtureTypes;
using Xunit;

namespace Xapu.SourceGen.DiConstructors.Tests
{
    public class GeneratorTests
    {
        [Fact]
        public void BasicControllerGeneration()
        {
            var calculator = new BasicCalculator(new Addition(), new Subtraction());

            Assert.Equal(3, calculator.Add(2, 1));
            Assert.Equal(1, calculator.Subtract(2, 1));
        }

        [Fact]
        public void WithInheritedInjectedFields()
        {
            var calculator = new FullPowerCalculator(new Addition(), new Subtraction(), new Multiplication(), new Division());

            Assert.Equal(6, calculator.Add(4, 2));
            Assert.Equal(2, calculator.Subtract(4, 2));
            Assert.Equal(8, calculator.Multiply(4, 2));
            Assert.Equal(3, calculator.Divide(6, 2));
        }

        [Fact]
        public void WithOnlyInheritedInjectedFields()
        {
            var calculator = new ChildBasicCalculator(new Addition(), new Subtraction());

            Assert.Equal(3, calculator.Add(2, 1));
            Assert.Equal(1, calculator.Subtract(2, 1));
        }

        [Fact]
        public void ForGenericTypes()
        {
            var relayer2 = new DoubleDummyRelayer<int, string>(new Relayer<int>(), new Relayer<string>());

            Assert.Equal(42, relayer2.RelayT(42));
            Assert.Equal("Foo", relayer2.RelayU("Foo"));
        }
    }
}
