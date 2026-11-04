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
                    sql = @"
                        SELECT u.id, u.nome, u.email, u.matricula, u.ativo,
                            (SELECT GROUP_CONCAT(DISTINCT CONCAT(c.id, ' - ', c.titulo)
                                ORDER BY c.id SEPARATOR '; ')
                             FROM usuario_cargo uc
                             INNER JOIN cargo c ON c.id = uc.cargo_id
                             WHERE uc.usuario_id = u.id) AS cargos,
                            (SELECT GROUP_CONCAT(DISTINCT CONCAT(a.id, ' - ', a.titulo)
                                ORDER BY a.id SEPARATOR '; ')
                             FROM acao_usuario au
                             INNER JOIN acao a ON a.id = au.acao_id
                             WHERE au.usuario_id = u.id) AS acoes,
                            (SELECT GROUP_CONCAT(CONCAT(a.id, ' - ', a.titulo,
                                ' | Motivo registrado: ', v.motivo_rejeicao)
                                ORDER BY a.id SEPARATOR '; ')
                             FROM validacao v
                             INNER JOIN acao a ON a.id = v.acao_id
                             WHERE v.usuario_id = u.id) AS validacoes_realizadas
                        FROM usuario u ORDER BY u.id";
                    break;
                case 2:
                    sql = @"
                        SELECT a.id, a.titulo, a.descricao, a.data_de_realizacao,
                            a.status_acao, a.categoria_id, c.nome AS categoria,
                            (SELECT GROUP_CONCAT(DISTINCT CONCAT(u.id, ' - ', u.nome)
                                ORDER BY u.id SEPARATOR '; ')
                             FROM acao_usuario au
                             INNER JOIN usuario u ON u.id = au.usuario_id
                             WHERE au.acao_id = a.id) AS participantes,
                            (SELECT GROUP_CONCAT(CONCAT(u.id, ' - ', u.nome,
                                ' | Motivo registrado: ', v.motivo_rejeicao)
                                ORDER BY u.id SEPARATOR '; ')
                             FROM validacao v
                             INNER JOIN usuario u ON u.id = v.usuario_id
                             WHERE v.acao_id = a.id) AS validacoes
                        FROM acao a
                        LEFT JOIN categoria c ON c.id = a.categoria_id
                        ORDER BY a.id";
                    break;
                case 3:
                    sql = "SELECT id, nome FROM categoria ORDER BY id";
                    break;
                case 4:
                    sql = @"
                        SELECT c.id, c.titulo,
                            (SELECT GROUP_CONCAT(DISTINCT CONCAT(u.id, ' - ', u.nome,
                                ' | Email: ', u.email, ' | Matrícula: ', u.matricula)
                                ORDER BY u.id SEPARATOR '; ')
                             FROM usuario_cargo uc
                             INNER JOIN usuario u ON u.id = uc.usuario_id
                             WHERE uc.cargo_id = c.id) AS usuarios
                        FROM cargo c ORDER BY c.id";
                    break;
                case 5:
                    sql = @"
                        SELECT l.id, u.matricula, u.nome AS responsavel,
                            l.operacao, l.tabela_afetada, l.id_registrado, l.data_hora
                        FROM log_usuario l
                        LEFT JOIN usuario u ON u.id = l.matricula_id
                        ORDER BY l.data_hora DESC, l.id DESC";
                    break;
                case 6:
                    sql = @"
                        SELECT v.usuario_id AS id_usuario, u.nome AS responsavel,
                            v.acao_id AS id_acao, a.titulo AS acao,
                            NULLIF(v.motivo_rejeicao, '') AS motivo_rejeicao
                        FROM validacao v
                        LEFT JOIN acao a ON a.id = v.acao_id
                        LEFT JOIN usuario u ON u.id = v.usuario_id
                        ORDER BY v.acao_id, v.usuario_id";
                    break;
                case 7:
                    sql = @"
                        SELECT a.id, a.titulo, a.descricao, a.data_de_realizacao,
                            a.status_acao, c.nome AS categoria
                        FROM acao a
                        LEFT JOIN categoria c ON c.id = a.categoria_id
                        WHERE a.status_acao = 'Em Avaliação'
                        ORDER BY a.data_de_realizacao, a.id";
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
