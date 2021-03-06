﻿using AutoMapper;
using DFC.App.FindACourse.Data.Models;
using System.Diagnostics.CodeAnalysis;

namespace DFC.App.FindACourse.AutoMapperProfiles
{
    [ExcludeFromCodeCoverage]
    public class StaticContentItemModelProfile : Profile
    {
        public StaticContentItemModelProfile()
        {
            CreateMap<StaticContentItemApiDataModel, StaticContentItemModel>()
                .ForMember(d => d.Id, s => s.MapFrom(a => a.ItemId))
                .ForMember(d => d.Etag, s => s.Ignore())
                .ForMember(d => d.ParentId, s => s.Ignore())
                .ForMember(d => d.TraceId, s => s.Ignore())
                .ForMember(d => d.PartitionKey, s => s.Ignore())
                .ForMember(d => d.LastReviewed, s => s.MapFrom(a => a.Published))
                .ForMember(d => d.LastCached, s => s.Ignore());
        }
    }
}
