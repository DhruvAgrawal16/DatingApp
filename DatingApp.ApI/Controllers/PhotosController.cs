using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using DatingApp.ApI.Data;
using DatingApp.ApI.Dtos;
using DatingApp.ApI.Helpers;
using DatingApp.ApI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;


namespace DatingApp.ApI.Controllers
{
    [Authorize]
    [Route("api/users/{userId}/photos")]
    [ApiController]
    public class PhotosController : ControllerBase
    {
        private readonly IDatingRepository repo;
        private readonly IMapper mapper;
        private readonly IOptions<CloudinarySettings> cloudinaryConfig;
        private Cloudinary _cloudinary;
        public PhotosController(IDatingRepository repo, IMapper mapper, IOptions<CloudinarySettings> cloudinaryConfig)
        {
            this.cloudinaryConfig = cloudinaryConfig;
            this.mapper = mapper;
            this.repo = repo;

            Account acc = new Account(
                cloudinaryConfig.Value.CloudName,
                cloudinaryConfig.Value.ApiKey,
                cloudinaryConfig.Value.ApiSecret
            );

            _cloudinary = new Cloudinary(acc); 
        }
    [HttpGet("{id}",Name = "GetPhoto")]
     public async Task<IActionResult> GetPhoto(int id)
     {
         var photoFromRepo = await repo.GetPhoto(id);

         var photo = mapper.Map<PhotoForReturnDto>(photoFromRepo);

         return Ok(photo);
     }


    [HttpPost]
    public async Task<IActionResult> AddPhotoForUser(int userId,
                        [FromForm]PhotoForCreationDto photoForCreationDto)
    {
         if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            {
                return Unauthorized();
            }
        var userFromRepo = await repo.GetUser(userId);
        
        var file = photoForCreationDto.File;

        var uploadResult = new ImageUploadResult();

        if(file.Length>0){
            using (var stream = file.OpenReadStream())
            {
                var uploadParams = new ImageUploadParams()
                {
                    File = new FileDescription(file.Name,stream),
                    Transformation = new Transformation()
                    .Width(500).Height(500).Crop("fill").Gravity("face")
                };

                uploadResult = _cloudinary.Upload(uploadParams);

            }
        }

        photoForCreationDto.Url = uploadResult.Uri.ToString();
        photoForCreationDto.PublicId = uploadResult.PublicId.ToString();

        var photo = mapper.Map<Photo>(photoForCreationDto);
        if(!userFromRepo.Photos.Any(u => u.IsMain)){
            photo.IsMain = true;
        }
        userFromRepo.Photos.Add(photo);

        if(await repo.SaveAll())
        {
            var photoToReturn = mapper.Map<PhotoForReturnDto>(photo);
            return CreatedAtRoute("GetPhoto",new {id = photo.Id,userId=userId},photoToReturn);
        }
        return BadRequest("Could not add the photo");
    }

    [HttpPost("{id}/setMain")]
    public async Task<IActionResult> SetMainPhoto(int userId, int id)
    {
        if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            {
                return Unauthorized();
            }
        var userFromRepo = await repo.GetUser(userId);
        
        if(!userFromRepo.Photos.Any(p=> p.Id == id)){
            return Unauthorized();
        }

        var photoFromRepo = await repo.GetPhoto(id);

        if (photoFromRepo.IsMain)
        {
            return BadRequest("Photo already main photo");
        }

        var currentMainPhoto = await repo.GetMainPhoto(userId);
        currentMainPhoto.IsMain = false;

        photoFromRepo.IsMain = true;

        if(await repo.SaveAll())
        {
            return NoContent();
        }
        return BadRequest("Could not set photo to main");
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePhoto(int userId, int id)
    {
        if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            {
                return Unauthorized();
            }
        var userFromRepo = await repo.GetUser(userId);
        
        if(!userFromRepo.Photos.Any(p=> p.Id == id)){
            return Unauthorized();
        }

        var photoFromRepo = await repo.GetPhoto(id);

        if (photoFromRepo.IsMain)
        {
            return BadRequest("Photo already main photo, Not able to delete!!");
        }

        if(photoFromRepo.PublicId != null){
        var DeleteParams = new DeletionParams(photoFromRepo.PublicId);
        var response  = _cloudinary.Destroy(DeleteParams);
        
        if(response.Result == "ok"){
            repo.Delete(photoFromRepo);
        }
        
        }
                
        repo.Delete(photoFromRepo);
        
        if(await repo.SaveAll())
        {
            return Ok();
        }
        return BadRequest("Failed to Delete photo");
    }
    }
}