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
        private string _connectionString = "";
        private bool _IsAlreadyCheck = false;
        private LibsGlobal _libsglobal = new LibsGlobal();


        public void IsBddExisting()
        {
            string DirPath = "";
            if (_libsglobal.GetCurrentOS().OS == "Windows")
            {
                string CurrentDirectory = AppDomain.CurrentDomain.BaseDirectory;
                DirPath = Path.Combine(CurrentDirectory, ".data");
                _BDDFilePath = Path.Combine(DirPath, "BDD_Files_Info.db");
            }
            else if (_libsglobal.GetCurrentOS().OS == "Linux")
            {
                string CurrentDirectory = "/usr/share/storg/";
                DirPath = Path.Combine(CurrentDirectory, ".data");
                _BDDFilePath = Path.Combine(DirPath, "BDD_Files_Info.db");
            }
            _connectionString = @$"Data Source={_BDDFilePath};";
            if (Directory.Exists(DirPath))
            {
                if (!File.Exists(_BDDFilePath))
                {
                    File.Create(_BDDFilePath);
                }
            }
            else
            {
                Directory.CreateDirectory(DirPath);
                File.SetAttributes(DirPath, FileAttributes.Hidden);
                File.Create(_BDDFilePath);
            }


            string sqlcreatetable = @"CREATE TABLE IF NOT EXISTS Files (Name TEXT NOT NULL, Date TEXT NOT NULL, Time TEXT NOT NULL, Weight INTEGER, StoredFolder TEXT NOT NULL)";
            using (SqliteConnection conn = new SqliteConnection(_connectionString))
            {
                conn.Open();
                using (SqliteCommand cmd = new SqliteCommand(sqlcreatetable, conn))
                {
                    cmd.ExecuteNonQuery();
                }
                conn.Close();
            }
            _IsAlreadyCheck = true;
        }



        public IList<ModelFile> LoadStoredFile()
        {
            IList<ModelFile> listFile = new List<ModelFile>();
            if (!_IsAlreadyCheck)
            {
                this.IsBddExisting();
            }

            string sqlselectall = @$"SELECT * FROM Files";
            using (SqliteConnection conn = new SqliteConnection(_connectionString))
            {
                conn.Open();
                using (SqliteCommand command = new SqliteCommand(sqlselectall, conn))
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
                                Weight = Convert.ToInt32(reader["Weight"])!,
                                StoredFolder = reader["StoredFolder"].ToString()!,

                            };
                            listFile.Add(file);
                        }
                    }
                }
                conn.Close();
            }
            return listFile;

        }


        public bool CheckIfFileExist(string NameFIle)
        {
            string sqlselectall = @$"SELECT * FROM Files WHERE Name = @NameFile";
            using (SqliteConnection conn = new SqliteConnection(_connectionString))
            {
                conn.Open();
                using (SqliteCommand command = new SqliteCommand(sqlselectall, conn))
                {
                    command.Parameters.AddWithValue("@NameFile", NameFIle);
                    using (SqliteDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read()) return true;
                    }
                }
                conn.Close();
            }
            return false;
        }




    }


}

