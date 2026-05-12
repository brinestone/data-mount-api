using AutoMapper;
using DataMount.Api.Payloads;
using DataMount.App.Inputs;
using DataMount.Domain.Exceptions;
using DataMount.Domain.Models.Identity;

namespace DataMount.Api.AutoMapper;

public class PayloadMapperProfile<TKey> : Profile where TKey : struct, IEquatable<TKey>
{
    public PayloadMapperProfile()
    {
        CreateMap<Session<TKey>, SessionDto<TKey>>();
        CreateMap<User<TKey>, UserDto<TKey>>();
        CreateMap<Contact<TKey>, ContactDto<TKey>>()
            .ForMember(d => d.Type, cfg => cfg.MapFrom(src => src.Type.ToString().ToLowerInvariant()));
        CreateMap<EmailSignInRequest, CreateCredentialSessionInput>()
            .ForMember(i => i.Identifier, cfg => cfg.MapFrom(src => src.Email))
            .ForMember(i => i.ContactType, c => c.MapFrom(_ => ContactType.Email));
        CreateMap<AppException, ErrorMessagePayload>()
            .ForMember(i => i.Status, p => p.MapFrom(src => src.Code));
        CreateMap<Exception, ErrorMessagePayload>();
    }
}