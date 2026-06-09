using System;
using System.Data.SQLite;
using System.IO;
using System.Windows.Forms;

namespace Odev_Project
{
    public static class DatabaseHelper
    {
        private static string dbFolder = Path.Combine(Application.StartupPath, "Database");
        private static string dbPath = Path.Combine(dbFolder, "kou_yemekhanem.db");

        private static string connectionString = "Data Source=" + dbPath + ";Version=3;";

        public static SQLiteConnection GetConnection()
        {
            return new SQLiteConnection(connectionString);
        }

        public static void CreateDatabaseIfNotExists()
        {
            if (!Directory.Exists(dbFolder))
            {
                Directory.CreateDirectory(dbFolder);
            }

            if (!File.Exists(dbPath))
            {
                SQLiteConnection.CreateFile(dbPath);
            }

            CreateTables();
        }

        private static void CreateTables()
        {
            using (SQLiteConnection connection = GetConnection())
            {
                connection.Open();

                string menusTable = @"
                CREATE TABLE IF NOT EXISTS Menus (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    MenuDate TEXT NOT NULL UNIQUE,
                    Corba TEXT,
                    AnaYemek TEXT,
                    YardimciYemek TEXT,
                    Tatli TEXT,
                    ImagePath TEXT,
                    CreatedAt TEXT
                );";

                string commentsTable = @"
                CREATE TABLE IF NOT EXISTS Comments (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    MenuId INTEGER NOT NULL,
                    FoodName TEXT,
                    Rating INTEGER,
                    CommentText TEXT,
                    UserEmail TEXT,
                    CreatedAt TEXT,
                    FOREIGN KEY(MenuId) REFERENCES Menus(Id)
                );";

                string usersTable = @"
                CREATE TABLE IF NOT EXISTS Users (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Email TEXT NOT NULL UNIQUE,
                    VerificationCode TEXT,
                    IsVerified INTEGER DEFAULT 0,
                    CreatedAt TEXT
                );";

                using (SQLiteCommand command = new SQLiteCommand(menusTable, connection))
                {
                    command.ExecuteNonQuery();
                }

                using (SQLiteCommand command = new SQLiteCommand(commentsTable, connection))
                {
                    command.ExecuteNonQuery();
                }

                using (SQLiteCommand command = new SQLiteCommand(usersTable, connection))
                {
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}