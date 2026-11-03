using System;
using MySql.Data.MySqlClient;

namespace embrapa_bd.Data
{
    public class LogRepositorio
    {
        // No DER matricula_id referencia o ID do usuário, não o número de matrícula.
        public void Registrar(string operacao, string tabelaAfetada, int idRegistrado, int matriculaId)
        {
            if (string.IsNullOrWhiteSpace(operacao) || string.IsNullOrWhiteSpace(tabelaAfetada) ||
                operacao.Length > 100 || tabelaAfetada.Length > 100 || idRegistrado <= 0 || matriculaId <= 0)
                throw new ArgumentException("Informe operação, tabela e IDs válidos.");
            using MySqlConnection con = new Conexao().Conectar();
            using MySqlCommand comando = new MySqlCommand(
                "INSERT INTO log_usuario (operacao, tabela_afetada, id_registrado, data_hora, matricula_id) " +
                "VALUES (@operacao, @tabela, @registro, @data, @usuario)", con);
            comando.Parameters.AddWithValue("@operacao", operacao);
            comando.Parameters.AddWithValue("@tabela", tabelaAfetada);
            comando.Parameters.AddWithValue("@registro", idRegistrado);
            comando.Parameters.AddWithValue("@data", DateTime.Now);
            comando.Parameters.AddWithValue("@usuario", matriculaId);
            con.Open();
            comando.ExecuteNonQuery();
        }
    }
}
