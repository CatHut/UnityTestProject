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
using UnityEngine;

namespace CatHut
{
    public static class CatHutCommon
    {

        /// <summary>
        /// 指定されたクラスのインスタンスをディープコピー（インスタンス自体を複製する）する
        /// </summary>
        /// <typeparam name="T">型</typeparam>
        /// <param name="obj">コピーするインスタンス</param>
        /// <returns>コピーされたインスタンス</returns>
        public static T DeepClone<T>(T obj)
        {
            using (var ms = new MemoryStream())
            {
                XmlSerializer xs = new XmlSerializer(typeof(T));
                xs.Serialize(XmlWriter.Create(ms), obj);

                ms.Flush();
                ms.Seek(0, SeekOrigin.Begin);
                return (T)xs.Deserialize(XmlReader.Create(ms));

            }
        }

        /// <summary>
        /// 指定されたクラスのインスタンスをディープコピーする。
        /// JsonUtilityを使用してJSON形式でシリアライズおよびデシリアライズを行う。
        /// </summary>
        /// <typeparam name="T">型</typeparam>
        /// <param name="obj">コピーするインスタンス</param>
        /// <returns>コピーされたインスタンス</returns>
        public static T DeepCloneJson<T>(T obj)
        {
            // オブジェクトをJSON文字列にシリアライズ
            string json = JsonUtility.ToJson(obj);

            // JSON文字列から新しいインスタンスにデシリアライズ
            return JsonUtility.FromJson<T>(json);
        }
    }
}
