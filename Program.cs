using System;
using System.Collections.Generic;
using System.Data;
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

    // - Cadastrar categoria:
    static void CadastrarCategoria(CategoriaRepositorio categoriaRepo)
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
            categoriaRepo.Cadastrar(new Categoria { Nome = nome });
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

    // - Editar usuário:
    static void EditarUsuarioConsole(UsuarioRepositorio usuarioRepo)
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
            if (!string.Equals(Console.ReadLine()?.Trim(), "S", StringComparison.OrdinalIgnoreCase))
            {
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

    // - Editar ação:
    static void EditarAcaoConsole(AcaoRepositorio acaoRepo)
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
            if (!string.Equals(Console.ReadLine()?.Trim(), "S", StringComparison.OrdinalIgnoreCase))
            {
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

    // - Validar ou rejeitar ação:
    static void ValidarAcaoConsole(ValidacaoAcaoRepositorio validacaoRepo, int usuarioId)
    {
        Console.WriteLine("ID da ação:");
        if (!int.TryParse(Console.ReadLine(), out int acaoId) || acaoId <= 0)
        {
            Console.WriteLine("ID da ação inválido.");
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
            Console.WriteLine("Motivo da rejeição:");
            motivo = Console.ReadLine();
        }

        Console.WriteLine("Confirmar validação? (S/N):");
        if (!string.Equals(Console.ReadLine()?.Trim(), "S", StringComparison.OrdinalIgnoreCase))
            return;

        try
        {
            Console.WriteLine(validacaoRepo.Validar(acaoId, usuarioId, aprovar, motivo)
                ? "Validação registrada com sucesso!"
                : "Ação não encontrada.");
        }
        catch (ArgumentException ex)
        {
            Console.WriteLine(ex.Message);
        }
        catch (MySqlException)
        {
            Console.WriteLine("Não foi possível validar. Verifique o banco e o ID do usuário.");
        }
    }

    // - Consultar tabelas:
    static void LerTabelaDoBancoDeDadosConsole(LeituraTabelaRepositorio leituraRepo, int? tabelaFixa = null)
    {
        while (true)
        {
            int opcao;
            string titulo;
            if (tabelaFixa.HasValue)
            {
                opcao = tabelaFixa.Value;
                titulo = opcao switch
                {
                    7 => "Ações pendentes",
                    5 => "Logs de operações",
                    _ => "Consulta"
                };
            }
            else
            {
                Console.WriteLine("\nConsultar tabela:");
                Console.WriteLine("1 - Usuários");
                Console.WriteLine("2 - Ações");
                Console.WriteLine("3 - Categorias");
                Console.WriteLine("4 - Cargos");
                Console.WriteLine("5 - Logs de operações");
                Console.WriteLine("6 - Validações");
                Console.WriteLine("0 - Voltar");
                switch (Console.ReadLine()?.Trim())
                {
                    case "1": opcao = 1; titulo = "Usuários"; break;
                    case "2": opcao = 2; titulo = "Ações"; break;
                    case "3": opcao = 3; titulo = "Categorias"; break;
                    case "4": opcao = 4; titulo = "Cargos"; break;
                    case "5": opcao = 5; titulo = "Logs de operações"; break;
                    case "6": opcao = 6; titulo = "Validações"; break;
                    case "0": case null: return;
                    default: Console.WriteLine("Opção inválida."); continue;
                }
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

            if (tabelaFixa.HasValue)
                return;
        }
    }

    static void ExibirTabela(DataTable tabela, string titulo)
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
        CategoriaRepositorio categoriaRepo,
        int usuarioLogadoId)
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
                    EditarUsuarioConsole(usuarioRepo);
                    Pausar();
                    break;

                case "4":
                    DesativarUsuario(usuarioRepo);
                    break;

                case "5":
                    CadastrarCategoria(categoriaRepo);
                    Pausar();
                    break;

                case "6":
                    EditarAcaoConsole(acaoRepo);
                    Pausar();
                    break;

                case "7":
                    ValidarAcaoConsole(new ValidacaoAcaoRepositorio(), usuarioLogadoId);
                    Pausar();
                    break;

                case "8":
                    LerTabelaDoBancoDeDadosConsole(new LeituraTabelaRepositorio());
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
    static void MenuValidador(int usuarioLogadoId)
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
                    LerTabelaDoBancoDeDadosConsole(new LeituraTabelaRepositorio(), 7);
                    Pausar();
                    break;

                case "2":
                    ValidarAcaoConsole(new ValidacaoAcaoRepositorio(), usuarioLogadoId);
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
                    LerTabelaDoBancoDeDadosConsole(new LeituraTabelaRepositorio(), 5);
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
                MenuAdministrador(usuarioRepo, categoriaRepo, usuarioLogado.Id);
                break;

            case "Pesquisador":
                MenuPesquisador(acaoRepo, usuarioLogado);
                break;

            case "Validador":
                MenuValidador(usuarioLogado.Id);
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