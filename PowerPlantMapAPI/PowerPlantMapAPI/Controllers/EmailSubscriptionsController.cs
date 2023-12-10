using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using PowerPlantMapAPI.Data;
using PowerPlantMapAPI.Services;

namespace PowerPlantMapAPI.Controllers;

/// <summary>
/// Manage the email subscriptions
/// </summary>
[EnableCors]
[Route("api/[controller]/[action]")]
[ApiController]
public class EmailSubscriptionsController : ControllerBase
{
    private readonly IEmailService _emailService;

    /// <summary>
    /// Instantiate the dependencies
    /// </summary>
    public EmailSubscriptionsController(IEmailService emailService)
    {
        _emailService = emailService;
    }
    
    /// <summary>
    /// Get all subscriptions
    /// </summary>
    /// <returns>List of all subscriptions</returns>
    [Authorize(Roles = "user, admin")]
    [HttpGet("")]
    public List<EmailSubscriptionModel>? Get()
    {
        return _emailService.Get();
    }

    /// <summary>
    /// Get subscription by id
    /// </summary>
    /// <param name="id">Id of the subscription</param>
    /// <returns>Subscription with the given id</returns>
    [Authorize(Roles = "user, admin")]
    [HttpGet("")]
    public EmailSubscriptionModel? GetById(Guid id)
    {
        return _emailService.GetById(id);
    }

    /// <summary>
    /// Get subscription by email
    /// </summary>
    /// <param name="email">Email of the subscription</param>
    /// <returns>Subscription with the given email</returns>
    [Authorize(Roles = "user, admin")]
    [HttpGet("")]
    public EmailSubscriptionModel? GetByEmail(string email)
    {
        return _emailService.GetByEmail(email);
    }

    /// <summary>
    /// Add new subscription
    /// </summary>
    /// <param name="email">New email to subscribe</param>
    /// <returns>Result of the subscription</returns>
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

    /// <summary>
    /// Update old email to new email
    /// </summary>
    /// <param name="oldEmail">Old email to change</param>
    /// <param name="newEmail">New email to save</param>
    /// <returns>Result of the subscription</returns>
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

    /// <summary>
    /// Delete subscription by id
    /// </summary>
    /// <param name="id">id of the subscription</param>
    /// <returns>Result of the delete operation</returns>
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