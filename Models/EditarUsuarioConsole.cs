using System;
using embrapa_bd.Data;
using embrapa_bd.Models;
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
                Usuario? usuario = usuarioRepo.BuscarPorId(id);
                if (usuario == null)
                {
                    Console.WriteLine("Usuário não encontrado.");
                    return;
                }

                Console.WriteLine($"Usuário selecionado: {usuario.Id} - {usuario.Nome}");
                Console.WriteLine("1 - Alterar nome");
                Console.WriteLine("2 - Alterar senha");
                Console.WriteLine("3 - Alterar email");
                Console.WriteLine("0 - Cancelar");
                if (!int.TryParse(Console.ReadLine(), out int opcao))
                {
                    Console.WriteLine("Opção inválida.");
                    return;
                }

                string valorAtual;
                switch (opcao)
                {
                    case 1:
                        Console.WriteLine($"Nome atual: {usuario.Nome}");
                        Console.WriteLine("Novo nome:");
                        valorAtual = usuario.Nome;
                        break;
                    case 2:
                        Console.WriteLine("Nova senha:");
                        valorAtual = usuario.Senha;
                        break;
                    case 3:
                        Console.WriteLine($"Email atual: {usuario.Email}");
                        Console.WriteLine("Novo email:");
                        valorAtual = usuario.Email;
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
                if (opcao != 2)
                    valor = valor.Trim();
                if (valor == valorAtual)
                {
                    Console.WriteLine("O valor informado já está cadastrado para este usuário.");
                    return;
                }

                Console.WriteLine($"Confirmar edição do usuário {id}? (S/N):");
                switch (Console.ReadLine()?.Trim().ToUpperInvariant())
                {
                    case "S":
                        break;
                    case "N":
                    default:
                        Console.WriteLine("Edição cancelada.");
                        return;
                }

                Console.WriteLine(usuarioRepo.Editar(id, opcao, valor)
                    ? "Usuário atualizado com sucesso!"
                    : "Nenhuma alteração foi aplicada. O usuário pode ter sido removido ou o valor já estava atualizado.");
            }
            catch (MySqlException ex) when (ex.Number == 1062)
            {
                Console.WriteLine("Não foi possível editar: email ou outro campo único já cadastrado.");
            }
            catch (MySqlException)
            {
                Console.WriteLine("Não foi possível editar o usuário.");
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
