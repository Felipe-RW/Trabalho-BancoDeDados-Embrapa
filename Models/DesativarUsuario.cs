using System;
using embrapa_bd.Data;
using embrapa_bd.Models;
using MySql.Data.MySqlClient;

namespace embrapa_bd
{
    public static class DesativarUsuario
    {
        public static void usuario(UsuarioRepositorio usuarioRepo)
        {
            Console.WriteLine("ID do usuário que deseja desativar:");
            if (!int.TryParse(Console.ReadLine(), out int id) || id <= 0)
            {
                Console.WriteLine("ID inválido. Informe um número inteiro positivo.");
                return;
            }

            try
            {
                Usuarios? usuario = usuarioRepo.BuscarPorId(id);
                if (usuario == null)
                {
                    Console.WriteLine("Usuário não encontrado.");
                    return;
                }

                Console.WriteLine($"Usuário encontrado: {usuario.Nome} ({usuario.Email})");
                Console.WriteLine("Confirmar desativação? (S/N):");
                if (!string.Equals(Console.ReadLine()?.Trim(), "S", StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine("Desativação cancelada.");
                    return;
                }

                Console.WriteLine(usuarioRepo.Desativar(id)
                    ? "Usuário desativado com sucesso!"
                    : "Nenhuma alteração foi aplicada. O usuário pode já estar desativado.");
            }
            catch (MySqlException)
            {
                Console.WriteLine("Não foi possível desativar o usuário. Verifique a conexão e a estrutura do banco de dados.");
            }
        }
    }
}