using AutoMapper;
using melodia_api.Exceptions;
using melodia_api.Models.ArtistAccount;
using melodia_api.Repositories;
using melodia.Configurations;
using melodia.Entities;
using Microsoft.AspNetCore.Mvc;
using melodia_api.Entities;

namespace melodia_api.Controllers;

[Route("api/[controller]s")]
[ApiController]
public class ArtistAccountController : ControllerBase
{
    private readonly IMapper _mapper;
    private readonly ILogger<ArtistAccountController> _logger;
    private readonly IArtistAccountRepository _repository;

    public ArtistAccountController(IArtistAccountRepository repository, IMapper mapper,
        ILogger<ArtistAccountController> logger)
    {
        _logger = logger;
        _repository = repository;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllArtistAccounts()
    {
        var all = await _repository.GetAllArtistAccounts();
        return Ok(all);
    }

    [HttpGet("byAgent/{id}")]
    public async Task<IActionResult> GetArtistsByAgent(long id)
    {
        var artists = await _repository.GetArtistsByAgent(id);
        return Ok(artists);
    }

    [HttpPost]
    public async Task<IActionResult> CreateArtistAndAccount([FromBody] ArtistAccountCreateDto createArtistAccount)
    {
        if (!ModelState.IsValid)
            return BadRequest(new
                { message = "Validation failed", errors = ModelState.Values.SelectMany(v => v.Errors) });

        try
        {
            var createdArtist = await _repository.CreateArtistAndAccount(createArtistAccount);

            return Ok(new { message = "Artist Account Created Successfully", artist = createdArtist });
        }
        catch (Exception ex)
        {
            return StatusCode(500,
                new { message = "An error occurred while creating the artist account", error = ex.Message });
        }
    }

    [HttpPost("byAgent")]
    public async Task<IActionResult> CreateArtistByAgent([FromForm] ArtistCreateByAgentDto createArtist)
    {
        if (!ModelState.IsValid)
            return BadRequest(new
                { message = "Validation failed", errors = ModelState.Values.SelectMany(v => v.Errors) });

        try
        {
            var createdArtist = await _repository.CreateArtistByAgent(createArtist);

            return Ok(new { message = "Artist Account Created Successfully!", artist = createArtist });
        }
        catch (Exception ex)
        {
            return StatusCode(500,
                new { message = "An error occurred while creating the artist account", error = ex.Message });
        }
    }

    [HttpPut("byAgent/{artistId}")]
    [Produces("application/json")]
    [Consumes("multipart/form-data")] // Ensures that Swagger knows it's a form with a file upload
    public async Task<IActionResult> UpdateArtistByAgent(
        [FromRoute] long artistId,
        [FromForm] ArtistUpdateByAgentDto updateArtist)
    {
        // Validate the model state
        if (!ModelState.IsValid)
        {
            return BadRequest(new
                { message = "Validation failed", errors = ModelState.Values.SelectMany(v => v.Errors) });
        }

        // Ensure the artistId in the route matches the Artistid in the DTO
        if (artistId != updateArtist.Artistid)
        {
            return BadRequest(new { message = "Artist ID mismatch." });
        }

        try
        {
            // Call the repository to update the artist
            var updatedArtist = await _repository.UpdateArtistByAgent(updateArtist);

            return Ok(new { message = "Artist Account Updated Successfully!", artist = updatedArtist });
        }
        catch (Exception ex)
        {
            return StatusCode(500,
                new { message = "An error occurred while updating the artist account", error = ex.Message });
        }
    }

    [HttpPut("{artistId}/{accountId}")]
    public async Task<IActionResult> UpdateIntegratorAndAccountAsync([FromForm] long artistId, string accountId,
        [FromForm] ArtistAccountUpdateDto artistAccountUpdate)
    {
        if (artistId != artistAccountUpdate.ArtistId || accountId != artistAccountUpdate.accountId) return BadRequest();

        try
        {
            var result = await _repository.UpdateArtistAndAccountAsync(artistAccountUpdate);

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
    public async Task<IActionResult> ChangePassword([FromBody] ArtistAccountChangePasswordDto passwordDto)
    {
        var result =
            await _repository.ChangePassword(passwordDto.Email, passwordDto.CurrentPassword, passwordDto.NewPassword);
        if (result.Succeeded) return Ok();
        return BadRequest(result.Errors);
    }

    [HttpGet("AccountById/{id}")]
    public async Task<IActionResult> GetArtistAccountById(long id)
    {
        var account = await _repository.GetArtistAccountById(id);
        if (account == null) return NotFound();
        var accountDto = _mapper.Map<ArtistAccountViewDto>(account);
        return Ok(accountDto);
    }

    [HttpGet("accountByEmail/{email}")]
    public async Task<IActionResult> GetArtistAccountByEmail(string email)
    {
        var account = await _repository.GetArtistAccountByEmail(email);
        if (account == null) return NotFound();
        var accountDto = _mapper.Map<ArtistAccountViewDto>(account);
        return Ok(accountDto);
    }

    [HttpPut("deactivate/{accountId}")]
    public async Task<IActionResult> DeactivateArtistAccountById(string accountId)
    {
        var result = await _repository.DeactivateArtistAccountById(accountId);
        if (result)
        {
            return NoContent();
        }

        return NotFound();
    }


    [HttpPut("activate/{accountId}")]
    public async Task<IActionResult> ActivateArtistAccountById(string accountId)
    {
        var result = await _repository.ActivateArtistAccountById(accountId);
        if (result)
        {
            return NoContent();
        }

        return NotFound();
    }

    [HttpPost("record_visit")]
    public async Task<IActionResult> RecordVisit(long artistId, long radioStationId)
    {
        await _repository.RecordVisitAsync(artistId, radioStationId);
        return Ok();
    }

    [HttpGet("all_visits")]
    public async Task<ActionResult<IEnumerable<Visit>>> GetAllVisits(long artistId)
    {
        var visits = await _repository.GetAllVisitsByArtists(artistId);
        return Ok(visits);
    }

    [HttpGet("mensual_visits")]
    public async Task<IActionResult> GetMensualVisitsByArtist(long artistId)
    {
        var result = await _repository.GetMensualVisitsByArtist(artistId);
        return Ok(result);
    }

    [HttpGet("annual_visits")]
    public async Task<IActionResult> GetAnnualVisitsByArtist(long artistId)
    {
        var result = await _repository.GetAnnualVisitsByArtist(artistId);
        return Ok(result);
    }

    [HttpGet("comparaison_plays")]
    public async Task<IActionResult> GetArtistComparisonPlays(long artistId, long comparedId)
    {
        var result = await _repository.GetArtistComparison(artistId, comparedId);
        return Ok(result);
    }
}