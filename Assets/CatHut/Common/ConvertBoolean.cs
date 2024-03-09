using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using UnityEditor.Build.Pipeline;

namespace CatHut
{
    public static class ConvertBoolean
    {

        public static string ToBoolString(string value)
        {
            var temp = value.ToLower();
            switch (temp)
            {
                case "true":
                case "1":
                    {
                        return "true";
                    }
                case "false":
                case "0":
                default:
                    {
                        return "false";
                    }
            }
        }
        public static string ToBoolString(bool value)
        {
            return value ? "true" : "false";
        }

        public static bool ToBool(string value)
        {
            var temp = value.ToLower();
            switch (temp)
            {
                case "true":
                case "1":
                    {
                        return true;
                    }
                case "false":
                case "0":
                default:
                    {
                        return false;
                    }
            }
        }
        public static bool ToBool(bool value)
        {
            return value ? true : false;
        }

    }
}
