using AutoMapper;
using DevIO.API.ViewModels;
using DevIO.Business.Intefaces;
using DevIO.Business.Models;
using Microsoft.AspNetCore.Mvc;

namespace DevIO.API.Controllers;

[Route("[controller]")]
public class FornecedoresController : MainController
{
    private readonly IFornecedorService _fornecedorService;
    private readonly IFornecedorRepository _fornecedorRepository;
    private readonly IMapper _mapper;
    private readonly INotificador _notificador;

    public FornecedoresController(
        IFornecedorRepository fornecedorRepository,
        IMapper mapper,
        INotificador notificador,
        IFornecedorService fornecedorService)
    {
        _fornecedorRepository = fornecedorRepository;
        _mapper = mapper;
        _notificador = notificador;
        _fornecedorService = fornecedorService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<FornecedorViewModel>>> ObterTodos()
    {
        var fornecedores = _mapper.Map<IEnumerable<FornecedorViewModel>>(await _fornecedorRepository.ObterTodos());

        return Ok(fornecedores);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<FornecedorViewModel>> ObterPorId(Guid id)
    {
        var fornecedor = _mapper.Map<FornecedorViewModel>(
            await _fornecedorRepository.ObterFornecedorProdutosEndereco(id));

        if (fornecedor == null)
            return NotFound();

        return Ok(fornecedor);
    }

    [HttpPost]
    public async Task<ActionResult> Adicionar(FornecedorViewModel viewModel)
    {
        if (!ModelState.IsValid)
            return BadRequest();

        await _fornecedorService.Adicionar(_mapper.Map<Fornecedor>(viewModel));

        if (_notificador.TemNotificacao())
            return BadRequest(_notificador.ObterNotificacoes());

        return Ok();
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult> Atualizar(Guid id, FornecedorViewModel viewModel)
    {
        if (!ModelState.IsValid)
            return BadRequest();

        if (id != viewModel.Id)
            return BadRequest();

        await _fornecedorService.Atualizar(_mapper.Map<Fornecedor>(viewModel));

        if (_notificador.TemNotificacao())
            return BadRequest(_notificador.ObterNotificacoes());

        return Ok();
    }

    [HttpDelete]
    public async Task<ActionResult> Deletar(Guid id)
    {
        var fornecedor = _mapper.Map<Fornecedor>(await _fornecedorRepository.ObterPorId(id));

        if (fornecedor == null)
            return NotFound();

        await _fornecedorService.Remover(id);

        if (_notificador.TemNotificacao())
            return BadRequest(_notificador.ObterNotificacoes());

        return Ok();
    }
}
