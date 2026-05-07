using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.Xml;

namespace Sistem_za_upravljanje_sadrzajima.Helper
{
    public class DataIO_User
    {
        public void SerializeObject<T>(T serializableObject,string filename)
        {
            if (serializableObject == null) { return; }
            try
            {
               XmlDocument xmlDocument = new XmlDocument();
                XmlSerializer xmlSerializer = new XmlSerializer(serializableObject.GetType());
                using (MemoryStream stream = new MemoryStream())
                {
                    xmlSerializer.Serialize(stream, serializableObject);
                    stream.Position = 0;
                    xmlDocument.Load(stream);
                    xmlDocument.Save(filename);
                    stream.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error serializing object: " + ex.Message);
            }
        }

        public T DeSerializeObject<T>(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                return default(T);
            }

            T objectOut = default(T);

            try
            {
                string attributeXml = string.Empty;
                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.Load(fileName);
                string xmlString = xmlDocument.OuterXml;

                using (StringReader read = new StringReader(xmlString))
                {
                    Type outType = typeof(T);

                    XmlSerializer serializer = new XmlSerializer(outType);
                    using (XmlReader reader = new XmlTextReader(read))
                    {
                        objectOut = (T)serializer.Deserialize(reader);
                        reader.Close();
                    }
                    read.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error deserializing object: " + ex.Message);
            }

            return objectOut;
        }
    }
}
