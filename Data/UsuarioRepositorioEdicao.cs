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
            const string sql = "SELECT id, nome, email, senha, matricula FROM usuario WHERE id = @id";
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
                Matricula = Convert.ToString(leitor["matricula"]) ?? string.Empty
            };
        }

        public static bool Editar(this UsuarioRepositorio repositorio, int id, int opcao, string valor)
        {
            if (id <= 0)
                throw new ArgumentOutOfRangeException(nameof(id), "O ID deve ser positivo.");
            if (string.IsNullOrWhiteSpace(valor))
                throw new ArgumentException("O novo valor não pode ficar em branco.", nameof(valor));

            // Cada opção altera apenas uma coluna de um único usuário.
            // Os comandos são fixos; nenhum nome de coluna vem da entrada do usuário.
            string sql;
            switch (opcao)
            {
                case 1:
                    sql = "UPDATE usuario SET nome = @valor WHERE id = @id";
                    valor = valor.Trim();
                    break;
                case 2:
                    sql = "UPDATE usuario SET senha = @valor WHERE id = @id";
                    break;
                case 3:
                    sql = "UPDATE usuario SET email = @valor WHERE id = @id";
                    valor = valor.Trim();
                    break;
                default:
                    throw new ArgumentException("Opção de edição inválida.", nameof(opcao));
            }

            using MySqlConnection con = new Conexao().Conectar();
            using MySqlCommand comando = new MySqlCommand(sql, con);
            comando.Parameters.AddWithValue("@id", id);
            comando.Parameters.AddWithValue("@valor", valor);

            con.Open();
            return comando.ExecuteNonQuery() > 0;
        }
    }
}
