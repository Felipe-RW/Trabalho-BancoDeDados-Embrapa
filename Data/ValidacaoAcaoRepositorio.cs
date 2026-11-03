using System;
using MySql.Data.MySqlClient;

namespace embrapa_bd.Data
{
    // Utiliza as tabelas acao e validacao definidas no DER.
    public class ValidacaoAcaoRepositorio
    {
        public bool Validar(int acaoId, int usuarioId, bool aprovar, string? motivoRejeicao)
        {
            if (acaoId <= 0 || usuarioId <= 0)
                throw new ArgumentException("Os IDs da ação e do usuário devem ser positivos.");

            string motivo = aprovar ? string.Empty : (motivoRejeicao ?? string.Empty).Trim();
            if (!aprovar && string.IsNullOrWhiteSpace(motivo))
                throw new ArgumentException("Informe o motivo da rejeição.");
            if (motivo.Length > 100)
                throw new ArgumentException("O motivo da rejeição deve ter no máximo 100 caracteres.");

            using MySqlConnection con = new Conexao().Conectar();
            con.Open();
            using MySqlTransaction transacao = con.BeginTransaction();

            // O bloqueio impede duas decisões simultâneas sobre a mesma ação.
            using (MySqlCommand consulta = new MySqlCommand(
                "SELECT status_acao FROM acao WHERE id = @id FOR UPDATE", con, transacao))
            {
                consulta.Parameters.AddWithValue("@id", acaoId);
                object? status = consulta.ExecuteScalar();
                if (status == null || status == DBNull.Value)
                    return false;
                if (!string.Equals(Convert.ToString(status), "Em Avaliação", StringComparison.Ordinal))
                    throw new ArgumentException("Esta ação já foi avaliada.");
            }

            using (MySqlCommand atualizar = new MySqlCommand(
                "UPDATE acao SET status_acao = @status WHERE id = @id", con, transacao))
            {
                atualizar.Parameters.AddWithValue("@status", aprovar ? "Aprovado" : "Negado");
                atualizar.Parameters.AddWithValue("@id", acaoId);
                atualizar.ExecuteNonQuery();
            }

            using (MySqlCommand registrar = new MySqlCommand(
                "INSERT INTO validacao (motivo_rejeicao, usuario_id, acao_id) " +
                "VALUES (@motivo, @usuario_id, @acao_id)", con, transacao))
            {
                registrar.Parameters.AddWithValue("@motivo", motivo);
                registrar.Parameters.AddWithValue("@usuario_id", usuarioId);
                registrar.Parameters.AddWithValue("@acao_id", acaoId);
                registrar.ExecuteNonQuery();
            }

            transacao.Commit();
            return true;
        }
    }
}
