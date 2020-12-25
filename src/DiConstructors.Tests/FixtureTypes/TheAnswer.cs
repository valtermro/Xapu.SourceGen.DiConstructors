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

        public virtual void Initialize()
        {
            PostConstructorCallCount += 1;
            ToEverything = _sage.TheAnswerToEverything();
        }
    }

    public partial class TheNewAnswer : TheAnswer
    {
        public new void Initialize()
        {
            PostConstructorCallCount += 1;
        }
    }

    public partial class TheImpostorAnswer : TheAnswer
    {
        public override void Initialize()
        {
            PostConstructorCallCount += 1;
        }
    }
}
