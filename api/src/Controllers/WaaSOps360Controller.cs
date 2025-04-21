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
    public class WaaSOps360Controller : ControllerBase
    {
        private readonly Kernel _kernel;
        private readonly IPromptsFactory _promptsFactory;
        private readonly ILogger<WaaSOps360Controller> _logger;

        public WaaSOps360Controller(Kernel kernel, IPromptsFactory promptsFactory, ILogger<WaaSOps360Controller> logger)
        {
            _kernel = kernel;
            _promptsFactory = promptsFactory;
            _logger = logger;
        }

        [HttpPost]
        public async Task<ActionResult<WaaSOps360?>> Create(WaaSOps360CreateDto waaSOps360CreateDto)
        {
            _logger.LogInformation("Creating WaaSOps360 entity.");

            var waaSOps360Skill = new WaaSOps360Skill(_kernel, _promptsFactory);

            var waaSOps360 = await waaSOps360Skill.CreateWaaSOps360(
                waaSOps360CreateDto.Name,
                waaSOps360CreateDto.Description,
                waaSOps360CreateDto.Context
            );

            return Ok(waaSOps360);
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<WaaSOps360?>> Get(Guid id)
        {
            _logger.LogInformation("Fetching WaaSOps360 entity with ID: {Id}", id);

            var waaSOps360Skill = new WaaSOps360Skill(_kernel, _promptsFactory);

            var waaSOps360 = await waaSOps360Skill.GetWaaSOps360(id);

            if (waaSOps360 == null)
            {
                _logger.LogWarning("WaaSOps360 entity with ID: {Id} not found.", id);
                return NotFound(new { Message = $"WaaSOps360 entity with ID {id} not found." });
            }

            return Ok(waaSOps360);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, WaaSOps360UpdateDto waaSOps360UpdateDto)
        {
            _logger.LogInformation("Updating WaaSOps360 entity with ID: {Id}", id);

            var waaSOps360Skill = new WaaSOps360Skill(_kernel, _promptsFactory);

            var isUpdated = await waaSOps360Skill.UpdateWaaSOps360(
                id,
                waaSOps360UpdateDto.Name,
                waaSOps360UpdateDto.Description,
                waaSOps360UpdateDto.Context
            );

            if (!isUpdated)
            {
                _logger.LogWarning("WaaSOps360 entity with ID: {Id} not found.", id);
                return NotFound(new { Message = $"WaaSOps360 entity with ID {id} not found." });
            }

            _logger.LogInformation("WaaSOps360 entity with ID: {Id} successfully updated.", id);
            return NoContent();
        }

        [HttpDelete("{id:guid}")]
        [Authorize]
        public async Task<IActionResult> Delete(Guid id)
        {
            _logger.LogInformation("Deleting WaaSOps360 entity with ID: {Id}", id);

            try
            {
                var waaSOps360Skill = new WaaSOps360Skill(_kernel, _promptsFactory);

                var isDeleted = await waaSOps360Skill.DeleteWaaSOps360(id);

                if (!isDeleted)
                {
                    _logger.LogWarning("WaaSOps360 entity with ID: {Id} not found.", id);
                    return NotFound(new { Message = $"WaaSOps360 entity with ID {id} not found." });
                }

                _logger.LogInformation("WaaSOps360 entity with ID: {Id} successfully deleted.", id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting WaaSOps360 entity with ID: {Id}", id);
                return StatusCode(500, new { Message = "An error occurred while processing your request." });
            }
        }
    }
}
