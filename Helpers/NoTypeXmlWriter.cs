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
        public NoTypeXmlWriter(string filename, Encoding encoding)
                   : base(filename, encoding) { }

        private bool _skip;

        public override void WriteStartAttribute(string prefix,
                                                 string localName,
                                                 string ns)
        {
            if (prefix?.StartsWith("xmlns") ?? false || !string.IsNullOrEmpty(ns))
            {
                _skip = true;
                return;
            }
            
            base.WriteStartAttribute(prefix, localName, string.Empty);
        }

        public override void WriteString(string text)
        {
            if (_skip) return;
            base.WriteString(text);
        }

        public override void WriteEndAttribute()
        {
            if (_skip)
            {
                _skip = false;
                return;
            }

            base.WriteEndAttribute();
        }

        public override void WriteStartElement(string prefix,
                                               string localName,
                                               string ns)
        {
            base.WriteStartElement(prefix, localName, string.Empty);
        }
    }
}