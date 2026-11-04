using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using embrapa_bd.Data;
using embrapa_bd.Models;

//- Cadastro de usuário:

class Program
{
    // - Repositórios:
    static UsuarioRepositorio usuarioRepo = new UsuarioRepositorio();
    static AcaoRepositorio acaoRepo = new AcaoRepositorio();
    static CategoriaRepositorio categoriaRepo = new CategoriaRepositorio();

    // - Cadastro de usuário:
    
    static void CadastrarUsuario(UsuarioRepositorio usuarioRepo)
    {
        Usuario usuario = new Usuario();

        Console.WriteLine("Nome:");
        usuario.Nome = Console.ReadLine();

        Console.WriteLine("Email:");
        usuario.Email = Console.ReadLine();

        Console.WriteLine("Senha:");
        usuario.Senha = Console.ReadLine();

        Console.WriteLine("Matrícula:");

        if (!int.TryParse(Console.ReadLine(), out int matricula))
        {
            Console.WriteLine("Matrícula inválida.");
            Pausar();
            return;
        }

        usuario.Matricula = matricula;

        // Busca todos os cargos disponíveis no banco.
        List<Cargo> cargos = usuarioRepo.ListarCargos();

        if (cargos.Count == 0)
        {
            Console.WriteLine("Não há cargos cadastrados no banco.");
            Pausar();
            return;
        }

        Console.WriteLine("\nEscolha os cargos do usuário.");
        Console.WriteLine("Digite os números separados por vírgula. Exemplo: 1,3");

        for (int i = 0; i < cargos.Count; i++)
        {
            Console.WriteLine($"{i + 1} - {cargos[i].Titulo}");
        }

        Console.Write("Cargos: ");
        string entrada = Console.ReadLine();

        List<int> cargosIds = new List<int>();

        string[] escolhas = entrada.Split(',');

        foreach (string escolhaTexto in escolhas)
        {
            if (!int.TryParse(escolhaTexto.Trim(), out int escolha) ||
                escolha < 1 || escolha > cargos.Count)
            {
                Console.WriteLine("Uma das opções informadas é inválida.");
                Pausar();
                return;
            }

            int cargoId = cargos[escolha - 1].Id;

            // Evita adicionar o mesmo cargo mais de uma vez.
            if (!cargosIds.Contains(cargoId))
            {
                cargosIds.Add(cargoId);
            }
        }

        try
        {
            usuarioRepo.CadastrarUsuario(usuario, cargosIds);
            Console.WriteLine("Usuário cadastrado com sucesso!");
        }
        catch (MySqlException ex) when (ex.Number == 1062)
        {
            Console.WriteLine("Email ou matrícula já cadastrados.");
        }
        catch (MySqlException)
        {
            Console.WriteLine("Não foi possível cadastrar o usuário.");
        }

        Pausar();
    }

    // - Cadastro de Ação:
    static void CadastrarAcao(AcaoRepositorio acaoRepo, int usuarioId)
    {
        Acao acao = new Acao();

        Console.WriteLine("Título da ação:");
        acao.Titulo = Console.ReadLine();

        Console.WriteLine("Descrição:");
        acao.Descricao = Console.ReadLine();

        Console.WriteLine("Data de realização (dd/mm/aaaa):");

        if (!DateTime.TryParse(Console.ReadLine(), out DateTime data))
        {
            Console.WriteLine("Data inválida.");
            Pausar();
            return;
        }

        acao.DataRealizacao = data;

        Console.WriteLine("ID da categoria:");

        if (!int.TryParse(Console.ReadLine(), out int categoriaId))
        {
            Console.WriteLine("ID da categoria inválido.");
            Pausar();
            return;
        }

        acao.CategoriaId = categoriaId;
        acao.Status = "Em Avaliação";

        try
        {
            acaoRepo.Cadastrar(acao, usuarioId);
            Console.WriteLine("Ação cadastrada e enviada para avaliação!");
        }
        catch (MySqlException)
        {
            Console.WriteLine("Não foi possível cadastrar a ação. Confira a categoria e os dados informados.");
        }

        Pausar();
    }

    // - Cadastro de Categoria:
    static void CadastrarCategoria(CategoriaRepositorio categoriaRepo)
    {
        Categoria categoria = new Categoria();

        Console.WriteLine("Nome da categoria:");
        categoria.Nome = Console.ReadLine();

        categoriaRepo.Cadastrar(categoria);

        Console.WriteLine("Categoria cadastrada com sucesso!");
        Pausar();
    }

    // - Editar Usuário:
    static void EditarUsuario(UsuarioRepositorio usuarioRepo)
    {
        ListarNaTela(usuarioRepo);

        Console.WriteLine("\nID do usuário a editar:");

        if (!int.TryParse(Console.ReadLine(), out int id))
        {
            Console.WriteLine("ID inválido.");
            Pausar();
            return;
        }

        Usuario usuario = new Usuario();
        usuario.Id = id;

        Console.WriteLine("Novo nome:");
        usuario.Nome = Console.ReadLine();

        Console.WriteLine("Novo email:");
        usuario.Email = Console.ReadLine();

        Console.WriteLine("Nova senha:");
        usuario.Senha = Console.ReadLine();

        try
        {
            if (usuarioRepo.EditarUsuario(usuario))
            {
                Console.WriteLine("Usuário atualizado!");
            }
            else
            {
                Console.WriteLine("ID não encontrado.");
            }
        }
        catch (MySqlException ex) when (ex.Number == 1062)
        {
            Console.WriteLine("Email já cadastrado por outro usuário.");
        }

        Pausar();
    }

    // - Desativar Usuário:
    static void DesativarUsuario(UsuarioRepositorio usuarioRepo)
    {
        ListarNaTela(usuarioRepo);

        Console.Write("\nDigite o ID do usuário que deseja desativar: ");

        if (!int.TryParse(Console.ReadLine(), out int id))
        {
            Console.WriteLine("ID inválido.");
            Pausar();
            return;
        }

        Console.Write("Deseja realmente desativar esse usuário? (s/n): ");
        string confirmacao = Console.ReadLine();

        if (confirmacao.ToLower() == "s")
        {
            bool desativado = usuarioRepo.DesativarUsuario(id);

            if (desativado)
            {
                Console.WriteLine("Usuário desativado com sucesso!");
            }
            else
            {
                Console.WriteLine("Não foi possível desativar. Verifique o ID.");
            }
        }

        Pausar();
    }

    // - Listar Usuários na Tela:
    static void ListarNaTela(UsuarioRepositorio usuarioRepo)
    {
        Console.WriteLine("--- Usuários cadastrados ---");

        List<Usuario> lista = usuarioRepo.Listar();

        Console.WriteLine($"{"ID",-4} {"Nome",-25} Email");

        foreach (Usuario usuario in lista)
        {
            Console.WriteLine($"{usuario.Id,-4} {usuario.Nome,-25} {usuario.Email}");
        }
    }

    // - Menu do Administrador:
    static void MenuAdministrador(
        UsuarioRepositorio usuarioRepo,
        CategoriaRepositorio categoriaRepo)
    {
        while (true)
        {
            Console.Clear();

            Console.WriteLine("=== Menu do Administrador ===");
            Console.WriteLine("1 - Cadastrar usuário");
            Console.WriteLine("2 - Listar usuários");
            Console.WriteLine("3 - Editar usuário");
            Console.WriteLine("4 - Desativar usuário");
            Console.WriteLine("5 - Cadastrar categoria");
            Console.WriteLine("6 - Editar ação");
            Console.WriteLine("7 - Validar ação");
            Console.WriteLine("8 - Consultar tabelas");
            Console.WriteLine("0 - Sair");
            Console.Write("Opção: ");

            switch (Console.ReadLine())
            {
                case "1":
                    CadastrarUsuario(usuarioRepo);
                    break;

                case "2":
                    ListarNaTela(usuarioRepo);
                    Pausar();
                    break;

                case "3":
                    embrapa_bd.EditarUsuarioConsole.EditarUsuario(usuarioRepo);
                    Pausar();
                    break;

                case "4":
                    DesativarUsuario(usuarioRepo);
                    break;

                case "5":
                    CadastrarCategoriaConsole.CadastrarCategoria(categoriaRepo);
                    Pausar();
                    break;

                case "6":
                    EditarAcaoConsole.EditarAcao(acaoRepo);
                    Pausar();
                    break;

                case "7":
                    ValidacaoAcaoConsole.ValidarAcao(new ValidacaoAcaoRepositorio());
                    Pausar();
                    break;

                case "8":
                    LerTabelaDoBancoDeDadosConsole.LerTabelaDoBancoDeDados(new LeituraTabelaRepositorio());
                    break;

                case "0":
                    return;

                default:
                    Console.WriteLine("Opção inválida.");
                    Pausar();
                    break;
            }
        }
    }

    // - Menu do Pesquisador:
    static void MenuPesquisador(AcaoRepositorio acaoRepo, Usuario usuarioLogado)
    {
        while (true)
        {
            Console.Clear();

            Console.WriteLine("=== Menu do Pesquisador ===");
            Console.WriteLine("1 - Cadastrar ação");
            Console.WriteLine("0 - Sair");
            Console.Write("Opção: ");

            switch (Console.ReadLine())
            {
                case "1":
                    CadastrarAcao(acaoRepo, usuarioLogado.Id);
                    break;

                case "0":
                    return;

                default:
                    Console.WriteLine("Opção inválida.");
                    Pausar();
                    break;
            }
        }
    }

    // - Menu do Validador:
    static void MenuValidador()
    {
        while (true)
        {
            Console.Clear();

            Console.WriteLine("=== Menu do Validador ===");
            Console.WriteLine("1 - Consultar ações pendentes");
            Console.WriteLine("2 - Validar ou rejeitar ação");
            Console.WriteLine("0 - Sair");
            Console.Write("Opção: ");

            switch (Console.ReadLine())
            {
                case "1":
                    Console.WriteLine("Consulta de ações em avaliação.");
                    Pausar();
                    break;

                case "2":
                    Console.WriteLine("Validação ou rejeição de ação.");
                    Pausar();
                    break;

                case "0":
                    return;

                default:
                    Console.WriteLine("Opção inválida.");
                    Pausar();
                    break;
            }
        }
    }

    // - Menu do Comitê:
    static void MenuComite()
    {
        while (true)
        {
            Console.Clear();

            Console.WriteLine("=== Menu do Comitê ===");
            Console.WriteLine("1 - Consultar log");
            Console.WriteLine("0 - Sair");
            Console.Write("Opção: ");

            switch (Console.ReadLine())
            {
                case "1":
                    Console.WriteLine("Consulta do registro de atividades.");
                    Pausar();
                    break;

                case "0":
                    return;

                default:
                    Console.WriteLine("Opção inválida.");
                    Pausar();
                    break;
            }
        }
    }

    // - Login e escolha de cargo:
    static void Login()
    {
        Console.Clear();

        Console.WriteLine("=== LOGIN ===");

        Console.WriteLine("Email:");
        string email = Console.ReadLine();

        Console.WriteLine("Senha:");
        string senha = Console.ReadLine();

        Usuario usuarioLogado = usuarioRepo.Login(email, senha);

        if (usuarioLogado == null)
        {
            Console.WriteLine("Email ou senha inválidos, ou usuário desativado.");
            Pausar();
            return;
        }

        List<string> cargos = usuarioRepo.BuscarCargos(usuarioLogado.Id);

        if (cargos.Count == 0)
        {
            Console.WriteLine("Este usuário não possui cargos cadastrados.");
            Pausar();
            return;
        }

        Console.WriteLine("\nEscolha o cargo:");

        for (int i = 0; i < cargos.Count; i++)
        {
            Console.WriteLine($"{i + 1} - {cargos[i]}");
        }

        Console.Write("Opção: ");

        if (!int.TryParse(Console.ReadLine(), out int escolha) ||
            escolha < 1 || escolha > cargos.Count)
        {
            Console.WriteLine("Opção inválida.");
            Pausar();
            return;
        }

        string cargoEscolhido = cargos[escolha - 1];

        switch (cargoEscolhido)
        {
            case "Administrador":
                MenuAdministrador(usuarioRepo, categoriaRepo);
                break;

            case "Pesquisador":
                MenuPesquisador(acaoRepo, usuarioLogado);
                break;

            case "Validador":
                MenuValidador();
                break;

            case "Comitê":
                MenuComite();
                break;

            default:
                Console.WriteLine("Cargo não reconhecido.");
                Pausar();
                break;
        }
    }

    // - Pausar a Tela:
    static void Pausar()
    {
        Console.WriteLine("\nPressione ENTER para continuar...");
        Console.ReadLine();
    }

    // - Programa Principal:
    static void Main(string[] args)
    {
        while (true)
        {
            Login();
        }
    }
}