using HEX.HEX.Helpers;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HEX.HEX.Services
{
    public class DBEngine
    {
        private string connectionString { get; set; }

        public DBEngine(string str)
        {
            connectionString = str;
        }

        public async Task<(bool, ResponseStatus)> StoreUserAsync(User user)
        {
            var userNo = await GetTotalUsersAsync() + 1;
            if (userNo == 0) throw new Exception();

            if(!await CheckUsernameAvailabilityAsync(user.UserID))
            {
                return (false, ResponseStatus.Taken);
            }

            try
            {
                using (var conn = new NpgsqlConnection(connectionString))
                {
                    await conn.OpenAsync();

                    string query = "INSERT INTO userdata.\"users\" (userno, pfp, userid, username, password, status, about) " +
                                  $"VALUES ({userNo}, '{user.Pfp}', '{user.UserID}', '{user.Username}', '{user.Password}', '{user.Status}', '{user.About}');";


                    using (var cmd = new NpgsqlCommand(query, conn))
                    {
                        await cmd.ExecuteNonQueryAsync();
                    }
                }
                return (true, ResponseStatus.Success);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return (false, ResponseStatus.UnexpectedFailure);
            }

        }

        public async Task<(bool, User, ResponseStatus)> GetUserByAuthenticationAsync(string userID, string password)
        {
            User result = new User();

            try
            {
                using (var conn = new NpgsqlConnection(connectionString))
                {
                    await conn.OpenAsync();

                    string query = "SELECT u.userno, u.pfp, u.userid, u.username, u.password, u.status, u.about " +
                                   "FROM userdata.users u " +
                                   $"WHERE userid = '{userID}' AND password = '{password}'";

                    using (var cmd = new NpgsqlCommand(query, conn))
                    {
                        var reader = await cmd.ExecuteReaderAsync();
                        if (await reader.ReadAsync())
                        {
                            result = new User
                            {
                                UserID = reader.GetString(reader.GetOrdinal("userid")),
                                Pfp = reader.GetString(reader.GetOrdinal("pfp")),
                                Username = reader.GetString(reader.GetOrdinal("username")),
                                Password = reader.GetString(reader.GetOrdinal("password")),
                                Status = reader.GetString(reader.GetOrdinal("status")),
                                About = reader.GetString(reader.GetOrdinal("about"))
                            };

                            await reader.CloseAsync();
                        }
                        else
                        {
                            return (false, result, ResponseStatus.IncorrectCredentials);
                        }
                    }
                }

                return (true, result, ResponseStatus.Success);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                return (false, result, ResponseStatus.UnexpectedFailure);
            }
        }


        public async Task<(bool, User?)> GetUserDataAsync(string username, ulong serverID)
        {
            User result;

            try
            {
                using (var conn = new NpgsqlConnection(connectionString))
                {
                    await conn.OpenAsync();

                    string query = "SELECT u.userno, u.username, u.servername, u.serverid " +
                                  $"FROM testdata.testuserinfo u " +
                                  $"WHERE username = '{username}' AND serverid = {serverID}";

                    using (var cmd = new NpgsqlCommand(query, conn))
                    {
                        var reader = await cmd.ExecuteReaderAsync();
                        await reader.ReadAsync();

                        result = new User
                        {
                            
                        };
                    }
                }

                return (true, result);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                return (false, null);
            }

        }
        private async Task<long> GetTotalUsersAsync()
        {
            try
            {
                using (var conn = new NpgsqlConnection(connectionString))
                {
                    await conn.OpenAsync();

                    string query = "SELECT COUNT (*) FROM userdata.users";

                    using (var cmd = new NpgsqlCommand(query, conn))
                    {
                        var userCount = await cmd.ExecuteScalarAsync();
                        return Convert.ToInt64(userCount);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                return -1;
            }
        }

        public async Task<bool> CheckUsernameAvailabilityAsync(string username)
        {
            try
            {
                using (var conn = new NpgsqlConnection(connectionString))
                {
                    await conn.OpenAsync();

                    string query = $"SELECT EXISTS (SELECT 1 FROM userdata.users WHERE userid = '{username}' LIMIT 1);";

                    using (var cmd = new NpgsqlCommand(query, conn))
                    {
                        var userCount = await cmd.ExecuteScalarAsync();
                        return !Convert.ToBoolean(userCount);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                return false;
            }
        }
    }
    public class User
    {
        public string Pfp { get; set; }
        public string UserID { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string Status { get; set; }
        public string About { get; set; }
    }
}
