namespace Xapu.SourceGen.DiConstructors.Tests.FixtureTypes
{
    public class Sage
    {
        public object TheAnswerToEverything() => 42;
    }

    public partial class TheAnswer
    {
        [Injected] private readonly Sage _sage;

        public object ToEverything;

        public int PostConstructorCallCount = 0;

        public void AfterConstructor()
        {
            PostConstructorCallCount += 1;
            ToEverything = _sage.TheAnswerToEverything();
        }
    }

    public partial class TheNewAnswer : TheAnswer
    {
        public new void AfterConstructor()
        {
            PostConstructorCallCount += 1;
        }
    }
}
