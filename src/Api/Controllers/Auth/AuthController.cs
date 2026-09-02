using Api.Contracts.Validation;
using Api.Controllers.Auth.Login;
using Api.Presenters.Auth;
using Application.UseCases.Administrativo.Usuario.Commands.Login;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.Auth;

[ApiController]
[Route("api/[controller]")]
public class AuthController(IMediator mediator, AuthPresenter presenter, IValidator<LoginRequest> validator) : ControllerBase
{
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var validationResult = validator.Validate(request);
        if (!validationResult.IsValid)
        {
            return BadRequest(new { validationResult.Errors });
        }

        var response = await mediator.Send(new LoginCommand { Login = request.Login, Senha = request.Senha });
        return Ok(presenter.Present(response));
    }
}
