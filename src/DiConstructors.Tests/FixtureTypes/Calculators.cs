namespace Xapu.SourceGen.DiConstructors.Tests.FixtureTypes
{
    public class Addition
    {
        public int Execute(int a, int b) => a + b;
    }

    public class Subtraction
    {
        public int Execute(int a, int b) => a - b;
    }

    public class Multiplication
    {
        public int Execute(int a, int b) => a * b;
    }

    public class Division
    {
        public int Execute(int a, int b) => a / b;
    }

    public partial class BasicCalculator
    {
        [Injected] protected readonly Addition Addition;
        [Injected] protected readonly Subtraction Subtraction;

        public int Add(int a, int b) => Addition.Execute(a, b);
        public int Subtract(int a, int b) => Subtraction.Execute(a, b);
    }

    public partial class ChildBasicCalculator : BasicCalculator
    {
    }

    public partial class IntermediateCalculator : BasicCalculator
    {
        [Injected] private readonly Multiplication Multiplication;

        public int Multiply(int a, int b) => Multiplication.Execute(a, b);
    }

    public partial class FullPowerCalculator : IntermediateCalculator
    {
        [Injected] private readonly Division Division;

        public int Divide(int a, int b) => Division.Execute(a, b);
    }
}
