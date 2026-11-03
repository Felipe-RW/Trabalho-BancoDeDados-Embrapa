using MySql.Data.MySqlClient;
using embrapa_bd.Models;

namespace embrapa_bd.Data
{
    public class AcaoRepositorio
    {
        public void Cadastrar(Acao acao)
        {
            Conexao conexao = new Conexao();
            MySqlConnection con = conexao.Conectar();

            string sql = "INSERT INTO acoes (pesquisador_id, ano, nome, descricao, tipo, data) " +
                         "VALUES (@pesquisador_id, @ano, @nome, @descricao, @tipo, @data)";

            MySqlCommand comando = new MySqlCommand(sql, con);

            comando.Parameters.AddWithValue("@pesquisador_id", acao.PesquisadorId);
            comando.Parameters.AddWithValue("@ano", acao.Ano);
            comando.Parameters.AddWithValue("@nome", acao.Nome);
            comando.Parameters.AddWithValue("@descricao", acao.Descricao);
            comando.Parameters.AddWithValue("@tipo", acao.Tipo);
            comando.Parameters.AddWithValue("@data", acao.Data);

            con.Open();

            comando.ExecuteNonQuery();

            con.Close();
        }
    }
}
