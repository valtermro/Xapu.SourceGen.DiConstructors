namespace Xapu.SourceGen.DiConstructors.TextBuffers
{
    internal interface ISourceTextBuffer
    {
        void Indent();
        void Dedent();
        void Write(string str);
        void WriteLine();
        void WriteLine(string line);
        void BeginBlock();
        void EndBlock();
        string ToString();
    }
}
