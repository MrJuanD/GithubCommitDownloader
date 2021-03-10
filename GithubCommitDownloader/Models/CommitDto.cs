namespace GithubCommitDownloader.Models
{
	public class CommitDto
	{
		public int? Id { get; set; }
		public string Sha { get; set; }
		public string Message { get; set; }
		public string Committer { get; set; }
		public int? RepositoryId { get; set; }
	}
}
