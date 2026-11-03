using Cadastro_de_usuario.Data;
using Cadastro_de_usuario.Models;
using MySql.Data.MySqlClient;

var repositorio = new UsuarioRepositorio(new Conexao());

try
{
    if(!Login(repositorio)) return;

    MenuPrincipal(repositorio);
}
catch (MySqlException ex)
{
    Console.WriteLine($"Erro de Banco de dados: {ex.Message}");
    Console.WriteLine("Verifique a conexão");
}

static bool Login(UsuarioRepositorio repo)
{
    for(int tentativa = 1; tentativa <= 3; tentativa ++)
    {
        Console.Clear();
        Console.WriteLine("=== LOGIN ===");
        string email = Ler("Email: ");
        string senha = Ler("Senha: ");

        var usuario = repo.Autenticar(email, senha);
        if(usuario != null)
        {
            Console.WriteLine($"Bem Vindo, {usuario.Nome} !");
            return true;
        }
        Console.WriteLine($"Email ou senha Inválidos ({tentativa}/3)");
        Pausar();
    }
    Console.WriteLine($"Número maximo de tentativas atingido");
    return false;
}

static void MenuPrincipal(UsuarioRepositorio repo)
{
    while (true)
    {
        Console.Clear();
        Console.WriteLine("=== Programa Maneiro no Banco de Dados melhor do world best dos bests forever and never");
        Console.WriteLine("1 - Cadastrar");
        Console.WriteLine("2 - Mostrar Cadastros");
        Console.WriteLine("3 - Editar");
        Console.WriteLine("4 - Excluir");
        Console.Write("Opção: ");

        switch (Console.ReadLine())
        {
            case "1": Cadastrar(repo); break;
            case "2": Mostrar(repo); break;
            case "3": Editar(repo); break;
            case "4": Excluir(repo); break;
            case "0": return;
            default: Console.WriteLine("Opção invalida meu consagrado."); Pausar(); break;
        }
    }
}

static void Cadastrar(UsuarioRepositorio repo)
{
    System.Console.WriteLine("\n--- Novo Usuario ---");

    var u = new Usuarios
    {
        Nome = Ler("Nome: "),
        Email = Ler("Email: "),
        Senha = Ler("Senha: "),
    };
    try
    {
        repo.Cadastrar(u);
        Console.WriteLine("Cadastrado com sucesso!");
    }
    catch (MySqlException ex) when (ex.Number == 1062)
    {
        Console.WriteLine("Já existe um usuario com esse email");
    }
    Pausar();
}
static void Mostrar(UsuarioRepositorio repo)
{
    Console.WriteLine("Usuarios Cadastrados!");
    var lista = repo.Listar();
    if(lista.Count == 0)
        Console.WriteLine("Nenhum usuario cadastrado!");
    else
    {
        System.Console.WriteLine($"{"ID", -4} {"NOME", -25} Email");

        lista.ForEach(Console.WriteLine);
    }
    Pausar();
}

static void Editar(UsuarioRepositorio repo)
{
    ListarNaTela(repo);
    if(!int.TryParse(Ler("\nID do usuário a editar: "), out int id))
    {
        Console.WriteLine("ID Inválido");
        Pausar();
        return;
    }
    var u = new Usuarios {Id = id, Nome = Ler("Novo noome: "), Email = Ler("Novo email: ")};
    try
    {
        System.Console.WriteLine(repo.Editar(u) ? "Usuario Atualizado!" : "ID não achado");
    }
    catch (MySqlException ex) when (ex.Number == 1062)
    {
        System.Console.WriteLine("Email já cadastrado por outro usuário.");
    }
    Pausar();
}
static void Excluir(UsuarioRepositorio repo)
{
    ListarNaTela(repo);
    if(!int.TryParse(Ler("\nID do usuário a editar: "), out int id))
    {
        Console.WriteLine("ID Inválido");
        Pausar();
        return;
    }
    if (Ler("Tem Certeza? (s/n): ").ToLower() != "s")
        Console.WriteLine("Operação Cancelada!");
    else
        System.Console.WriteLine(repo.Excluir(id) ? "Usuário Excluido" : "ID não encontrado.");
    Pausar();
}

static void ListarNaTela(UsuarioRepositorio repo)
{
    Console.WriteLine("--- Usuário Cadastrados ---");
    var lista= repo.Listar();
    Console.WriteLine($"{"ID", -4} {"NOME", -25} Email");
    lista.ForEach(Console.WriteLine);
}

static void Pausar()
{
    System.Console.WriteLine("Pressione ENTER para Continuar...");
    Console.ReadLine();
}
static string Ler(string texto)
{
    Console.Write(texto);
    return Console.ReadLine()?.Trim()?? "";
}