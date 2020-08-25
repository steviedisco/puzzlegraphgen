using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Xml;

namespace PuzzleGraphGenerator.Helpers
{
    public class NoTypeXmlWriter : XmlTextWriter
    {
        public NoTypeXmlWriter(TextWriter w)
               : base(w) { }
        public NoTypeXmlWriter(Stream w, Encoding encoding)
                   : base(w, encoding) { }
        public NoTypeXmlWriter(string filename, Encoding encoding)
                   : base(filename, encoding) { }

        bool _skip;

        public override void WriteStartAttribute(string prefix,
                                                 string localName,
                                                 string ns)
        {
            if (prefix?.StartsWith("xmlns") ?? false || !string.IsNullOrEmpty(ns))
            {
                _skip = true;
            }
            else
            {
                base.WriteStartAttribute(prefix, localName, "");
            }
        }

        public override void WriteStartElement(string prefix,
                                               string localName,
                                               string ns)
        {
            Debug.WriteLine($"p:{prefix} l:{localName} n:{ns}");
            base.WriteStartElement(prefix, localName, "");
        }

        public override void WriteString(string text)
        {
            if (!_skip) base.WriteString(text);
        }

        public override void WriteEndAttribute()
        {
            if (!_skip) base.WriteEndAttribute();
            _skip = false;
        }

        public override void WriteEndElement()
        {
            base.WriteEndElement();
        }
    }
}