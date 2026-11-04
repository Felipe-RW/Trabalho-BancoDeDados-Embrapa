using MySql.Data.MySqlClient;
using embrapa_bd.Models;

namespace embrapa_bd.Data
{
    public class AcaoRepositorio
    {
        public void Cadastrar(Acao acao, int usuarioId)
        {
            Conexao conexao = new Conexao();
            MySqlConnection con = conexao.Conectar();

            con.Open();

            MySqlTransaction transacao = con.BeginTransaction();

            try
            {
                // Cadastra a ação.
                string sql = "INSERT INTO acao " +
                             "(titulo, descricao, data_de_realizacao, status_acao, categoria_id) " +
                             "VALUES (@titulo, @descricao, @data, @status, @categoria_id)";

                MySqlCommand comando = new MySqlCommand(sql, con, transacao);

                comando.Parameters.AddWithValue("@titulo", acao.Titulo);
                comando.Parameters.AddWithValue("@descricao", acao.Descricao);
                comando.Parameters.AddWithValue("@data", acao.DataRealizacao);
                comando.Parameters.AddWithValue("@status", "Em Avaliação");
                comando.Parameters.AddWithValue("@categoria_id", acao.CategoriaId);

                comando.ExecuteNonQuery();

                // Recupera o ID da ação cadastrada.
                int acaoId = (int)comando.LastInsertedId;

                // Relaciona a ação ao pesquisador.
                string sqlRelacao = "INSERT INTO acao_usuario (usuario_id, acao_id) " +
                                    "VALUES (@usuario_id, @acao_id)";

                MySqlCommand comandoRelacao = new MySqlCommand(sqlRelacao, con, transacao);

                comandoRelacao.Parameters.AddWithValue("@usuario_id", usuarioId);
                comandoRelacao.Parameters.AddWithValue("@acao_id", acaoId);

                comandoRelacao.ExecuteNonQuery();

                transacao.Commit();
            }
            catch
            {
                transacao.Rollback();
                throw;
            }
            finally
            {
                con.Close();
            }
        }
    }
}