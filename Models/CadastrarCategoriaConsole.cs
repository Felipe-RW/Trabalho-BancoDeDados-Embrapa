using System;
using embrapa_bd.Data;
using MySql.Data.MySqlClient;

namespace embrapa_bd.Models
{
    public static class CadastrarCategoriaConsole
    {
        // Chamada no menu: CadastrarCategoriaConsole.CadastrarCategoria(categoriaRepo);
        public static void CadastrarCategoria(CategoriaRepositorio categoriaRepo)
        {
            Console.WriteLine("Nome da categoria da ação:");
            string? nome = Console.ReadLine()?.Trim();
            if (string.IsNullOrWhiteSpace(nome))
            {
                Console.WriteLine("Informe o nome da categoria.");
                return;
            }
            if (nome.Length > Categoria.LimiteNome)
            {
                Console.WriteLine("O nome da categoria é muito longo.");
                return;
            }

            try
            {
                Categoria categoria = new Categoria { Nome = nome };
                categoriaRepo.Cadastrar(categoria);
                Console.WriteLine("Categoria cadastrada com sucesso!");
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine(ex.Message);
            }
            catch (MySqlException ex) when (ex.Number == 1062)
            {
                Console.WriteLine("Não foi possível cadastrar: um campo único já está cadastrado.");
            }
            catch (MySqlException)
            {
                Console.WriteLine("Não foi possível cadastrar a categoria.");
            }
        }
    }
}
