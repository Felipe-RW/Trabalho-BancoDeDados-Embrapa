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

            string sql = "INSERT INTO acoes (titulo, descricao, status, categoria_id, data) " +
                         "VALUES (@titulo, @descricao, @status, @categoria_id, @data)";

            MySqlCommand comando = new MySqlCommand(sql, con);

            comando.Parameters.AddWithValue("@titulo", acao.Titulo);
            comando.Parameters.AddWithValue("@descricao", acao.Descricao);
            comando.Parameters.AddWithValue("@status", acao.Status);
            comando.Parameters.AddWithValue("@categoria_id", acao.CategoriaId);
            comando.Parameters.AddWithValue("@data", acao.DataRealizacao);

            con.Open();

            comando.ExecuteNonQuery();

            con.Close();
        }
    }
}
