using AutoMapper;
using DevIO.API.ViewModels;
using DevIO.Business.Intefaces;
using DevIO.Business.Models;
using Microsoft.AspNetCore.Mvc;

namespace DevIO.API.Controllers;

public class ProdutosController : MainController
{
    private readonly IProdutoRepository _produtoRepository;
    private readonly IProdutoService _produtoService;
    private readonly IMapper _mapper;

    public ProdutosController(
        INotificador notificador,
        IProdutoRepository produtoRepository,
        IProdutoService produtoService,
        IMapper mapper,
        IUser appUser) : base(notificador, appUser)
    {
        _produtoRepository = produtoRepository;
        _produtoService = produtoService;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult> ObterTodos()
    {
        var produtosViewModel = _mapper.Map<IEnumerable<ProdutoViewModel>>(
            await _produtoRepository.ObterProdutosFornecedores());

        return CustomResponse(produtosViewModel);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult> ObterPorId(Guid id)
    {
        var viewModel = await ObterProduto(id);

        if (viewModel == null)
            return NotFound();

        return CustomResponse(viewModel);
    }

    [HttpPost]
    public async Task<ActionResult> Adicionar(ProdutoViewModel viewModel)
    {
        if (!ModelState.IsValid)
            return CustomResponse(ModelState);

        var nome = $"{Guid.NewGuid()}_{viewModel.Imagem}";

        if (!UploadArquivoAsync(viewModel.ImagemUpload, nome).Result)
            return CustomResponse(viewModel);

        viewModel.Imagem = nome;

        await _produtoService.Adicionar(_mapper.Map<Produto>(viewModel));

        return CustomResponse(viewModel);
    }

    [HttpPut]
    public async Task<ActionResult> Atualizar(ProdutoViewModel viewModel)
    {
        if (!ModelState.IsValid)
            return CustomResponse(ModelState);

        if (!string.IsNullOrEmpty(viewModel.ImagemUpload))
        {
            var model = await ObterProduto(viewModel.Id);

            if (!string.IsNullOrEmpty(model?.Imagem))
                RemoverArquivo(model.Imagem);

            var nome = $"{Guid.NewGuid()}_{viewModel.Imagem}";

            if (!UploadArquivoAsync(viewModel.ImagemUpload, nome).Result)
                return CustomResponse(viewModel);

            viewModel.Imagem = nome;

            await _produtoService.Atualizar(_mapper.Map<Produto>(viewModel));
        }


        return CustomResponse(viewModel);
    }

    [HttpDelete]
    public async Task<ActionResult> Deletar(Guid id)
    {
        var viewModel = await ObterProduto(id);

        if (viewModel == null)
            return NotFound();

        await _produtoService.Remover(id);

        return CustomResponse();
    }

    private async Task<bool> UploadArquivoAsync(string arquivo, string nome)
    {
        if (!string.IsNullOrEmpty(arquivo))
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/imagens", nome);

            if (!System.IO.File.Exists(filePath))
            {
                var data = Convert.FromBase64String(arquivo);
                await System.IO.File.WriteAllBytesAsync(filePath, data);
                return true;
            }

            NotificarErro("Já existe um arquivo com este nome!");
            return false;
        }

        NotificarErro("Forneça uma imagem pra este prouto!");
        return false;
    }

    private void RemoverArquivo(string nome)
    {
        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/imagens", nome);

        if (System.IO.File.Exists(filePath))
            System.IO.File.Delete(filePath);
    }

    private async Task<ProdutoViewModel> ObterProduto(Guid id) =>
        _mapper.Map<ProdutoViewModel>(await _produtoRepository.ObterPorId(id));
}