using DevIO.Business.Intefaces;
using DevIO.Business.Notificacoes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace DevIO.API.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
public abstract class MainController : ControllerBase
{
    private readonly INotificador _notificador;
    public readonly IUser AppUser;

    protected Guid UsuarioId { get; set; }
    protected bool UsuarioAtenticado { get; set; }

    protected MainController(INotificador notificador, IUser appUser)
    {
        _notificador = notificador;
        AppUser = appUser;

        if (appUser.IsAuthenticated())
        {
            UsuarioId = appUser.GetUserId();
            UsuarioAtenticado = true;
        }
    }

    protected ActionResult CustomResponse(ModelStateDictionary modelState)
    {
        if (!modelState.IsValid)
            NotificarErroModelInvalida(modelState);

        return CustomResponse();
    }

    protected ActionResult CustomResponse(object result = null)
    {
        if (OperacaoInvalida())
        {
            return Ok(new
            {
                success = true,
                data = result
            });
        }

        return base.BadRequest(new
        {
            success = false,
            errors = ObterMessagensErro()
        });
    }

    private IEnumerable<string> ObterMessagensErro()
    {
        return _notificador.ObterNotificacoes().Select(x => x.Mensagem);
    }

    private bool OperacaoInvalida()
    {
        return !_notificador.TemNotificacao();
    }

    private void NotificarErroModelInvalida(ModelStateDictionary modelState)
    {
        var erros = modelState.Values.SelectMany(e => e.Errors);

        foreach (var erro in erros)
        {
            var errorMessage = erro.Exception == null ? erro.ErrorMessage : erro.Exception.Message;
            NotificarErro(errorMessage);
        }
    }

    protected void NotificarErro(string errorMessage)
    {
        _notificador.Handle(new Notificacao(errorMessage));
    }
}
