using MySql.Data.MySqlClient;
using System.Collections.Generic;
using embrapa_bd.Models;

namespace embrapa_bd.Data
{
    public class UsuarioRepositorio
    {
        // Cadastrar usuário e associar cargo
        public void CadastrarUsuario(Usuario usuario, List<int> cargosIds)
        {
            Conexao conexao = new Conexao();
            MySqlConnection con = conexao.Conectar();

            con.Open();

            MySqlTransaction transacao = con.BeginTransaction();

            try
            {
                // Cadastra o usuário.
                string sql = "INSERT INTO usuario (nome, email, senha, matricula, ativo) " +
                            "VALUES (@nome, @email, @senha, @matricula, @ativo)";

                MySqlCommand comando = new MySqlCommand(sql, con, transacao);

                comando.Parameters.AddWithValue("@nome", usuario.Nome);
                comando.Parameters.AddWithValue("@email", usuario.Email);
                comando.Parameters.AddWithValue("@senha", usuario.Senha);
                comando.Parameters.AddWithValue("@matricula", usuario.Matricula);
                comando.Parameters.AddWithValue("@ativo", true);

                comando.ExecuteNonQuery();

                // Recupera o ID do usuário recém-cadastrado.
                int usuarioId = (int)comando.LastInsertedId;

                // Associa cada cargo escolhido ao usuário.
                foreach (int cargoId in cargosIds)
                {
                    string sqlCargo = "INSERT INTO usuario_cargo (usuario_id, cargo_id) " +
                                    "VALUES (@usuario_id, @cargo_id)";

                    MySqlCommand comandoCargo =
                        new MySqlCommand(sqlCargo, con, transacao);

                    comandoCargo.Parameters.AddWithValue("@usuario_id", usuarioId);
                    comandoCargo.Parameters.AddWithValue("@cargo_id", cargoId);

                    comandoCargo.ExecuteNonQuery();
                }

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

        // Editar usuário
        public bool EditarUsuario(Usuario usuario)
        {
            Conexao conexao = new Conexao();
            MySqlConnection con = conexao.Conectar();

            string sql = "UPDATE usuario SET nome = @nome, email = @email, " +
                         "senha = @senha WHERE id = @id";

            MySqlCommand comando = new MySqlCommand(sql, con);

            comando.Parameters.AddWithValue("@nome", usuario.Nome);
            comando.Parameters.AddWithValue("@email", usuario.Email);
            comando.Parameters.AddWithValue("@senha", usuario.Senha);
            comando.Parameters.AddWithValue("@id", usuario.Id);

            con.Open();
            int resultado = comando.ExecuteNonQuery();
            con.Close();

            return resultado > 0;
        }

        // Listar usuários
        public List<Usuario> Listar()
        {
            List<Usuario> lista = new List<Usuario>();

            Conexao conexao = new Conexao();
            MySqlConnection con = conexao.Conectar();

            string sql = "SELECT id, nome, email, matricula, ativo " +
                         "FROM usuario ORDER BY id";

            MySqlCommand comando = new MySqlCommand(sql, con);

            con.Open();
            MySqlDataReader leitor = comando.ExecuteReader();

            while (leitor.Read())
            {
                Usuario usuario = new Usuario();

                usuario.Id = leitor.GetInt32("id");
                usuario.Nome = leitor.GetString("nome");
                usuario.Email = leitor.GetString("email");
                usuario.Matricula = leitor.GetInt32("matricula");
                usuario.Ativo = leitor.GetBoolean("ativo");

                lista.Add(usuario);
            }

            leitor.Close();
            con.Close();

            return lista;
        }

        // Desativar usuário
        public bool DesativarUsuario(int id)
        {
            Conexao conexao = new Conexao();
            MySqlConnection con = conexao.Conectar();

            string sql = "UPDATE usuario SET ativo = false WHERE id = @id";

            MySqlCommand comando = new MySqlCommand(sql, con);
            comando.Parameters.AddWithValue("@id", id);

            con.Open();
            int resultado = comando.ExecuteNonQuery();
            con.Close();

            return resultado > 0;
        }

        // Login
        public Usuario Login(string email, string senha)
        {
            Conexao conexao = new Conexao();
            MySqlConnection con = conexao.Conectar();

            string sql = "SELECT id, nome, email, matricula, ativo FROM usuario " +
                         "WHERE email = @email AND senha = @senha AND ativo = true";

            MySqlCommand comando = new MySqlCommand(sql, con);

            comando.Parameters.AddWithValue("@email", email);
            comando.Parameters.AddWithValue("@senha", senha);

            con.Open();
            MySqlDataReader leitor = comando.ExecuteReader();

            Usuario usuario = null;

            if (leitor.Read())
            {
                usuario = new Usuario();

                usuario.Id = leitor.GetInt32("id");
                usuario.Nome = leitor.GetString("nome");
                usuario.Email = leitor.GetString("email");
                usuario.Matricula = leitor.GetInt32("matricula");
                usuario.Ativo = leitor.GetBoolean("ativo");
            }

            leitor.Close();
            con.Close();

            return usuario;
        }

        // Buscar cargos do usuário
        public List<string> BuscarCargos(int idUsuario)
        {
            List<string> cargos = new List<string>();

            Conexao conexao = new Conexao();
            MySqlConnection con = conexao.Conectar();

            string sql = "SELECT c.titulo FROM cargo c " +
                         "INNER JOIN usuario_cargo uc ON c.id = uc.cargo_id " +
                         "WHERE uc.usuario_id = @id_usuario";

            MySqlCommand comando = new MySqlCommand(sql, con);
            comando.Parameters.AddWithValue("@id_usuario", idUsuario);

            con.Open();
            MySqlDataReader leitor = comando.ExecuteReader();

            while (leitor.Read())
            {
                cargos.Add(leitor.GetString("titulo"));
            }

            leitor.Close();
            con.Close();

            return cargos;
        }

        // Listar cargos disponíveis
        public List<Cargo> ListarCargos()
        {
            List<Cargo> lista = new List<Cargo>();

            Conexao conexao = new Conexao();
            MySqlConnection con = conexao.Conectar();

            string sql = "SELECT id, titulo FROM cargo ORDER BY id";

            MySqlCommand comando = new MySqlCommand(sql, con);

            con.Open();
            MySqlDataReader leitor = comando.ExecuteReader();

            while (leitor.Read())
            {
                Cargo cargo = new Cargo();

                cargo.Id = leitor.GetInt32("id");
                cargo.Titulo = leitor.GetString("titulo");

                lista.Add(cargo);
            }

            leitor.Close();
            con.Close();

            return lista;
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