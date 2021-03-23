using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using PuzzleGraphGenerator.Helpers;
using PuzzleGraphGenerator.Models;

namespace PuzzleGraphGenerator
{
    class Program
    {
        static void Main()
        {
            var graph = GraphContainer.CreateDigGraph(); 
            var serializer = new XmlSerializer(typeof(GraphContainer));

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            var writer = new NoTypeXmlWriter("./output.xgml", CodePagesEncodingProvider.Instance.GetEncoding(1252))
            {
                Formatting = Formatting.Indented,
                Indentation = 4,
                IndentChar = ' '
            };

            serializer.Serialize(writer, graph);
            writer.Close();

            Console.WriteLine("Generated Successfully");
        }
    }
}
