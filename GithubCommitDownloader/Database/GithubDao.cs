using System;
using System.Data.SQLite;
using System.IO;
using System.Data;
using GithubCommitDownloader.Models;
using GithubCommitDownloader.Resources;
using System.Collections.Generic;

namespace GithubCommitDownloader.Database
{
	public partial class GithubDao
	{
		private const string connectionString = "Data Source=github-db.sqlite";
		private const string databaseLocation = "./";
		private const string databaseName = "github-db.sqlite";
		public SQLiteConnection connection;
		private static GithubDao instance = new GithubDao();
		public GithubDao()
		{
			if (!File.Exists(String.Format("{0}{1}", databaseLocation, databaseName)))
			{
				Console.WriteLine(ProjectResources.DatabaseNotFound);
				SQLiteConnection.CreateFile(String.Format("{0}{1}", databaseLocation, databaseName));
				connection = new SQLiteConnection(connectionString);
				this.OpenConnection();
				if (this.CreateUsersTable() && this.CreateRepositoriesTable() && this.CreateCommitsTable())
				{
					Console.WriteLine(ProjectResources.DatabaseCreateSuccess);
				}
				this.CloseConnection();
			}
			else
			{
				connection = new SQLiteConnection(connectionString);
			}
		}
		public static GithubDao Instance
		{
			get { return instance; }
		}
		#region Create tables
		private bool CreateUsersTable()
		{
			try
			{
				var command = connection.CreateCommand();
				command.CommandText = GithubDao.createUsersTable;
				var result = command.ExecuteNonQuery();
				return true;
			}
			catch (Exception ex)
			{
				Console.WriteLine(String.Format(ProjectResources.DatabaseError, ex.Message));
				File.Delete(String.Format("{0}{1}", databaseLocation, databaseName));
				this.CloseConnection();
				return false;
			}
		}
		private bool CreateRepositoriesTable()
		{
			try
			{
				var command = connection.CreateCommand();
				command.CommandText = GithubDao.createRepositoriesTable;
				var result = command.ExecuteNonQuery();
				return true;
			}
			catch (Exception ex)
			{
				Console.WriteLine(String.Format(ProjectResources.DatabaseError, ex.Message));
				File.Delete(String.Format("{0}{1}", databaseLocation, databaseName));
				this.CloseConnection();
				return false;
			}
		}
		private bool CreateCommitsTable()
		{
			try
			{
				var command = connection.CreateCommand();
				command.CommandText = GithubDao.createCommitsTable;
				var result = command.ExecuteNonQuery();
				return true;
			}
			catch (Exception ex)
			{
				Console.WriteLine(String.Format(ProjectResources.DatabaseError, ex.Message));
				File.Delete(String.Format("{0}{1}", databaseLocation, databaseName));
				this.CloseConnection();
				return false;
			}
		}
		#endregion
		#region Read
		public bool CheckIfRepositoryExist(out int? repositoryId, string username, string repositoryName)
		{
			try
			{
				var command = connection.CreateCommand();
				command.CommandText = GithubDao.checkIfRepositoryExist;
				command.Parameters.AddWithValue("@username", username);
				command.Parameters.AddWithValue("@repository_name", repositoryName);
				var result = command.ExecuteReader();
				if (result.HasRows)
				{
					result.Read();
					repositoryId = Convert.ToInt32(result["id"]);
				}
				else
				{
					repositoryId = null;
				}
				return true;
			}
			catch (Exception ex)
			{
				Console.WriteLine(String.Format(ProjectResources.DatabaseError, ex.Message));
				this.CloseConnection();
				repositoryId = null;
				return false;
			}
		}
		public bool CheckIfUserExist(out int? userId, string username)
		{
			try
			{
				var command = connection.CreateCommand();
				command.CommandText = GithubDao.checkIfUserExist;
				command.Parameters.AddWithValue("@username", username);
				var result = command.ExecuteReader();
				if (result.HasRows)
				{
					result.Read();
					userId = Convert.ToInt32(result["id"]);
				}
				else
				{
					userId = null;
				}
				return true;
			}
			catch (Exception ex)
			{
				Console.WriteLine(String.Format(ProjectResources.DatabaseError, ex.Message));
				this.CloseConnection();
				userId = null;
				return false;
			}
		}
		#endregion
		#region Insert
		private bool InsertUser(out int? userId, UserDto user)
		{
			try
			{
				var command = connection.CreateCommand();
				command.CommandText = GithubDao.insertUser;
				command.Parameters.AddWithValue("@username", user.Username);
				var result = command.ExecuteNonQuery();
				userId = Convert.ToInt32(connection.LastInsertRowId);
				return true;
			}
			catch (Exception ex)
			{
				Console.WriteLine(String.Format(ProjectResources.DatabaseError, ex.Message));
				this.CloseConnection();
				userId = null;
				return false;
			}
		}
		private bool InsertRepository(out int? repositoryId, RepositoryDto repository)
		{
			try
			{
				var command = connection.CreateCommand();
				command.CommandText = GithubDao.insertRepository;
				command.Parameters.AddWithValue("@repository_name", repository.Name);
				command.Parameters.AddWithValue("@user_id", repository.UserId);
				var result = command.ExecuteNonQuery();
				repositoryId = Convert.ToInt32(connection.LastInsertRowId);
				return true;
			}
			catch (Exception ex)
			{
				Console.WriteLine(String.Format(ProjectResources.DatabaseError, ex.Message));
				this.CloseConnection();
				repositoryId = null;
				return false;
			}
		}
		private bool InsertCommit(out int? commitId, CommitDto commit, int repositoryId)
		{
			try
			{
				var command = connection.CreateCommand();
				command.CommandText = GithubDao.insertCommit;
				command.Parameters.AddWithValue("@sha", commit.Sha);
				command.Parameters.AddWithValue("@message", commit.Message);
				command.Parameters.AddWithValue("@committer", commit.Committer);
				command.Parameters.AddWithValue("@repository_id", repositoryId);
				var result = command.ExecuteNonQuery();
				commitId = Convert.ToInt32(connection.LastInsertRowId);
				return true;
			}
			catch (Exception ex)
			{
				Console.WriteLine(String.Format(ProjectResources.DatabaseError, ex.Message));
				this.CloseConnection();
				commitId = null;
				return false;
			}
		}
		public bool InsertCommits(UserDto user, RepositoryDto repository, List<CommitDto> commits)
		{
			int? userId, repositoryId;
			this.OpenConnection();
			if (CheckIfRepositoryExist(out repositoryId, user.Username, repository.Name))
			{
				if (repositoryId.HasValue)
				{
					DeleteCommits(repositoryId.Value);
					foreach (var item in commits)
					{
						int? commitId;
						if (!InsertCommit(out commitId, item, repositoryId.Value))
						{
							return false;
						}
					}
					Console.WriteLine(ProjectResources.DatabaseOperationSuccess);
					this.CloseConnection();
					return true;
				}
			}
			else
			{
				return false;
			}
			if (CheckIfUserExist(out userId, user.Username))
			{
				if (userId.HasValue)
				{
					repository.UserId = userId.Value;
					if (!InsertRepository(out repositoryId, repository))
					{
						return false;
					}
					repository.Id = repositoryId.Value;
				}
				else
				{
					if (!InsertUser(out userId, user))
					{
						return false;
					}
					user.Id = userId.Value;
					repository.UserId = userId.Value;
					if (!InsertRepository(out repositoryId, repository))
					{
						return false;
					}
					repository.Id = repositoryId.Value;
				}
				foreach (var item in commits)
				{
					int? commitId;
					if (!InsertCommit(out commitId, item, repositoryId.Value))
					{
						return false;
					}
				}
				Console.WriteLine(ProjectResources.DatabaseOperationSuccess);
				this.CloseConnection();
				return true;
			}
			return false;
		}
		#endregion
		#region Delete
		private bool DeleteCommits(int repositoryId)
		{
			try
			{
				var command = connection.CreateCommand();
				command.CommandText = GithubDao.deleteCommits;
				command.Parameters.AddWithValue("@repository_id", repositoryId);
				command.ExecuteNonQuery();
				return true;
			}
			catch (Exception ex)
			{
				Console.WriteLine(String.Format(ProjectResources.DatabaseError, ex.Message));
				this.CloseConnection();
				return false;
			}
		}
		#endregion
		#region Helpers
		private void OpenConnection()
		{
			if (connection != null && connection.State != ConnectionState.Open)
			{
				connection.Open();
			}
		}
		private void CloseConnection()
		{
			if (connection != null && connection.State != ConnectionState.Closed)
			{
				connection.Close();
			}
		}
		#endregion
	}
}
