using AutoMapper;
using DFC.App.FindACourse.ViewModels;
using DFC.CompositeInterfaceModels.FindACourseClient;
using System.Diagnostics.CodeAnalysis;

namespace DFC.App.FindACourse.AutoMapperProfiles
{
    [ExcludeFromCodeCoverage]
    public class ProviderModelProfile : Profile
    {
        public ProviderModelProfile()
        {
            CreateMap<ProviderDetails, ProviderViewModel>();
        }
    }
}