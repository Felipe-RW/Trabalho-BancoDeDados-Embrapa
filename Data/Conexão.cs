using MySql.Data.MySqlClient;

namespace embrapa_bd.Data
{
    public class Conexao
    {
        public MySqlConnection Conectar()
        {
            MySqlConnection con = new MySqlConnection(
                "Server=localhost;Database=der;Uid=root;Pwd=;");

            return con;
        }
    }
}