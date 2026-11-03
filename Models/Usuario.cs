namespace emprapa_bd.Models
{
    public class Usuario
    {
        public int Id { get; set; }
        public string Nome { get; set; } = " ";
        public string Email { get; set; } = " ";
        public string Senha { get; set; } = " ";
        public string Matricula { get; set; } = " ";

        public override string ToString() => $"{Id, 4} { Nome, -25} {Email}";
    }
}
