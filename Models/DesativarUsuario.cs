using System;
using embrapa_bd.Data;
using MySql.Data.MySqlClient;

namespace embrapa_bd.Models
{
    public static class UsuarioRepositorioDesativacao
    {
        public static bool DesativarUsuario(this UsuarioRepositorio repositorio, int id)
        {
            if (id <= 0) throw new ArgumentException("ID inválido.");
            using MySqlConnection con = new Conexao().Conectar();
            using MySqlCommand comando = new MySqlCommand(
                "UPDATE usuario SET ativo = 0 WHERE id = @id AND ativo = 1", con);
            comando.Parameters.AddWithValue("@id", id);
            con.Open();
            return comando.ExecuteNonQuery() > 0;
        }
    }

    public static class DesativarUsuario
    {
        public static void Desativar(UsuarioRepositorio usuarioRepo)
        {
            Console.WriteLine("Digite o ID do usuário a desativar:");
            if (!int.TryParse(Console.ReadLine(), out int id) || id <= 0)
            {
                Console.WriteLine("ID inválido.");
                return;
            }
            try
            {
                var usuario = usuarioRepo.BuscarPorId(id);
                if (usuario == null)
                {
                    Console.WriteLine("Usuário não encontrado.");
                    return;
                }
                Console.WriteLine($"Usuário encontrado: {usuario.Nome} ({usuario.Email})");
                Console.WriteLine("Confirmar desativação? (S/N):");
                if (!string.Equals(Console.ReadLine()?.Trim(), "S", StringComparison.OrdinalIgnoreCase)) return;
                Console.WriteLine(usuarioRepo.DesativarUsuario(id)
                    ? "Usuário desativado com sucesso!" : "Usuário não encontrado ou já desativado.");
            }
            catch (MySqlException) { Console.WriteLine("Não foi possível desativar o usuário. Verifique o banco."); }
        }
    }
}
