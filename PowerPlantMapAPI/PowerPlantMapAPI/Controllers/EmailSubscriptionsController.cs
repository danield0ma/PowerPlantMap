using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using PowerPlantMapAPI.Data;
using PowerPlantMapAPI.Services;

namespace PowerPlantMapAPI.Controllers;

[EnableCors]
[Route("api/[controller]/[action]")]
[ApiController]
public class EmailSubscriptionsController : ControllerBase
{
    private readonly IEmailService _emailService;

    public EmailSubscriptionsController(IEmailService emailService)
    {
        _emailService = emailService;
    }
    
    [Authorize]
    [HttpGet("")]
    public List<EmailSubscriptionModel>? Get()
    {
        return _emailService.Get();
    }

    [Authorize]
    [HttpGet("")]
    public EmailSubscriptionModel? GetById(Guid id)
    {
        return _emailService.GetById(id);
    }

    [Authorize]
    [HttpGet("")]
    public EmailSubscriptionModel? GetByEmail(string email)
    {
        return _emailService.GetByEmail(email);
    }

    [HttpPost("")]
    public ActionResult Add(string email)
    {
        try
        {
            _emailService.Add(email);
            return Ok();
        }
        catch (ArgumentException e)
        {
            Console.WriteLine(e);
            return BadRequest();
        }
    }

    [HttpPut("")]
    public ActionResult Update(string oldEmail, string newEmail)
    {
        try
        {
            _emailService.Update(oldEmail, newEmail);
            return Ok();
        }
        catch (ArgumentException e)
        {
            Console.WriteLine(e);
            return BadRequest();
        }
    }

    [HttpGet("")]
    public ActionResult Delete(Guid id)
    {
        try
        {
            _emailService.Delete(id);
            return Ok("Sikeres leiratkozás!");
        }
        catch (ArgumentException e)
        {
            Console.WriteLine(e);
            return BadRequest();
        }
    }
}