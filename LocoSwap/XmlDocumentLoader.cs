using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace LocoSwap
{
    // A XDocument.Load wrapper that tolerates invalid XML characters
    class XmlDocumentLoader
    {
        public static XDocument Load(string path)
        {
            try
            {
                XDocument document = XDocument.Load(path);
                return document;
            }
            catch (XmlException)
            {
                Log.Debug("Malformed XML document detected at {0}", path);
            }

            string xmlContent = File.ReadAllText(path, Encoding.UTF8);
            xmlContent = Regex.Replace(
                xmlContent,
                @"[^\u0009\u000A\u000D\u0020-\uD7FF\uE000-\uFFFD\u10000-\u10FFFF]",
                string.Empty);

            return XDocument.Parse(xmlContent);
        }
    }
}
