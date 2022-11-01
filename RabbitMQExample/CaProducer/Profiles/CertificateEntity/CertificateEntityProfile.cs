using AutoMapper;
using CaProducer.Models;

namespace CaProducer.Profiles.CertificateEntity;

public class CertificateEntityProfile : Profile
{
    public CertificateEntityProfile()
    {
        CreateMap<CertificateRequestModel, Domain.Models.CertificateEntity>()
            .ForMember(dest => dest.CertId, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Issuer, opt => opt.MapFrom(src => src.CertInfo.Issuer))
            .ForMember(dest => dest.NotAfter, opt => opt.MapFrom(src => src.CertInfo.NotAfter))
            .ForMember(dest => dest.NotBefore, opt => opt.MapFrom(src => src.CertInfo.NotBefore))
            .ForMember(dest => dest.Serial, opt => opt.MapFrom(src => src.CertInfo.Serial))
            .ForMember(dest => dest.Subject, opt => opt.MapFrom(src => src.CertInfo.Subject))
            .ForMember(dest => dest.Thumbprint, opt => opt.MapFrom(src => src.CertInfo.Thumbprint.ToLower()))
            .ForMember(dest => dest.Data, opt => opt.Ignore())
            .AfterMap<CertificateEntityDataMappingAction>()
            ;
    }
}