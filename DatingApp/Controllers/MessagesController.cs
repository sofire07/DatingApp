using Logic;
using Logic.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Model;
using Model.DataTransfer;
using Model.Extensions;
using Model.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DatingApp.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize]
    public class MessagesController : ControllerBase
    {
        private readonly IMessageLogic _messageLogic;
        private readonly IUserLogic _userLogic;

        public MessagesController(IMessageLogic messageLogic, IUserLogic userLogic)
        {
            _messageLogic = messageLogic;
            _userLogic = userLogic;
        }

        [HttpPost]
        public async Task<IActionResult> SendMessage([FromBody] CreateMessageDto dto)
        {
            var username = User.GetUsername();
            var result = await _messageLogic.AddMessage(dto, username);
            if (result != null) return Ok(result);
            return BadRequest("The message was not sent. Please try again.");
        }

        [HttpGet("Message/{id}")]
        public async Task<IActionResult> GetMessage(int id)
        {
            var result = await _messageLogic.GetMessage(id);
            if (result == null) return NotFound("Message with that was not found");
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetMessages([FromQuery] MessageParams messageParams)
        {
            messageParams.Username = User.GetUsername();
            var messages = await _messageLogic.GetMessagesForUser(messageParams);
            Response.AddPaginationHeader(messages.CurrentPage, messages.TotalPages, messages.PageSize, messages.TotalCount);
            return Ok(messages);
        }

        [HttpGet("{username}")]
        public async Task<IActionResult> GetMessageThread(string username)
        {
            var loggedIn = User.GetUsername();
            return Ok(await _messageLogic.GetMessageThread(loggedIn, username));
        }
    }
}
