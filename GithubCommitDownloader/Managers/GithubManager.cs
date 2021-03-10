using AutoMapper;
using GithubCommitDownloader.Database;
using GithubCommitDownloader.Helpers;
using GithubCommitDownloader.Models;
using System.Collections.Generic;

namespace GithubCommitDownloader.Managers
{
	public class GithubManager
	{
		private static GithubManager instance = new GithubManager();
		private GithubDao GithubDao;
		private IMapper mapper;
		private GithubManager()
		{
			GithubDao = GithubDao.Instance;
			var mapperConfiguration = new MapperConfiguration(cfg => cfg.AddProfile(new MappingProfile()));
			mapper = mapperConfiguration.CreateMapper();
		}
		public static GithubManager Instance
		{
			get { return instance; }
		}
		public void AddCommits(string username, string repositoryName, List<Response> commits)
		{
			var user = new UserDto() { Username = username };
			var repository = new RepositoryDto() { Name = repositoryName };
			GithubDao.InsertCommits(user, repository, mapper.Map<List<CommitDto>>(commits));
		}
	}
}
