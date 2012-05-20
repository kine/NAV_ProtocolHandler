using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Xml.Serialization;

namespace NVR_DynamicsNAVProtocolHandler
{
    public class Mapping
    {
        private String dbServer;

        public String DbServer
        {
            get { return dbServer; }
            set { dbServer = value; }
        }
        private String db;

        public String Db
        {
            get { return db; }
            set { db = value; }
        }
        private String company;

        public String Company
        {
            get { return company; }
            set { company = value; }
        }
        private String navServer;

        public String NavServer
        {
            get { return navServer; }
            set { navServer = value; }
        }
        private String instance;

        public String Instance
        {
            get { return instance; }
            set { instance = value; }
        }
    }

    class NAVClient2URI
    {
        static public ObservableCollection<Mapping> mappings;
        

        static public Mapping GetByServerDB(string DBServer, string DB, string company)
        {
            if (mappings == null)
                LoadMapping();
            var resultServer = from r in mappings where r.DbServer == DBServer select r;
            var resultDB = from r in resultServer where r.Db == DB select r;
            if ((resultDB.Count() == 0) || (resultDB==null))
            {
                var mapping = new Mapping();
                mapping.Db = DB;
                mapping.DbServer = DBServer;
                mapping.Company = company;
                var editor = new MappingEditor(mapping);
                if (editor.ShowDialog()==true)
                {
                }
                var resultServerRetry = from r in mappings where r.DbServer == DBServer select r;
                var resultDBRetry = from r in resultServerRetry where r.Db == DB select r;
                if ((resultDBRetry.Count() == 0) || (resultDBRetry == null))
                {
                    return null;
                }
                return resultDBRetry.First();
            }
            return resultDB.First();
        }

        static public void LoadMapping()
        {
            string _path = NVR_DynamicsNAVProtocolHandler.Properties.Settings.Default.MappingFile;
            if (File.Exists(_path))
            {
                mappings = new ObservableCollection<Mapping>();
                XmlSerializer serializer = new XmlSerializer(typeof(ObservableCollection<Mapping>));
                TextReader reader = new StreamReader(_path);
                mappings = (ObservableCollection<Mapping>)serializer.Deserialize(reader);
                reader.Close();
            }
            else
            {
                mappings = new ObservableCollection<Mapping>();
                var mapping=new Mapping();
                mapping.Db="DBName";
                mapping.DbServer="SQLServername";
                mapping.Instance="DynamicsNAV";
                mapping.NavServer="NAVServer:7046";
                mappings.Add(mapping);
                SaveMapping();
            }
        }

        static public void SaveMapping()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(ObservableCollection<Mapping>));
            TextWriter writer = new StreamWriter(NVR_DynamicsNAVProtocolHandler.Properties.Settings.Default.MappingFile);
            serializer.Serialize(writer, mappings);
            writer.Close();
         }
    }
}
