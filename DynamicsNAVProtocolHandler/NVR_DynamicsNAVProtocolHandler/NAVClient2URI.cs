using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Xml.Serialization;

namespace NVR_DynamicsNAVProtocolHandler
{
    public class Mapping
    {
        public String DBServer;
        public String DB;
        public String Company;
        public String NAVServer;
        public String Instance;
    }

    class NAVClient2URI
    {
        static List<Mapping> mappings;

        static public Mapping GetByServerDB(string DBServer, string DB, string company)
        {
            if (mappings == null)
                LoadMapping();
            var resultServer = from r in mappings where r.DBServer == DBServer select r;
            var resultDB = from r in resultServer where r.DB == DB select r;
            if (resultDB.Count() == 0)
            {
            }
            return resultDB.First();
        }

        static public void LoadMapping()
        {
            string _path = NVR_DynamicsNAVProtocolHandler.Properties.Settings.Default.MappingFile;
            if (File.Exists(_path))
            {
                mappings = new List<Mapping>();
                XmlSerializer serializer = new XmlSerializer(typeof(List<Mapping>));
                TextReader reader = new StreamReader(_path);
                mappings = (List<Mapping>)serializer.Deserialize(reader);
                reader.Close();
            }
            else
            {
                mappings = new List<Mapping>();
                var mapping=new Mapping();
                mapping.DB="DBName";
                mapping.DBServer="SQLServername";
                mapping.Instance="DynamicsNAV";
                mapping.NAVServer="NAVServer:7046";
                mappings.Add(mapping);
                SaveMapping();
            }
        }

        static public void SaveMapping()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(List<Mapping>));
            TextWriter writer = new StreamWriter(NVR_DynamicsNAVProtocolHandler.Properties.Settings.Default.MappingFile);
            serializer.Serialize(writer, mappings);
            writer.Close();
         }
    }
}
