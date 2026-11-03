//- Cadastro de usuário:

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

    usuarioRepo.Cadastrar(usuario);

    Console.WriteLine("Usuário cadastrado com sucesso!");
}

// - Cadastro de Ação:

static void CadastrarAcao(AcaoRepositorio acaoRepo)
{
    Acao acao = new Acao();

    Console.WriteLine("Nome da ação:");
    acao.Nome = Console.ReadLine();

    Console.WriteLine("Descrição:");
    acao.Descricao = Console.ReadLine();

    Console.WriteLine("Tipo:");
    acao.Tipo = Console.ReadLine();

    Console.WriteLine("Data (dd/mm/aaaa):");
    acao.Data = DateTime.Parse(Console.ReadLine());

    acaoRepo.Cadastrar(acao);

    Console.WriteLine("Ação cadastrada com sucesso!");
}

// - Integração com o Menu:

UsuarioRepositorio usuarioRepo = new UsuarioRepositorio();
AcaoRepositorio acaoRepo = new AcaoRepositorio();

MenuAdministrador(usuarioRepo, acaoRepo);