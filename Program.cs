//- Cadastro de usuário:

using embrapa_bd.Data;
using embrapa_bd.Models;
using emprapa_bd.Models;

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
    usuario.Matricula = Console.ReadLine();

    usuarioRepo.CadastrarUsuario(usuario);

    Console.WriteLine("Usuário cadastrado com sucesso!");
}

// - Cadastro de Ação:

static void CadastrarAcao(AcaoRepositorio acaoRepo)
{
    Acao acao = new Acao();

    Console.WriteLine("Nome da ação:");
    acao.Titulo = Console.ReadLine();

    Console.WriteLine("Descrição:");
    acao.Descricao = Console.ReadLine();

    Console.WriteLine("Tipo:");
    acao.Status = Console.ReadLine();

    Console.WriteLine("Data (dd/mm/aaaa):");
    acao.DataRealizacao = DateTime.Parse(Console.ReadLine());

    acaoRepo.Cadastrar(acao);

    Console.WriteLine("Ação cadastrada com sucesso!");
}

// - Cadastro de Categoria:

static void CadastrarCategoria(AcaoRepositorio acaoRepo)
{
    Acao acao = new Acao();

    Console.WriteLine("Nome da Categoria:");
    acao.CategoriaId= Console.ReadLine();

    acaoRepo.Cadastrar(acao);

    Console.WriteLine("Categoria cadastrada com sucesso!");
}

// - Editar Usuario

static void EditarUsuario(UsuarioRepositorio usuarioRepo)
{
    ListarNaTela(usuarioRepo);
    if(!int.TryParse(Usuario.Id))
    {
        Console.WriteLine("ID Inválido");
        Pausar();
        return;
    }
    var u = new Usuario {Id = id, Nome = Console.ReadLine("Novo nome: "), Email = Ler("Novo email: ")};
    try
    {
        System.Console.WriteLine(usuarioRepo.Editar(u) ? "Usuario Atualizado!" : "ID não encontrado");
    }
    catch(MySqlException ex) when (ex.Number == 1062)
    {
        System.Console.WriteLine("Email ja cadastrado por outro usuario.");
    }
    Pausar();
}


// - Desativar usuario

static void DesativarUsuario(UsuarioRepositorio usuarioRepo)
{
    ListarNaTela(repo);
    if(!int.TryParse(("\nID do usuario a desativar: "), out int id))
    {
        Console.WriteLine("ID Inválido");
        Pausar();
        return;
    }
    if(Ler("Tem Certeza ?(s/n)").ToLower() != "s")
        Console.WriteLine("Operação Cancelada !");
    else
        System.Console.WriteLine(usuarioRepo.Desativar(id) ? "Usuario Excluido" : "ID não encontrado.");
    Pausar();
}

// - Listar Na Tela

static void ListarNaTela(UsuarioRepositorio usuarioRepo)
{
    Console.WriteLine("--- Usuarios Cadastrados ---");
    var lista = usuarioRepo.Listar();
    Console.WriteLine($"{"ID", -4} {"Nome", -25} Email");
    lista.ForEach(Console.WriteLine);
}

// - Integração com o Menu:

UsuarioRepositorio usuarioRepo = new UsuarioRepositorio();
AcaoRepositorio acaoRepo = new AcaoRepositorio();

static void MenuAdministrador(UsuarioRepositorio usuarioRepo, AcaoRepositorio acaoRepo)
{
    while (true)
    {
        Console.Clear();
        Console.WriteLine("=== Menu do Administrador ===");
        Console.WriteLine("1 - Cadastrar Categoria");
        Console.WriteLine("2 - Cadastrar Usarios");
        Console.WriteLine("3 - Editar Usuarios");
        Console.WriteLine("4 - Desativar");
        Console.WriteLine("0 - Sair");
        Console.Write("Opção: ");

        switch (Console.ReadLine())
        {
            case "1": CadastrarUsuario(usuarioRepo); break;
            case "2": CadastrarCategoria(acaoRepo); break;
            case "3": EditarUsuario(usuarioRepo); break;
            case "4": DesativarUsuario(usuarioRepo); break;
            case "0": return;
            default: Console.WriteLine("Opção invalida meu consagrado.");Pausar(); break;
        }
    }
}