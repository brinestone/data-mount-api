using AutoMapper;
using DataMount.Api.Payloads;
using DataMount.App.Inputs;
using DataMount.Domain.Models.Identity;

namespace DataMount.Api.AutoMapper;

public class PayloadMapperProfile<TKey> : Profile where TKey : struct, IEquatable<TKey>
{
    public PayloadMapperProfile()
    {
        CreateMap<Session<TKey>, SessionDto<TKey>>();
        CreateMap<User<TKey>, UserDto<TKey>>();
        CreateMap<EmailSignInRequest, CreateCredentialSessionInput>()
            .ForMember(i => i.Identifier, cfg => cfg.MapFrom(src => src.Email))
            .ForMember(i => i.ContactType, c => c.MapFrom(_ => ContactType.Email));
    }
}