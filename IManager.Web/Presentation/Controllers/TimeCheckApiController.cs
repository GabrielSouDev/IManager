using AutoMapper;
using IManager.Web.Domain.Entities.TimeTrackings;
using IManager.Web.Domain.Interfaces.Persistence;
using IManager.Web.Domain.Interfaces.Repositories;
using IManager.Web.Shared.DTO.TimeTrackings;
using IManager.Web.Shared.Requests;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Asn1.Ocsp;
using System.Security.Claims;
namespace IManager.Web.Presentation.Controllers;

[ApiController]
[Route("api/timecheck")]
public class TimeCheckApiController : ControllerBase
{
    private readonly IRepository<TimeEntry> _timeEntryRepository;
    private readonly IRepository<TimeCheck> _timeCheckRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public TimeCheckApiController(IRepository<TimeEntry> timeEntryRepository, IRepository<TimeCheck> timeCheckRepository, 
        IUnitOfWork unitOfWork, IMapper mapper)
    {
        _timeEntryRepository = timeEntryRepository;
        _timeCheckRepository = timeCheckRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [HttpPost]
    public async Task<IActionResult> Include(TimeCheckRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest("TimeCheckRequest invalido.");
        }

        await _unitOfWork.BeginTransactionAsync();

        try
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var timeEntry = (await _timeEntryRepository.GetAllAsync(q => q.Where(te =>
                                                                    te.EmployeeId == userId
                                                                    && te.Date == DateOnly.FromDateTime(request.Timestamp))))
                                                                    .FirstOrDefault();

            if (timeEntry == null)
            {
                timeEntry = new TimeEntry
                {
                    EmployeeId = userId,
                    Date = DateOnly.FromDateTime(request.Timestamp)
                };

                await _timeEntryRepository.AddAsync(timeEntry);
            }

            var timeCheck = new TimeCheck
            {
                TimeEntryId = timeEntry.Id,
                Timestamp = request.Timestamp,
            };

            await _timeCheckRepository.AddAsync(timeCheck);

            await _unitOfWork.CommitAsync();

            return NoContent();
        }
        catch (Exception)
        {
            await _unitOfWork.RollbackAsync();
            throw;
        }
    }

    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var timeEntry = await _timeEntryRepository.GetAllAsync(q => q.Where(te =>
                                                        te.EmployeeId == userId).Include(te => te.Checks));

        var dto = _mapper.Map<IEnumerable<TimeEntryDTO>>(timeEntry);
        return Ok(dto);
    }

    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [HttpGet("/{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        if (id == Guid.Empty)
            return NotFound();

        var timeEntry = await _timeEntryRepository.GetByIdAsync(id, q => q.Include(te => te.Checks));
        if(timeEntry is null)
            return NotFound();

        var dto = _mapper.Map<IEnumerable<TimeEntryDTO>>(timeEntry);
        return Ok(dto);
    }

    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [HttpDelete("/{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var exists = await _timeEntryRepository.ExistsAsync(te => te.Id == id);
        if (id == Guid.Empty || !exists)
            return NotFound();

        await _timeEntryRepository.DeleteAsync(id);

        return NoContent();
    }

    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [HttpPut]
    public async Task<IActionResult> Update(TimeCheckDTO request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest("TimeCheckRequest invalido.");
        }

        var entity = _mapper.Map<TimeCheck>(request);

        await _timeCheckRepository.UpdateAsync(entity);
        return NoContent();
    }

   

    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [HttpGet("today")]
    public async Task<IActionResult> GetTodayTimeChecks()
    {
        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        var timeEntry = (await _timeEntryRepository.GetAllAsync(q => q.Where(te =>
                                                                te.EmployeeId == userId
                                                                && te.Date == DateOnly.FromDateTime(DateTime.UtcNow))
                                                                .Include(te => te.Checks))).FirstOrDefault();


        var dto = _mapper.Map<TimeEntryDTO>(timeEntry);
        return Ok(dto);
    }
}