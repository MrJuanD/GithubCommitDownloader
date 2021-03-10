using System;
using System.Threading.Tasks;
using GithubCommitDownloader.Api;
using GithubCommitDownloader.Managers;
using GithubCommitDownloader.Resources;

namespace GithubCommitDownloader
{
	class Program
	{
		static async Task Main(string[] args)
		{
			string username;
			string repositoryName;
			var client = GithubApiClient.Instance;
			var manager = GithubManager.Instance;

			Console.Write(ProjectResources.UsernameRequest);
			username = Console.ReadLine();
			Console.Write(ProjectResources.RepositoryNameRequest);
			repositoryName = Console.ReadLine();
			var result = await client.GetRepositoryInfo(username, repositoryName);
			if (result != null)
			{
				foreach (var item in result)
				{
					Console.WriteLine($"{repositoryName}/{item.Sha}: {item.Commit.Message} {item.Commit.Author.Name}");
				}
				manager.AddCommits(username, repositoryName, result);
			}
		}
	}
}
