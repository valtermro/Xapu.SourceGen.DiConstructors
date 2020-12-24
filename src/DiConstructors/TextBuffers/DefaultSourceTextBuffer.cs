using System.CodeDom.Compiler;
using System.IO;

namespace Xapu.SourceGen.DiConstructors.TextBuffers
{
    internal class DefaultSourceTextBuffer : ISourceTextBuffer
    {
        private readonly StringWriter _stringWriter;
        private readonly IndentedTextWriter _indentedTextWriter;

        public DefaultSourceTextBuffer()
        {
            _stringWriter = new StringWriter();
            _indentedTextWriter = new IndentedTextWriter(_stringWriter);
        }

        public void Indent()
        {
            _indentedTextWriter.Indent += 1;
        }

        public void Dedent()
        {
            _indentedTextWriter.Indent -= 1;
        }

        public void Write(string str)
        {
            _indentedTextWriter.Write(str);
        }

        public void WriteLine()
        {
            _indentedTextWriter.WriteLine();
        }

        public void WriteLine(string line)
        {
            _indentedTextWriter.WriteLine(line);
        }

        public void BeginBlock()
        {
            WriteLine("{");
            Indent();
        }

        public void EndBlock()
        {
            Dedent();
            WriteLine("}");
        }

        public override string ToString()
        {
            return _stringWriter.ToString();
        }
    }
}
