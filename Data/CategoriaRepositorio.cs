using MySql.Data.MySqlClient;
using embrapa_bd.Models;

namespace embrapa_bd.Data
{
    public class CategoriaRepositorio
    {
        // Cadastrar categoria
        public void Cadastrar(Categoria categoria)
        {
            Conexao conexao = new Conexao();
            MySqlConnection con = conexao.Conectar();

            string sql = "INSERT INTO categoria (nome) VALUES (@nome)";

            MySqlCommand comando = new MySqlCommand(sql, con);

            comando.Parameters.AddWithValue("@nome", categoria.Nome);

            con.Open();
            comando.ExecuteNonQuery();
            con.Close();
        }
    }
}