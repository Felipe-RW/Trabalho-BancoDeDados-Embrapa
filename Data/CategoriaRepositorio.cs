using System;
using embrapa_bd.Models;
using MySql.Data.MySqlClient;

namespace embrapa_bd.Data
{
    // Utiliza a tabela categoria definida no DER.
    public class CategoriaRepositorio
    {
        public void Cadastrar(Categoria categoria)
        {
            ArgumentNullException.ThrowIfNull(categoria);
            string nome = (categoria.Nome ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(nome))
                throw new ArgumentException("Informe o nome da categoria.");
            if (nome.Length > Categoria.LimiteNome)
                throw new ArgumentException("O nome da categoria é muito longo.");

            using MySqlConnection con = new Conexao().Conectar();
            const string sql = "INSERT INTO categoria (nome) VALUES (@nome)";
            using MySqlCommand comando = new MySqlCommand(sql, con);
            comando.Parameters.AddWithValue("@nome", nome);

            con.Open();
            comando.ExecuteNonQuery();
        }
    }
}
