using MySql.Data.MySqlClient;
using emprapa_bd.Models;

namespace embrapa_bd.Data
{
    public class UsuarioRepositorio
    {
        public void Cadastrar(Usuarios usuario)
        {
            Conexao conexao = new Conexao();
            MySqlConnection con = conexao.Conectar();

            string sql = "INSERT INTO usuarios (nome, email, senha, matricula) " +
                         "VALUES (@nome, @email, @senha, @matricula)";

            MySqlCommand comando = new MySqlCommand(sql, con);

            comando.Parameters.AddWithValue("@nome", usuario.Nome);
            comando.Parameters.AddWithValue("@email", usuario.Email);
            comando.Parameters.AddWithValue("@senha", usuario.Senha);
            comando.Parameters.AddWithValue("@matricula", usuario.Matricula);

            con.Open();

            comando.ExecuteNonQuery();

            con.Close();
        }
    }
}