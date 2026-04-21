using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace FCG.API.Middlewares;

public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(HttpContext context, Exception exception, CancellationToken cancellationToken)
    {
        _logger.LogError(exception, "Uma exceção não tratada ocorreu: {Message}", exception.Message);

        var problem = new ProblemDetails
        {
            Status = StatusCodes.Status500InternalServerError,
            Title = "Erro interno no servidor.",
            Detail = exception.Message // Mantivemos a mensagem de erro direto da exception para facilitar depuração no Tech Challenge.
        };

        if (exception is FluentValidation.ValidationException || exception.Message.Contains("já está em uso") || exception.Message.Contains("inválidas") || exception.Message.Contains("não encontrado"))
        {
            problem.Status = StatusCodes.Status400BadRequest;
            problem.Title = "Erro de validação ou regra de negócio.";
        }

        context.Response.StatusCode = problem.Status.Value;
        await context.Response.WriteAsJsonAsync(problem, cancellationToken);

        return true;
    }
}