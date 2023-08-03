using AutoMapper;
using DevIO.API.Extensions;
using DevIO.API.ViewModels;
using DevIO.Business.Intefaces;
using DevIO.Business.Models;
using Microsoft.AspNetCore.Mvc;

namespace DevIO.API.Controllers;

public class FornecedoresController : MainController
{
    private readonly IFornecedorService _fornecedorService;
    private readonly IFornecedorRepository _fornecedorRepository;
    private readonly IEnderecoRepository _enderecoRepository;
    private readonly IMapper _mapper;

    public FornecedoresController(
        IFornecedorRepository fornecedorRepository,
        IMapper mapper,
        INotificador notificador,
        IFornecedorService fornecedorService,
        IEnderecoRepository enderecoRepository,
        IUser appUser) : base(notificador, appUser)
    {
        _fornecedorRepository = fornecedorRepository;
        _mapper = mapper;
        _fornecedorService = fornecedorService;
        _enderecoRepository = enderecoRepository;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<FornecedorViewModel>>> ObterTodos()
    {
        var fornecedoresViewModel = _mapper.Map<IEnumerable<FornecedorViewModel>>(await _fornecedorRepository.ObterTodos());

        return CustomResponse(fornecedoresViewModel);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<FornecedorViewModel>> ObterPorId(Guid id)
    {
        var viewModel = _mapper.Map<FornecedorViewModel>(
            await _fornecedorRepository.ObterFornecedorProdutosEndereco(id));

        if (viewModel == null)
            return NotFound();

        return CustomResponse(viewModel);
    }

    [HttpPost]
    [ClaimsAuthorize("Fornecedor", "Adicionar")]
    public async Task<ActionResult> Adicionar(FornecedorViewModel viewModel)
    {
        if (!ModelState.IsValid)
            return CustomResponse(ModelState);

        await _fornecedorService.Adicionar(_mapper.Map<Fornecedor>(viewModel));

        return CustomResponse(viewModel);
    }

    [HttpPut]
    [ClaimsAuthorize("Fornecedor", "Atualizar")]
    public async Task<ActionResult> Atualizar(FornecedorViewModel viewModel)
    {
        if (!ModelState.IsValid)
            return CustomResponse(ModelState);

        await _fornecedorService.Atualizar(_mapper.Map<Fornecedor>(viewModel));

        return CustomResponse(viewModel);
    }

    [HttpDelete]
    [ClaimsAuthorize("Fornecedor", "Deletar")]
    public async Task<ActionResult> Deletar(Guid id)
    {
        var fornecedor = _mapper.Map<Fornecedor>(await _fornecedorRepository.ObterPorId(id));

        if (fornecedor == null)
            return NotFound();

        await _fornecedorService.Remover(id);

        return CustomResponse();
    }

    [HttpGet("ObterEnderecoPorId/{id:guid}")]
    public async Task<ActionResult> ObterEnderecoPorId(Guid id)
    {
        var viewModel = _mapper.Map<EnderecoViewModel>(await _enderecoRepository.ObterPorId(id));

        if (viewModel == null)
            return NotFound();

        return CustomResponse(viewModel);
    }

    [HttpPut("AtualizarEndereco")]
    public async Task<ActionResult> AtualizarEndereco(EnderecoViewModel viewModel)
    {
        if (!ModelState.IsValid)
            return CustomResponse(ModelState);

        await _fornecedorService.AtualizarEndereco(_mapper.Map<Endereco>(viewModel));

        return CustomResponse(viewModel);
    }
}
