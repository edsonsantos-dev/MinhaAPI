using DevIO.Business.Intefaces;

namespace DevIO.API.Controllers;

public class ProdutosController : MainController
{
    public ProdutosController(INotificador notificador) : base(notificador)
    {

    }
}