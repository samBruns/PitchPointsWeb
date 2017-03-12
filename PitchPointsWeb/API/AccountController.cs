﻿using PitchPointsWeb.Models;
using System.Web.Http;
using System;
using static PitchPointsWeb.API.AccountVerifier;
using System.Data.SqlClient;
using System.Data;
using PitchPointsWeb.Models.API;
using PitchPointsWeb.Models.API.Response;

namespace PitchPointsWeb.API
{
    public class AccountController : MasterController
    {

        /// <summary>
        /// Attempts to register a user from a RegisterAPIUser model and returns a HttpResponseMessage
        /// </summary>
        /// <param name="user">The user to register</param>
        /// <returns>A HttpResponseMessage based on the success / failure of the insertion</returns>
        [AllowAnonymous]
        [HttpPost]
        public PrivateApiResponse Register([FromBody] RegisterModel user)
        {
            var response = new PrivateApiResponse();
            try
            {
                using (var connection = GetConnection())
                {
                    connection.Open();
                    if (DoesUserExist(user.Email))
                    {
                        response.ApiResponseCode = ApiResponseCode.UserAlreadyExistsEmail;
                    }
                    else
                    {
                        var insertResponse = InsertUser(user, connection);
                        response.ResponseCode = insertResponse.ResponseCode;
                        if (insertResponse.Success)
                        {
                            response.PrivateKeyInfo = insertResponse.PrivateKeyInfo;
                        }
                    }
                }
            }
            catch
            {
                response.ApiResponseCode = ApiResponseCode.InternalError;
            }
            return response;
        }

        /// <summary>
        /// Attempts to 'login' to the website by using a LoginAPIUser model. A successful dictionary would contain
        /// PrivateKeyInfo. An unsuccessful dictionary would include an error pertaining to why a login was invalid.
        /// </summary>
        /// <param name="user">The user to login with</param>
        /// <returns>A JSON dictionary with a private key if the login was successful</returns>
        [AllowAnonymous]
        [HttpPost]
        public PrivateApiResponse Login([FromBody] LoginModel user)
        {
            var response = new PrivateApiResponse();
            try
            {
                var databaseUser = GetUserFrom(user.Email);
                if (databaseUser == null)
                {
                    response.ApiResponseCode = ApiResponseCode.UserDoesNotExistEmail;
                    return response;
                }
                var connection = GetConnection();
                connection.Open();
                if (databaseUser.PasswordsMatch(user.Password))
                {
                    response.ApiResponseCode = ApiResponseCode.Success;
                    response.PrivateKeyInfo = CreateAndInsertPublicKeyFrom(databaseUser.Id.Value, connection);
                }
                else
                {
                    response.ApiResponseCode = ApiResponseCode.IncorrectPassword;
                }
                connection.Close();
            }
            catch
            {
                response.ApiResponseCode = ApiResponseCode.InternalError;
            }
            return response;
        }

        /// <summary>
        /// Modifies a user based on their ID to grant / revoke admin permission(s)
        /// </summary>
        /// <param name="userId">The ID of a user that exists in the User table</param>
        /// <param name="isAdmin">Determines if this user is an admin. Set this to false to remove this user from the admin table</param>
        /// <param name="canModifyScores">True if this admin can modify climber scores in a competition</param>
        /// <param name="canCreateCompetitions">True if this admin has competition creation privileges</param>
        /// <returns>True if the modification was successful, false otherwise</returns>
        internal bool ModifyAdmin(int userId, bool isAdmin, bool canModifyScores, bool canCreateCompetitions)
        {
            try
            {
                using (var connection = GetConnection())
                {
                    connection.Open();
                    using (var command = new SqlCommand("ModifyAdminUser", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@userId", userId);
                        command.Parameters.AddWithValue("@isAdmin", isAdmin);
                        command.Parameters.AddWithValue("@canModifyScores", canModifyScores);
                        command.Parameters.AddWithValue("@canCreateCompetitions", canCreateCompetitions);
                        command.ExecuteNonQuery();
                    }
                }
            } catch
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Modifies a user based on their ID to promote / revoke judge role
        /// </summary>
        /// <param name="userId">The ID of a user that exists in the User table</param>
        /// <param name="isJudge">Determines if this user is a judge. Set this to false to remove this user from the judge table</param>
        /// <param name="compId">The ID of a competition that this user can be a judge for</param>
        /// <returns>True if the modification was successful, false otherwise</returns>
        internal bool ModifyJudge(int userId, bool isJudge, int compId)
        {
            try
            {
                using (var connection = GetConnection())
                {
                    connection.Open();
                    using (var command = new SqlCommand("ModifyJudgeUser", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@userId", userId);
                        command.Parameters.AddWithValue("@isJudge", isJudge);
                        command.Parameters.AddWithValue("@compId", compId);
                        command.ExecuteNonQuery();
                    }
                }
            } catch
            {
                return false;
            }
            return true;
        }

        [HttpPost]
        public UserSnapshotResponse GetUserSnapshot([FromBody] UserIDSignedData data)
        {
            var snapshot = new UserSnapshotResponse();
            if (!data.IsValid())
            {
                snapshot.ApiResponseCode = ApiResponseCode.AuthError;
                return snapshot;
            }
            try
            {
                using (var connection = GetConnection())
                {
                    connection.Open();
                    using (var command = new SqlCommand("GetUserSnapshot", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@userId", data.UserID);
                        using (var reader = command.ExecuteReader())
                        {
                            snapshot.Points = ReadObject(reader, "Points", 0);
                            snapshot.Falls = ReadObject(reader, "Falls", 0);
                            snapshot.ParticipatedCompetitions = ReadObject(reader, "ParticipatedCompetitions", 0);
                            if (reader.NextResult())
                            {
                                while (reader.Read())
                                {
                                    snapshot.UpcomingCompetitions.Add(CompetitionsController.ReadCompetition(reader));
                                }
                            }
                        }
                    }
                }
            }
            catch
            {
                snapshot.ApiResponseCode = ApiResponseCode.InternalError;
            }
            return snapshot;
        }

        /// <summary>
        /// Creates a user object from the database with the given email
        /// </summary>
        /// <param name="email">The email to use to query the database</param>
        /// <returns>A User if the email exists within the database, null otherwise</returns>
        /// <exception cref="SqlException">Throws if there is an error connecting to the database or any SQL related exception or warning thrown</exception>
        private static User GetUserFrom(string email)
        {
            User user;
            using (var connection = GetConnection())
            {
                connection.Open();
                using (var command = new SqlCommand("GetAllUserInfo", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@email", email);
                    using (var reader = command.ExecuteReader())
                    {
                        if (!reader.HasRows) return null;
                        reader.Read();
                        user = ReadUserFrom(reader);
                    }
                }
            }
            return user;
        }

        /// <summary>
        /// Determines if a user exists in the database based on their email
        /// </summary>
        /// <param name="email">the email to check within the database</param>
        /// <returns>True if the user exists in the database</returns>
        /// <exception cref="SqlException">Throws if there is an error connecting to the database or any SQL related exception or warning thrown</exception> 
        private static bool DoesUserExist(string email)
        {
            return GetUserFrom(email) != null;
        }

        private static InsertUserResponse InsertUser(RegisterModel model, SqlConnection connection)
        {
            var user = Models.User.CreateFrom(model);
            var response = new InsertUserResponse();
            try
            {
                if (connection.State == ConnectionState.Closed)
                {
                    connection.Open();
                }
                using (var command = new SqlCommand("CreateUser", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    var userIdParam = new SqlParameter("@UserId", SqlDbType.Int) { Direction = ParameterDirection.Output };
                    var errorCode = new SqlParameter("@ResponseCode", SqlDbType.Int) { Direction = ParameterDirection.Output };
                    command.Parameters.Add(userIdParam);
                    command.Parameters.Add(errorCode);
                    command.Parameters.AddWithValue("@email", user.Email);
                    command.Parameters.AddWithValue("@firstName", user.FirstName);
                    command.Parameters.AddWithValue("@lastName", user.LastName);
                    command.Parameters.AddWithValue("@dob", user.DateOfBirth);
                    command.Parameters.AddWithValue("@password", user.PasswordHash);
                    command.Parameters.AddWithValue("@salt", user.Salt);
                    command.ExecuteNonQuery();
                    var outErrorCode = (errorCode.Value as int?) ?? 0;
                    if (outErrorCode == 0)
                    {
                        var userId = (int)userIdParam.Value;
                        response.ApiResponseCode = ApiResponseCode.Success;
                        response.UserId = userId;
                        response.PrivateKeyInfo = CreateAndInsertPublicKeyFrom(userId, connection);
                    }
                    else
                    {
                        response.ApiResponseCode = (ApiResponseCode)outErrorCode;
                    }
                }
            } catch
            {
                response.ApiResponseCode = ApiResponseCode.InternalError;
            }
            return response;
        }

        /// <summary>
        /// Creates and inserts a Public Key / Public Key ID into the database with a given connection.
        /// The connection provided is not closed and must be closed by the invoked methods used to
        /// call this method.
        /// </summary>
        /// <param name="userId">The user ID to create a key pair for</param>
        /// <param name="connection">The pre-existing connection to use to access the database</param>
        /// <returns>A PrivateKeyInfo for the user</returns>
        private static PrivateKeyInfo CreateAndInsertPublicKeyFrom(int userId, SqlConnection connection)
        {
            var pair = GenerateKeyPair();
            int id;
            if (connection.State == ConnectionState.Closed)
            {
                connection.Open();
            }
            using (var command = new SqlCommand("InsertPublicKey", connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                var idParam = new SqlParameter("@id", SqlDbType.Int) { Direction = ParameterDirection.Output };
                command.Parameters.Add(idParam);
                command.Parameters.AddWithValue("@userId", userId);
                command.Parameters.AddWithValue("@key", pair.Item1);
                command.ExecuteNonQuery();
                id = (idParam.Value as int?) ?? 0;
            }
            return new PrivateKeyInfo
            {
                PrivateKey = Convert.ToBase64String(pair.Item2, Base64FormattingOptions.None),
                PublicKeyId = id
            };
        }

        /// <summary>
        /// Attempts to extract a user from a SqlDataReader
        /// </summary>
        /// <param name="reader">A SqlDataReader obtained from a SqlCommand execution</param>
        /// <returns>A user with all data provided in the reader</returns>
        private static User ReadUserFrom(SqlDataReader reader)
        {
            var user = new User
            {
                Id = ReadObject(reader, "UserId", 0),
                FirstName = ReadObject(reader, "FirstName", ""),
                LastName = ReadObject(reader, "LastName", ""),
                Email = ReadObject(reader, "Email", ""),
                EmailVerified = ReadObject(reader, "EmailConfirmed", (byte) 0) == (byte) 1,
                PasswordHash = ReadObject<byte[]>(reader, "PasswordHash", null),
                Salt = ReadObject<byte[]>(reader, "Salt", null)
            };
            var dob = ReadObjectOrNull<DateTime>(reader, "DOB");
            if (dob.HasValue)
            {
                user.DateOfBirth = dob.Value;
            }
            var dateRegistered = ReadObjectOrNull<DateTime>(reader, "DateRegistered");
            if (dateRegistered.HasValue)
            {
                user.DateRegistered = dateRegistered.Value;
            }
            return user;
        }

    }
}
