using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace PVDataSampler
{
    internal class AppSettings
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        public static AppSettings Instance = new AppSettings();

        private AppSettings()
        {
            Read();
        }


        public string GridMeter
        {
            get;
            set;
        }

        public void Save()
        {
            Write();
        }

        private string GetPath()
        {
            var appConfig = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData, Environment.SpecialFolderOption.Create);
            appConfig = Path.Combine(appConfig, "Andreas Rossi");
            appConfig = Path.Combine(appConfig, "PVDataSampler");
            appConfig = Path.Combine(appConfig, "settings.config");
            return appConfig;
        }

        private void Read()
        {
            if (!File.Exists(GetPath()))
                return;

            XmlReader reader = XmlReader.Create(GetPath(),new XmlReaderSettings() { CloseInput = true, IgnoreWhitespace = true });
            try
            {
                var doc = new XmlDocument();
                doc.Load(reader);

                var gridcounterportAttribute = doc.SelectSingleNode(@"settings/GridMeter/@port");
                GridMeter = gridcounterportAttribute != null ? gridcounterportAttribute.Value : "";
            }
            catch(Exception ex)
            {
                logger.Error(ex);
            }
            reader?.Dispose();
        }

        private void Write()
        {
            XmlWriter writer = null;
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(GetPath()));

                writer = XmlWriter.Create(GetPath(), new XmlWriterSettings() { CloseOutput = true, Indent = true, NewLineChars = Environment.NewLine });
                var doc = new XmlDocument();
                var settingsnode = doc.CreateElement("settings");
                doc.AppendChild(settingsnode);

                var gridcounternode = doc.CreateElement("GridMeter");
                var gridcounterportAttribute = doc.CreateAttribute("port");
                gridcounterportAttribute.Value = GridMeter;
                gridcounternode.Attributes.Append(gridcounterportAttribute);
                settingsnode.AppendChild(gridcounternode);

                doc.WriteTo(writer);
            }
            catch(Exception ex)
            {
                logger.Error(ex);
            }
            writer?.Dispose();
        }

    }
}
