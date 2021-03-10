using System.Runtime.Serialization;

namespace GithubCommitDownloader.Models
{
	[DataContract]
	public class Response
	{
		[DataMember]
		public string Sha { get; set; }
		[DataMember]
		public Commit Commit { get; set; }
	}
	[DataContract]
	public class Commit
	{
		[DataMember]
		public string Message { get; set; }
		[DataMember]
		public Author Author { get; set; }
	}
	[DataContract]
	public class Author
	{
		[DataMember]
		public string Name { get; set; }
	}
}
