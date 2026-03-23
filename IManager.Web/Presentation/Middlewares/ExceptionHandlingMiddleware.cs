namespace IManager.Web.Presentation.Middlewares;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro na requisição {Method} {Path}",
                context.Request.Method,
                context.Request.Path);

            var isAjax = context.Request.Headers["X-Requested-With"] == "XMLHttpRequest";

            if (isAjax)
            {
                context.Response.StatusCode = 500;
                context.Response.ContentType = "application/json";

                var json = System.Text.Json.JsonSerializer.Serialize(new
                {
                    error = "Ocorreu um erro inesperado. Tente novamente mais tarde."
                });

                await context.Response.WriteAsync(json);
            }
            else
            {
                context.Response.Redirect("/Home/Error?statusCode=500");
            }
        }
    }
}
