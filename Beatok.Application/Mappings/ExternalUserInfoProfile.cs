using AutoMapper;
using Beatok.Application.DTOs;

namespace Beatok.Application.Mappings;

public class ExternalUserInfoProfile: Profile
{
    public ExternalUserInfoProfile()
    {
        CreateMap<GoogleUserInfo, ExternalUserInfo>();
    }
}