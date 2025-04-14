using MySql.Data.MySqlClient;
using static votingsystem.Pages.RegistrationModel;
using static votingsystem.Pages.Shared.ADManageElectionsModel;

namespace votingsystem.Database_Helper
{
    public class DbHelper
    {
        private static string GetConnectionString()
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            
            var connectionString = configuration.GetConnectionString("MySqlConnection");
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("Connection string 'MySqlConnection' is missing or empty in appsettings.json.");
            }

            return connectionString;
        }

        
        public static MySqlConnection GetConnection()
        {
            string connectionString = GetConnectionString();
            return new MySqlConnection(GetConnectionString());
        }

       public static bool RegisterUser(User user)
        {
            using (var con = GetConnection())
            {
                string query = @"INSERT INTO Users (FirstName, LastName, Email, Address, PasswordHash, Month, Day, Year, UserName, Age)
                         VALUES (@FirstName, @LastName, @Email, @Address, @PasswordHash, @Month, @Day, @Year, @UserName, @Age)";

                using (var cmd = new MySqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@FirstName", user.FirstName);
                    cmd.Parameters.AddWithValue("@LastName", user.LastName);
                    cmd.Parameters.AddWithValue("@Email", user.Email);
                    cmd.Parameters.AddWithValue("@Address", user.Address);
                    cmd.Parameters.AddWithValue("@PasswordHash", user.PasswordHash);
                    cmd.Parameters.AddWithValue("@Month", user.Month);
                    cmd.Parameters.AddWithValue("@Day", user.Day);
                    cmd.Parameters.AddWithValue("@Year", user.Year);
                    cmd.Parameters.AddWithValue("@UserName", user.UserName);
                    cmd.Parameters.AddWithValue("@Age", user.Age);

                    try
                    {
                        con.Open();
                        Console.WriteLine("Database connection opened.");
                        cmd.ExecuteNonQuery();
                        Console.WriteLine("SQL query executed successfully.");
                        return true; // success
                    }
                    catch (MySqlException ex)
                    {
                        Console.WriteLine($"MySQL Error: {ex.Message}");
                        return false; // fail
                    }
                    finally
                    {
                        con.Close();
                        Console.WriteLine("Database connection closed.");
                    }
                }
            }
        }




        // verify user info
        public static bool VerifyUser(string UserName, string Password)
        {
            using (var con = GetConnection())
            {
                // retrieve the hashed password for the given username
                string query = "SELECT PasswordHash FROM Users WHERE UserName = @username";

                using (var cmd = new MySqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@username", UserName);

                    try
                    {
                        Console.WriteLine("Opening database connection...");
                        con.Open();

                        Console.WriteLine("Executing query...");
                        var storedPasswordHash = cmd.ExecuteScalar()?.ToString();

                        if (string.IsNullOrEmpty(storedPasswordHash))
                        {
                            Console.WriteLine("User not found.");
                            return false; 
                        }

                        // verify the entered password against the stored hash
                        bool isPasswordValid = BCrypt.Net.BCrypt.Verify(Password, storedPasswordHash);
                        Console.WriteLine($"Password verification result: {isPasswordValid}");

                        return isPasswordValid; // true if password matches
                    }
                    catch (MySqlException ex)
                    {
                        Console.WriteLine($"Database error: {ex.Message}");
                        return false; // handle database errors 
                    }
                    finally
                    {
                        Console.WriteLine("Closing database connection...");
                        con.Close();
                    }
                }
            }
        }

        public static List<Candidate> GetCandidates()
        {
            var candidates = new List<Candidate>();
            using (var con = new MySqlConnection(GetConnectionString()))
            {
                con.Open();
                string query = "SELECT Id, Name, Age, Address, Position FROM candidates";
                using (var cmd = new MySqlCommand(query, con))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            candidates.Add(new Candidate)
                            {
                                Id = reader.GetInt32("Id"),
                                Name = reader.GetString("Name"),
                                Age = reader.GetInt32("Age"),
                                Address = reader.GetString("Address"),
                                Position = reader.GetString("Position")
                            });
                        }
                    }
                }
            }
            return candidates;
        }



        // cast a vote
        public static bool CastVote(string PresidentVote, string VicePresidentVote)
        {
            using (var con = GetConnection())
            {
                string query = "INSERT INTO VoteCast (president, vice_president) VALUES (@president, @vicePresident)";
                MySqlCommand cmd = new MySqlCommand(query, con);

                cmd.Parameters.AddWithValue("@president", PresidentVote);
                cmd.Parameters.AddWithValue("@vicePresident", VicePresidentVote);

                try
                {
                    con.Open();
                    cmd.ExecuteNonQuery();
                    return true; // vote successful
                }
                finally
                {
                    con.Close();
                }
            }
        }

        // get vote tally for a category
        public static Dictionary<string, int> GetVoteTally(string category)
        {
            var voteTally = new Dictionary<string, int>();

            using (var con = GetConnection())
            {
                string query = $"SELECT {category}, COUNT(*) AS VoteCount FROM VoteCast GROUP BY {category}";
                MySqlCommand cmd = new MySqlCommand(query, con);

                try
                {
                    con.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string candidate = reader[0]?.ToString() ?? "Unknown";
                            int count = reader[1] != DBNull.Value ? Convert.ToInt32(reader[1]) : 0;

                            voteTally[candidate] = count;
                        }
                    }
                }
                finally
                {
                    con.Close();
                }
            }

            return voteTally;
        }

    }
}

