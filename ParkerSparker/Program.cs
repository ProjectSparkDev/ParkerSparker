using System;
using System.Data.SQLite;
using System.IO;

namespace ParkerSparker
{


    public class FileEntry
    {
        public string FileName { get; set; } //URL
        public string LocalPath { get; set; } //URL
        public long URID { get; set; }
        public string URL { get; set; }
        public string Hash { get; set; }
        public long Offset { get; set; }
        public long BundleSize { get; set; }
        public long GameSize { get; set; }
        public long ContentPackID { get; set; }
        public int ContentType { get; set; }
        public string BundlePath { get; set; }

        public FileEntry(string url, string hash, long urid, long offset, long packSize, long gameSize, int typeindex, string bundlePath)
        {
            URL = url;
            FileName = Path.GetFileName(URL);
            LocalPath = Path.GetDirectoryName(URL);
            URID = urid;
            Hash = hash;
            Offset = offset;
            BundleSize = packSize;
            GameSize = gameSize;
            ContentType = typeindex;
            BundlePath = bundlePath;
        }
        public bool WriteFile(string ExportPath)
        {
            try
            {
                LogConsole();

                FileStream bundle = File.OpenRead(BundlePath);
                bundle.Seek(Offset, SeekOrigin.Begin);
                Directory.CreateDirectory(ExportPath + LocalPath);
                FileStream output = File.Open(ExportPath + LocalPath + '\\' + FileName, FileMode.Create);
                byte[] buffer = new byte[BundleSize];
                bundle.ReadAsync(buffer, 0, (int)BundleSize).Wait();
                Console.WriteLine(buffer.Length);
                output.WriteAsync(buffer, 0, buffer.Length).Wait();
                output.Close();
                bundle.Close();

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Source + "\n" + ex.Message + "\n" + ex.StackTrace + "\n");
                return false;
            }
        }
        public void LogConsole()
        {
            Console.WriteLine("filename:         " + FileName);
            Console.WriteLine("Local Path:       " + LocalPath);
            Console.WriteLine("URID:             " + URID);
            Console.WriteLine("URL:              " + URL);
            Console.WriteLine("Hash:             " + Hash);
            Console.WriteLine("Offset:           " + Offset);
            Console.WriteLine("Size on bundle:   " + BundleSize);
            Console.WriteLine("Size on game:     " + GameSize);
            Console.WriteLine("Content Pack ID:  " + ContentPackID);
            Console.WriteLine("Type ID:          " + ContentType);
            Console.WriteLine("Bundle Path:      " + BundlePath);
        }
    }
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            /*
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
            */
            Console.WriteLine("started");


            string databasePath = "C:\\Program Files (x86)\\Project Spark\\Assets\\c9e5f84f41d092f2.assetdb";

            string output = "C:\\Users\\aliso\\Desktop\\out";
            string blobPath = "C:\\Program Files (x86)\\Project Spark\\Assets\\c9e5f84f41d092f2.bundle";


            SQLiteConnection conn = new SQLiteConnection("Data Source=" + databasePath + ";Version=3;\r\n");
            conn.Open();
            SQLiteDataReader datareader;
            SQLiteCommand select = conn.CreateCommand();
            //(string url, string hash, long urid, long offset, long packSize, long gameSize, int typeindex, string bundlePath)
            select.CommandText = "SELECT Url,Hash,ResourceHash.Urid,Offset,SizeInBundle,SizeInGame,Type from ResourceHash join UridToUrl on UridToUrl.Urid = ResourceHash.Urid";
            datareader = select.ExecuteReader();
            while (datareader.Read())
            {
                string url = datareader.GetString(0);
                string hash = datareader.GetString(1);
                long urid = datareader.GetInt64(2);
                long offset = datareader.GetInt64(3);
                long bundleSize = datareader.GetInt64(4);
                long gameSize = datareader.GetInt64(5);
                int type = datareader.GetInt32(6);
                FileEntry tmp = new FileEntry(url, hash, urid, offset, bundleSize, gameSize, type, blobPath);
                tmp.LogConsole();
                tmp.WriteFile(output);
                //Console.ReadKey();
            }


            //FileEntry dummy = new FileEntry("/assets/asset_transfers.xml", "Wi0CcU9jUuPuNkr5Vl2khSqf6aZ+PrTR2KkyzWQcHC4=", -1058066169740589635, 0, 1000206, 1000206, 30, blobPath);
            //dummy.WriteFile(output);



            /*
            SQLiteConnection conn = new SQLiteConnection("Data Source=" + databasePath + ";Version=3;\r\n");
            conn.Open();
            SQLiteDataReader urlDr,offsetDr;

            SQLiteCommand selectUrl = conn.CreateCommand();
            selectUrl.CommandText = "SELECT * from UridToUrl";

            SQLiteCommand selectOffset = conn.CreateCommand();
            selectOffset.CommandText = "SELECT * from UridToUrl";

            urlDr = selectUrl.ExecuteReader();
            offsetDr = selectOffset.ExecuteReader();

            urlDr.Read();
            offsetDr.Read();
            while (dr.Read())
            {
                string url = dr.GetValue(1).ToString();
                string id = dr.GetValue(0).ToString();

            }
            /*DataTable schem = conn.
            foreach (DataRow row in schem.Rows)
            {
                foreach (object o in row.ItemArray)
                {
                    Console.WriteLine(o.ToString());
                }
            }
            //Console.WriteLine(conn.GetSchema());
            */
            //conn.Close();
            Console.WriteLine("finished");
            Console.ReadKey();
        }
    }
}
