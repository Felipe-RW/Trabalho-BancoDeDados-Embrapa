using System;
using System.Data;
using embrapa_bd.Data;
using MySql.Data.MySqlClient;

namespace embrapa_bd.Models
{
    public static class LerTabelaDoBancoDeDadosConsole
    {
        // Chamada no menu: LerTabelaDoBancoDeDadosConsole.LerTabelaDoBancoDeDados(leituraRepo);
        public static void LerTabelaDoBancoDeDados(LeituraTabelaRepositorio leituraRepo)
        {
            while (true)
            {
                Console.WriteLine("\nConsultar tabela:");
                Console.WriteLine("1 - Usuários");
                Console.WriteLine("2 - Ações");
                Console.WriteLine("3 - Categorias");
                Console.WriteLine("4 - Cargos");
                Console.WriteLine("5 - Logs de operações");
                Console.WriteLine("6 - Validações");
                Console.WriteLine("0 - Voltar");

                string titulo;
                int opcao;
                switch (Console.ReadLine()?.Trim())
                {
                    case "1":
                        opcao = 1;
                        titulo = "Usuários";
                        break;
                    case "2":
                        opcao = 2;
                        titulo = "Ações";
                        break;
                    case "3":
                        opcao = 3;
                        titulo = "Categorias";
                        break;
                    case "4":
                        opcao = 4;
                        titulo = "Cargos";
                        break;
                    case "5":
                        opcao = 5;
                        titulo = "Logs de operações";
                        break;
                    case "6":
                        opcao = 6;
                        titulo = "Validações";
                        break;
                    case "0":
                    case null:
                        return;
                    default:
                        Console.WriteLine("Opção inválida.");
                        continue;
                }

                try
                {
                    using DataTable tabela = leituraRepo.LerTabela(opcao);
                    ExibirTabela(tabela, titulo);
                }
                catch (MySqlException)
                {
                    Console.WriteLine("Não foi possível consultar os dados.");
                }
            }
        }

        private static void ExibirTabela(DataTable tabela, string titulo)
        {
            Console.WriteLine($"\n{titulo}");
            if (tabela.Rows.Count == 0)
            {
                Console.WriteLine("Nenhum registro encontrado.");
                return;
            }

            foreach (DataRow registro in tabela.Rows)
            {
                Console.WriteLine("------------------------------");
                foreach (DataColumn coluna in tabela.Columns)
                {
                    object valor = registro[coluna];
                    string texto = valor switch
                    {
                        DBNull _ => "Não informado",
                        DateTime data => data.ToString(coluna.ColumnName == "data_hora"
                            ? "dd/MM/yyyy HH:mm:ss" : "dd/MM/yyyy"),
                        bool ativo => ativo ? "Sim" : "Não",
                        _ => Convert.ToString(valor) ?? string.Empty
                    };
                    Console.WriteLine($"{coluna.ColumnName}: {texto}");
                }
            }
            Console.WriteLine($"Total de registros: {tabela.Rows.Count}");
        }
    }
}
