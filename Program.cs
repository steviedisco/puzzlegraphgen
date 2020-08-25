using PuzzleGraphGenerator.Models;
using System;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using Attribute = PuzzleGraphGenerator.Models.Attribute;

namespace PuzzleGraphGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            var node = new Graph(); // CreateSingleNode();
            var serializer = new XmlSerializer(node.GetType());

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            var settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.IndentChars = "    ";
            settings.Encoding = CodePagesEncodingProvider.Instance.GetEncoding(1252);

            var writer = XmlWriter.Create("./output.xgml", settings);
            
            var ns = new XmlSerializerNamespaces();
            ns.Add("", "");

            serializer.Serialize(writer, node, ns);
            writer.Close();

            Console.WriteLine("Generated Successfully");
        }

        private static Section CreateSingleNode()
        {
            var root = Section.CreateSection("xgml");
            root.AddAttribute(Attribute.CreateAttribute("Creator", "String", "yFiles"));
            root.AddAttribute(Attribute.CreateAttribute("Version", "String", 2.17));

            var section = root.AddSection(Section.CreateSection("graph"));
            section.AddAttribute(Attribute.CreateAttribute("hierarchic", "int", 1));
            section.AddAttribute(Attribute.CreateAttribute("label", "String", ""));
            section.AddAttribute(Attribute.CreateAttribute("directed", "int", 1));

            var node = section.AddSection(Section.CreateSection("node"));
            node.AddAttribute(Attribute.CreateAttribute("id", "int", 0));
            node.AddAttribute(Attribute.CreateAttribute("label", "String", "Finish"));

            section = node.AddSection(Section.CreateSection("graphics"));
            //section.AddAttribute(Attribute.CreateAttribute("x", "double", -28.5));
            //section.AddAttribute(Attribute.CreateAttribute("y", "double", -321.0));
            section.AddAttribute(Attribute.CreateAttribute("w", "double", 40.0));
            section.AddAttribute(Attribute.CreateAttribute("h", "double", 40.0));
            section.AddAttribute(Attribute.CreateAttribute("type", "String", "roundrectangle"));
            section.AddAttribute(Attribute.CreateAttribute("raisedBorder", "boolean", false));
            section.AddAttribute(Attribute.CreateAttribute("fill", "String", "#FFCC00"));
            section.AddAttribute(Attribute.CreateAttribute("outline", "String", "#000000"));

            section = node.AddSection(Section.CreateSection("LabelGraphics"));
            section.AddAttribute(Attribute.CreateAttribute("text", "String", "Finish"));
            section.AddAttribute(Attribute.CreateAttribute("fontSize", "int", 12));
            section.AddAttribute(Attribute.CreateAttribute("fontName", "String", "Calibri"));
            section.AddAttribute(Attribute.CreateAttribute("model"));

            return root;
        }
    }
}
