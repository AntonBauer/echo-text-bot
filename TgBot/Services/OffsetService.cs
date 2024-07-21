using Microsoft.Data.Sqlite;

namespace TgBot.Services;

internal sealed class OffsetService(string ConnectionString)
{
    private const string SaveOffsetCommand = "INSERT INTO Offset (offset, timestamp) VALUES ($offset, datetime('now', 'localtime'));";
    private const string ReadOffsetCommand = "SELECT offset FROM Offset ORDER BY timestamp DESC LIMIT 1;";

    public async Task<int?> ReadOffset(CancellationToken cancellationToken)
    {
        await using var connection = await OpenConnection(cancellationToken);
        await using var command = connection.CreateCommand();
        command.CommandText = ReadOffsetCommand;

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        if (!reader.HasRows)
            return default;

        while (await reader.ReadAsync(cancellationToken))
        {
            var offset = reader.GetInt32(0);
            return offset;
        }

        return default;
    }

    public async Task SaveOffset(int offset, CancellationToken cancellationToken)
    {
        await using var connection = await OpenConnection(cancellationToken);
        await using var command = connection.CreateCommand();
        command.CommandText = SaveOffsetCommand;
        command.Parameters.AddWithValue("$offset", offset);

        await command.ExecuteNonQueryAsync(cancellationToken);
    }

    private async Task<SqliteConnection> OpenConnection(CancellationToken cancellationToken)
    {
        var connection = new SqliteConnection(ConnectionString);
        await connection.OpenAsync(cancellationToken);

        return connection;
    }
}
