using System;
using System.Data.SqlClient;
using System.Diagnostics;

namespace ContractMonthlyClaimSystem.Data
{
    public class DatabaseInitializer
    {
        private string connectionString = @"Server=(localdb)\claim_system;Integrated Security=true;";

        public void CreateClaimSystemInstance()
        {
            if (CheckInstanceExists())
            {
                Console.WriteLine("LocalDB instance 'claim_system' already exists.");
                return;
            }

            var processStartInfo = new ProcessStartInfo
            {
                FileName = "cmd.exe",
                Arguments = "/c sqllocaldb create \"claim_system\"",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            };

            using (var process = new Process())
            {
                process.StartInfo = processStartInfo;
                process.Start();

                string output = process.StandardOutput.ReadToEnd();
                string error = process.StandardError.ReadToEnd();

                process.WaitForExit();

                if (process.ExitCode == 0)
                {
                    Console.WriteLine("LocalDB instance 'claim_system' created successfully!");
                    Console.WriteLine(output);
                }
                else
                {
                    Console.WriteLine($"Error creating instance: {error}");
                }
            }
        }

        private bool CheckInstanceExists()
        {
            var processStartInfo = new ProcessStartInfo
            {
                FileName = "cmd.exe",
                Arguments = "/c sqllocaldb info \"claim_system\"",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            };

            using (var process = new Process())
            {
                process.StartInfo = processStartInfo;
                process.Start();

                string output = process.StandardOutput.ReadToEnd();
                string error = process.StandardError.ReadToEnd();

                process.WaitForExit();

                return process.ExitCode == 0;
            }
        }

        public void CreateDatabaseAndTables()
        {
            try
            {
                CreateDatabase();
                CreateTables();
                Console.WriteLine("Database and tables created successfully!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating database and tables: {ex.Message}");
            }
        }

        private void CreateDatabase()
        {
            string createDbQuery = @"
                IF NOT EXISTS(SELECT * FROM sys.databases WHERE name = 'claims_database')
                BEGIN
                    CREATE DATABASE claims_database;
                END";

            using (var connection = new SqlConnection(connectionString))
            {
                using (var command = new SqlCommand(createDbQuery, connection))
                {
                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }

        private void CreateTables()
        {
            string createUsersTable = @"
                USE claims_database;
                IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Users' AND xtype='U')
                BEGIN
                    CREATE TABLE Users (
                        userID INT PRIMARY KEY IDENTITY(1,1),
                        full_names VARCHAR(100),
                        surname VARCHAR(100),
                        email VARCHAR(100),
                        role VARCHAR(100),
                        gender VARCHAR(100),
                        password VARCHAR(100),
                        date DATE
                    );
                END";

            string createClaimsTable = @"
                USE claims_database;
                IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Claims' AND xtype='U')
                BEGIN
                    CREATE TABLE Claims (
                        claimID INT PRIMARY KEY IDENTITY(1,1),
                        number_of_sessions INT,
                        number_of_hours INT,
                        amount_of_rate INT,
                        module_name VARCHAR(100),
                        faculty_name VARCHAR(100),
                        supporting_documents VARCHAR(100),
                        claim_status VARCHAR(100),
                        creating_date DATE,
                        lecturerID INT,
                        FOREIGN KEY (lecturerID) REFERENCES Users(userID)
                    );
                END";

            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // Create Users table
                using (var command = new SqlCommand(createUsersTable, connection))
                {
                    command.ExecuteNonQuery();
                }

                // Create Claims table
                using (var command = new SqlCommand(createClaimsTable, connection))
                {
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
