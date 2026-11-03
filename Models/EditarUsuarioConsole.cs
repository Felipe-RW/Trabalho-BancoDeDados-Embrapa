using System;
using embrapa_bd.Data;
using emprapa_bd.Models;
using MySql.Data.MySqlClient;

namespace embrapa_bd
{
    public static class EditarUsuarioConsole
    {
        // Chamada no menu: embrapa_bd.EditarUsuarioConsole.EditarUsuario(usuarioRepo);
        public static void EditarUsuario(UsuarioRepositorio usuarioRepo)
        {
            Console.WriteLine("ID do usuário que deseja editar:");
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

                Console.WriteLine("Deixe o campo em branco para manter o valor atual.");
                Console.WriteLine($"Nome atual: {usuario.Nome}");
                string? nome = Console.ReadLine();
                Console.WriteLine($"Email atual: {usuario.Email}");
                string? email = Console.ReadLine();
                Console.WriteLine("Nova senha (em branco para manter a atual):");
                string? senha = Console.ReadLine();
                Console.WriteLine($"Matrícula atual: {usuario.Matricula}");
                string? matricula = Console.ReadLine();

                bool alterado = false;
                if (!string.IsNullOrWhiteSpace(nome) && nome.Trim() != usuario.Nome)
                {
                    usuario.Nome = nome.Trim();
                    alterado = true;
                }
                if (!string.IsNullOrWhiteSpace(email) && email.Trim() != usuario.Email)
                {
                    usuario.Email = email.Trim();
                    alterado = true;
                }
                if (!string.IsNullOrWhiteSpace(senha) && senha != usuario.Senha)
                {
                    usuario.Senha = senha;
                    alterado = true;
                }
                if (!string.IsNullOrWhiteSpace(matricula) && matricula.Trim() != usuario.Matricula)
                {
                    usuario.Matricula = matricula.Trim();
                    alterado = true;
                }

                if (!alterado)
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

                Console.WriteLine(usuarioRepo.Editar(usuario)
                    ? "Usuário atualizado com sucesso!"
                    : "Nenhuma alteração foi aplicada. O usuário pode ter sido removido ou os dados já estavam atualizados.");
            }
            catch (MySqlException ex) when (ex.Number == 1062)
            {
                Console.WriteLine("Não foi possível editar: email ou outro campo único já cadastrado.");
            }
            catch (MySqlException)
            {
                Console.WriteLine("Não foi possível editar o usuário. Verifique a conexão e a estrutura do banco de dados.");
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
