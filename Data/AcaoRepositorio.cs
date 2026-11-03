using MySql.Data.MySqlClient;
using embrapa_bd.Models;

namespace embrapa_bd.Data
{
    public class AcaoRepositorio
    {
        public void Cadastrar(Acao acao)
        {
            if (string.IsNullOrWhiteSpace(acao.Titulo) || acao.Titulo.Trim().Length > 100 ||
                string.IsNullOrWhiteSpace(acao.Descricao) || acao.CategoriaId <= 0 ||
                acao.DataRealizacao == default)
                throw new ArgumentException("Informe título (até 100 caracteres), descrição, categoria e data válidos.");

            using MySqlConnection con = new Conexao().Conectar();
            using MySqlCommand comando = new MySqlCommand(
                "INSERT INTO acao (titulo, descricao, categoria_id, data_de_realizacao) " +
                "VALUES (@titulo, @descricao, @categoria, @data)", con);
            comando.Parameters.AddWithValue("@titulo", acao.Titulo.Trim());
            comando.Parameters.AddWithValue("@descricao", acao.Descricao.Trim());
            comando.Parameters.AddWithValue("@categoria", acao.CategoriaId);
            comando.Parameters.AddWithValue("@data", acao.DataRealizacao.Date);
            con.Open();
            comando.ExecuteNonQuery();
        }
    }
}
