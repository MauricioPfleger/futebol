namespace futebol.Objetos
{
    public class JogadorRequest
    {
        public string Nome { get; set; }
        public int? Numero { get; set; }
        public int? idClube { get; set; }
        public decimal? Salario {get; set; }
    }
}