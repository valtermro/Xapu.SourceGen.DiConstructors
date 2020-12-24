namespace Xapu.SourceGen.DiConstructors.Tests.FixtureTypes
{
    public class Relayer<T>
    {
        public T Relay(T arg) => arg;
    }

    public partial class DummyRelayer<T>
    {
        [Injected] private readonly Relayer<T> _relayerT;

        public T RelayT(T arg) => _relayerT.Relay(arg);
    }

    public partial class DoubleDummyRelayer<T, U> : DummyRelayer<T>
    {
        [Injected] private readonly Relayer<U> _relayerU;

        public U RelayU(U arg) => _relayerU.Relay(arg);
    }
}
