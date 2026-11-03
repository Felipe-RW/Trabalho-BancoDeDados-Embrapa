using MySql.Data.MySqlClient;
using embrapa_bd.Models;

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

        public Usuarios? BuscarPorId(int id)
        {
            Conexao conexao = new Conexao();
            using MySqlConnection con = conexao.Conectar();
            string sql = "SELECT id, nome, email, senha, matricula, ativo FROM usuarios WHERE id = @id";

            using MySqlCommand comando = new MySqlCommand(sql, con);
            comando.Parameters.AddWithValue("@id", id);

            con.Open();
            using MySqlDataReader leitor = comando.ExecuteReader();

            if (!leitor.Read())
                return null;

            return new Usuarios
            {
                Id = leitor.GetInt32("id"),
                Nome = leitor.GetString("nome"),
                Email = leitor.GetString("email"),
                Senha = leitor.GetString("senha"),
                Matricula = leitor.IsDBNull(leitor.GetOrdinal("matricula")) ? string.Empty : leitor.GetString("matricula"),
                Ativo = !leitor.IsDBNull(leitor.GetOrdinal("ativo")) && leitor.GetBoolean("ativo")
            };
        }

        public bool Desativar(int id)
        {
            Conexao conexao = new Conexao();
            using MySqlConnection con = conexao.Conectar();
            string sql = "UPDATE usuarios SET ativo = 0 WHERE id = @id AND ativo <> 0";

            using MySqlCommand comando = new MySqlCommand(sql, con);
            comando.Parameters.AddWithValue("@id", id);

            con.Open();
            return comando.ExecuteNonQuery() > 0;
        }
    }
}