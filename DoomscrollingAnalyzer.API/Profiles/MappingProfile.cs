using AutoMapper;
using DoomscrollingAnalyzer.API.DTOs;
using DoomscrollingAnalyzer.API.Models;

namespace DoomscrollingAnalyzer.API.Profiles;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<CreateScreenTimeEntryDto, ScreenTimeEntry>();
        CreateMap<ScreenTimeEntry, ScreenTimeEntryResponseDto>();
    }
}
