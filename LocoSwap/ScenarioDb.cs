using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace LocoSwap
{
    public static class ScenarioDb
    {
        public enum DBState
        {
            Loading,
            Loaded,
            Error,
            Init
        }
        public enum ScenarioCompletion
        {
            CompletedSuccessfully,
            CompletedFailed,
            NotCompleted,
            NotInDB,
            Unknown
        }

        private static Dictionary<string, ScenarioCompletion> scenarioDb = new Dictionary<string, ScenarioCompletion>();
        public static DBState dbState = DBState.Init;

        public static ScenarioCompletion getScenarioDbInfos(string id)
        {
            if (scenarioDb.ContainsKey(id))
            {
                return scenarioDb[id];
            } else if (dbState == DBState.Loaded)
            {
                return ScenarioCompletion.NotInDB;
            }
            return ScenarioCompletion.Unknown;
        }
        public static void ParseScenarioDb()
        {
            dbState = DBState.Loading;

            string dbPath = Path.Combine(Properties.Settings.Default.TsPath, "Content", "SDBCache.bin");
            if (File.Exists(dbPath))
            {
                try
                {
                    string xmlScenarioDbPath = TsSerializer.BinToXml(dbPath);
                    FileStream origStream = File.OpenRead(xmlScenarioDbPath);
                    CleanTextReader streamReader = new CleanTextReader(origStream);

                    XmlReader XReaderSDB = XmlReader.Create(streamReader);

                    // Browse scenarios
                    while (XReaderSDB.ReadToFollowing("ScenarioID"))
                    {
                        // Read Id
                        XReaderSDB.ReadToFollowing("DevString");
                        XReaderSDB.Read();
                        string id = XReaderSDB.Value;

                        // Read completion status
                        XReaderSDB.ReadToFollowing("Completion");
                        XReaderSDB.Read();
                        ScenarioCompletion completion = ScenarioCompletion.Unknown;
                        switch (XReaderSDB.Value)
                        {
                            case "NotCompleted":
                                completion = ScenarioCompletion.NotCompleted;
                                break;
                            case "CompletedSuccessfully":
                                completion = ScenarioCompletion.CompletedSuccessfully;
                                break;
                            case "CompletedFailed":
                                completion = ScenarioCompletion.CompletedFailed;
                                break;
                        }

                        // Add completion status to our internal DB
                        scenarioDb[id] = completion;
                    }
                    dbState = DBState.Loaded;

                    // Uncompressed DB can be quite large, we delete it now instead of waiting for the next LocoSwap launch
                    origStream.Close();
                    File.Delete(xmlScenarioDbPath);
                }
                catch
                {
                    dbState = DBState.Error;
                }

            } else
            {
                dbState = DBState.Error;
            }
        }
    }

    /**
     * Thanks to https://stackoverflow.com/a/38155562, we have a nice XML cleaner
     */
    internal class CleanTextReader : StreamReader
    {
        public CleanTextReader(Stream stream) : base(stream)
        {
        }

        public override int Read()
        {
            var val = base.Read();
            return XmlConvert.IsXmlChar((char)val) ? val : (char)' ';
        }

        public override int Read(char[] buffer, int index, int count)
        {
            var ret = base.Read(buffer, index, count);

            for (int i = 0; i < ret; i++)
            {
                int idx = index + i;
                if (!XmlConvert.IsXmlChar(buffer[idx]))
                    buffer[idx] = ' ';
            }

            return ret;
        }

        public override int ReadBlock(char[] buffer, int index, int count)
        {
            var ret = base.ReadBlock(buffer, index, count);

            for (int i = 0; i < ret; i++)
            {
                int idx = index + i;
                if (!XmlConvert.IsXmlChar(buffer[idx]))
                    buffer[idx] = ' ';
            }

            return ret;
        }
    }
}
