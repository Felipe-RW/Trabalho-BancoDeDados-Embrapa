using System;
using embrapa_bd.Models;
using MySql.Data.MySqlClient;

namespace embrapa_bd.Data
{
    public static class AcaoRepositorioEdicao
    {
        public static Acao? BuscarPorId(this AcaoRepositorio repositorio, int id)
        {
            if (id <= 0) throw new ArgumentException("ID inválido.");
            using MySqlConnection con = new Conexao().Conectar();
            using MySqlCommand comando = new MySqlCommand(
                "SELECT id, titulo, descricao, categoria_id, data_de_realizacao, status_acao FROM acao WHERE id = @id", con);
            comando.Parameters.AddWithValue("@id", id);
            con.Open();
            using MySqlDataReader leitor = comando.ExecuteReader();
            if (!leitor.Read()) return null;
            return new Acao
            {
                Id = leitor.GetInt32("id"),
                Titulo = leitor.GetString("titulo"),
                Descricao = leitor.GetString("descricao"),
                CategoriaId = leitor.GetInt32("categoria_id"),
                DataRealizacao = leitor.GetDateTime("data_de_realizacao"),
                Status = leitor.GetString("status_acao")
            };
        }

        public static bool Editar(this AcaoRepositorio repositorio, int id, int opcao, string valor)
        {
            if (id <= 0) throw new ArgumentException("ID inválido.");
            if (string.IsNullOrWhiteSpace(valor)) throw new ArgumentException("Informe o novo valor.");
            string sql;
            object parametro = valor.Trim();
            switch (opcao)
            {
                case 1:
                    if (valor.Trim().Length > 100) throw new ArgumentException("O título deve ter no máximo 100 caracteres.");
                    sql = "UPDATE acao SET titulo = @valor WHERE id = @id";
                    break;
                case 2:
                    sql = "UPDATE acao SET descricao = @valor WHERE id = @id";
                    break;
                case 3:
                    if (!int.TryParse(valor, out int categoriaId) || categoriaId <= 0)
                        throw new ArgumentException("ID da categoria inválido.");
                    parametro = categoriaId;
                    sql = "UPDATE acao SET categoria_id = @valor WHERE id = @id";
                    break;
                case 4:
                    if (!DateTime.TryParse(valor, System.Globalization.CultureInfo.GetCultureInfo("pt-BR"),
                        System.Globalization.DateTimeStyles.None, out DateTime data))
                        throw new ArgumentException("Data inválida. Use dd/mm/aaaa.");
                    parametro = data.Date;
                    sql = "UPDATE acao SET data_de_realizacao = @valor WHERE id = @id";
                    break;
                case 5:
                    if (!int.TryParse(valor, out int pesquisadorId) || pesquisadorId <= 0)
                        throw new ArgumentException("ID do pesquisador inválido.");
                    // O DER atual permite vários participantes: não substituir os já vinculados.
                    sql = "INSERT INTO acao_usuario (usuario_id, acao_id) " +
                        "SELECT @valor, @id FROM acao WHERE id = @id AND NOT EXISTS " +
                        "(SELECT 1 FROM acao_usuario WHERE usuario_id = @valor AND acao_id = @id)";
                    parametro = pesquisadorId;
                    break;
                default:
                    throw new ArgumentException("Opção inválida.");
            }
            using MySqlConnection con = new Conexao().Conectar();
            using MySqlCommand comando = new MySqlCommand(sql, con);
            comando.Parameters.AddWithValue("@id", id);
            comando.Parameters.AddWithValue("@valor", parametro);
            con.Open();
            return comando.ExecuteNonQuery() > 0;
        }
    }
}
