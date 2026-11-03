using System;
using embrapa_bd.Data;
using MySql.Data.MySqlClient;

namespace embrapa_bd.Models
{
    public static class ValidacaoAcaoConsole
    {
        // O menu deve fornecer o ID do usuário autenticado e autorizado a avaliar.
        // Chamada: ValidacaoAcaoConsole.FazerValidacaoDaAcao(validacaoRepo, usuarioId);
        public static void FazerValidacaoDaAcao(ValidacaoAcaoRepositorio validacaoRepo, int usuarioId)
        {
            if (usuarioId <= 0)
            {
                Console.WriteLine("Usuário responsável pela validação inválido.");
                return;
            }

            Console.WriteLine("ID da ação que deseja avaliar:");
            if (!int.TryParse(Console.ReadLine(), out int acaoId) || acaoId <= 0)
            {
                Console.WriteLine("ID inválido. Informe um número inteiro positivo.");
                return;
            }

            Console.WriteLine("1 - Aprovar ação");
            Console.WriteLine("2 - Negar ação");
            string? opcao = Console.ReadLine()?.Trim();
            if (opcao != "1" && opcao != "2")
            {
                Console.WriteLine("Opção inválida. Validação cancelada.");
                return;
            }

            bool aprovar = opcao == "1";
            string? motivo = null;
            if (!aprovar)
            {
                Console.WriteLine("Motivo da rejeição (obrigatório, até 100 caracteres):");
                motivo = Console.ReadLine()?.Trim();
                if (string.IsNullOrWhiteSpace(motivo) || motivo.Length > 100)
                {
                    Console.WriteLine("Informe um motivo de rejeição com até 100 caracteres.");
                    return;
                }
            }

            Console.WriteLine($"Confirmar {(aprovar ? "aprovação" : "rejeição")} da ação {acaoId}? (S/N):");
            if (!string.Equals(Console.ReadLine()?.Trim(), "S", StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine("Validação cancelada.");
                return;
            }

            try
            {
                bool registrada = validacaoRepo.Validar(acaoId, usuarioId, aprovar, motivo);
                Console.WriteLine(registrada
                    ? (aprovar ? "Ação aprovada com sucesso!" : "Ação negada com sucesso!")
                    : "Ação não encontrada.");
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine(ex.Message);
            }
            catch (MySqlException ex) when (ex.Number == 1452)
            {
                Console.WriteLine("Não foi possível registrar a validação: usuário ou ação não encontrado.");
            }
            catch (MySqlException)
            {
                Console.WriteLine("Não foi possível validar a ação. Verifique a conexão e as tabelas acao e validacao do DER.");
            }
        }
    }
}
