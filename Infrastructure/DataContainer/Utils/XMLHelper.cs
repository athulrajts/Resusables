using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace KEI.Infrastructure
{
    public static class XmlHelper
    {
        /// <summary>
        /// Serializes Object to file
        /// </summary>
        /// <typeparam name="T">Type of object</typeparam>
        /// <param name="data">object to serialize</param>
        /// <param name="filePath">file path to serialize to</param>
        /// <returns>Is success</returns>
        public static bool SerializeToFile(object data, string filePath)
        {
            try
            {
                if(string.IsNullOrEmpty(filePath))
                {
                    throw new ArgumentException("Invalid path");
                }

                var dir = Path.GetDirectoryName(filePath);
                if (Directory.Exists(dir) == false)
                {
                    Directory.CreateDirectory(dir);
                }

                using FileStream fileStream = new FileStream(filePath, FileMode.Create);
                XmlSerializer serializer = new XmlSerializer(data.GetType());
                serializer.Serialize(fileStream, data);
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.TraceError(ex.ToString());
                return false;
            }
        }

        /// <summary>
        /// Serialize object to string
        /// </summary>
        /// <typeparam name="T">Type of of object</typeparam>
        /// <param name="data">Data to serialize</param>
        /// <returns>XML string</returns>
        public static string SerializeToString<T>(T data, bool indent = false)
        {
            try
            {
                XmlSerializer serializer = new XmlSerializer(data.GetType());
                
                var settings = new XmlWriterSettings
                {
                    Indent = indent,
                    OmitXmlDeclaration = true,
                };

                using var stream = new StringWriter();
                using var writer = XmlWriter.Create(stream, settings);
                
                serializer.Serialize(writer, data, new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty}));

                return stream.ToString();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.TraceError(ex.ToString());
                return string.Empty;
            }
        }

        /// <summary>
        /// Deserialize object from file
        /// </summary>
        /// <typeparam name="T">Type of Object</typeparam>
        /// <param name="filePath">file to deserialize from</param>
        /// <returns>.NET Object</returns>
        public static T DeserializeFromFile<T>(string filePath)
        {
            return (T)DeserializeFromFile(filePath, typeof(T));
        }

        public static object DeserializeFromFile(string filePath, Type type)
        {
            if(File.Exists(filePath) == false)
            {
                return default;
            }
            try
            {
                using var fileStream = new FileStream(filePath, FileMode.Open);
                XmlSerializer serializer = new XmlSerializer(type);
                object data = serializer.Deserialize(fileStream);
                return data;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.TraceError(ex.ToString());
                return default;
            }
        }


        public static T DeserializeFromString<T>(string xml)
        {
            return (T)DeserializeFromString(typeof(T), xml);
        }

        public static object DeserializeFromString(Type type, string xml)
        {
            var serializer = new XmlSerializer(type);
            using var reader = new StringReader(xml);
            return serializer.Deserialize(reader);
        }


        public static T ReadObjectXml<T>(this XmlReader reader, bool inplace = true)
        {
            return (T)ReadObjectXml(reader, typeof(T), inplace);
        }

        public static object ReadObjectXml(this XmlReader reader, Type type, bool inplace = true)
        {
            var s = new XmlSerializer(type);
            return s.Deserialize(inplace ? reader : reader.ReadSubtree());
        }

        public static void WriteObjectXml<T>(this XmlWriter writer, T obj)
        {
            var s = new XmlSerializer(obj.GetType());
            s.Serialize(writer, obj, new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty }));
        }
    }
}
