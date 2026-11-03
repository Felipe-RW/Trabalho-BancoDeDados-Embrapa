using System;
using embrapa_bd.Data;
using MySql.Data.MySqlClient;

namespace embrapa_bd.Models
{
    public static class ValidacaoAcaoConsole
    {
        public static void ValidarAcao(ValidacaoAcaoRepositorio validacaoRepo)
        {
            Console.WriteLine("ID da ação:");
            if (!int.TryParse(Console.ReadLine(), out int acaoId) || acaoId <= 0)
            {
                Console.WriteLine("ID da ação inválido.");
                return;
            }
            Console.WriteLine("ID do usuário responsável pela validação:");
            if (!int.TryParse(Console.ReadLine(), out int usuarioId) || usuarioId <= 0)
            {
                Console.WriteLine("ID do usuário inválido.");
                return;
            }
            Console.WriteLine("1 - Aprovar ação\n2 - Rejeitar ação\n0 - Cancelar");
            bool aprovar;
            switch (Console.ReadLine()?.Trim())
            {
                case "1": aprovar = true; break;
                case "2": aprovar = false; break;
                case "0": case null: return;
                default: Console.WriteLine("Opção inválida."); return;
            }
            string? motivo = null;
            if (!aprovar)
            {
                Console.WriteLine("Motivo da rejeição (até 100 caracteres):");
                motivo = Console.ReadLine();
            }
            Console.WriteLine("Confirmar validação? (S/N):");
            if (!string.Equals(Console.ReadLine()?.Trim(), "S", StringComparison.OrdinalIgnoreCase)) return;
            try
            {
                Console.WriteLine(validacaoRepo.Validar(acaoId, usuarioId, aprovar, motivo)
                    ? "Validação registrada com sucesso!" : "Ação não encontrada.");
            }
            catch (ArgumentException ex) { Console.WriteLine(ex.Message); }
            catch (MySqlException) { Console.WriteLine("Não foi possível validar. Verifique o banco e o ID do usuário."); }
        }
    }
}
