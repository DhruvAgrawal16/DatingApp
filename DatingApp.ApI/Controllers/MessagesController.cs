using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using DatingApp.ApI.Data;
using DatingApp.ApI.Dtos;
using DatingApp.ApI.Helpers;
using DatingApp.ApI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DatingApp.ApI.Controllers
{   
    [Authorize]
    [Route("api/users/{userId}/[controller]")]
    [ApiController]
    public class MessagesController : ControllerBase
    {
      
        private readonly IDatingRepository repo;
        private readonly IMapper mapper;
        public MessagesController(IDatingRepository repo, IMapper mapper)
        {
            this.mapper = mapper;
            this.repo = repo;

        }

        [HttpGet("{id}", Name="GetMessage")]
        public async Task<IActionResult> GetMessage(int userId, int id)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            {
                return Unauthorized();
            }

            var messageFromRepo = await repo.GetMessage(id);

            if(messageFromRepo == null)
            {
                return NotFound();
            }
            return Ok(messageFromRepo);
        }

        [HttpGet]
        public async Task<IActionResult> GetMessagesForUser(int userId, 
                    [FromQuery]MessageParams messageParams)
        {
         if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            {
                return Unauthorized();
            }

        messageParams.UserId = userId;

        var messagesFromRepo = await repo.GetMessagesForUser(messageParams);

        var messages = mapper.Map<IEnumerable<MessageToReturn>>(messagesFromRepo);
        
        Response.AddPagination(messagesFromRepo.CurrentPage, 
        messagesFromRepo.PageSize,messagesFromRepo.TotalCount,messagesFromRepo.TotalPages);

        return Ok(messages);

        }

        [HttpGet("thread/{recipientId}")]
        public async Task<IActionResult> GetMessageThread(int userId, int recipientId)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            {
                return Unauthorized();
            }

            var messagesFromRepo = await repo.GetMessageThread(userId,recipientId);

            var messageThread = mapper.Map<IEnumerable<MessageToReturn>>(messagesFromRepo);

            return Ok(messageThread);

        }



        [HttpPost]

        public async Task<IActionResult> CreateMessage(int userId, MessageForCreationDto messageForCreationDto)
        {
            var sender = await repo.GetUser(userId);
             if (sender.Id != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            {
                return Unauthorized();
            }

            messageForCreationDto.SenderId = userId;

            var recipient = await repo.GetUser(messageForCreationDto.RecipientId);

            if(recipient == null){
                return BadRequest("COuld not find User");}

            var message = mapper.Map<Message>(messageForCreationDto);

            repo.Add(message);
           

            if(await repo.SaveAll()){
                 var messageToReturn = mapper.Map<MessageToReturn>(message);
                return CreatedAtRoute("GetMessage", new {userId, id = message.Id}, messageToReturn);
            }

            throw new System.Exception("Creating Message Failed");

        }




        
    }
}