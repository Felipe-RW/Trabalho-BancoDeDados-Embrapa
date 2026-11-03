using MySql.Data.MySqlClient;

namespace embrapa_bd.Data
{
    public class Conexao
    {
        public MySqlConnection Conectar()
        {
            MySqlConnection con = new MySqlConnection(
                "Server=localhost;Database=embrapa_bd;Uid=root;Pwd=;");

            return con;
        }
    }
}