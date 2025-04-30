using MySql.Data.MySqlClient;
using static votingsystem.Pages.RegistrationModel;
using static votingsystem.Pages.Shared.ADManageElectionsModel;
using static votingsystem.Pages.Shared.ADManageVotersModel;
using static votingsystem.Pages.Shared.VotingPageModel;
using static votingsystem.Pages.Shared.UPDoneVotingModel;
using static votingsystem.Pages.Shared.UPResultsModel;
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

                        return isPasswordValid; 
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

        public static bool CheckIfEmailExist(string email)
        {
            using (var con = GetConnection())
            {
                string query = "SELECT COUNT(*) FROM Users WHERE Email = @Email";
                using (var cmd = new MySqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@Email", email);
                    con.Open();
                    return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
                }
            }
        }

        public static void SaveResetToken(string email, string token)
        {
            using (var con = GetConnection())
            {
                string query = "UPDATE Users SET ResetToken = @Token, TokenExpiry = @Expiry WHERE Email = @Email";
                using (var cmd = new MySqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@Token", token);
                    cmd.Parameters.AddWithValue("@Expiry", DateTime.Now.AddHours(1)); //  expires in 1hr
                    cmd.Parameters.AddWithValue("@Email", email);
                    con.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public static bool ValidateResetToken(string token)
        {
            using (var con = GetConnection())
            {
                string query = "SELECT COUNT(*) FROM Users WHERE ResetToken = @Token AND TokenExpiry > NOW()";
                using (var cmd = new MySqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@Token", token);
                    con.Open();
                    return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
                }
            }
        }

        public static bool ResetPassword(string token, string password)
        {
            using (var con = GetConnection())
            {
                string query = "UPDATE Users SET PasswordHash = @Password, ResetToken = NULL, TokenExpiry = NULL WHERE ResetToken = @Token";
                using (var cmd = new MySqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@Password", BCrypt.Net.BCrypt.HashPassword(password)); // Hash the password
                    cmd.Parameters.AddWithValue("@Token", token);
                    con.Open();
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }




        public static string GetUserRole(string userName)
        {
            using (var con = GetConnection())
            {
                string query = "SELECT Role FROM Users WHERE UserName = @UserName";

                using (var cmd = new MySqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@UserName", userName);

                    try
                    {
                        con.Open();
                        var role = cmd.ExecuteScalar()?.ToString();
                        return role;
                    }
                    catch (MySqlException ex)
                    {
                        Console.WriteLine($"MySQL Error: {ex.Message}");
                        return null;
                    }
                }
            }
        }


        public static List<Voter> GetVoters()
        {
            var voters = new List<Voter>();

            using (var con = GetConnection())
            {
                con.Open();
                string query = "SELECT Id, FirstName, LastName, Email, Address, PasswordHash, Month, Day, Year, UserName, Age, Role FROM Users";

                using (var command = new MySqlCommand(query, con))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            voters.Add(new Voter
                            {
                                Id = reader.GetInt32("Id"),
                                FirstName = reader.GetString("FirstName"),
                                LastName = reader.GetString("LastName"),
                                Email = reader.GetString("Email"),
                                Address = reader.GetString("Address"),
                                PasswordHash = reader.GetString("PasswordHash"),
                                Month = reader.GetString("Month"),
                                Day = reader.GetInt32("Day"),
                                Year = reader.GetInt32("Year"),
                                UserName = reader.GetString("UserName"),
                                Age = reader.GetInt32("Age"),
                                Role = reader.GetString("Role")
                            });
                        }
                    }
                }
            }

            return voters;
        }

        public static bool DeleteVoter(int id)
        {
            using (var con = GetConnection())
            {
                string query = "DELETE FROM Users WHERE Id = @Id";

                using (var cmd = new MySqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@Id", id);

                    try
                    {
                        con.Open();
                        cmd.ExecuteNonQuery();
                        return true;
                    }
                    catch (MySqlException ex)
                    {
                        Console.WriteLine($"MySQL Error: {ex.Message}");
                        return false;
                    }
                    finally
                    {
                        con.Close();
                    }
                }
            }
        }



        public static void CreateElection(Election election)
        {
            using (var con = GetConnection())
            {
                string query = "INSERT INTO Elections (Title, Description, Start_time, End_time) VALUES (@Title, @Description, @StartTime, @EndTime)";

                using (var cmd = new MySqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@Title", election.Title);
                    cmd.Parameters.AddWithValue("@Description", election.Description);
                    cmd.Parameters.AddWithValue("@StartTime", election.StartTime);
                    cmd.Parameters.AddWithValue("@EndTime", election.EndTime);

                    try
                    {
                        con.Open();
                        cmd.ExecuteNonQuery();
                        Console.WriteLine("Election inserted successfully.");
                    }
                    catch (MySqlException ex)
                    {
                        Console.WriteLine($"MySQL Error: {ex.Message}");
                    }
                    finally
                    {
                        con.Close();
                    }
                }
            }
        }


        // in order to display the list of elections
        public static List<Election> GetElections()
        {
            var elections = new List<Election>();

            using (var con = GetConnection())
            {
                con.Open();
                string query = "SELECT ElectionId, Title, Description, Start_time, End_time FROM Elections";

                using (var command = new MySqlCommand(query, con))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            elections.Add(new Election
                            {
                                ElectionId = reader.GetInt32("ElectionId"),
                                Title = reader.GetString("Title"),
                                Description = reader.IsDBNull(reader.GetOrdinal("Description")) ? null : reader.GetString("Description"),
                                StartTime = reader.GetDateTime("Start_time"),
                                EndTime = reader.GetDateTime("End_time")
                            });
                        }
                    }
                }
            }

            return elections;
        }



        public static bool IsElectionTitleRegistered(string title)
        {
            using (var con = GetConnection())
            {
                string query = "SELECT * FROM Elections WHERE Title = @Title";

                using (var cmd = new MySqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@Title", title);

                    try
                    {
                        con.Open();
                        int count = Convert.ToInt32(cmd.ExecuteScalar());
                        return count > 0;
                    }
                    catch (MySqlException ex)
                    {
                        Console.WriteLine($"MySQL Error: {ex.Message}");
                        return false;
                    }
                    finally
                    {
                        con.Close();
                    }
                }
            }
        }

        // fetch the details of the election
        public static Election GetElectionById(int id)
        {
            Election election = null;

            using (var con = GetConnection())
            {
                string query = "SELECT ElectionId, Title, Description, Start_time, End_time FROM Elections WHERE ElectionId = @ElectionId";

                using (var cmd = new MySqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@ElectionId", id);

                    try
                    {
                        con.Open();
                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                election = new Election
                                {
                                    ElectionId = reader.GetInt32("ElectionId"),
                                    Title = reader.GetString("Title"),
                                    Description = reader.IsDBNull(reader.GetOrdinal("Description")) ? null : reader.GetString("Description"),
                                    StartTime = reader.GetDateTime("Start_time"),
                                    EndTime = reader.GetDateTime("End_time")
                                };
                            }
                        }
                    }
                    catch (MySqlException ex)
                    {
                        Console.WriteLine($"MySQL Error: {ex.Message}");
                    }
                }
            }

            return election;
        }


        public static bool UpdateElection(Election election)
        {
            using (var con = GetConnection())
            {
                string query = @"UPDATE Elections 
                         SET Title = @Title, 
                             Description = @Description, 
                             Start_time = @StartTime, 
                             End_time = @EndTime 
                         WHERE ElectionId = @Id";

                using (var cmd = new MySqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@Id", election.ElectionId);
                    cmd.Parameters.AddWithValue("@Title", election.Title);
                    cmd.Parameters.AddWithValue("@Description", election.Description);
                    cmd.Parameters.AddWithValue("@StartTime", election.StartTime);
                    cmd.Parameters.AddWithValue("@EndTime", election.EndTime);

                    try
                    {
                        con.Open();
                        cmd.ExecuteNonQuery();
                        return true;
                    }
                    catch (MySqlException ex)
                    {
                        Console.WriteLine($"MySQL Error: {ex.Message}");
                        return false;
                    }
                    finally
                    {
                        con.Close();
                    }
                }
            }
        }



        public static bool DeleteElection(int id)
        {
            using (var con = GetConnection())
            {
                string query = "DELETE FROM Elections WHERE ElectionId = @Id";

                using (var cmd = new MySqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@Id", id);

                    try
                    {
                        con.Open();
                        cmd.ExecuteNonQuery();
                        return true;
                    }
                    catch (MySqlException ex)
                    {
                        Console.WriteLine($"MySQL Error: {ex.Message}");
                        return false;
                    }
                    finally
                    {
                        con.Close();
                    }
                }
            }
        }

        // in order to display list of candidates
        public static List<Candidate> GetCandidates()
        {
            var candidates = new List<Candidate>();

            using (var con = GetConnection())
            {
                string query = "SELECT Id, Name, Age, Address, Position, PartyList, ElectionId FROM Candidates";

                using (var cmd = new MySqlCommand(query, con))
                {
                    try
                    {
                        con.Open();
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                candidates.Add(new Candidate
                                {
                                    Id = reader.GetInt32("Id"),
                                    Name = reader.GetString("Name"),
                                    Age = reader.GetInt32("Age"),
                                    Address = reader.GetString("Address"),
                                    Position = reader.GetString("Position"),
                                    PartyList = reader.GetString("PartyList"),
                                    ElectionId = reader.IsDBNull(reader.GetOrdinal("ElectionId")) ? 0 : reader.GetInt32("ElectionId")
                                });
                            }
                        }
                    }
                    catch (MySqlException ex)
                    {
                        Console.WriteLine($"MySQL Error: {ex.Message}");
                    }
                    finally
                    {
                        con.Close();
                        Console.WriteLine("Database connection closed.");
                    }
                }
            }

            return candidates;
        }

        public static bool CreateCandidate(Candidate candidate)
        {
            using (var con = GetConnection())
            {
                string query = @"INSERT INTO Candidates (Name, Age, Address, Position, PartyList, ElectionId, PictureUrl)
                         VALUES (@Name, @Age, @Address, @Position, @PartyList, @ElectionId, @PictureUrl)";

                using (var cmd = new MySqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@Name", candidate.Name);
                    cmd.Parameters.AddWithValue("@Age", candidate.Age);
                    cmd.Parameters.AddWithValue("@Address", candidate.Address);
                    cmd.Parameters.AddWithValue("@Position", candidate.Position);
                    cmd.Parameters.AddWithValue("@PartyList", candidate.PartyList);
                    cmd.Parameters.AddWithValue("@ElectionId", candidate.ElectionId);
                    cmd.Parameters.AddWithValue("@PictureUrl", candidate.PictureUrl);

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

        public static bool IsCandidateNameRegistered(string name)
        {
            using (var con = GetConnection())
            {
                con.Open();
                string query = "SELECT * FROM Candidates WHERE Name = @Name";
                using (var cmd = new MySqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@Name", name);

                    var count = Convert.ToInt32(cmd.ExecuteScalar());
                    return count > 0; 
                }
            }
        }

        public static bool DeleteCandidate(int id)
        {
            using (var con = GetConnection())
            {
                string query = @"DELETE FROM Candidates WHERE Id = @Id";

                using (var cmd = new MySqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@Id", id);

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

        public static int GetTotalElections()
        {
            using (var con = GetConnection())
            {
                string query = "SELECT COUNT(*) FROM Elections";

                using (var cmd = new MySqlCommand(query, con))
                {
                    try
                    {
                        con.Open();
                        return Convert.ToInt32(cmd.ExecuteScalar());
                    }
                    catch (MySqlException ex)
                    {
                        Console.WriteLine($"MySQL Error: {ex.Message}");
                        return 0;
                    }
                    finally
                    {
                        con.Close();
                    }
                }
            }
        }

        public static int GetTotalVoters()
        {
            using (var con = GetConnection())
            {
                string query = "SELECT COUNT(*) FROM Users WHERE Role = 'User'";

                using (var cmd = new MySqlCommand(query, con))
                {
                    try
                    {
                        con.Open();
                        Console.WriteLine("Executing query to fetch voter count...");
                        int VoterCount = Convert.ToInt32(cmd.ExecuteScalar());
                        Console.WriteLine($"Query executed successfully. Voter count: {VoterCount}");
                        return VoterCount;
                    }
                    catch (MySqlException ex)
                    {
                        Console.WriteLine($"MySQL Error: {ex.Message}");
                        return 0;
                    }
                    finally
                    {
                        con.Close();
                        Console.WriteLine("Database connection closed.");
                    }
                }
            }
        }

        public static int GetTotalVotes()
        {
            using (var con = GetConnection())
            {
                string query = "SELECT * FROM Votes";

                using (var cmd = new MySqlCommand(query, con))
                {
                    try
                    {
                        con.Open();
                        return Convert.ToInt32(cmd.ExecuteScalar());
                    }
                    catch (MySqlException ex)
                    {
                        Console.WriteLine($"MySQL Error: {ex.Message}");
                        return 0;
                    }
                    finally
                    {
                        con.Close();
                    }
                }
            }
        }

        public static List<Election> GetAvailableElections()
        {
            var elections = new List<Election>();

            try
            {
                using (var con = GetConnection())
                {
                    string query = "SELECT ElectionId, Title, Description, Start_time, End_time " +
                                   "FROM Elections WHERE End_time > NOW() AND Start_time <= NOW()";

                    using (var cmd = new MySqlCommand(query, con))
                    {
                        con.Open();

                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var election = new Election
                                {
                                    ElectionId = Convert.ToInt32(reader["ElectionId"]),
                                    Title = reader["Title"].ToString(),
                                    Description = reader["Description"].ToString(),
                                    StartTime = DateTime.Parse(reader["Start_time"].ToString()),
                                    EndTime = DateTime.Parse(reader["End_time"].ToString()),
                                    Status = DateTime.Parse(reader["End_time"].ToString()) > DateTime.Now ? "Ongoing" : "Ended"
                                };

                                elections.Add(election);
                            }
                        }
                    }
                }
            }
            catch (MySqlException ex)
            {
                Console.WriteLine($"SQL Error: {ex.Message}");
            }

            return elections;
        }



        public static List<Election> GetUpcomingElections()
        {
            var elections = new List<Election>();

            try
            {
                using (var con = GetConnection())
                {
                    string query = "SELECT Title, Description, Start_time, End_time, ElectionId " +
                                   "FROM Elections WHERE Start_time > NOW()";

                    using (var cmd = new MySqlCommand(query, con))
                    {
                        con.Open();

                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var election = new Election
                                {
                                    Title = reader["Title"].ToString(),
                                    Description = reader["Description"].ToString(),
                                    StartTime = DateTime.Parse(reader["Start_time"].ToString()),
                                    EndTime = DateTime.Parse(reader["End_time"].ToString()),
                                    ElectionId = reader.GetInt32("ElectionId"),
                                    Status = "Upcoming"
                                };

                                elections.Add(election);
                            }
                        }
                    }
                }
            }
            catch (MySqlException ex)
            {
                Console.WriteLine($"SQL Error: {ex.Message}");
            }

            return elections;
        }

        public static List<Candidate> GetCandidatesByElectionId(int electionId)
        {
            var candidates = new List<Candidate>();

            using (var con = GetConnection())
            {
                string query = "SELECT Id, Name, Address, Age, Position, PartyList, PictureUrl FROM Candidates WHERE ElectionId = @ElectionId";
                using (var cmd = new MySqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@ElectionId", electionId);
                    con.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            candidates.Add(new Candidate
                            {
                                Id = Convert.ToInt32(reader["Id"]),
                                Name = reader["Name"].ToString(),
                                Address = reader["Address"].ToString(),
                                Age = Convert.ToInt32(reader["Age"]),
                                Position = reader["Position"].ToString(),
                                PartyList = reader["PartyList"].ToString(),
                                PictureUrl = reader["PictureUrl"].ToString()
                            });
                        }
                    }
                }
            }

            return candidates;
        }

        public static bool HasUserVoted(int userId, int electionId)
        {
            using (var con = GetConnection())
            {
                string query = "SELECT COUNT(*) FROM Votes WHERE UserId = @UserId AND ElectionId = @ElectionId";
                using (var cmd = new MySqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@UserId", userId);
                    cmd.Parameters.AddWithValue("@ElectionId", electionId);

                    con.Open();
                    int count = Convert.ToInt32(cmd.ExecuteScalar());
                    return count > 0; 
                }
            }
        }

        public static void RecordVote(Vote vote)
        {
            try
            {
                Console.WriteLine($"Attempting to insert vote: UserId={vote.UserId}, ElectionId={vote.ElectionId}, CandidateId={vote.CandidateId}, Position={vote.Position}");

                using (var con = GetConnection())
                {
                    string query = "INSERT INTO Votes (UserId, ElectionId, CandidateId, Position) VALUES (@UserId, @ElectionId, @CandidateId, @Position)";
                    using (var cmd = new MySqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@UserId", vote.UserId);
                        cmd.Parameters.AddWithValue("@ElectionId", vote.ElectionId);
                        cmd.Parameters.AddWithValue("@CandidateId", vote.CandidateId);
                        cmd.Parameters.AddWithValue("@Position", vote.Position);

                        con.Open();
                        cmd.ExecuteNonQuery();
                        Console.WriteLine("Vote successfully recorded in the database.");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during RecordVote: {ex.Message}");
                throw;
            }
        }


        public static int ValidateUser(string username, string password)
        {
            using (var con = GetConnection())
            {
                string query = "SELECT Id, PasswordHash FROM Users WHERE UserName = @Username";
                using (var cmd = new MySqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@Username", username);

                    con.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            int userId = Convert.ToInt32(reader["Id"]);
                            string storedPasswordHash = reader["PasswordHash"].ToString();

                            // Verify the provided password against the stored hash
                            if (BCrypt.Net.BCrypt.Verify(password, storedPasswordHash))
                            {
                                return userId; 
                            }
                        }
                    }
                }
            }

            return 0; 
        }



        public static int GetUserIdByUsername(string username)
        {
            using (var con = GetConnection())
            {
                string query = "SELECT Id FROM Users WHERE Username = @Username";
                using (var cmd = new MySqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@Username", username);
                    con.Open();
                    var result = cmd.ExecuteScalar();
                    Console.WriteLine($"[DbHelper] Querying UserId for Username: {username}, Result: {result}");
                    return result != null ? Convert.ToInt32(result) : 0;
                }
            }
        }


        public static string GetElectionTitle(int electionId)
        {
            using (var con = GetConnection())
            {
                string query = "SELECT Title FROM Elections WHERE ElectionId = @ElectionId";
                using (var cmd = new MySqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@ElectionId", electionId);

                    con.Open();
                    return cmd.ExecuteScalar()?.ToString();
                }
            }
        }

        //public static List<CandidateResult> GetElectionResults(int electionId)
        //{
        //    var results = new List<CandidateResult>();

        //    try
        //    {
        //        using (var con = GetConnection())
        //        {
        //            string query = @"
        //              SELECT c.Id, c.Name, c.Position, c.PartyList, c.PictureUrl, COUNT(v.Id) AS VoteCount
        //              FROM Candidates c
        //              LEFT JOIN Votes v ON c.Id = v.CandidateId
        //              WHERE c.ElectionId = @ElectionId
        //              GROUP BY c.Id, c.Name, c.Position, c.PartyList, c.PictureUrl
        //              ORDER BY c.Position, VoteCount DESC";

        //            using (var cmd = new MySqlCommand(query, con))
        //            {
        //                // Ensure the parameter is added correctly
        //                cmd.Parameters.AddWithValue("@ElectionId", electionId);

        //                // Open the connection
        //                con.Open();

        //                // Execute the query and read the results
        //                using (var reader = cmd.ExecuteReader())
        //                {
        //                    while (reader.Read())
        //                    {
        //                        results.Add(new CandidateResult
        //                        {
        //                            CandidateId = reader.GetInt32(reader.GetOrdinal("Id")),
        //                            Name = reader.GetString(reader.GetOrdinal("Name")),
        //                            Position = reader.GetString(reader.GetOrdinal("Position")),
        //                            PartyList = reader.GetString(reader.GetOrdinal("PartyList")),
        //                            PictureUrl = reader.GetString(reader.GetOrdinal("PictureUrl")),
        //                            VoteCount = reader.GetInt32(reader.GetOrdinal("VoteCount"))
        //                        });
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine($"Error while fetching election results: {ex.Message}");
        //        // Optionally, rethrow or handle the error appropriately
        //    }

        //    return results;
        //}

        public static List<Candidate> GetUserVotedCandidates(int userId, int electionId)
        {
            var candidates = new List<Candidate>();

            using (var con = GetConnection())
            {
                string query = @"
                  SELECT c.Id, c.Name, c.Position, c.PartyList, c.PictureUrl
                  FROM Votes v
                  INNER JOIN Candidates c ON v.CandidateId = c.Id
                  WHERE v.UserId = @UserId AND v.ElectionId = @ElectionId";

                using (var cmd = new MySqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@UserId", userId);
                    cmd.Parameters.AddWithValue("@ElectionId", electionId);

                    con.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            candidates.Add(new Candidate
                            {
                                Id = Convert.ToInt32(reader["Id"]),
                                Name = reader["Name"].ToString(),
                                Position = reader["Position"].ToString(),
                                PartyList = reader["PartyList"].ToString(),
                                PictureUrl = reader["PictureUrl"].ToString()
                            });
                        }
                    }
                }
            }

            return candidates;
        }


        public static List<Candidate> GetVotedCandidates(int electionId, int userId)
        {
            var candidates = new List<Candidate>();

            using (var con = GetConnection())
            {
                string query = @"
                  SELECT c.Id, c.Name, c.Address, c.Position, c.PartyList, c.PictureUrl
                  FROM Votes v
                  INNER JOIN Candidates c ON v.CandidateId = c.Id
                  WHERE v.ElectionId = @ElectionId AND v.UserId = @UserId";

                using (var cmd = new MySqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@ElectionId", electionId);
                    cmd.Parameters.AddWithValue("@UserId", userId);

                    con.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            candidates.Add(new Candidate
                            {
                                Id = Convert.ToInt32(reader["Id"]),
                                Name = reader["Name"].ToString(),
                                Address = reader["Address"].ToString(),
                                Position = reader["Position"].ToString(),
                                PartyList = reader["PartyList"].ToString(),
                                PictureUrl = reader["PictureUrl"].ToString()
                            });
                        }
                    }
                }
            }

            return candidates;
        }

        public static List<ElectionResults> GetElectionResults()
        {
            var results = new List<ElectionResults>();

            try
            {
                using (var con = GetConnection())
                {
                    string query = @"
                 SELECT 
                    Votes.ElectionId, 
                    Votes.CandidateId, 
                    Candidates.Name AS CandidateName, 
                    COUNT(Votes.Id) AS VoteCount
                 FROM 
                    Votes
                 JOIN 
                    Candidates ON Votes.CandidateId = Candidates.Id
                 GROUP BY 
                    Votes.ElectionId, Votes.CandidateId, Candidates.Name
                 ORDER BY 
                    Votes.ElectionId, VoteCount DESC;
            ";

                    using (var cmd = new MySqlCommand(query, con))
                    {
                        con.Open();
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                results.Add(new ElectionResults
                                {
                                    ElectionId = reader.GetInt32("ElectionId"),
                                    CandidateId = reader.GetInt32("CandidateId"),
                                    CandidateName = reader.GetString("CandidateName"),
                                    VoteCount = reader.GetInt32("VoteCount")
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetElectionResults: {ex.Message}");
            }

            return results;
        }


        public static List<VoteReceipt> GetVoteReceipt(int userId, int electionId)
        {
            var receipt = new List<VoteReceipt>();
            using (var con = GetConnection())
            {
                string query = @"
            SELECT Candidates.Name AS CandidateName, Votes.Position
            FROM Votes
            JOIN Candidates ON Votes.CandidateId = Candidates.Id
            WHERE Votes.UserId = @UserId AND Votes.ElectionId = @ElectionId";

                using (var cmd = new MySqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@UserId", userId);
                    cmd.Parameters.AddWithValue("@ElectionId", electionId);

                    con.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            receipt.Add(new VoteReceipt
                            {
                                CandidateName = reader.GetString("CandidateName"),
                                Position = reader.GetString("Position")
                            });
                        }
                    }
                }
            }
            return receipt;
        }



    }
}

