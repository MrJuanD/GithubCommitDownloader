namespace GithubCommitDownloader.Database
{
	public partial class GithubDao
	{
		private const string createUsersTable = "CREATE TABLE `users` (`id`	INTEGER NOT NULL UNIQUE, `username`	TEXT NOT NULL, PRIMARY KEY(`id` AUTOINCREMENT));";
		private const string createRepositoriesTable = "CREATE TABLE `repositories` (`id`	INTEGER NOT NULL UNIQUE, `repository_name`	TEXT NOT NULL, `user_id`	INTEGER NOT NULL, PRIMARY KEY(`id` AUTOINCREMENT));";
		private const string createCommitsTable = "CREATE TABLE `commits` (`id`	INTEGER NOT NULL UNIQUE, `sha`	TEXT NOT NULL, `message`	TEXT NOT NULL, `committer`	TEXT NOT NULL, `repository_id`	INTEGER NOT NULL, PRIMARY KEY(`id` AUTOINCREMENT));";
		private const string insertUser = "INSERT INTO users (username) VALUES(@username);";
		private const string insertRepository = "INSERT INTO repositories (repository_name, user_id) VALUES(@repository_name, @user_id);";
		private const string insertCommit = "INSERT INTO commits (sha, message, committer, repository_id) VALUES(@sha, @message, @committer, @repository_id);";
		private const string checkIfUserExist = "SELECT id FROM users WHERE LOWER(username) = LOWER(@username)";
		private const string checkIfRepositoryExist = "SELECT r.id FROM repositories r INNER JOIN users u ON r.user_id = u.id WHERE LOWER(u.username) = LOWER(@username) AND LOWER(r.repository_name) = LOWER(@repository_name);";
		private const string deleteCommits = "DELETE FROM commits WHERE repository_id = @repository_id;";
	}
}
