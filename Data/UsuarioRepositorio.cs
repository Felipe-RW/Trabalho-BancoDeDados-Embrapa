using MySql.Data.MySqlClient;
using emprapa_bd.Models;

namespace embrapa_bd.Data
{
    public class UsuarioRepositorio
    {
        public void CadastrarUsuario(Usuario usuario)
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

        public bool EditarUsuario(Usuario usuario)
        {
            Conexao conexao = new Conexao();
            MySqlConnection con = conexao.Conectar();

            string sql = "UPDATE usuarios SET nome = @nome, email = @email senha = @senha WHERE id = @id";

            MySqlCommand comando = new MySqlCommand(sql, con);

            comando.Parameters.AddWithValue("@nome", usuario.Nome);
            comando.Parameters.AddWithValue("@email", usuario.Email);
            comando.Parameters.AddWithValue("@senha", usuario.Senha);

            con.Open();

            comando.ExecuteNonQuery();

            con.Close();

            return comando.ExecuteNonQuery() > 0;
                
        }

        public List<Usuario> Listar()
        {
            var lista = new List<Usuario>();

            Conexao conexao = new Conexao();
            using var con = conexao.Conectar();
            con.Open();

            var cmd = new MySqlCommand("SELECT id, nome, email FROM usuarios ORDER BY id",con);
            using var leitor = cmd.ExecuteReader();

        while (leitor.Read())
        {
            lista.Add(new Usuario{
                Id = leitor.GetInt32("id"),
                Nome = leitor.GetString("nome"),
                Email = leitor.GetString("email"),
                Matricula = leitor.GetString("matricula")
            });
        }
        return lista;
        }

        public bool ExcluirUsuario(int id)
        {   
            Conexao conexao = new Conexao();
            MySqlConnection con = conexao.Conectar();
            
            string sql = "DELETE FROM usuarios WHERE id = @id";

            MySqlCommand comando = new MySqlCommand(sql, con);

            comando.Parameters.AddWithValue("@id",id);

            con.Open();

            comando.ExecuteNonQuery();

            con.Close();
            
            return comando.ExecuteNonQuery() > 0;
        }
    }
}