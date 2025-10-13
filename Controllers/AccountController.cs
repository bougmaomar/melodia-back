using AutoMapper;
using melodia_api.Models.Account;
using melodia_api.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace melodia_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ILogger<AccountController> _logger;
        private readonly IAccountRepository _repository;

        public AccountController(IMapper mapper, ILogger<AccountController> logger, IAccountRepository repository)
        {
            _mapper = mapper;
            _logger = logger;
            _repository = repository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAccounts()
        {
            var all = await _repository.GetAllAccounts();
            return Ok(all);
        }
        [HttpGet("id/{id}")]
        public async Task<IActionResult> GetAccountById(string id)
        {
            var account = await _repository.GetAccountById(id);
            return Ok(account);
        }
        [HttpGet("email/{email}")]
        public async Task<IActionResult> GetAccountByEmail(string email)
        {
            var account = await _repository.GetAccountByEmail(email);
            return Ok(account);
        }

        [HttpPut("password/{id}")]
        public async Task<IActionResult> PutAccountPassword([FromBody]  AccountResetPasswordDto accountResetPasswordDto, string id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { message = "Validation failed", errors = ModelState.Values.SelectMany(v => v.Errors) });
            }

            try
            {
                var account = await _repository.PutAccountPassword(id, accountResetPasswordDto.NewPassword);
                return Ok(new { message = "Password updated successfully", account });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating account password");
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }
        

    }
}
