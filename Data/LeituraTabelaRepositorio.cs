using System;
using System.Data;
using MySql.Data.MySqlClient;

namespace embrapa_bd.Data
{
    public class LeituraTabelaRepositorio
    {
        public DataTable LerTabela(int opcao)
        {
            // Consultas fixas do DER: não aceita nomes de tabelas ou SQL digitados.
            string sql;
            switch (opcao)
            {
                case 1:
                    sql = "SELECT id, nome, email, matricula, ativo FROM usuario ORDER BY id";
                    break;
                case 2:
                    sql = "SELECT id, titulo, descricao, data_de_realizacao, status_acao, categoria_id FROM acao ORDER BY id";
                    break;
                case 3:
                    sql = "SELECT id, nome FROM categoria ORDER BY id";
                    break;
                default:
                    throw new ArgumentException("Opção de tabela inválida.", nameof(opcao));
            }

            using MySqlConnection con = new Conexao().Conectar();
            using MySqlCommand comando = new MySqlCommand(sql, con);
            con.Open();
            using MySqlDataReader leitor = comando.ExecuteReader();
            DataTable tabela = new DataTable();
            tabela.Load(leitor);
            return tabela;
        }
    }
}
