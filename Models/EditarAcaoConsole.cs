using System;
using System.Globalization;
using embrapa_bd.Data;
using MySql.Data.MySqlClient;

namespace embrapa_bd.Models
{
    public static class EditarAcaoConsole
    {
        // Chamada no menu: embrapa_bd.Models.EditarAcaoConsole.EditarAcao(acaoRepo);
        public static void EditarAcao(AcaoRepositorio acaoRepo)
        {
            Console.WriteLine("ID da ação que deseja editar:");
            if (!int.TryParse(Console.ReadLine(), out int id) || id <= 0)
            {
                Console.WriteLine("ID inválido. Informe um número inteiro positivo.");
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

                Console.WriteLine("Deixe o campo em branco para manter o valor atual.");
                Console.WriteLine($"ID do pesquisador atual: {acao.PesquisadorId}");
                string? pesquisador = Console.ReadLine();
                int pesquisadorId = acao.PesquisadorId;
                if (!string.IsNullOrWhiteSpace(pesquisador) &&
                    (!int.TryParse(pesquisador, out pesquisadorId) || pesquisadorId <= 0))
                {
                    Console.WriteLine("ID do pesquisador inválido. Informe um número inteiro positivo.");
                    return;
                }

                Console.WriteLine($"Nome atual: {acao.Nome}");
                string? nome = Console.ReadLine();
                Console.WriteLine($"Descrição atual: {acao.Descricao}");
                string? descricao = Console.ReadLine();
                Console.WriteLine($"Tipo atual: {acao.Tipo}");
                string? tipo = Console.ReadLine();
                Console.WriteLine($"Data atual: {acao.Data:dd/MM/yyyy}. Nova data (dd/mm/aaaa):");
                string? entradaData = Console.ReadLine();
                DateTime data = acao.Data;
                if (!string.IsNullOrWhiteSpace(entradaData) &&
                    !DateTime.TryParseExact(entradaData.Trim(), "dd/MM/yyyy",
                        CultureInfo.InvariantCulture, DateTimeStyles.None, out data))
                {
                    Console.WriteLine("Data inválida. Utilize o formato dd/mm/aaaa.");
                    return;
                }

                string novoNome = string.IsNullOrWhiteSpace(nome) ? acao.Nome : nome.Trim();
                string novaDescricao = string.IsNullOrWhiteSpace(descricao) ? acao.Descricao : descricao.Trim();
                string novoTipo = string.IsNullOrWhiteSpace(tipo) ? acao.Tipo : tipo.Trim();

                if (pesquisadorId == acao.PesquisadorId && novoNome == acao.Nome &&
                    novaDescricao == acao.Descricao && novoTipo == acao.Tipo && data == acao.Data)
                {
                    Console.WriteLine("Nenhuma alteração informada.");
                    return;
                }

                Console.WriteLine("Confirmar edição? (S/N):");
                if (!string.Equals(Console.ReadLine()?.Trim(), "S", StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine("Edição cancelada.");
                    return;
                }

                acao.PesquisadorId = pesquisadorId;
                acao.Nome = novoNome;
                acao.Descricao = novaDescricao;
                acao.Tipo = novoTipo;
                acao.Data = data;

                Console.WriteLine(acaoRepo.Editar(acao)
                    ? "Ação atualizada com sucesso!"
                    : "Nenhuma alteração foi aplicada. A ação pode ter sido removida ou os dados já estavam atualizados.");
            }
            catch (MySqlException ex) when (ex.Number == 1452)
            {
                Console.WriteLine("Não foi possível editar: pesquisador ou outro registro relacionado não encontrado.");
            }
            catch (MySqlException ex) when (ex.Number == 1062)
            {
                Console.WriteLine("Não foi possível editar: um campo único já está cadastrado.");
            }
            catch (MySqlException)
            {
                Console.WriteLine("Não foi possível editar a ação. Verifique a conexão e a estrutura do banco de dados.");
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
