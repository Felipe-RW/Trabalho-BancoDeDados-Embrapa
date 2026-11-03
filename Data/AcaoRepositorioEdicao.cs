using System;
using MySql.Data.MySqlClient;
using embrapa_bd.Models;

namespace embrapa_bd.Data
{
    // Métodos de extensão para preservar o repositório existente.
    public static class AcaoRepositorioEdicao
    {
        public static Acao? BuscarPorId(this AcaoRepositorio repositorio, int id)
        {
            if (id <= 0)
                throw new ArgumentOutOfRangeException(nameof(id), "O ID deve ser positivo.");

            using MySqlConnection con = new Conexao().Conectar();
            const string sql = "SELECT id, pesquisador_id, nome, descricao, tipo, data FROM acoes WHERE id = @id";
            using MySqlCommand comando = new MySqlCommand(sql, con);
            comando.Parameters.AddWithValue("@id", id);

            con.Open();
            using MySqlDataReader leitor = comando.ExecuteReader();
            if (!leitor.Read())
                return null;

            return new Acao
            {
                Id = leitor.GetInt32("id"),
                PesquisadorId = leitor.GetInt32("pesquisador_id"),
                Nome = leitor.GetString("nome"),
                Descricao = leitor.GetString("descricao"),
                Tipo = leitor.GetString("tipo"),
                Data = leitor.GetDateTime("data")
            };
        }

        public static bool Editar(this AcaoRepositorio repositorio, Acao acao)
        {
            ArgumentNullException.ThrowIfNull(acao);
            if (acao.Id <= 0 || acao.PesquisadorId <= 0)
                throw new ArgumentException("Os IDs da ação e do pesquisador devem ser positivos.", nameof(acao));

            if (string.IsNullOrWhiteSpace(acao.Nome) ||
                string.IsNullOrWhiteSpace(acao.Descricao) ||
                string.IsNullOrWhiteSpace(acao.Tipo))
                throw new ArgumentException("Preencha nome, descrição e tipo da ação.", nameof(acao));

            if (acao.Data == default)
                throw new ArgumentException("Informe uma data válida para a ação.", nameof(acao));

            using MySqlConnection con = new Conexao().Conectar();
            const string sql = "UPDATE acoes SET pesquisador_id = @pesquisador_id, ano = @ano, " +
                               "nome = @nome, descricao = @descricao, tipo = @tipo, data = @data WHERE id = @id";
            using MySqlCommand comando = new MySqlCommand(sql, con);
            comando.Parameters.AddWithValue("@id", acao.Id);
            comando.Parameters.AddWithValue("@pesquisador_id", acao.PesquisadorId);
            comando.Parameters.AddWithValue("@ano", acao.Ano);
            comando.Parameters.AddWithValue("@nome", acao.Nome);
            comando.Parameters.AddWithValue("@descricao", acao.Descricao);
            comando.Parameters.AddWithValue("@tipo", acao.Tipo);
            comando.Parameters.AddWithValue("@data", acao.Data);

            con.Open();
            return comando.ExecuteNonQuery() > 0;
        }
    }
}
