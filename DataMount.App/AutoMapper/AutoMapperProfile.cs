using AutoMapper;
using DataMount.App.Inputs;
using DataMount.Domain.Models.Identity;

namespace DataMount.App.AutoMapper;

public class AutoMapperProfile<TKey> : Profile where TKey : struct, IEquatable<TKey>
{
    public AutoMapperProfile()
    {
        CreateMap<CreateUserWithCredentialInput, User<TKey>>();
    }
}