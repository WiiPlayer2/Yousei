using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yousei.Core
{
    public class IndentedTextWriter : TextWriter
    {
        private readonly TextWriter baseWriter;

        private readonly int indentLength = 4;

        private int indentLevel = 0;

        private bool indentNextTime = false;

        public IndentedTextWriter(TextWriter baseWriter, int indentLength = 4)
        {
            this.baseWriter = baseWriter;
            this.indentLength = indentLength;
        }

        public override Encoding Encoding => baseWriter.Encoding;

        public IDisposable Indent()
        {
            indentLevel++;
            return new ActionDisposable(() => indentLevel--);
        }

        public override void Write(char value)
        {
            if (value == '\n')
            {
                indentNextTime = true;
            }
            else if (indentNextTime)
            {
                WriteIndent();
                indentNextTime = false;
            }

            baseWriter.Write(value);
        }

        private void WriteIndent()
        {
            baseWriter.Write(string.Concat(Enumerable.Repeat(' ', indentLevel * indentLength)));
        }

        private class ActionDisposable : IDisposable
        {
            private readonly Action action;

            public ActionDisposable(Action action)
            {
                this.action = action;
            }

            public void Dispose() => action();
        }
    }
}