using Npgsql;

namespace FinanceManagerAPI.Extensions;

public static class DatabaseConnectionString
{
    public static bool TryConvertDatabaseUrlToNpgsql(string databaseUrl, out string npgsqlConnectionString)
    {
        npgsqlConnectionString = string.Empty;

        if (string.IsNullOrWhiteSpace(databaseUrl))
        {
            return false;
        }

        if (!databaseUrl.StartsWith("postgres://", StringComparison.OrdinalIgnoreCase)
            && !databaseUrl.StartsWith("postgresql://", StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        if (!Uri.TryCreate(databaseUrl, UriKind.Absolute, out var uri))
        {
            return false;
        }

        var userInfo = uri.UserInfo ?? string.Empty;
        var colonIndex = userInfo.IndexOf(':');
        var username = colonIndex >= 0 ? userInfo[..colonIndex] : userInfo;
        var password = colonIndex >= 0 ? userInfo[(colonIndex + 1)..] : string.Empty;

        username = Uri.UnescapeDataString(username);
        password = Uri.UnescapeDataString(password);

        var database = uri.AbsolutePath.TrimStart('/');
        database = Uri.UnescapeDataString(database);

        var builder = new NpgsqlConnectionStringBuilder
        {
            Host = uri.Host,
            Port = uri.IsDefaultPort ? 5432 : uri.Port,
            Database = string.IsNullOrWhiteSpace(database) ? "postgres" : database,
            Username = username,
            Password = password,
            Pooling = true,
            SslMode = SslMode.Require
        };

        var sslmode = TryGetQueryValue(uri.Query, "sslmode");
        if (!string.IsNullOrWhiteSpace(sslmode))
        {
            builder.SslMode = sslmode.Trim().ToLowerInvariant() switch
            {
                "disable" => SslMode.Disable,
                "allow" => SslMode.Allow,
                "prefer" => SslMode.Prefer,
                "require" => SslMode.Require,
                "verify-ca" => SslMode.VerifyCA,
                "verify-full" => SslMode.VerifyFull,
                _ => builder.SslMode
            };
        }

        npgsqlConnectionString = builder.ConnectionString;
        return true;
    }

    public static bool LooksLikeNpgsqlConnectionString(string connectionString)
    {
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            return false;
        }

        return connectionString.Contains("Host=", StringComparison.OrdinalIgnoreCase)
               || connectionString.Contains("Username=", StringComparison.OrdinalIgnoreCase)
               || connectionString.Contains("Ssl Mode=", StringComparison.OrdinalIgnoreCase);
    }

    private static string? TryGetQueryValue(string query, string key)
    {
        if (string.IsNullOrWhiteSpace(query) || string.IsNullOrWhiteSpace(key))
        {
            return null;
        }

        var trimmed = query;
        if (trimmed.StartsWith("?", StringComparison.Ordinal))
        {
            trimmed = trimmed[1..];
        }

        foreach (var part in trimmed.Split('&', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
        {
            var equalsIndex = part.IndexOf('=');
            var partKey = equalsIndex >= 0 ? part[..equalsIndex] : part;
            if (!partKey.Equals(key, StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            var value = equalsIndex >= 0 ? part[(equalsIndex + 1)..] : string.Empty;
            return Uri.UnescapeDataString(value.Replace('+', ' '));
        }

        return null;
    }
}
