// Usa o LogUsuario.cs (LogUsuarioRepository e Operacao).
// Pacote: dotnet add package MySqlConnector
using MySqlConnector;

namespace Der.Log;

// =========================================================
// MODELOS
// =========================================================
public class Usuario
{
    public int Id { get; set; }
    public string Nome { get; set; } = "";
    public string Email { get; set; } = "";
    public int Matricula { get; set; }
    public bool Ativo { get; set; } = true;
}

public class Acao
{
    public int Id { get; set; }
    public string Titulo { get; set; } = "";
    public string Descricao { get; set; } = "";
    public DateTime DataDeRealizacao { get; set; }
    public string StatusAcao { get; set; } = "Em Avaliação"; // Aprovado | Negado | Em Avaliação
    public int CategoriaId { get; set; }
}

// =========================================================
// BASE: executa o comando e grava o log na MESMA transação.
// Se o comando falhar, o log não é gravado (rollback).
// =========================================================
public abstract class RepositorioBase
{
    protected readonly string ConnectionString;
    protected readonly LogUsuarioRepository Log;

    protected RepositorioBase(string connectionString)
    {
        ConnectionString = connectionString;
        Log = new LogUsuarioRepository(connectionString);
    }

    /// <param name="idRegistrado">id do registro afetado; se null, usa o id gerado pelo INSERT</param>
    /// <param name="usuarioLogadoId">usuario.id de quem está executando a ação</param>
    protected async Task<int> ExecutarComLogAsync(
        string sql,
        Action<MySqlParameterCollection> parametros,
        string operacao,
        string tabela,
        int? idRegistrado,
        int usuarioLogadoId)
    {
        await using var conexao = new MySqlConnection(ConnectionString);
        await conexao.OpenAsync();
        await using var transacao = await conexao.BeginTransactionAsync();

        try
        {
            await using var cmd = new MySqlCommand(sql, conexao, transacao);
            parametros(cmd.Parameters);

            var afetadas = await cmd.ExecuteNonQueryAsync();
            if (afetadas == 0)
                throw new InvalidOperationException("Nenhum registro foi afetado.");

            var id = idRegistrado ?? (int)cmd.LastInsertedId;

            await Log.RegistrarAsync(operacao, tabela, id, usuarioLogadoId, conexao, transacao);

            await transacao.CommitAsync();
            return id;
        }
        catch
        {
            await transacao.RollbackAsync();
            throw;
        }
    }
}

// =========================================================
// CATEGORIA
// =========================================================
public class CategoriaRepository : RepositorioBase
{
    public CategoriaRepository(string cs) : base(cs) { }

    public Task<int> CriarAsync(string nome, int usuarioLogadoId) =>
        ExecutarComLogAsync(
            "INSERT INTO categoria (nome) VALUES (@nome);",
            p => p.AddWithValue("@nome", nome),
            Operacao.Insert, "categoria", null, usuarioLogadoId);

    public Task<int> AtualizarAsync(int id, string nome, int usuarioLogadoId) =>
        ExecutarComLogAsync(
            "UPDATE categoria SET nome = @nome WHERE id = @id;",
            p => { p.AddWithValue("@nome", nome); p.AddWithValue("@id", id); },
            Operacao.Update, "categoria", id, usuarioLogadoId);

    public Task<int> ExcluirAsync(int id, int usuarioLogadoId) =>
        ExecutarComLogAsync(
            "DELETE FROM categoria WHERE id = @id;",
            p => p.AddWithValue("@id", id),
            Operacao.Delete, "categoria", id, usuarioLogadoId);
}

// =========================================================
// CARGO
// =========================================================
public class CargoRepository : RepositorioBase
{
    public CargoRepository(string cs) : base(cs) { }

    public Task<int> CriarAsync(string titulo, int usuarioLogadoId) =>
        ExecutarComLogAsync(
            "INSERT INTO cargo (titulo) VALUES (@titulo);",
            p => p.AddWithValue("@titulo", titulo),
            Operacao.Insert, "cargo", null, usuarioLogadoId);

    public Task<int> AtualizarAsync(int id, string titulo, int usuarioLogadoId) =>
        ExecutarComLogAsync(
            "UPDATE cargo SET titulo = @titulo WHERE id = @id;",
            p => { p.AddWithValue("@titulo", titulo); p.AddWithValue("@id", id); },
            Operacao.Update, "cargo", id, usuarioLogadoId);

    public Task<int> ExcluirAsync(int id, int usuarioLogadoId) =>
        ExecutarComLogAsync(
            "DELETE FROM cargo WHERE id = @id;",
            p => p.AddWithValue("@id", id),
            Operacao.Delete, "cargo", id, usuarioLogadoId);
}

// =========================================================
// USUARIO (+ vínculo com cargo)
// =========================================================
public class UsuarioRepository : RepositorioBase
{
    public UsuarioRepository(string cs) : base(cs) { }

    public Task<int> CriarAsync(Usuario u, int usuarioLogadoId) =>
        ExecutarComLogAsync(
            @"INSERT INTO usuario (nome, email, matricula, ativo)
              VALUES (@nome, @email, @matricula, @ativo);",
            p =>
            {
                p.AddWithValue("@nome", u.Nome);
                p.AddWithValue("@email", u.Email);
                p.AddWithValue("@matricula", u.Matricula);
                p.AddWithValue("@ativo", u.Ativo);
            },
            Operacao.Insert, "usuario", null, usuarioLogadoId);

    public Task<int> AtualizarAsync(Usuario u, int usuarioLogadoId) =>
        ExecutarComLogAsync(
            @"UPDATE usuario
              SET nome = @nome, email = @email, matricula = @matricula, ativo = @ativo
              WHERE id = @id;",
            p =>
            {
                p.AddWithValue("@nome", u.Nome);
                p.AddWithValue("@email", u.Email);
                p.AddWithValue("@matricula", u.Matricula);
                p.AddWithValue("@ativo", u.Ativo);
                p.AddWithValue("@id", u.Id);
            },
            Operacao.Update, "usuario", u.Id, usuarioLogadoId);

    public Task<int> DesativarAsync(int id, int usuarioLogadoId) =>
        ExecutarComLogAsync(
            "UPDATE usuario SET ativo = 0 WHERE id = @id;",
            p => p.AddWithValue("@id", id),
            Operacao.Update, "usuario", id, usuarioLogadoId);

    public Task<int> ExcluirAsync(int id, int usuarioLogadoId) =>
        ExecutarComLogAsync(
            "DELETE FROM usuario WHERE id = @id;",
            p => p.AddWithValue("@id", id),
            Operacao.Delete, "usuario", id, usuarioLogadoId);

    /// <summary>Vincula um cargo ao usuário (tabela usuario_cargo). Log: id do vínculo.</summary>
    public Task<int> AtribuirCargoAsync(int usuarioId, int cargoId, int usuarioLogadoId) =>
        ExecutarComLogAsync(
            "INSERT INTO usuario_cargo (usuario_id, cargo_id) VALUES (@usuarioId, @cargoId);",
            p => { p.AddWithValue("@usuarioId", usuarioId); p.AddWithValue("@cargoId", cargoId); },
            Operacao.Insert, "usuario_cargo", null, usuarioLogadoId);
}

// =========================================================
// ACAO (+ vínculo com usuário e validação)
// =========================================================
public class AcaoRepository : RepositorioBase
{
    public AcaoRepository(string cs) : base(cs) { }

    public Task<int> CriarAsync(Acao a, int usuarioLogadoId) =>
        ExecutarComLogAsync(
            @"INSERT INTO acao (titulo, descricao, data_de_realizacao, status_acao, categoria_id)
              VALUES (@titulo, @descricao, @data, @status, @categoriaId);",
            p =>
            {
                p.AddWithValue("@titulo", a.Titulo);
                p.AddWithValue("@descricao", a.Descricao);
                p.AddWithValue("@data", a.DataDeRealizacao.Date);
                p.AddWithValue("@status", a.StatusAcao);
                p.AddWithValue("@categoriaId", a.CategoriaId);
            },
            Operacao.Insert, "acao", null, usuarioLogadoId);

    public Task<int> AtualizarAsync(Acao a, int usuarioLogadoId) =>
        ExecutarComLogAsync(
            @"UPDATE acao
              SET titulo = @titulo, descricao = @descricao, data_de_realizacao = @data,
                  status_acao = @status, categoria_id = @categoriaId
              WHERE id = @id;",
            p =>
            {
                p.AddWithValue("@titulo", a.Titulo);
                p.AddWithValue("@descricao", a.Descricao);
                p.AddWithValue("@data", a.DataDeRealizacao.Date);
                p.AddWithValue("@status", a.StatusAcao);
                p.AddWithValue("@categoriaId", a.CategoriaId);
                p.AddWithValue("@id", a.Id);
            },
            Operacao.Update, "acao", a.Id, usuarioLogadoId);

    /// <param name="status">"Aprovado", "Negado" ou "Em Avaliação"</param>
    public Task<int> AlterarStatusAsync(int id, string status, int usuarioLogadoId) =>
        ExecutarComLogAsync(
            "UPDATE acao SET status_acao = @status WHERE id = @id;",
            p => { p.AddWithValue("@status", status); p.AddWithValue("@id", id); },
            Operacao.Update, "acao", id, usuarioLogadoId);

    public Task<int> ExcluirAsync(int id, int usuarioLogadoId) =>
        ExecutarComLogAsync(
            "DELETE FROM acao WHERE id = @id;",
            p => p.AddWithValue("@id", id),
            Operacao.Delete, "acao", id, usuarioLogadoId);

    /// <summary>Vincula um usuário a uma ação (acao_usuario). Log: id da ação.</summary>
    public Task<int> VincularUsuarioAsync(int acaoId, int usuarioId, int usuarioLogadoId) =>
        ExecutarComLogAsync(
            "INSERT INTO acao_usuario (usuario_id, acao_id) VALUES (@usuarioId, @acaoId);",
            p => { p.AddWithValue("@usuarioId", usuarioId); p.AddWithValue("@acaoId", acaoId); },
            Operacao.Insert, "acao_usuario", acaoId, usuarioLogadoId);

    /// <summary>Registra a validação (motivo de rejeição) de uma ação. Log: id da ação.</summary>
    public Task<int> RegistrarValidacaoAsync(int acaoId, int usuarioId, string motivoRejeicao, int usuarioLogadoId) =>
        ExecutarComLogAsync(
            @"INSERT INTO validacao (motivo_rejeicao, usuario_id, acao_id)
              VALUES (@motivo, @usuarioId, @acaoId);",
            p =>
            {
                p.AddWithValue("@motivo", motivoRejeicao);
                p.AddWithValue("@usuarioId", usuarioId);
                p.AddWithValue("@acaoId", acaoId);
            },
            Operacao.Insert, "validacao", acaoId, usuarioLogadoId);
}

// =========================================================
// EXEMPLO DE USO (por exemplo, no Program.cs)
// =========================================================
// var cs = "Server=localhost;Database=der;User=root;Password=suasenha;";
// int logado = 1;                                   // usuario.id de quem está usando o sistema
//
// var categorias = new CategoriaRepository(cs);
// var acoes      = new AcaoRepository(cs);
// var logs       = new LogUsuarioRepository(cs);
//
// int catId  = await categorias.CriarAsync("Palestra", logado);
// int acaoId = await acoes.CriarAsync(new Acao
// {
//     Titulo = "Feira de Ciências",
//     Descricao = "Participação na feira",
//     DataDeRealizacao = DateTime.Today,
//     CategoriaId = catId
// }, logado);
//
// await acoes.AlterarStatusAsync(acaoId, "Aprovado", logado);
//
// foreach (var l in await logs.ListarAsync())
//     Console.WriteLine($"{l.DataHora:dd/MM/yyyy HH:mm} | {l.Operacao} | {l.TabelaAfetada} #{l.IdRegistrado} | por {l.MatriculaId}");
