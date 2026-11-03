using MySql.Data.MySqlClient;

namespace Cadastro_de_usuario.Data;

public class LogRepositorio
{
    private readonly Conexao _conexao;

    public LogRepositorio(Conexao conexao)
    {
        _conexao = conexao;
    }

    public void Registrar(
        string email,
        string acao,
        string descricao,
        string resultado)
    {
        using var conn = _conexao.Criar();
        conn.Open();

        const string sql = @"
            INSERT INTO logs
                (usuario_email, acao, descricao, resultado)
            VALUES
                (@email, @acao, @descricao, @resultado);";

        using var cmd = new MySqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@email", email);
        cmd.Parameters.AddWithValue("@acao", acao);
        cmd.Parameters.AddWithValue("@descricao", descricao);
        cmd.Parameters.AddWithValue("@resultado", resultado);

        cmd.ExecuteNonQuery();
    }
}
