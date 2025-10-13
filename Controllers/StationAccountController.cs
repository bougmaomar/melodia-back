using AutoMapper;
using melodia_api.Models.StationAccount;
using melodia_api.Repositories;
using melodia.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace melodia_api.Controllers
{
    [Route("api/[controller]s")]
    [ApiController]
    public class StationAccountController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ILogger<StationAccountController> _logger;
        private readonly UserManager<Account> _userManager;
        private readonly IStationAccountRepository _repository;

        public StationAccountController(IStationAccountRepository repository, IMapper mapper, ILogger<StationAccountController> logger, UserManager<Account> userManager)
        {
            _logger = logger;
            _repository = repository;
            _userManager = userManager;
            _mapper = mapper;
        }
        
        [HttpGet("{userId}")]
        public async Task<IActionResult> GetRadioStationByUserId(long userId)
        {
            var radioStation = await _repository.GetRadioStationById(userId);
            if (radioStation == null)
            {
                return NotFound("Radio station not found.");
            }
            var station = _mapper.Map<StationAccountViewDto>(radioStation);

            return Ok(station);
        }

        
        [HttpGet("logged-in")]
        public async Task<IActionResult> GetLoggedInRadioStation()
        {
            _logger.LogInformation("GetLoggedInRadioStation called");

            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(userId))
            {
                _logger.LogWarning("User ID is null or empty. Unauthorized access attempt.");
                return Unauthorized();
            }

            _logger.LogInformation("Fetching radio station for user ID: {UserId}", userId);
            var radioStation = await _repository.GetRadioStationByAccountId(userId);
            if (radioStation == null)
            {
                _logger.LogWarning("Radio station not found for user ID: {UserId}", userId);
                return NotFound("Radio station not found.");
            }

            _logger.LogInformation("Radio station found for user ID: {UserId}", userId);
            return Ok(radioStation);
        }
        
        [HttpGet]
        public async Task<IActionResult> GetAllStationAccounts()
        {
            var all = await _repository.GetAllStationAccounts();
            return Ok(all);
        }

        [HttpGet("top_station")]
        public async Task<IActionResult> GetTopStation()
        {
            var top = await _repository.GetTopStation();
            return Ok(top);
        }
        
        [HttpPost]
        public async Task<IActionResult> CreateStationAndAccount([FromBody] StationAccountCreateDto createStationAccount)
        {
            if (!ModelState.IsValid) return BadRequest(new { message = "Validation failed", errors = ModelState.Values.SelectMany(v => v.Errors) });

            try
            {
                var createdStation = await _repository.CreateStationAndAccount(createStationAccount);

                return Ok(new { message = "Station Account Created Successfully", station = createdStation });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while creating the Station account", error = ex.Message });
            }
        }

        [HttpPut("{stationId}/{accountId}")]
        public async Task<IActionResult> UpdateIntegratorAndAccountAsync([FromForm] long stationId, string accountId, [FromForm] StationAccountUpdateDto stationAccountUpdate)
        {
            if (stationId != stationAccountUpdate.StationId || accountId != stationAccountUpdate.accountId) return BadRequest();

            try
            {
                var result = await _repository.UpdateStationAndAccountAsync(stationAccountUpdate);

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
        public async Task<IActionResult> ChangePassword([FromBody] StationAccountChangePasswordDto passwordDto)
        {
            var result = await _repository.ChangePassword(passwordDto.Email, passwordDto.CurrentPassword, passwordDto.NewPassword);
            if (result.Succeeded) return Ok();
            return BadRequest(result.Errors);
        }

        
        [HttpGet("AccountById/{id}")]
        public async Task<IActionResult> GetStationAccountById(long id)
        {
            var account = await _repository.GetRadioStationById(id);
            if (account == null) return NotFound();
            //var accountDto = _mapper.Map<StationAccountViewDto>(account);
            return Ok(account);
        }
        
        
        [HttpGet("accountByEmail/{email}")]
        public async Task<IActionResult> GetStationAccountByEmail(string email)
        {
            var account = await _repository.GetStationAccountByEmail(email);
            if (account == null) return NotFound();
            var accountDto = _mapper.Map<StationAccountViewDto>(account);
            return Ok(accountDto);
        }
        
        
        [HttpPut("accept/{id}")]
        public async Task<IActionResult> AcceptStationAccount(long id)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                var station = await _repository.AcceptStationAccount(id);
                return Ok(station);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while accepting the agent account");
                return StatusCode(500, new { message = "An error occurred while accepting the agent account", error = ex.Message });
            }
        }
        
        
        [HttpPut("reject/{id}")]
        public async Task<IActionResult> RejectStationAccount(long id)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                var station = await _repository.RejectStationAccount(id);
                return Ok(station);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while rejecting the agent account");
                return StatusCode(500, new { message = "An error occurred while rejecting the agent account", error = ex.Message });
            }
        }

        
        [HttpPut("deactivate/{accountId}")]
        public async Task<IActionResult> DeactivateStationAccountById(string accountId)
        {
            var result = await _repository.DeactivateStationAccountById(accountId);
            if (result) { return NoContent(); }

            return NotFound();
        }


        [HttpPut("activate/{accountId}")]
        public async Task<IActionResult> ActivateStationAccountById(string accountId)
        {
            var result = await _repository.ActivateStationAccountById(accountId);
            if (result) { return NoContent(); }

            return NotFound();
        }

    }
}
