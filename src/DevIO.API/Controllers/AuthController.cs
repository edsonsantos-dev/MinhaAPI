﻿using DevIO.API.ViewModels;
using DevIO.Business.Intefaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace DevIO.API.Controllers;


[AllowAnonymous]
public class AuthController : MainController
{
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly UserManager<IdentityUser> _userManager;

    public AuthController(
        INotificador notificador,
        SignInManager<IdentityUser> signInManager,
        UserManager<IdentityUser> userManager) : base(notificador)
    {
        _signInManager = signInManager;
        _userManager = userManager;
    }

    [HttpPost(nameof(Registrar))]
    public async Task<ActionResult> Registrar(RegisterUserViewModel registerUser)
    {
        if (!ModelState.IsValid)
            return CustomResponse(ModelState);

        var user = new IdentityUser
        {
            UserName = registerUser.Email,
            Email = registerUser.Email,
            EmailConfirmed = true
        };

        var result = await _userManager.CreateAsync(user, registerUser.Password);

        if (result.Succeeded)
        {
            await _signInManager.SignInAsync(user, isPersistent: false);
            return CustomResponse(registerUser);
        }

        foreach (var erro in result.Errors)
            NotificarErro(erro.Description);

        return CustomResponse(registerUser);
    }

    [HttpPost(nameof(Login))]
    public async Task<ActionResult> Login(LoginUserViewModel loginUser)
    {
        if (!ModelState.IsValid)
            return CustomResponse(ModelState);

        var result = await _signInManager
            .PasswordSignInAsync(
                loginUser.Email,
                loginUser.Password,
                isPersistent: false,
                lockoutOnFailure: true);

        if (result.Succeeded)
        {
            return CustomResponse(loginUser);
        }

        if (result.IsLockedOut)
        {
            NotificarErro("Usuário temporariamente bloqueado por tentativas inválidas.");
            return CustomResponse(loginUser);
        }

        NotificarErro("Usuário ou Senha incorretos");
        return CustomResponse(loginUser);
    }
}
