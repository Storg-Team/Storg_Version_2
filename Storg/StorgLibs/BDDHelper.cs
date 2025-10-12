using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Data.Sqlite;
using StorgCommon;
using System.Runtime.CompilerServices;

namespace StorgLibs
{
    public class BDDHelper
    {
        private string _BDDFilePath = "";
        private ModelCurrentOS _currentOs = new ModelCurrentOS();
        private SystemHelper _systemhelper = new SystemHelper();
        private string home = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);



        private string SetConnectionString()
        {
            if (_systemhelper.GetCurrentOS() == _currentOs.Windows)
            {
                string CurrentDirectory = AppDomain.CurrentDomain.BaseDirectory;
                _BDDFilePath = Path.Combine(Path.Combine(CurrentDirectory, ".data"), "BDD_Files_Info.db");
            }
            else if (_systemhelper.GetCurrentOS() == _currentOs.Linux)
            {
                string CurrentDirectory = Path.Combine(home, "storg");
                _BDDFilePath = Path.Combine(Path.Combine(CurrentDirectory, ".data"), "BDD_Files_Info.db");
            }
            return @$"Data Source={_BDDFilePath};";
        }

        public void IsBddExisting()
        {
            string DirPath = "";
            if (_systemhelper.GetCurrentOS() == _currentOs.Windows)
            {
                string CurrentDirectory = AppDomain.CurrentDomain.BaseDirectory;
                DirPath = Path.Combine(CurrentDirectory, ".data");
                _BDDFilePath = Path.Combine(DirPath, "BDD_Files_Info.db");
            }
            else if (_systemhelper.GetCurrentOS() == _currentOs.Linux)
            {
                string CurrentDirectory = Path.Combine(home, "storg");
                DirPath = Path.Combine(CurrentDirectory, ".data");
                _BDDFilePath = Path.Combine(DirPath, "BDD_Files_Info.db");
            }

            if (Directory.Exists(DirPath))
            {
                if (!File.Exists(_BDDFilePath))
                {
                    using (File.Create(_BDDFilePath)) { }
                }
            }
            else
            {
                Directory.CreateDirectory(DirPath);
                File.SetAttributes(DirPath, FileAttributes.Hidden);
                using (File.Create(_BDDFilePath)) { }
            }


            string sqlcreatetable = @"CREATE TABLE IF NOT EXISTS Files (Name TEXT NOT NULL, Date TEXT NOT NULL, Time TEXT NOT NULL, Weight TEXT NOT NULL, StoredFolder TEXT NOT NULL)";
            using (SqliteConnection conn = new SqliteConnection(this.SetConnectionString()))
            {
                conn.Open();
                using (SqliteCommand cmd = new SqliteCommand(sqlcreatetable, conn))
                {
                    cmd.ExecuteNonQuery();
                }
            }
        }



        public IList<ModelFile> LoadStoredFile()
        {
            this.IsBddExisting();
            IList<ModelFile> listFile = new List<ModelFile>();

            string sqlrequest = @$"SELECT * FROM Files";
            using (SqliteConnection conn = new SqliteConnection(this.SetConnectionString()))
            {
                conn.Open();
                using (SqliteCommand command = new SqliteCommand(sqlrequest, conn))
                {
                    using (SqliteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            ModelFile file = new ModelFile()
                            {
                                Name = reader["Name"].ToString()!,
                                Date = reader["Date"].ToString()!,
                                Time = reader["Time"].ToString()!,
                                Weight = reader["Weight"].ToString()!,
                                StoredFolder = reader["StoredFolder"].ToString()!,

                            };
                            listFile.Add(file);
                        }
                    }
                }
            }
            return listFile;

        }


        public bool CheckIfFileExist(string NameFIle)
        {
            string sqlrequest = @$"SELECT * FROM Files WHERE Name = @NameFile";
            using (SqliteConnection conn = new SqliteConnection(this.SetConnectionString()))
            {
                conn.Open();
                using (SqliteCommand command = new SqliteCommand(sqlrequest, conn))
                {
                    command.Parameters.AddWithValue("@NameFile", NameFIle);
                    using (SqliteDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read()) return true;
                    }
                }
            }
            return false;
        }


        public void StoreFileToBDD(ModelFile file)
        {
            string sqlrequest = @$"INSERT INTO Files (Name, Date, Time, Weight, StoredFolder) VALUES(@name, @date, @time, @weight, @storedfolder)";
            using (SqliteConnection conn = new SqliteConnection(this.SetConnectionString()))
            {
                conn.Open();
                using (SqliteCommand command = new SqliteCommand(sqlrequest, conn))
                {
                    command.Parameters.AddWithValue("@name", file.Name);
                    command.Parameters.AddWithValue("@date", file.Date);
                    command.Parameters.AddWithValue("@time", file.Time);
                    command.Parameters.AddWithValue("@weight", file.Weight);
                    command.Parameters.AddWithValue("@storedfolder", file.StoredFolder);
                    command.ExecuteNonQuery();
                }
            }
        }



        public string GetStoredPath(string FileName)
        {
            string StoredFolder = "";
            string sqlrequest = @$"SELECT StoredFolder FROM Files WHERE Name = @NameFile";
            using (SqliteConnection conn = new SqliteConnection(this.SetConnectionString()))
            {
                conn.Open();
                using (SqliteCommand command = new SqliteCommand(sqlrequest, conn))
                {
                    command.Parameters.AddWithValue("@NameFile", FileName);
                    using (SqliteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            StoredFolder = reader["StoredFolder"].ToString()!;
                        }
                    }
                }
            }
            return StoredFolder;
        }

        public void DeleteFileInBDD(string FileName)
        {
            string sqlrequest = @$"DELETE FROM Files WHERE Name = @NameFile";
            using (SqliteConnection conn = new SqliteConnection(this.SetConnectionString()))
            {
                conn.Open();
                using (SqliteCommand command = new SqliteCommand(sqlrequest, conn))
                {
                    command.Parameters.AddWithValue("@NameFile", FileName);
                    command.ExecuteNonQuery();
                }
            }
        }

        public IList<ModelFile> ResearchFileByName(string ResearchText)
        {
            IList<ModelFile> FileList = new List<ModelFile>();
            string sqlrequest = "SELECT * FROM Files WHERE Name LIKE @search";
            using (SqliteConnection conn = new SqliteConnection(this.SetConnectionString()))
            {
                conn.Open();
                using (SqliteCommand command = new SqliteCommand(sqlrequest, conn))
                {
                    command.Parameters.AddWithValue("@search", @$"%{ResearchText}%");
                    using (SqliteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            FileList.Add(new ModelFile()
                            {
                                Name = reader["Name"].ToString()!,
                                Date = reader["Date"].ToString()!,
                                Time = reader["Time"].ToString()!,
                                Weight = reader["Weight"].ToString()!,
                                StoredFolder = reader["StoredFolder"].ToString()!,
                            });
                        }
                    }
                }
            }
            return FileList;
        }


    }
}

