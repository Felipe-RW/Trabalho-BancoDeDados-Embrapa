using System;
using embrapa_bd.Data;
using MySql.Data.MySqlClient;

namespace embrapa_bd.Models
{
    public static class EditarAcaoConsole
    {
        public static void EditarAcao(AcaoRepositorio acaoRepo)
        {
            Console.WriteLine("ID da ação:");
            if (!int.TryParse(Console.ReadLine(), out int id) || id <= 0)
            {
                Console.WriteLine("ID inválido.");
                return;
            }

            try
            {
                Acao? acao = acaoRepo.BuscarPorId(id);
                if (acao == null)
                {
                    Console.WriteLine("Ação não encontrada.");
                    return;
                }

                Console.WriteLine($"Ação selecionada: {acao.Id} - {acao.Titulo}");
                Console.WriteLine("1 - Alterar nome");
                Console.WriteLine("2 - Alterar descrição");
                Console.WriteLine("3 - Alterar categoria");
                Console.WriteLine("4 - Alterar data");
                Console.WriteLine("5 - Vincular pesquisador");
                Console.WriteLine("0 - Cancelar");
                if (!int.TryParse(Console.ReadLine(), out int opcao))
                {
                    Console.WriteLine("Opção inválida.");
                    return;
                }

                switch (opcao)
                {
                    case 1:
                        Console.WriteLine($"Nome atual: {acao.Titulo}");
                        Console.WriteLine("Novo nome:");
                        break;
                    case 2:
                        Console.WriteLine($"Descrição atual: {acao.Descricao}");
                        Console.WriteLine("Nova descrição:");
                        break;
                    case 3:
                        Console.WriteLine($"Categoria atual: {acao.CategoriaId}");
                        Console.WriteLine("ID da nova categoria:");
                        break;
                    case 4:
                        Console.WriteLine($"Data atual: {acao.DataRealizacao:dd/MM/yyyy}");
                        Console.WriteLine("Nova data (dd/mm/aaaa):");
                        break;
                    case 5:
                        Console.WriteLine("ID do pesquisador a vincular (preserva participantes existentes):");
                        break;
                    case 0:
                        Console.WriteLine("Edição cancelada.");
                        return;
                    default:
                        Console.WriteLine("Opção inválida.");
                        return;
                }

                string? valor = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(valor))
                {
                    Console.WriteLine("Nenhuma alteração informada.");
                    return;
                }

                Console.WriteLine("Confirmar edição? (S/N):");
                switch (Console.ReadLine()?.Trim().ToUpperInvariant())
                {
                    case "S":
                        break;
                    case "N":
                    default:
                        Console.WriteLine("Edição cancelada.");
                        return;
                }

                Console.WriteLine(acaoRepo.Editar(id, opcao, valor)
                    ? "Ação atualizada com sucesso!"
                    : "Nenhuma alteração aplicada. A ação pode ter sido removida ou o valor já estava atualizado.");
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine(ex.Message);
            }
            catch (MySqlException ex) when (ex.Number == 1452)
            {
                Console.WriteLine("Pesquisador não encontrado.");
            }
            catch (MySqlException)
            {
                Console.WriteLine("Não foi possível editar a ação.");
            }
        }
    }
}
