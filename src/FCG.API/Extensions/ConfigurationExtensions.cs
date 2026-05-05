namespace FCG.API.Extensions;

public sealed record ApiConfiguration(
    string ConnectionString,
    string SecretKey,
    string JwtKey,
    string JwtIssuer,
    string JwtAudience,
    int PageSize);

public static class ConfigurationExtensions
{
    public static ApiConfiguration GetApiConfiguration(this IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("A configuracao 'ConnectionStrings:DefaultConnection' e obrigatoria.");

        var secretKey = configuration["chave_secreta"]
            ?? throw new InvalidOperationException("A configuracao 'chave_secreta' e obrigatoria.");

        var jwtKey = configuration["Jwt_Key"]
            ?? throw new InvalidOperationException("A configuracao 'Jwt_Key' e obrigatoria.");

        var jwtIssuer = configuration["Jwt_Issuer"] ?? "FCG.API";
        var jwtAudience = configuration["Jwt_Audience"] ?? "FCG.API";

        var pageSize = configuration.GetValue<int?>("Pagination:PageSize") ?? 30;
        if (pageSize < 1)
        {
            throw new InvalidOperationException("A configuracao 'Pagination:PageSize' deve ser maior ou igual a 1.");
        }

        return new ApiConfiguration(connectionString, secretKey, jwtKey, jwtIssuer, jwtAudience, pageSize);
    }
}
