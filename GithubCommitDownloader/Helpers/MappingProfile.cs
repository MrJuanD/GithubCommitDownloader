using AutoMapper;
using GithubCommitDownloader.Models;

namespace GithubCommitDownloader.Helpers
{
	public class MappingProfile : Profile
	{
		public MappingProfile()
		{
			CreateMap<Response, CommitDto>().ForMember(dest => dest.Sha, opts => opts.MapFrom(src => src.Sha))
				.ForMember(dest => dest.Message, opts => opts.MapFrom(src => src.Commit.Message))
				.ForMember(dest => dest.Committer, opts => opts.MapFrom(src => src.Commit.Author.Name));
		}
	}
}
