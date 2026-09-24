using System.Configuration;

namespace Semana04_XS;

public static class DbConfig
{
    public static string ConnectionString =>
        Environment.GetEnvironmentVariable("NEPTUNO_CONNECTION_STRING")
        ?? ConfigurationManager.ConnectionStrings["NeptunoDB"]?.ConnectionString
        ?? throw new ConfigurationErrorsException("No se encontró la cadena de conexión NeptunoDB en el App.config del proyecto WPF.");
}
