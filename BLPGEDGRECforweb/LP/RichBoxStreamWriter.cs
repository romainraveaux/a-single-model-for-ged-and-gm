using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;

namespace LP
{
    class RichBoxStreamWriter : TextWriter
    {
        RichTextBox _output = null;
 
        public RichBoxStreamWriter(RichTextBox output)
        {
            _output = output;
        }
 
        public override void Write(char value)
        {
            base.Write(value);
            _output.AppendText(value.ToString());
        }
 
        public override Encoding Encoding
        {
            get { return System.Text.Encoding.UTF8; }
            
        }

    }
}
