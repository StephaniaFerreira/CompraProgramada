namespace Core.Expections
{
    public class RegraNegocioException : Exception
    {
        public string Codigo { get; }

        public RegraNegocioException(string mensagem, string codigo)
            : base(mensagem)
        {
            Codigo = codigo;
        }
    }
}
