using System;
using MySql.Data.MySqlClient;
using emprapa_bd.Models;

namespace embrapa_bd.Data
{
    // Métodos de extensão: adicionam operações sem alterar o repositório existente.
    public static class UsuarioRepositorioEdicao
    {
        public static Usuarios? BuscarPorId(this UsuarioRepositorio repositorio, int id)
        {
            if (id <= 0)
                throw new ArgumentOutOfRangeException(nameof(id), "O ID deve ser positivo.");

            using MySqlConnection con = new Conexao().Conectar();
            const string sql = "SELECT id, nome, email, senha, matricula FROM usuarios WHERE id = @id";
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
                Matricula = leitor.GetString("matricula")
            };
        }

        public static bool Editar(this UsuarioRepositorio repositorio, Usuarios usuario)
        {
            ArgumentNullException.ThrowIfNull(usuario);
            if (usuario.Id <= 0)
                throw new ArgumentException("O ID deve ser positivo.", nameof(usuario));

            if (string.IsNullOrWhiteSpace(usuario.Nome) ||
                string.IsNullOrWhiteSpace(usuario.Email) ||
                string.IsNullOrWhiteSpace(usuario.Senha) ||
                string.IsNullOrWhiteSpace(usuario.Matricula))
                throw new ArgumentException("Preencha todos os campos do usuário.", nameof(usuario));

            using MySqlConnection con = new Conexao().Conectar();
            const string sql = "UPDATE usuarios SET nome = @nome, email = @email, " +
                               "senha = @senha, matricula = @matricula WHERE id = @id";
            using MySqlCommand comando = new MySqlCommand(sql, con);
            comando.Parameters.AddWithValue("@id", usuario.Id);
            comando.Parameters.AddWithValue("@nome", usuario.Nome);
            comando.Parameters.AddWithValue("@email", usuario.Email);
            comando.Parameters.AddWithValue("@senha", usuario.Senha);
            comando.Parameters.AddWithValue("@matricula", usuario.Matricula);

            con.Open();
            return comando.ExecuteNonQuery() > 0;
        }
    }
}
