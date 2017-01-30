using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System.Xml.Serialization;
using System.IO;

namespace Front_End
{
    static class Utility
    {
        // This class is used as a place for utility methods. Things that can be useful anywhere.

        public static string SerializeToString(object obj)
        {
            // Create an instance of the XML Serializer.
            XmlSerializer xmlSerializer = new XmlSerializer(obj.GetType());

            // Create a StringWriter, this will store the output of the XML.
            StringWriter sw = new StringWriter();

            // Serialize the object.
            xmlSerializer.Serialize(sw, obj);

            return sw.ToString();
        }

        public static T DeserializeFromString<T>(string xml)
        {
            // Code based on code from StackOverflow:
            // http://stackoverflow.com/questions/2347642/deserialize-from-string-instead-textreader
            return (T)DeserializeFromString(xml, typeof(T));
        }

        public static object DeserializeFromString(string xml, Type type)
        {
            // Code based on code from StackOverflow:
            // http://stackoverflow.com/questions/2347642/deserialize-from-string-instead-textreader
            // Create an instance of the XML Serializer.
            XmlSerializer xmlSerializer = new XmlSerializer(type);
            object deserialized;

            using(TextReader reader = new StringReader(xml))
            {
                deserialized = xmlSerializer.Deserialize(reader);
            }

            return deserialized;
        }

        public static void WriteToFile(string data, string filename)
        {
            // This method is used to store data on the disk.
            // This will mostly be used to serialize objects to be retrieved between activities.
            // Code based on code from: https://forums.xamarin.com/discussion/79/saving-objects-to-file
            string directory = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
            string path = Path.Combine(directory, filename);

            using(var stream = new StreamWriter(path, false))
            {
                stream.Write(data);
            }
        }

        public static string ReadFromFile(string filename)
        {
            // This method is used to store data on the disk.
            // This will mostly be used to serialize objects to be retrieved between activities.
            // Code based on code from: https://forums.xamarin.com/discussion/79/saving-objects-to-file
            string directory = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
            string path = Path.Combine(directory, filename);
            string data = "";

            using (var stream = new StreamReader(path))
            {
                data = stream.ReadToEnd();
            }

            return data;
        }
    }
}