using System.Security.Cryptography;
using System.Text;
using Cadastro_de_usuario.Models;
using MySql.Data.MySqlClient;

namespace Cadastro_de_usuario.Data;

public class UsuarioRepositorio
{
    private readonly Conexao _conexao;

    public UsuarioRepositorio(Conexao conexao) => _conexao = conexao;

    private static string GerarHash(string senha) =>
        Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(senha))).ToLower();

    public Usuarios? Autenticar(string email, string senha)
    {
        using var con = _conexao.Criar();
        con.Open();

        var cmd = new MySqlCommand(
            "SELECT id, nome, email FROM usuarios WHERE email = @email AND senha = @senha", con);
        cmd.Parameters.AddWithValue("@email", email);
        cmd.Parameters.AddWithValue("@senha", GerarHash(senha));

        using var leitor = cmd.ExecuteReader();

        if(!leitor.Read()) return null;

        return new Usuarios
        {
            Id = leitor.GetInt32("id"),
            Nome = leitor.GetString("nome"),
            Email = leitor.GetString("email"),
        };
    }

    public void Cadastrar(Usuarios u)
    {
        using var con = _conexao.Criar();
        con.Open();

        var cmd = new MySqlCommand(
            "INSERT INTO usuarios(nome, email, senha) VALUES (@nome, @email, @senha)", con);
        cmd.Parameters.AddWithValue("@nome", u.Nome);
        cmd.Parameters.AddWithValue("@email", u.Email);
        cmd.Parameters.AddWithValue("@senha", GerarHash(u.Senha));

        cmd.ExecuteNonQuery();
    }

    public List<Usuarios> Listar()
    {
        var lista = new List<Usuarios>();
        using var con = _conexao.Criar();
        con.Open();

        var cmd = new MySqlCommand("SELECT id, nome, email FROM usuarios ORDER BY id", con);
        using var leitor = cmd.ExecuteReader();

        while (leitor.Read())
        {
            lista.Add(new Usuarios
            {
                Id = leitor.GetInt32("id"),
                Nome = leitor.GetString("nome"),
                Email = leitor.GetString("email"),
            });
        }
        return lista;
    }

    public bool Editar(Usuarios u)
    {
        using var con = _conexao.Criar();
        con.Open();

        var cmd = new MySqlCommand(
            "UPDATE usuarios SET nome = @nome, email = @email WHERE id =", con);
        
        cmd.Parameters.AddWithValue("@nome", u.Nome);
        cmd.Parameters.AddWithValue("@email", u.Email);
        cmd.Parameters.AddWithValue("@id", u.Id);

        return cmd.ExecuteNonQuery() > 0;
    }

    public bool Excluir(int id)
    {
        using var con = _conexao.Criar();
        con.Open();
        var cmd = new MySqlCommand("DELETE FROM usuarios WHERE id = @id", con);
        cmd.Parameters.AddWithValue("@id", id);
        return cmd.ExecuteNonQuery() > 0;
    }
}