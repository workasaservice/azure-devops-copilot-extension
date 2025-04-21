// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using AzDoCopilotSK.SK;
using Microsoft.AspNetCore.Mvc;
using Microsoft.SemanticKernel;
using System.Text.Json;
using AzDoCopilotSK.Extensions;
using AzDoCopilotSK.Models;
using Microsoft.AspNetCore.Authorization;

namespace AzDoCopilotSK.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserStoryController : ControllerBase
    {
        private readonly Kernel _kernel;
        private readonly IPromptsFactory _promptsFactory;
        private readonly ILogger<UserStoryController> _logger;

        public UserStoryController(Kernel kernel, IPromptsFactory skillsFactory, ILogger<UserStoryController> logger)
        {
            _kernel = kernel;
            _promptsFactory = skillsFactory;
            _logger = logger;
        }

        [HttpPost]
        public async Task<ActionResult<UserStory?>> Create(UserStoryCreateDto userStoryCreateDto)
        {
            _logger.LogInformation("Creating user story.");

            var userStorySkill = new UserStorySkill(_kernel, _promptsFactory);

            var userStory = await userStorySkill.GetUserStory(
                userStoryCreateDto.UserStoryStyle,
                userStoryCreateDto.UserStoryDescription!,
                userStoryCreateDto.ProjectContext,
                userStoryCreateDto.PersonaName
            );
            
            return Ok(userStory);
        }
        [HttpDelete("{id:guid}")]
        [Authorize]
        public async Task<IActionResult> Delete(Guid id)
        {
            _logger.LogInformation("Deleting user story with ID: {Id}", id);

            try
            {
                var userStorySkill = new UserStorySkill(_kernel, _promptsFactory);

                var isDeleted = await userStorySkill.DeleteUserStory(id);

                if (!isDeleted)
                {
                    _logger.LogWarning("User story with ID: {Id} not found.", id);
                    return NotFound(new { Message = $"User story with ID {id} not found." });
                }

                _logger.LogInformation("User story with ID: {Id} successfully deleted.", id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting user story with ID: {Id}", id);
                return StatusCode(500, new { Message = "An error occurred while processing your request." });
            }
        }
    }
}
