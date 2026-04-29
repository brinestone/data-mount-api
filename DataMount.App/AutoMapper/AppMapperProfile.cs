using AutoMapper;
using DataMount.App.Inputs;
using DataMount.Domain.Models.Identity;

namespace DataMount.App.AutoMapper;

public class AppMapperProfile<TKey> : Profile where TKey : struct, IEquatable<TKey>
{
    public AppMapperProfile()
    {
        CreateMap<CreateUserWithCredentialInput, User<TKey>>();
    }
}