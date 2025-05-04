using MySql.Data.MySqlClient;
using static votingsystem.Pages.RegistrationModel;
using static votingsystem.Pages.Shared.ADManageElectionsModel;
using static votingsystem.Pages.Shared.ADManageVotersModel;
using static votingsystem.Pages.Shared.VotingPageModel;
using static votingsystem.Pages.Shared.UPDoneVotingModel;
using static votingsystem.Pages.Shared.UPResultsModel;
using static votingsystem.Pages.Shared.ADManageCandidatesModel;
using static votingsystem.Pages.Shared.ADVotesRecordModel;
using static votingsystem.Pages.Shared.ADElectionsDataModel;
using static votingsystem.Pages.Shared.UserPageModel;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using Microsoft.EntityFrameworkCore.Storage;
using votingsystem.Pages.Shared;
namespace votingsystem.Database_Helper
{
    public class DbHelper
    {
        //connection string
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

        // registration page, add creating an account
        public static bool RegisterUser(User user)
        {
            using (var con = GetConnection())
            {
                string query = @"INSERT INTO Users (FirstName, LastName, Email, Address, PasswordHash, Month, Day, Year, UserName, Age, Department, Program, PhotoPath, IsApproved)
                         VALUES (@FirstName, @LastName, @Email, @Address, @PasswordHash, @Month, @Day, @Year, @UserName, @Age, @Department, @Program, @PhotoPath, 0)";

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
                    cmd.Parameters.AddWithValue("@Department", user.Department);
                    cmd.Parameters.AddWithValue("@Program", user.Program);
                    cmd.Parameters.AddWithValue("@PhotoPath", user.PhotoPath);

                    try
                    {
                        con.Open();
                        Console.WriteLine("Database connection opened.");
                        cmd.ExecuteNonQuery();
                        Console.WriteLine("SQL query executed successfully.");
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
                        Console.WriteLine("Database connection closed.");
                    }
                }
            }
        }

        //admin side, approve the user in order to get access
        public static bool ApproveVoter(int voterId)
        {
            using (var con = GetConnection())
            {
                string query = "UPDATE Users SET IsApproved = 1 WHERE Id = @Id";
                using (var cmd = new MySqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@Id", voterId);
                    con.Open();
                    int rowsAffected = cmd.ExecuteNonQuery();
                    return rowsAffected > 0; 
                }
            }
        }

        //rejecting the user will delete the account
        public static bool RejectVoter(int voterId)
        {
            using (var con = GetConnection())
            {
                string query = "DELETE FROM Users WHERE Id = @Id";
                using (var cmd = new MySqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@Id", voterId);
                    con.Open();
                    int rowsAffected = cmd.ExecuteNonQuery();
                    return rowsAffected > 0; 
                }
            }
        }

        // verify user info
        public static bool VerifyUser(string UserName, string Password)
        {
            using (var con = GetConnection())
            {
                string query = "SELECT PasswordHash, IsApproved FROM Users WHERE UserName = @username";

                using (var cmd = new MySqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@username", UserName);

                    try
                    {
                        Console.WriteLine("Opening database connection...");
                        con.Open();

                        Console.WriteLine("Executing query...");
                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                string storedPasswordHash = reader.GetString("PasswordHash");
                                bool isApproved = reader.GetBoolean("IsApproved");

                                if (!isApproved)
                                {
                                    Console.WriteLine($"Login blocked: User {UserName} is not approved.");
                                    return false; 
                                }

                                // verify password against the stored hash
                                bool isPasswordValid = BCrypt.Net.BCrypt.Verify(Password, storedPasswordHash);
                                Console.WriteLine($"Password verification result: {isPasswordValid}");

                                return isPasswordValid; 
                            }
                            else
                            {
                                Console.WriteLine("User not found.");
                                return false;
                            }
                        }
                    }
                    catch (MySqlException ex)
                    {
                        Console.WriteLine($"Database error: {ex.Message}");
                        return false; 
                    }
                    finally
                    {
                        Console.WriteLine("Closing database connection...");
                        con.Close();
                    }
                }
            }
        }

        //will check if the user loggin in is approved
        public static bool IsUserApproved(string UserName)
        {
            using (var con = GetConnection())
            {
                string query = "SELECT IsApproved FROM Users WHERE UserName = @username";

                using (var cmd = new MySqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@username", UserName);

                    try
                    {
                        Console.WriteLine("Opening database connection...");
                        con.Open();

                        Console.WriteLine("Executing query...");
                        var result = cmd.ExecuteScalar();
                        return result != null && Convert.ToBoolean(result); 
                    }
                    catch (MySqlException ex)
                    {
                        Console.WriteLine($"Database error: {ex.Message}");
                        return false; 
                    }
                    finally
                    {
                        Console.WriteLine("Closing database connection...");
                        con.Close();
                    }
                }
            }
        }

        // in order to prevent email duplication in registration
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

        // the reset token when using the forgot password
        public static void SaveResetToken(string email, string token)
        {
            using (var con = GetConnection())
            {
                string query = "UPDATE Users SET ResetToken = @Token, TokenExpiry = @Expiry WHERE Email = @Email";
                using (var cmd = new MySqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@Token", token);
                    cmd.Parameters.AddWithValue("@Expiry", DateTime.Now.AddHours(1)); //1hr limit
                    cmd.Parameters.AddWithValue("@Email", email);
                    con.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        //will check if the token is valid
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

        //will set the new passwordhash for the user
        public static bool ResetPassword(string token, string password)
        {
            using (var con = GetConnection())
            {
                string query = "UPDATE Users SET PasswordHash = @Password, ResetToken = NULL, TokenExpiry = NULL WHERE ResetToken = @Token";
                using (var cmd = new MySqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@Password", BCrypt.Net.BCrypt.HashPassword(password)); // will hash the password
                    cmd.Parameters.AddWithValue("@Token", token);
                    con.Open();
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        // will check the role of the user in the log in, admin dashboard if admin and userpage if user
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

        //will fetch all the info of the voters to be able to be displayed in the manage voters page
        public static List<Voter> GetVoters()
        {
            var voters = new List<Voter>();

            using (var con = GetConnection())
            {
                con.Open();
                string query = "SELECT Id, FirstName, LastName, Email, Address, PasswordHash, Month, Day, Year, UserName, Age, Role, Department, Program, IsApproved, PhotoPath FROM Users";

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
                                Role = reader.GetString("Role"),
                                Department = reader.GetString("Department"),
                                Program = reader.GetString("Program"),
                                IsApproved = reader.GetBoolean("IsApproved"),
                                PhotoPath = reader.IsDBNull(reader.GetOrdinal("PhotoPath")) ? null : reader.GetString("PhotoPath")
                            });
                        }
                    }
                }
            }

            return voters;
        }

        //will remove the user from the system
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

        //will get the percentage of the voters who voted
        public static List<VoterStats> GetVoterTurnout(int electionId)
        {
            var result = new List<VoterStats>();
            using (var conn = GetConnection())
            {
                conn.Open();

                
                string query = @"SELECT u.Department, u.Program,COUNT(DISTINCT u.Id) AS TotalVoters, COUNT(DISTINCT v.UserId) AS UsersVoted
                    FROM Users u
                    LEFT JOIN Votes v ON u.Id = v.UserId AND v.ElectionId = @electionId
                    GROUP BY u.Department, u.Program
                    ORDER BY u.Department, u.Program;";
                using (var cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@electionId", electionId);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int totalVoters = reader.GetInt32("TotalVoters");
                            int usersVoted = reader.GetInt32("UsersVoted");

                            result.Add(new VoterStats
                            {
                                Department = reader.GetString("Department"),
                                Program = reader.GetString("Program"),
                                TotalVoters = totalVoters,
                                UsersVoted = usersVoted,
                                VotePercentage = totalVoters == 0 ? 0 : Math.Round((double)usersVoted / totalVoters * 100, 2)
                            });
                        }
                    }
                }

                conn.Close();
            }
            return result;
        }

        //for the distribution of votes per candidate in the election
        public static List<VoteDistribution> GetVoteDistribution(int electionId)
        {
            var result = new List<VoteDistribution>();
            using (var conn = GetConnection())
            {
                conn.Open();
                string query = @"SELECT c.Position AS Position,c.Name AS CandidateName,COUNT(v.UserId) AS VoteCount
                    FROM Votes v
                    LEFT JOIN Candidates c ON (
                    v.PresidentCandidateId = c.Id AND c.Position = 'President' OR
                    v.VicePresidentCandidateId = c.Id AND c.Position = 'Vice President' OR
                    v.SecretaryCandidateId = c.Id AND c.Position = 'Secretary' OR
                    v.TreasurerCandidateId = c.Id AND c.Position = 'Treasurer' OR
                    v.AuditorCandidateId = c.Id AND c.Position = 'Auditor' OR
                    v.PROCandidateId = c.Id AND c.Position = 'PRO')
                    WHERE v.ElectionId = @electionId
                    GROUP BY c.Position, c.Name
                    ORDER BY c.Position, VoteCount DESC;";
                using (var cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@electionId", electionId);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var position = reader.GetString("Position");
                            var candidateName = reader.GetString("CandidateName");
                            var voteCount = reader.GetInt32("VoteCount");

                            result.Add(new VoteDistribution
                            {
                                Position = position,
                                CandidateName = candidateName,
                                VoteCount = voteCount
                            });
                        }
                    }
                }

                conn.Close();
            }
            // will calculate the percentage for each candidate depending on the total votes for each position
            var totalVotesByPosition = result
                .GroupBy(v => v.Position)
                .ToDictionary(g => g.Key, g => g.Sum(v => v.VoteCount));

            foreach (var voteDistribution in result)
            {
                voteDistribution.VotePercentage = totalVotesByPosition[voteDistribution.Position] == 0
                    ? 0
                    : Math.Round((double)voteDistribution.VoteCount / totalVotesByPosition[voteDistribution.Position] * 100, 2);
            }

            return result;
        }

        //fetches the record of the votes of the users
        public static List<VoteHistory> GetVoteHistories()
        {
            var histories = new List<VoteHistory>();

            using (var con = GetConnection())
            {
                string query = @"
            SELECT DISTINCT
                v.UserId, 
                v.ElectionId, 
                MAX(v.Id) AS VoteId, 
                u.UserName
            FROM Votes v
            JOIN Users u ON v.UserId = u.Id
            GROUP BY v.UserId, v.ElectionId, u.UserName
            ORDER BY VoteId DESC";

                using (var cmd = new MySqlCommand(query, con))
                {
                    con.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            histories.Add(new VoteHistory
                            {
                                VoteId = reader.GetInt32("VoteId"),
                                UserId = reader.GetInt32("UserId"),
                                ElectionId = reader.GetInt32("ElectionId"),
                                UserName = reader.GetString("UserName")
                            });
                        }
                    }
                }
            }

            return histories;
        }

        //will fetch the ballot of the user, will show the candidates he voted for
        public static VoteDetails GetVoteDetails(int userId, int electionId)
        {
            var voteDetails = new VoteDetails();

            using (var con = GetConnection())
            {
                string query = @"
            SELECT 
                C.Id AS CandidateId, 
                C.Name AS CandidateName,
                CASE 
                    WHEN v.PresidentCandidateId = C.Id THEN 'President'
                    WHEN v.VicePresidentCandidateId = C.Id THEN 'Vice President'
                    WHEN v.SecretaryCandidateId = C.Id THEN 'Secretary'
                    WHEN v.TreasurerCandidateId = C.Id THEN 'Treasurer'
                    WHEN v.AuditorCandidateId = C.Id THEN 'Auditor'
                    WHEN v.PROCandidateId = C.Id THEN 'PRO'
                    ELSE 'Unknown'
                END AS Position
            FROM Votes v
            LEFT JOIN Candidates C ON C.Id IN (
                v.PresidentCandidateId, v.VicePresidentCandidateId, 
                v.SecretaryCandidateId, v.TreasurerCandidateId, 
                v.AuditorCandidateId, v.PROCandidateId
            )
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
                            voteDetails.Selections.Add(new VoteSelection
                            {
                                CandidateId = reader.GetInt32("CandidateId"),
                                CandidateName = reader.GetString("CandidateName"),
                                Position = reader.GetString("Position")
                            });
                        }
                    }
                }
            }

            return voteDetails;
        }


        //will fetch the ballot of the user, will show the candidates he voted for
        public static VoteDetails GetVoteDetails(int voteId)
        {
            var details = new VoteDetails { Selections = new List<VoteSelection>() };

            using (var con = GetConnection())
            {
                string query = @"
            SELECT v.PresidentCandidateId, cp.Name AS PresidentName,
                   v.VicePresidentCandidateId, cvp.Name AS VicePresidentName,
                   v.SecretaryCandidateId, cs.Name AS SecretaryName,
                   v.TreasurerCandidateId, ct.Name AS TreasurerName,
                   v.AuditorCandidateId, ca.Name AS AuditorName,
                   v.PROCandidateId, cpro.Name AS PROName
            FROM Votes v
            LEFT JOIN Candidates cp ON v.PresidentCandidateId = cp.Id
            LEFT JOIN Candidates cvp ON v.VicePresidentCandidateId = cvp.Id
            LEFT JOIN Candidates cs ON v.SecretaryCandidateId = cs.Id
            LEFT JOIN Candidates ct ON v.TreasurerCandidateId = ct.Id
            LEFT JOIN Candidates ca ON v.AuditorCandidateId = ca.Id
            LEFT JOIN Candidates cpro ON v.PROCandidateId = cpro.Id
            WHERE v.Id = @voteId";

                using (var cmd = new MySqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@voteId", voteId);
                    con.Open();

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            details.Selections.Add(new VoteSelection
                            {
                                CandidateName = reader.GetString("PresidentName"), 
                                Position = "President" 
                            });                           
                        }
                    }
                }
            }

            return details;
        }

        //creates an election
        public static void CreateElection(Election election)
        {
            using (var con = GetConnection())
            {
                string query = "INSERT INTO Elections (Title, Description, Start_time, End_time, Department, Program) VALUES (@Title, @Description, @StartTime, @EndTime, @Department, @Program)";

                using (var cmd = new MySqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@Title", election.Title);
                    cmd.Parameters.AddWithValue("@Description", election.Description);
                    cmd.Parameters.AddWithValue("@StartTime", election.StartTime);
                    cmd.Parameters.AddWithValue("@EndTime", election.EndTime);
                    cmd.Parameters.AddWithValue("@Department", election.Department);
                    cmd.Parameters.AddWithValue("@Program", string.IsNullOrEmpty(election.Program) ? DBNull.Value : election.Program);


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

        //fetches the elections, in order to display the list 
        public static List<Election> GetElections()
        {
            var elections = new List<Election>();

            try
            {
                using (var con = GetConnection())
                {
                    con.Open();
                    string query = "SELECT ElectionId, Title, description, Start_time, End_time, Department, Program FROM Elections";

                    using (var command = new MySqlCommand(query, con))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var election = new Election
                                {
                                    ElectionId = reader.GetInt32("ElectionId"),
                                    Title = reader.GetString("Title"),
                                    Description = reader.IsDBNull(reader.GetOrdinal("description"))? null : reader.GetString("description"),
                                    StartTime = reader.GetDateTime("Start_time"),
                                    EndTime = reader.GetDateTime("End_time"),
                                    Department = reader.IsDBNull(reader.GetOrdinal("Department"))? null : reader.GetString("Department"),
                                    Program = reader.IsDBNull(reader.GetOrdinal("Program"))? null : reader.GetString("Program")
                                };
                                Console.WriteLine($"Fetched Election: Id={election.ElectionId}, Title={election.Title}, Department={election.Department}, Program={election.Program}");

                                elections.Add(election);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching elections: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
            }

            return elections;
        }

        //checks the title of created elections to prevent duplicatioin
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

        // fetches the details of the election, will be displayed in the manage elections page
        public static Election GetElectionById(int id)
        {
            Election election = null;

            using (var con = GetConnection())
            {
                string query = "SELECT ElectionId, Title, description, Start_time, End_time, Department, Program FROM Elections WHERE ElectionId = @ElectionId";

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
                                    Description = reader.IsDBNull(reader.GetOrdinal("description")) ? null : reader.GetString("description"),
                                    StartTime = reader.GetDateTime("Start_time"),
                                    EndTime = reader.GetDateTime("End_time"),
                                    Department = reader.IsDBNull(reader.GetOrdinal("Department")) ? null : reader.GetString("Department"),
                                    Program = reader.IsDBNull(reader.GetOrdinal("Program")) ? null : reader.GetString("Program")
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

        //will update the details of the election if edited
        public static bool UpdateElection(Election election)
        {
            using (var con = GetConnection())
            {
                string query = @"UPDATE Elections 
                         SET Title = @Title, 
                             description = @description, 
                             Start_time = @StartTime, 
                             End_time = @EndTime,
                             Department = @Department,
                             Program = @Program
                         WHERE ElectionId = @Id";

                using (var cmd = new MySqlCommand(query, con))
                {

                    cmd.Parameters.AddWithValue("@Id", election.ElectionId);
                    cmd.Parameters.AddWithValue("@Title", election.Title);
                    cmd.Parameters.AddWithValue("@description", string.IsNullOrEmpty(election.Description) ? DBNull.Value : election.Description);
                    cmd.Parameters.AddWithValue("@StartTime", election.StartTime);
                    cmd.Parameters.AddWithValue("@EndTime", election.EndTime);
                    cmd.Parameters.AddWithValue("@Department", string.IsNullOrEmpty(election.Department) ? DBNull.Value : election.Department);
                    cmd.Parameters.AddWithValue("@Program", string.IsNullOrEmpty(election.Program) ? DBNull.Value : election.Program);

                    try
                    {
                        con.Open();
                        int rowsAffected = cmd.ExecuteNonQuery();
                        if (rowsAffected == 0)
                        {
                            Console.WriteLine($"No rows updated for ElectionId {election.ElectionId}.");
                            return false;
                        }
                        Console.WriteLine($"Successfully updated ElectionId {election.ElectionId}: Department={election.Department}, Program={election.Program}");
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

        //deletes the election
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

        //will fetch the candidates, in order to display list in the manage candidates page
        public static List<Candidate> GetCandidates()
        {
            var candidates = new List<Candidate>();

            using (var con = GetConnection())
            {
                string query = "SELECT Id, Name, Age, Address, Position, PartyList, ElectionId, Department, Program, PictureUrl FROM Candidates";

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
                                    ElectionId = reader.IsDBNull(reader.GetOrdinal("ElectionId")) ? 0 : reader.GetInt32("ElectionId"),
                                    Department = reader.IsDBNull(reader.GetOrdinal("Department")) ? null : reader.GetString("Department"),
                                    Program = reader.IsDBNull(reader.GetOrdinal("Program")) ? null : reader.GetString("Program"),
                                    PictureUrl = reader.IsDBNull(reader.GetOrdinal("PictureUrl")) ? null : reader.GetString("PictureUrl")
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

        //creates a candidate
        public static bool CreateCandidate(Candidate candidate)
        {
            using (var con = GetConnection())
            {
                string query = @"INSERT INTO Candidates (Name, Age, Address, Position, PartyList, ElectionId, PictureUrl, Department, Program)
                         VALUES (@Name, @Age, @Address, @Position, @PartyList, @ElectionId, @PictureUrl, @Department, @Program)";

                using (var cmd = new MySqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@Name", candidate.Name);
                    cmd.Parameters.AddWithValue("@Age", candidate.Age);
                    cmd.Parameters.AddWithValue("@Address", candidate.Address);
                    cmd.Parameters.AddWithValue("@Position", candidate.Position);
                    cmd.Parameters.AddWithValue("@PartyList", candidate.PartyList);
                    cmd.Parameters.AddWithValue("@ElectionId", candidate.ElectionId);
                    cmd.Parameters.AddWithValue("@PictureUrl", candidate.PictureUrl);
                    cmd.Parameters.AddWithValue("@Department", candidate.Department);
                    cmd.Parameters.AddWithValue("@Program", string.IsNullOrEmpty(candidate.Program) ? DBNull.Value : candidate.Program);

                    try
                    {
                        con.Open();
                        Console.WriteLine("Database connection opened.");
                        cmd.ExecuteNonQuery();
                        Console.WriteLine("SQL query executed successfully.");
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
                        Console.WriteLine("Database connection closed.");
                    }
                }
            }
        }

        //checks if a candidate is already registered to avoid duplication
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

        //deletes a candidate
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
                        Console.WriteLine("Database connection closed.");
                    }
                }
            }
        }

        //gets the total number of elections
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

        //gets the total number of voters
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

        //gets the total number of votes 
        public static int GetTotalVotes()
        {
            using (var con = GetConnection())
            {
                string query = "SELECT COUNT(*) FROM Votes";

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

        //gets the number of users who are waiting for approval
        public static int GetPendingRegistrations()
        {
            int pendingCount = 0;

            using (var con = GetConnection())
            {
                string query = @"
            SELECT COUNT(*) FROM Users WHERE IsApproved = 0";

                using (var cmd = new MySqlCommand(query, con))
                {
                    con.Open();
                    pendingCount = Convert.ToInt32(cmd.ExecuteScalar());
                }
            }

            return pendingCount;
        }


        // in order to fetch the id,department, and program of the user for log in
        public static User GetUserDetailsByUsername(string username)
        {
            using (var con = GetConnection())
            {
                string query = "SELECT Id, Department, Program FROM Users WHERE UserName = @username";

                using (var cmd = new MySqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@username", username);

                    try
                    {
                        Console.WriteLine("Opening database connection...");
                        con.Open();

                        Console.WriteLine("Executing query...");
                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return new User
                                {
                                    Id = reader.GetInt32("Id"),
                                    Department = reader.GetString("Department"),
                                    Program = reader.GetString("Program")
                                };
                            }
                        }
                    }
                    catch (MySqlException ex)
                    {
                        Console.WriteLine($"Database error: {ex.Message}");
                    }
                    finally
                    {
                        Console.WriteLine("Closing database connection...");
                        con.Close();
                    }

                    return null; 
                }
            }
        }

        //fetches all the available elections
        public static List<Election> GetAvailableElections(string department, string program)
        {
            var elections = new List<Election>();

            try
            {
                using (var con = GetConnection())
                {
                    string query = @"
                        SELECT ElectionId, Title, Description, Start_time, End_time, Department, Program
                        FROM Elections 
                        WHERE End_time > NOW() 
                        AND Start_time <= NOW() 
                        AND (Department = @department OR Department = 'ALL')
                        AND (Program = @program OR Program IS NULL)";

                    using (var cmd = new MySqlCommand(query, con))
                    {
                        // Add parameters for filtering by department and program
                        cmd.Parameters.AddWithValue("@department", department);
                        cmd.Parameters.AddWithValue("@program", program);

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
                                    Department = reader["Department"].ToString(),
                                    Program = reader.IsDBNull(reader.GetOrdinal("Program")) ? null : reader["Program"].ToString(),
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

        //fetches all the available elections
        public static List<Election> GetAvailableElections()
        {
            var elections = new List<Election>();

            try
            {
                using (var con = GetConnection())
                {
                    string query = @"
                SELECT ElectionId, Title, Description, Start_time, End_time, Department, Program
                FROM Elections 
                WHERE End_time > NOW() 
                AND Start_time <= NOW()";

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
                                    Department = reader["Department"].ToString(),
                                    Program = reader.IsDBNull(reader.GetOrdinal("Program")) ? null : reader["Program"].ToString(),
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


        //fetches all the upcoming elections
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

        //fetches all upcoming elections that has department and program as arguments
        public static List<Election> GetUpcomingElectionsByUser(string department, string program)
        {
            var elections = new List<Election>();

            try
            {
                using (var con = GetConnection())
                {
                    string query = @"
                        SELECT ElectionId, Title, Description, Start_time, End_time, Department, Program
                        FROM Elections 
                        WHERE Start_time > NOW()
                        AND (Department = @department OR Department = 'ALL')
                        AND (Program = @program OR Program IS NULL)";

                    using (var cmd = new MySqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@department", department);
                        cmd.Parameters.AddWithValue("@program", program);

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
                                    Department = reader["Department"].ToString(),
                                    Program = reader.IsDBNull(reader.GetOrdinal("Program")) ? null : reader["Program"].ToString(),
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



        //fetches the candidates of an election
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

        //checks if the user casted a vote already
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

        //records the data of the vote of the user
        public static void RecordVote(Vote vote)
        {
            try
            {
                Console.WriteLine($"Attempting to insert vote: UserId={vote.UserId}, ElectionId={vote.ElectionId}");

                using (var con = GetConnection())
                {
                    string query = @"
                        INSERT INTO Votes (UserId, ElectionId, PresidentCandidateId, VicePresidentCandidateId, SecretaryCandidateId, TreasurerCandidateId, AuditorCandidateId, PROCandidateId) 
                        VALUES (@UserId, @ElectionId, @President, @VicePresident, @Secretary, @Treasurer, @Auditor, @PRO)
                        ON DUPLICATE KEY UPDATE
                        PresidentCandidateId = @President, 
                        VicePresidentCandidateId = @VicePresident, 
                        SecretaryCandidateId = @Secretary, 
                        TreasurerCandidateId = @Treasurer, 
                        AuditorCandidateId = @Auditor"; 
        
            using (var cmd = new MySqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@UserId", vote.UserId);
                        cmd.Parameters.AddWithValue("@ElectionId", vote.ElectionId);
                        cmd.Parameters.AddWithValue("@President", vote.PresidentCandidateId);
                        cmd.Parameters.AddWithValue("@VicePresident", vote.VicePresidentCandidateId);
                        cmd.Parameters.AddWithValue("@Secretary", vote.SecretaryCandidateId);
                        cmd.Parameters.AddWithValue("@Treasurer", vote.TreasurerCandidateId);
                        cmd.Parameters.AddWithValue("@Auditor", vote.AuditorCandidateId);
                        cmd.Parameters.AddWithValue("@PRO", vote.PROCandidateId);

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

        //validates if the details of the user is available in the database
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

        //fetches the details of the user
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

        //fetches the title of the election
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

        //fetches the voted candidates of the user to be able to show the voting receipt
        public static List<Candidate> GetUserVotedCandidates(int userId, int electionId)
        {
            var candidates = new List<Candidate>();

            using (var con = GetConnection())
            {
                string query = @"SELECT 
                C1.Id AS PresidentId, C1.Name AS PresidentName, C1.Position AS PresidentPosition, C1.PartyList AS PresidentPartyList, C1.PictureUrl AS PresidentPictureUrl,
                C2.Id AS VicePresidentId, C2.Name AS VicePresidentName, C2.Position AS VicePresidentPosition, C2.PartyList AS VicePresidentPartyList, C2.PictureUrl AS VicePresidentPictureUrl,
                C3.Id AS SecretaryId, C3.Name AS SecretaryName, C3.Position AS SecretaryPosition, C3.PartyList AS SecretaryPartyList, C3.PictureUrl AS SecretaryPictureUrl,
                C4.Id AS TreasurerId, C4.Name AS TreasurerName, C4.Position AS TreasurerPosition, C4.PartyList AS TreasurerPartyList, C4.PictureUrl AS TreasurerPictureUrl,
                C5.Id AS AuditorId, C5.Name AS AuditorName, C5.Position AS AuditorPosition, C5.PartyList AS AuditorPartyList, C5.PictureUrl AS AuditorPictureUrl,
                C6.Id AS PROId, C6.Name AS PROName, C6.Position AS PROPosition, C6.PartyList AS PROPartyList, C6.PictureUrl AS PROPictureUrl
                FROM Votes v
                LEFT JOIN Candidates C1 ON v.PresidentCandidateId = C1.Id
                LEFT JOIN Candidates C2 ON v.VicePresidentCandidateId = C2.Id
                LEFT JOIN Candidates C3 ON v.SecretaryCandidateId = C3.Id
                LEFT JOIN Candidates C4 ON v.TreasurerCandidateId = C4.Id
                LEFT JOIN Candidates C5 ON v.AuditorCandidateId = C5.Id
                LEFT JOIN Candidates C6 ON v.PROCandidateId = C6.Id
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
                            if (reader["PresidentId"] != DBNull.Value)
                                candidates.Add(new Candidate
                                {
                                    Id = Convert.ToInt32(reader["PresidentId"]),
                                    Name = reader["PresidentName"].ToString(),
                                    Position = reader["PresidentPosition"].ToString(),
                                    PartyList = reader["PresidentPartyList"].ToString(),
                                    PictureUrl = reader["PresidentPictureUrl"].ToString()
                                });

                            if (reader["VicePresidentId"] != DBNull.Value)
                                candidates.Add(new Candidate
                                {
                                    Id = Convert.ToInt32(reader["VicePresidentId"]),
                                    Name = reader["VicePresidentName"].ToString(),
                                    Position = reader["VicePresidentPosition"].ToString(),
                                    PartyList = reader["VicePresidentPartyList"].ToString(),
                                    PictureUrl = reader["VicePresidentPictureUrl"].ToString()
                                });

                            if (reader["SecretaryId"] != DBNull.Value)
                                candidates.Add(new Candidate
                                {
                                    Id = Convert.ToInt32(reader["SecretaryId"]),
                                    Name = reader["SecretaryName"].ToString(),
                                    Position = reader["SecretaryPosition"].ToString(),
                                    PartyList = reader["SecretaryPartyList"].ToString(),
                                    PictureUrl = reader["SecretaryPictureUrl"].ToString()
                                });

                            if (reader["TreasurerId"] != DBNull.Value)
                                candidates.Add(new Candidate
                                {
                                    Id = Convert.ToInt32(reader["TreasurerId"]),
                                    Name = reader["TreasurerName"].ToString(),
                                    Position = reader["TreasurerPosition"].ToString(),
                                    PartyList = reader["TreasurerPartyList"].ToString(),
                                    PictureUrl = reader["TreasurerPictureUrl"].ToString()
                                });

                            if (reader["AuditorId"] != DBNull.Value)
                                candidates.Add(new Candidate
                                {
                                    Id = Convert.ToInt32(reader["AuditorId"]),
                                    Name = reader["AuditorName"].ToString(),
                                    Position = reader["AuditorPosition"].ToString(),
                                    PartyList = reader["AuditorPartyList"].ToString(),
                                    PictureUrl = reader["AuditorPictureUrl"].ToString()
                                });

                            if (reader["PROId"] != DBNull.Value)
                                candidates.Add(new Candidate
                                {
                                    Id = Convert.ToInt32(reader["PROId"]),
                                    Name = reader["PROName"].ToString(),
                                    Position = reader["PROPosition"].ToString(),
                                    PartyList = reader["PROPartyList"].ToString(),
                                    PictureUrl = reader["PROPictureUrl"].ToString()
                                });
                        }
                    }
                }
            }

            return candidates;
        }

        //fetches the elections that the user has already voted
        public static List<Election> GetUserVotedElections(int userId)
        {
            var elections = new List<Election>();

            using (var con = GetConnection())
            {
                string query = @"
                    SELECT DISTINCT e.ElectionId, e.Title, e.Start_time, e.End_time
                    FROM Elections e
                    JOIN Votes v ON e.ElectionId = v.ElectionId
                    WHERE v.UserId = @UserId";
                using (var cmd = new MySqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@UserId", userId);
                    con.Open();

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            elections.Add(new Election
                            {
                                ElectionId = reader.GetInt32("ElectionId"),
                                Title = reader.GetString("Title"),
                                StartTime = reader.GetDateTime("Start_time"),
                                EndTime = reader.GetDateTime("End_time")
                            });
                        }
                    }
                }
            }

            return elections;
        }

        //fetches the results of an election
        public static List<ElectionResult> GetElectionResults()
        {
            var results = new List<ElectionResult>();

            try
            {
                using (var con = GetConnection())
                {
                    string query = @"SELECT C.Id AS CandidateId, C.Name AS CandidateName, C.Position,  -- This column exists in your table!COUNT(*) AS VoteCount
                        FROM Votes V
                        LEFT JOIN Candidates C ON C.Id IN 
                             (V.PresidentCandidateId, V.VicePresidentCandidateId, 
                             V.SecretaryCandidateId, V.TreasurerCandidateId, 
                             V.AuditorCandidateId, V.PROCandidateId)
                        GROUP BY C.Id, C.Name, C.Position
                        ORDER BY C.Position, VoteCount DESC;";
                    using (var cmd = new MySqlCommand(query, con))
                    {
                        con.Open();
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                results.Add(new ElectionResult
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


        //fetches the candidate the user voted 
        public static List<VoteReceipt> GetVoteReceipt(int userId, int electionId)
        {
            var receipt = new List<VoteReceipt>();
            using (var con = GetConnection())
            {
                string query = @"SELECT Candidates.Name AS CandidateName, Votes.Position
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

        //fetches all the elections
        public static List<Election> GetAllElections()
        {
            var elections = new List<Election>();

            using (var con = GetConnection())
            {
                string query = "SELECT ElectionId, Title FROM Elections";

                using (var cmd = new MySqlCommand(query, con))
                {
                    con.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            elections.Add(new Election
                            {
                                ElectionId = reader.GetInt32("ElectionId"),
                                Title = reader.GetString("Title"),
                            });
                        }
                    }
                }
            }
            return elections;
        }

        //fetches the results of the election, for the admin side
        public static List<ElectionResult> GetLiveElectionAdminResults(int electionId)
        {
            var results = new List<ElectionResult>();

            using (var con = GetConnection())
            {
                string query = @"SELECT C.Id AS CandidateId, C.Name AS CandidateName, C.Position, COUNT(*) AS VoteCount
                    FROM Votes V
                    LEFT JOIN Candidates C ON 
                        C.Id IN (V.PresidentCandidateId, 
                            V.VicePresidentCandidateId, 
                            V.SecretaryCandidateId, 
                            V.TreasurerCandidateId, 
                            V.AuditorCandidateId, 
                            V.PROCandidateId)
                    WHERE V.ElectionId = @ElectionId
                    GROUP BY C.Id, C.Name, C.Position
                    ORDER BY C.Position, VoteCount DESC;";
                using (var cmd = new MySqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@ElectionId", electionId);
                    con.Open();

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            results.Add(new ElectionResult
                            {
                                CandidateId = reader.GetInt32("CandidateId"),
                                CandidateName = reader.GetString("CandidateName"),
                                Position = reader.GetString("Position"),
                                VoteCount = reader.GetInt32("VoteCount")
                            });
                        }
                    }
                }
            }

            return results;
        }

        //fetches the results of the election
        public static List<ElectionResult> GetLiveElectionResults(int electionId)
        {
            var results = new List<ElectionResult>();

            using (var con = GetConnection())
            {
                string query = @"SELECT C.Id AS CandidateId, C.Name AS CandidateName, C.Position, COUNT(*) AS VoteCount
                    FROM Votes V
                    LEFT JOIN Candidates C ON 
                        C.Id IN (V.PresidentCandidateId, 
                            V.VicePresidentCandidateId, 
                            V.SecretaryCandidateId, 
                            V.TreasurerCandidateId, 
                            V.AuditorCandidateId, 
                            V.PROCandidateId)
                    WHERE V.ElectionId = @ElectionId
                    GROUP BY C.Id, C.Name, C.Position
                    ORDER BY C.Position, VoteCount DESC;";
                using (var cmd = new MySqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@ElectionId", electionId);
                    con.Open();

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            results.Add(new ElectionResult
                            {
                                CandidateId = reader.GetInt32("CandidateId"),
                                CandidateName = reader.GetString("CandidateName"),
                                Position = reader.GetString("Position"),
                                VoteCount = reader.GetInt32("VoteCount")
                            });
                        }
                    }
                }
            }

            return results;
        }
    }
}

