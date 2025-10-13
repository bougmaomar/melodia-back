using AutoMapper;
using melodia_api.Models.AgentAccount;
using melodia_api.Models.Album;
using melodia_api.Models.ArtistAccount;
using melodia_api.Models.Song;
using melodia_api.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace melodia_api.Controllers
{
    [Route("api/[controller]s")]
    [ApiController]
    public class AgentAccountController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ILogger<AgentAccountController> _logger;
        private readonly IAgentAccountRepository _repository;

        public AgentAccountController(IAgentAccountRepository repository, IMapper mapper, ILogger<AgentAccountController> logger)
        {
            _logger = logger;
            _repository = repository;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAgentAccounts()
        {
            var all = await _repository.GetAllAgentAccounts();
            return Ok(all);
        }

        [HttpGet("songs/agent/{id}")]
        public async Task<IActionResult> GetAllSongsByAgent(long id)
        {
            var songs = await _repository.GetAllSongByAgent(id);
            var songsView = _mapper.Map<List<SongViewDto>>(songs);
            return Ok(songsView);
        }

        [HttpGet("albums/agent/{id}")]
        public async Task<IActionResult> GetAllAlbumsByAgent(long id)
        {
            var albums = await _repository.GetAllAlbumsByAgent(id);
            return Ok(_mapper.Map<IEnumerable<AlbumViewDto>>(albums));
        }

        [HttpPost]
        public async Task<IActionResult> CreateAgentAndAccount([FromBody] AgentAccountCreateDto createAgentAccount)
        {
            if (!ModelState.IsValid) return BadRequest(new { message = "Validation failed", errors = ModelState.Values.SelectMany(v => v.Errors) });

            try
            {
                var createdAgent = await _repository.CreateAgentAndAccount(createAgentAccount);

                return Ok(new { message = "Agent Account Created Successfully", agent = createdAgent });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while creating the agent account", error = ex.Message });
            }
        }

        [HttpPut("{agentId}/{accountId}")]
        public async Task<IActionResult> UpdateIntegratorAndAccountAsync([FromForm] long agentId, string accountId, [FromForm] AgentAccountUpdateDto agentAccountUpdate)
        {
            if (agentId != agentAccountUpdate.AgentId || accountId != agentAccountUpdate.accountId) return BadRequest();

            try
            {
                var result = await _repository.UpdateAgentAndAccountAsync(agentAccountUpdate);

                if (result == null) return NotFound();

                return Ok(result);
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                {
                    return StatusCode(500, ex.InnerException.Message);
                }
                else
                    return StatusCode(500, ex.Message);
            }
        }


        [HttpPut("Change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] AgentAccountChangePasswordDto passwordDto)
        {
            var result = await _repository.ChangePassword(passwordDto.Email, passwordDto.CurrentPassword, passwordDto.NewPassword);
            if (result.Succeeded) return Ok();
            return BadRequest(result.Errors);
        }

        [HttpGet("AccountById/{id}")]
        public async Task<IActionResult> GetAgentAccountById(long id)
        {
            var agent = await _repository.GetAgentAccountById(id);
            if (agent == null) return NotFound();
            var agentDto = _mapper.Map<AgentAccountViewDto>(agent);
            return Ok(agentDto);
        }

        [HttpGet("accountByEmail/{email}")]
        public async Task<IActionResult> GetAgentAccountByEmail(string email)
        {
            var account = await _repository.GetAgentAccountByEmail(email);
            if (account == null) return NotFound();
            var accountDto = _mapper.Map<AgentAccountViewDto>(account);
            return Ok(accountDto);
        }

        [HttpPut("accept/{id}")]
        public async Task<IActionResult> AcceptAgentAccount(long id)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                var agent = await _repository.AcceptAgentAccount(id);
                return Ok(agent);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while accepting the agent account");
                return StatusCode(500, new { message = "An error occurred while accepting the agent account", error = ex.Message });
            }
        }

        [HttpPut("reject/{id}")]
        public async Task<IActionResult> RejectAgentAccount(long id)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                var agent = await _repository.RejectAgentAccount(id);
                return Ok(agent);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while rejecting the agent account");
                return StatusCode(500, new { message = "An error occurred while rejecting the agent account", error = ex.Message });
            }
        }

        [HttpPut("deactivate/{accountId}")]
        public async Task<IActionResult> DeactivateAgentAccountById(string accountId)
        {
            var result = await _repository.DeactivateAgentAccountById(accountId);
            if (result) { return NoContent(); }

            return NotFound();
        }


        [HttpPut("activate/{accountId}")]
        public async Task<IActionResult> ActivateAgentAccountById(string accountId)
        {
            var result = await _repository.ActivateAgentAccountById(accountId);
            if (result) { return NoContent(); }

            return NotFound();
        }

    }
}
