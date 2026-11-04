// Pacote necessário:  dotnet add package MySqlConnector
using System.Data;
using MySqlConnector;

namespace Der.Log;

// ---------- Operações padronizadas (evita digitar texto solto) ----------
public static class Operacao
{
    public const string Insert = "INSERT";
    public const string Update = "UPDATE";
    public const string Delete = "DELETE";
}

// ---------- Modelo: espelha a tabela log_usuario ----------
public class LogUsuario
{
    public int Id { get; set; }
    public string Operacao { get; set; } = "";
    public string TabelaAfetada { get; set; } = "";
    public int IdRegistrado { get; set; }
    public DateTime DataHora { get; set; }
    public int? MatriculaId { get; set; }  // FK -> usuario.id (quem executou a ação; NULL se não havia ninguém logado)
}

// ---------- Repositório do log ----------
public class LogUsuarioRepository
{
    private readonly string _connectionString;

    public LogUsuarioRepository(string connectionString)
        => _connectionString = connectionString;

    /// <summary>
    /// Registra uma linha em log_usuario.
    /// Se receber conexão/transação, usa a mesma (o log só é gravado se a operação principal der certo).
    /// </summary>
    public async Task<long> RegistrarAsync(
        string operacao,
        string tabelaAfetada,
        int idRegistrado,
        int usuarioId,
        MySqlConnection? conexao = null,
        MySqlTransaction? transacao = null)
    {
        const string sql = @"
            INSERT INTO log_usuario (operacao, tabela_afetada, id_registrado, data_hora, matricula_id)
            VALUES (@operacao, @tabela, @idRegistrado, @dataHora, @usuarioId);";

        var conexaoPropria = conexao is null;
        conexao ??= new MySqlConnection(_connectionString);

        try
        {
            if (conexao.State != ConnectionState.Open)
                await conexao.OpenAsync();

            await using var cmd = new MySqlCommand(sql, conexao, transacao);
            cmd.Parameters.AddWithValue("@operacao", operacao);
            cmd.Parameters.AddWithValue("@tabela", tabelaAfetada);
            cmd.Parameters.AddWithValue("@idRegistrado", idRegistrado);
            cmd.Parameters.AddWithValue("@dataHora", DateTime.Now);
            cmd.Parameters.AddWithValue("@usuarioId", usuarioId);

            await cmd.ExecuteNonQueryAsync();
            return cmd.LastInsertedId;
        }
        finally
        {
            if (conexaoPropria)
                await conexao.DisposeAsync();
        }
    }

    /// <summary>Lista os logs (mais recentes primeiro), opcionalmente filtrando por usuário.</summary>
    public async Task<List<LogUsuario>> ListarAsync(int? usuarioId = null)
    {
        var sql = @"SELECT id, operacao, tabela_afetada, id_registrado, data_hora, matricula_id
                    FROM log_usuario "
                  + (usuarioId.HasValue ? "WHERE matricula_id = @usuarioId " : "")
                  + "ORDER BY data_hora DESC;";

        var lista = new List<LogUsuario>();

        await using var conexao = new MySqlConnection(_connectionString);
        await conexao.OpenAsync();

        await using var cmd = new MySqlCommand(sql, conexao);
        if (usuarioId.HasValue)
            cmd.Parameters.AddWithValue("@usuarioId", usuarioId.Value);

        await using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            lista.Add(new LogUsuario
            {
                Id = reader.GetInt32("id"),
                Operacao = reader.GetString("operacao"),
                TabelaAfetada = reader.GetString("tabela_afetada"),
                IdRegistrado = reader.GetInt32("id_registrado"),
                DataHora = reader.GetDateTime("data_hora"),
                MatriculaId = reader.IsDBNull(reader.GetOrdinal("matricula_id")) ? null : reader.GetInt32("matricula_id")
            });
        }
        return lista;
    }
}
