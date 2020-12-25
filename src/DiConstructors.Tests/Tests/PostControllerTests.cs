using Xapu.SourceGen.DiConstructors.Tests.FixtureTypes;
using Xunit;

namespace Xapu.SourceGen.DiConstructors.Tests
{
    public class PostControllerTests
    {
        [Fact]
        public void AddPostControllerMethodCallIfDefined()
        {
            var theAswer = new TheAnswer(new Sage());

            Assert.Equal(42, theAswer.ToEverything);
        }

        [Fact]
        public void AddChildPostControllerMethodCallIfIsNew()
        {
            var theChildAnswer = new TheNewAnswer(new Sage());

            Assert.Equal(2, theChildAnswer.PostConstructorCallCount);
        }
    }
}
