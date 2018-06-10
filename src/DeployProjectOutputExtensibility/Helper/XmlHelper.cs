using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace TP.AutoDeploy.Helper
{
    public static class XmlHelper
    {
        /// <summary>
        /// Loads from file.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="xmlFilePath">The XML file path.</param>
        /// <returns></returns>
        public static T LoadFromFile<T>(string xmlFilePath)
        {
            try
            {
                if (!File.Exists(xmlFilePath))
                {
                    return default(T);
                }

                var s = new XmlSerializer(typeof(T));
                using (var fs = new FileStream(xmlFilePath, FileMode.Open, FileAccess.Read))
                {
                    using (var r = new StreamReader(fs))
                    {
                        var data = (T)s.Deserialize(r);
                        r.Close();
                        fs.Close();
                        return data;
                    }
                }
            }
            catch (Exception ex)
            {
                VSUIHelper.ShowMessageBox($"Could not load {Path.GetFileName(xmlFilePath)}. {ex.Message}",
                    Microsoft.VisualStudio.Shell.Interop.OLEMSGICON.OLEMSGICON_WARNING);
            }
            return default(T);
        }

        /// <summary>
        /// Saves to file.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj">The object.</param>
        /// <param name="xmlFilePath">The XML file path.</param>
        public static void SaveToFile<T>(T obj, string xmlFilePath) where T : class
        {
            try
            {

                var folder = Path.GetDirectoryName(xmlFilePath);
                if (!Directory.Exists(folder))
                {
                    Directory.CreateDirectory(folder);
                }

                var xmlString = ToString(obj);

                // Create an XmlDocument to read the data
                var xmlDocument = new XmlDocument();
                xmlDocument.LoadXml(xmlString);
                xmlDocument.Save(xmlFilePath);
            }
            catch (Exception ex)
            {
                VSUIHelper.ShowMessageBox($"Could not save {Path.GetFileName(xmlFilePath)}. {ex.Message}", 
                    Microsoft.VisualStudio.Shell.Interop.OLEMSGICON.OLEMSGICON_WARNING);
            }
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj">The object.</param>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public static string ToString<T>(T obj)
        {
            var xmlSrlz = new XmlSerializer(typeof(T));
            using (var memStream = new MemoryStream())
            {
                var xmlWriter = new XmlTextWriter(memStream, Encoding.UTF8)
                {
                    Formatting = Formatting.Indented,
                    Indentation = 2
                };

                xmlSrlz.Serialize(xmlWriter, obj);
                memStream.Position = 0;
                var text = string.Empty;

                using (var streamReader = new StreamReader(memStream))
                {
                    text = streamReader.ReadToEnd();
                }

                return text;
            }
        }
    }
}
