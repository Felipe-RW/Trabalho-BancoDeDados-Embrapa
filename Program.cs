using embrapa_bd;
using embrapa_bd.Data;
using embrapa_bd.Models;
using emprapa_bd.Models;
using MySql.Data.MySqlClient;

UsuarioRepositorio usuarioRepo = new UsuarioRepositorio();
AcaoRepositorio acaoRepo = new AcaoRepositorio();
CategoriaRepositorio categoriaRepo = new CategoriaRepositorio();
LeituraTabelaRepositorio leituraRepo = new LeituraTabelaRepositorio();
ValidacaoAcaoRepositorio validacaoRepo = new ValidacaoAcaoRepositorio();

while (true)
{
    Console.WriteLine("\nMenu Administrador");
    Console.WriteLine("1 - Cadastrar usuário\n2 - Editar usuário\n3 - Desativar usuário");
    Console.WriteLine("4 - Cadastrar ação\n5 - Editar ação\n6 - Validar ação");
    Console.WriteLine("7 - Cadastrar categoria\n8 - Consultar tabelas\n0 - Sair");
    try
    {
        switch (Console.ReadLine()?.Trim())
        {
            case "1": CadastrarUsuario(usuarioRepo); break;
            case "2": EditarUsuarioConsole.EditarUsuario(usuarioRepo); break;
            case "3": DesativarUsuario.Desativar(usuarioRepo); break;
            case "4": CadastrarAcao(acaoRepo); break;
            case "5": EditarAcaoConsole.EditarAcao(acaoRepo); break;
            case "6": ValidacaoAcaoConsole.ValidarAcao(validacaoRepo); break;
            case "7": CadastrarCategoriaConsole.CadastrarCategoria(categoriaRepo); break;
            case "8": LerTabelaDoBancoDeDadosConsole.LerTabelaDoBancoDeDados(leituraRepo); break;
            case "0": case null: return;
            default: Console.WriteLine("Opção inválida."); break;
        }
    }
    catch (ArgumentException ex) { Console.WriteLine(ex.Message); }
    catch (MySqlException) { Console.WriteLine("Não foi possível concluir a operação. Verifique o banco e os dados informados."); }
}

static void CadastrarUsuario(UsuarioRepositorio usuarioRepo)
{
    Usuarios usuario = new Usuarios();
    Console.WriteLine("Nome:");
    usuario.Nome = Console.ReadLine() ?? "";
    Console.WriteLine("Email:");
    usuario.Email = Console.ReadLine() ?? "";
    Console.WriteLine("Senha:");
    usuario.Senha = Console.ReadLine() ?? "";
    Console.WriteLine("Matrícula (número inteiro):");
    usuario.Matricula = Console.ReadLine() ?? "";
    if (string.IsNullOrWhiteSpace(usuario.Nome) || string.IsNullOrWhiteSpace(usuario.Email) ||
        string.IsNullOrWhiteSpace(usuario.Senha) || !int.TryParse(usuario.Matricula, out _))
        throw new ArgumentException("Preencha todos os campos e informe matrícula numérica.");
    usuarioRepo.Cadastrar(usuario);
    Console.WriteLine("Usuário cadastrado com sucesso!");
}

static void CadastrarAcao(AcaoRepositorio acaoRepo)
{
    Acao acao = new Acao();
    Console.WriteLine("Título da ação:");
    acao.Titulo = Console.ReadLine() ?? "";
    Console.WriteLine("Descrição:");
    acao.Descricao = Console.ReadLine() ?? "";
    Console.WriteLine("ID da categoria:");
    if (!int.TryParse(Console.ReadLine(), out int categoriaId) || categoriaId <= 0)
        throw new ArgumentException("Categoria inválida.");
    acao.CategoriaId = categoriaId;
    Console.WriteLine("Data (dd/mm/aaaa):");
    if (!DateTime.TryParse(Console.ReadLine(), System.Globalization.CultureInfo.GetCultureInfo("pt-BR"),
        System.Globalization.DateTimeStyles.None, out DateTime data))
        throw new ArgumentException("Data inválida.");
    acao.DataRealizacao = data;
    acaoRepo.Cadastrar(acao);
    Console.WriteLine("Ação cadastrada com sucesso! Status inicial: Em Avaliação.");
}
