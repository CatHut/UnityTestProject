using System;
using System.IO;
using System.Xml.Serialization;
using System.Diagnostics;
using System.Text;
using System.IO.Compression;

namespace CatHut
{
    /// <summary>
    /// T型のオブジェクトをXML形式で保存および読み込むためのクラス。
    /// オプションでデータの圧縮が可能。
    /// </summary>
    /// <typeparam name="T">シリアライズ可能な型。</typeparam>
    public class AppSetting<T> where T : new()
    {
        private string fileName;

        /// <summary>
        /// 圧縮の有無を制御するフラグ。
        /// trueの場合、データは圧縮して保存される。
        /// </summary>
        public bool EnableCompression { get; set; }

        /// <summary>
        /// 任意の保存データクラス
        /// 要件：シリアライズ可能であること
        /// 要件：引数なしコンストラクタを実装すること
        /// 注意：Dicitonaryで使用する場合、SerializableDicitonaryを使用すること。
        ///       また、デフォルトコンストラクタ後にシリアライズ処理が実行されるため、
        ///       ディクショナリのキー登録はコンストラクタ内では行わないこと。
        /// </summary>
        public T Data { get; set; }

        /// <summary>
        /// 指定されたファイル名でAppSettingの新しいインスタンスを初期化する。
        /// </summary>
        /// <param name="file">ファイル名。</param>
        /// <param name="enableCompression">圧縮を有効にするかどうか。</param>
        public AppSetting(string file, bool enableCompression = false)
        {
            fileName = file;
            EnableCompression = enableCompression;
            Initialize();
        }

        /// <summary>
        /// デフォルトのファイル名でAppSettingの新しいインスタンスを初期化する。
        /// </summary>
        public AppSetting(bool enableCompression)
        {
            fileName = "AppSetting.data";
            EnableCompression = enableCompression;
            Initialize();
        }

        /// <summary>
        /// デフォルトのファイル名でAppSettingの新しいインスタンスを初期化する。
        /// </summary>
        public AppSetting()
        {
            fileName = "AppSetting.data";
            Initialize();
        }

        /// <summary>
        /// アプリケーションの終了時に呼び出され、データを保存する。
        /// </summary>
        public void Exit()
        {
            Save();
        }

        /// <summary>
        /// アプリケーションの初期化時に呼び出され、データを読み込む。
        /// </summary>
        public void Initialize()
        {
            Load();
        }

        /// <summary>
        /// データをXMLファイルに保存する。圧縮が有効な場合はデータを圧縮する。
        /// </summary>
        public void Save()
        {
            var serializer = CreateXmlSerializer();

            try
            {
                using (var fileStream = new FileStream(fileName, FileMode.Create))
                {
                    Stream stream = fileStream;
                    if (EnableCompression)
                    {
                        stream = new GZipStream(fileStream, CompressionMode.Compress);
                    }

                    using (var sw = new StreamWriter(stream, new UTF8Encoding(false)))
                    {
                        serializer.Serialize(sw, Data);
                    }

                    if (EnableCompression)
                    {
                        stream.Dispose();  // GZipStreamを閉じる
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("書き込み失敗: " + ex.Message);
            }
        }

        /// <summary>
        /// XMLファイルからデータを読み込む。圧縮が有効な場合はデータを解凍する。
        /// </summary>
        public void Load()
        {
            var serializer = CreateXmlSerializer();

            if (File.Exists(fileName))
            {
                try
                {
                    using (var fileStream = new FileStream(fileName, FileMode.Open))
                    {
                        Stream stream = fileStream;
                        if (EnableCompression)
                        {
                            stream = new GZipStream(fileStream, CompressionMode.Decompress);
                        }

                        using (var sr = new StreamReader(stream, new UTF8Encoding(false)))
                        {
                            Data = (T)serializer.Deserialize(sr);
                        }

                        if (EnableCompression)
                        {
                            stream.Dispose();  // GZipStreamを閉じる
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("読み込み失敗: " + ex.Message);
                    Data = new T();
                }
            }
            else
            {
                Data = new T();
            }
        }

        /// <summary>
        /// T型のデータをシリアライズするためのXmlSerializerを作成する。
        /// </summary>
        /// <returns>XmlSerializerのインスタンス。</returns>
        private XmlSerializer CreateXmlSerializer()
        {
            return new XmlSerializer(typeof(T));
        }
    }
}
