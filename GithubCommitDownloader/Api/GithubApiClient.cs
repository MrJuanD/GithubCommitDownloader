using GithubCommitDownloader.Models;
using GithubCommitDownloader.Resources;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace GithubCommitDownloader.Api
{
	public class GithubApiClient
	{
		private static readonly string apiUrl = $"https://api.github.com/repos/";

		private static GithubApiClient instance = new GithubApiClient();
		private HttpClient httpClient;
		private GithubApiClient()
		{
			httpClient = new HttpClient();
			httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("request");
		}

		public static GithubApiClient Instance
		{
			get { return instance; }
		}

		public async Task<List<Response>> GetRepositoryInfo(string username, string repositoryName)
		{
			var result = new List<Response>();
			var items = new List<Response>();
			var page = 1;
			do
			{
				var url = apiUrl + $"{username}/{repositoryName}/commits?per_page=100&page={page}";
				page++;
				using (HttpResponseMessage response = await httpClient.GetAsync(url))
				{

					if (response.IsSuccessStatusCode)
					{
						items = await response.Content.ReadAsAsync<List<Response>>();
						result.AddRange(items);
					}
					else
					{
						Console.WriteLine(String.Format(ProjectResources.ApiError, (int)response.StatusCode, response.ReasonPhrase));
						return null;
					}
				}
			} while (items.Count == 100);
			return result;
		}
	}
}
