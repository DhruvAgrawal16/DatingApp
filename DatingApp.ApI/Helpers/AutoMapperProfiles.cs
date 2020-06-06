using System.Linq;
using AutoMapper;
using DatingApp.ApI.Dtos;
using DatingApp.ApI.Models;

namespace DatingApp.ApI.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<User,UserForListDto>()
            .ForMember(dest=>dest.PhotoUrl,
            opt=>opt.MapFrom(src => src.Photos.FirstOrDefault(p => p.IsMain).Url))
            .ForMember(dest => dest.Age,
            opt => opt.MapFrom(src => src.DateOfBirth.CalculateAge()));
            CreateMap<User,UserForDetailDto>()
            .ForMember(dest=>dest.PhotoUrl,
            opt=>opt.MapFrom(src => src.Photos.FirstOrDefault(p => p.IsMain).Url))
            .ForMember(dest => dest.Age,
            opt => opt.MapFrom(src => src.DateOfBirth.CalculateAge()));;
            CreateMap<Photo,PhotoForDetailDto>();
            CreateMap<UserForUpdateDto,User>();   
            CreateMap<Photo,PhotoForReturnDto>();
            CreateMap<PhotoForCreationDto,Photo>();     
            CreateMap<MessageForCreationDto,Message>().ReverseMap();  
            CreateMap<Message,MessageToReturn>()
            .ForMember(m => m.SenderPhotoUrl, 
            opt => opt.MapFrom(u => u.Sender.Photos.FirstOrDefault(p => p.IsMain == true).Url))
            .ForMember(m => m.RecipientPhotoUrl, 
            opt => opt.MapFrom(u => u.Recipient.Photos.FirstOrDefault(p => p.IsMain == true).Url));
              
             
        }
        
    }
}