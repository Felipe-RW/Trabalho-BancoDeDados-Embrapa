using MySql.Data.MySqlClient;

namespace Cadastro_de_usuario.Data;
public class Conexao
{
    private readonly string _stringConexao =
    " Server=localhost;  Port=3306; Database=cadastro_db; Uid=root; Pwd";

    public MySqlConnection Criar() => new MySqlConnection (_stringConexao);
}