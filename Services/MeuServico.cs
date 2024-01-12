namespace APICatalogo.Services
{
    public class MeuServico : IMeuServico
    {
        public string Saudacao(string nome)
        {
            return $"Bem vindo, {nome}! {DateTime.Now}";
        }
    }
}
