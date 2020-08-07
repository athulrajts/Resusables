using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using KEI.Infrastructure.Logging;

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
        public static bool Serialize(object data, string filePath)
        {
            try
            {
                var dir = Path.GetDirectoryName(filePath);
                if (string.IsNullOrEmpty(dir) == false && Directory.Exists(dir) == false)
                {
                    Directory.CreateDirectory(dir);
                }

                using FileStream fileStream = new FileStream(filePath, FileMode.Create);
                XmlSerializer serializer = new XmlSerializer(data.GetType());
                serializer.Serialize(fileStream, data);
                fileStream.Close();
                return true;
            }
            catch (Exception ex)
            {
                Logger.Error($"Serialization of {filePath} failed", ex);
                return false;
            }
        }

        /// <summary>
        /// Serialize object to string
        /// </summary>
        /// <typeparam name="T">Type of of object</typeparam>
        /// <param name="data">Data to serialize</param>
        /// <returns>XML string</returns>
        public static string Serialize<T>(T data)
        {
            try
            {
                XmlSerializer serializer = new XmlSerializer(data.GetType());
                
                var settings = new XmlWriterSettings
                {
                    Indent = false,
                    OmitXmlDeclaration = true,
                };

                using var stream = new StringWriter();
                using var writer = XmlWriter.Create(stream, settings);
                
                serializer.Serialize(writer, data, new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty}));

                return stream.ToString();
            }
            catch (Exception ex)
            {
                Logger.Error($"Serialization of {typeof(T).FullName} failed", ex);
                return string.Empty;
            }
        }

        /// <summary>
        /// Deserialize object from file
        /// </summary>
        /// <typeparam name="T">Type of Object</typeparam>
        /// <param name="filePath">file to deserialize from</param>
        /// <returns>.NET Object</returns>
        public static T Deserialize<T>(string filePath)
        {
            if (File.Exists(filePath))
            {

                try
                {
                    using var fileStream = new FileStream(filePath, FileMode.Open);
                    XmlSerializer serializer = new XmlSerializer(typeof(T));
                    object data = serializer.Deserialize(fileStream);
                    fileStream.Close();
                    if (data != null && data is T t)
                    {
                        return t;
                    }
                    return default;
                }
                catch (Exception ex)
                {
                    Logger.Error($"Unable to deserialize \"{filePath}\" to {typeof(T).FullName}", ex);
                    return default;
                }
            }
            return default;
        }

        public static object Deserialize(string filePath, Type type)
        {
            if (File.Exists(filePath))
            {

                try
                {
                    using var fileStream = new FileStream(filePath, FileMode.Open);
                    XmlSerializer serializer = new XmlSerializer(type);
                    object data = serializer.Deserialize(fileStream);
                    fileStream.Close();
                    if (data != null && data is { } o)
                    {
                        return o;
                    }
                    return default;
                }
                catch (Exception ex)
                {
                    Logger.Error($"Unable to deserialize \"{filePath}\" to {type.FullName}", ex);
                    return default;
                }
            }
            return default;
        }


        public static T DeserializeFromString<T>(string xml)
        {
            var serializer = new XmlSerializer(typeof(T));
            using var reader = new StringReader(xml);
            return (T)serializer.Deserialize(reader);
        }

        public static object DeserializeFromString(Type type, string xml)
        {
            var serializer = new XmlSerializer(type);
            using var reader = new StringReader(xml);
            return serializer.Deserialize(reader);
        }


        public static T ReadObjectXML<T>(this XmlReader reader)
        {
            var s = new XmlSerializer(typeof(T));
            return (T)s.Deserialize(reader.ReadSubtree());
        }

        public static object ReadObjectXML(this XmlReader reader, Type type)
        {
            var s = new XmlSerializer(type);
            return s.Deserialize(reader.ReadSubtree());
        }

        public static void WriteObjectXML<T>(this XmlWriter writer, T obj)
        {
            var s = new XmlSerializer(obj.GetType());
            s.Serialize(writer, obj, new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty }));
        }
    }
}
