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
using System.Configuration;

namespace StorgLibs
{
    public class BDDHelper
    {
        private static string _BDDFilePath = "";
        private static string _connectionString = "";
        private SystemHelper _systemhelper = new SystemHelper();
        private string home = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);



        public BDDHelper()
        {
            if (_systemhelper.IsWindows())
            {
                string CurrentDirectory = AppDomain.CurrentDomain.BaseDirectory;
                _BDDFilePath = Path.Combine(Path.Combine(CurrentDirectory, ".data"), "BDD_Files_Info.db");
            }
            else if (_systemhelper.IsLinux() || _systemhelper.IsOSX())
            {
                string CurrentDirectory = Path.Combine(home, "storg");
                _BDDFilePath = Path.Combine(Path.Combine(CurrentDirectory, ".data"), "BDD_Files_Info.db");
            }
            _connectionString = @$"Data Source={_BDDFilePath};";
        }

        public void IsBddExisting()
        {
            string DirPath = "";
            if (_systemhelper.IsWindows())
            {
                string CurrentDirectory = AppDomain.CurrentDomain.BaseDirectory;
                DirPath = Path.Combine(CurrentDirectory, ".data");
                _BDDFilePath = Path.Combine(DirPath, "BDD_Files_Info.db");
            }
            else if (_systemhelper.IsLinux() || _systemhelper.IsOSX())
            {
                string CurrentDirectory = Path.Combine(home, "storg");
                DirPath = Path.Combine(CurrentDirectory, ".data");
                _BDDFilePath = Path.Combine(DirPath, "BDD_Files_Info.db");
            }

            if (Directory.Exists(DirPath))
            {
                if (!File.Exists(_BDDFilePath))
                {
                    using (File.Create(_BDDFilePath)) { };
                }
            }
            else
            {
                Directory.CreateDirectory(DirPath);
                File.SetAttributes(DirPath, FileAttributes.Hidden);
                using (File.Create(_BDDFilePath)) { };
            }


            using (SqliteConnection conn = new SqliteConnection(_connectionString))
            {
                conn.Open();

                SqliteCommand command = conn.CreateCommand();
                command.CommandText = @"CREATE TABLE IF NOT EXISTS Files (Name TEXT NOT NULL, Date TEXT NOT NULL, Time TEXT NOT NULL, Weight TEXT NOT NULL, StoredFolder TEXT NOT NULL)";
                command.ExecuteNonQuery();

                command = conn.CreateCommand();
                command.CommandText = "CREATE TABLE IF NOT EXISTS Settings (id INTEGER PRIMARY KEY, userId INTEGER NOT NULL, lightMode INTEGER NOT NULL, login TEXT NOT NULL, password TEXT NOT NULL, isConnected INTEGER NOT NULL, stayConnected INTEGER NOT NULL)";
                command.ExecuteNonQuery();

                command = conn.CreateCommand();
                command.CommandText = "SELECT * FROM Settings;";

                using (SqliteDataReader reader = command.ExecuteReader())
                {
                    if (!reader.Read())
                    {
                        command = conn.CreateCommand();
                        command.CommandText = "INSERT INTO Settings (id, userId, lightMode, login, password, isConnected, stayConnected) VALUES(1, 0, true, '', '', false, false);";
                        command.ExecuteNonQuery();
                    }
                }

                conn.Close();
            }
        }

        #region BDD Files

        public IList<ModelFile> LoadStoredFile()
        {
            IList<ModelFile> listFile = new List<ModelFile>();

            string sqlrequest = @$"SELECT * FROM Files";
            using (SqliteConnection conn = new SqliteConnection(_connectionString))
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
                    conn.Close();
                }
            }
            return listFile;

        }


        public bool CheckIfFileExistInBDD(string NameFIle)
        {
            string sqlrequest = @$"SELECT * FROM Files WHERE Name = @NameFile";
            using (SqliteConnection conn = new SqliteConnection(_connectionString))
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


        public bool StoreFileToBDD(ModelFile file)
        {
            string sqlrequest = @$"INSERT INTO Files (Name, Date, Time, Weight, StoredFolder) VALUES(@name, @date, @time, @weight, @storedfolder)";

            using (SqliteConnection conn = new SqliteConnection(_connectionString))
            {
                conn.Open();
                using (SqliteCommand command = new SqliteCommand(sqlrequest, conn))
                {
                    command.Parameters.AddWithValue("@name", file.Name);
                    command.Parameters.AddWithValue("@date", file.Date);
                    command.Parameters.AddWithValue("@time", file.Time);
                    command.Parameters.AddWithValue("@weight", file.Weight);
                    command.Parameters.AddWithValue("@storedfolder", file.StoredFolder);

                    if (command.ExecuteNonQuery() == 1)
                    {
                        return true;
                    }
                }
                conn.Close();
            }
            return false;
        }



        public string GetStoredPath(string FileName)
        {
            string StoredFolder = "";
            string sqlrequest = @$"SELECT StoredFolder FROM Files WHERE Name = @NameFile";
            using (SqliteConnection conn = new SqliteConnection(_connectionString))
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
                conn.Close();
            }
            return StoredFolder;
        }

        public void DeleteFileInBDD(string FileName)
        {
            string sqlrequest = @$"DELETE FROM Files WHERE Name = @NameFile";
            using (SqliteConnection conn = new SqliteConnection(_connectionString))
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
            using (SqliteConnection conn = new SqliteConnection(_connectionString))
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
                conn.Close();
            }
            return FileList;
        }

        #endregion BDD Files



        #region BDD Settings

        public ModelSettings LoadSettings()
        {
            ModelSettings settings = new ModelSettings();
            using (SqliteConnection conn = new SqliteConnection(_connectionString))
            {
                conn.Open();

                SqliteCommand command = conn.CreateCommand();
                command.CommandText = "SELECT * FROM Settings;";

                using (SqliteDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        settings.userId = reader.GetInt32(1);
                        settings.lightMode = reader.GetBoolean(2);
                        settings.login = reader.GetString(3);
                        settings.password = reader.GetString(4);
                        settings.isConnected = reader.GetBoolean(5);
                        settings.stayConnected = reader.GetBoolean(6);
                    }
                }
                conn.Close();
            }
            return settings;
        }

        public bool UpdateSettingsThemeMode(bool lightMode)
        {
            using (SqliteConnection conn = new SqliteConnection(_connectionString))
            {
                conn.Open();

                SqliteCommand command = conn.CreateCommand();
                command.CommandText = "UPDATE Settings SET lightMode = @mode WHERE id = 1;";
                command.Parameters.AddWithValue("mode", lightMode);

                command.ExecuteNonQuery();

                conn.Close();
            }
            return true;
        }

        public bool UpdateSettingsStayConnected(bool stayConnected)
        {
            using (SqliteConnection conn = new SqliteConnection(_connectionString))
            {
                conn.Open();
                SqliteCommand command = conn.CreateCommand();
                command.CommandText = "UPDATE Settings SET stayConnected = @stayCo WHERE id= 1;";
                command.Parameters.AddWithValue("stayCo", stayConnected);

                command.ExecuteNonQuery();

                conn.Close();
            }
            return true;
        }

        public bool UpdateSettingsCredentials(string login, string password, int userId, bool isConnected = true)
        {
            using (SqliteConnection conn = new SqliteConnection(_connectionString))
            {
                conn.Open();

                SqliteCommand command = conn.CreateCommand();
                command.CommandText = "UPDATE Settings SET userId = @userId, login = @login, password = @password, isConnected = @connected WHERE id = 1;";
                command.Parameters.AddWithValue("userId", userId);
                command.Parameters.AddWithValue("login", login);
                command.Parameters.AddWithValue("password", password);
                command.Parameters.AddWithValue("connected", isConnected);

                command.ExecuteNonQuery();

                conn.Close();
            }
            return true;
        }

        #endregion BDD Settings
    }
}

