using Serilog;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace LocoSwap
{
    static class TsSerializer
    {
        private static Process InvokeSerz(string path)
        {
            var startInfo = new ProcessStartInfo();
            startInfo.FileName = Utilities.GetSerzPath();
            startInfo.Arguments = "\"" + path + "\"";
            startInfo.RedirectStandardOutput = true;
            startInfo.RedirectStandardError = true;
            startInfo.UseShellExecute = false;
            startInfo.CreateNoWindow = true;

            Process process = new Process();
            process.StartInfo = startInfo;
            process.EnableRaisingEvents = true;

            process.Start();
            return process;
        }
        public static XDocument Load(string binPath)
        {
            return XmlDocumentLoader.Load(BinToXml(binPath));
        }

        public static string BinToXml(string binPath)
        {
            FileInfo binInfo = new FileInfo(binPath);
            string baseName = Path.GetFileNameWithoutExtension(binInfo.Name);
            string tempName = string.Format("{0}-{1}.bin", baseName, Utilities.StaticRandom.Instance.Next(10000, 99999));

            string tempBinPath;
            string tempXmlPath;

            if (!binPath.StartsWith(Utilities.GetTempDir()))
            {
                tempBinPath = Path.Combine(Utilities.GetTempDir(), tempName);
                tempXmlPath = Path.ChangeExtension(tempBinPath, "xml");
                Utilities.RemoveFile(tempBinPath);
                Utilities.RemoveFile(tempXmlPath);

                File.Copy(binPath, tempBinPath);
            }
            else
            {
                tempBinPath = binPath;
                tempXmlPath = Path.ChangeExtension(tempBinPath, "xml");
            }

            Process serz = InvokeSerz(tempBinPath);
            serz.WaitForExit();

            return tempXmlPath;
        }

        public static void Save(XDocument document, string path)
        {
            string xmlPath = Path.ChangeExtension(path, "xml");
            Utilities.RemoveFile(xmlPath);
            Utilities.RemoveFile(path);

            XmlWriterSettings xmlWriterSettings = new XmlWriterSettings();
            xmlWriterSettings.Indent = true;
            xmlWriterSettings.IndentChars = "\t";
            xmlWriterSettings.Encoding = new UTF8Encoding(false);
            xmlWriterSettings.NewLineHandling = NewLineHandling.None;

            FileStream stream = new FileStream(xmlPath, FileMode.Create);
            using (XmlWriter writer = XmlWriter.Create(stream, xmlWriterSettings))
            {
                document.Save(writer);
            }
            stream.Flush();
            stream.Close();

            Process serz = InvokeSerz(xmlPath);
            serz.WaitForExit();

            string serzOutput = serz.StandardOutput.ReadToEnd();
            Log.Debug(string.Format("Serz output: {0}", serzOutput));

            return;
        }
    }
}
