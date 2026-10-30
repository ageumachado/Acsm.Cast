using Acsm.Cast.Dominio.Excecoes;

namespace Acsm.Cast.Dominio.Entidades;

public class Conta
{
    public string Id { get; private set; } = string.Empty;
    public long SaldoCentavos { get; private set; }

    // Construtor protegido exigido pelo EF Core
    protected Conta() { }

    public Conta(string id, long saldoInicialCentavos)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new RegraNegocioException("O identificador da conta é obrigatório.");

        if (saldoInicialCentavos < 0)
            throw new RegraNegocioException("O saldo inicial não pode ser negativo.");

        Id = id.Trim().ToUpperInvariant();
        SaldoCentavos = saldoInicialCentavos;
    }

    public void Debitar(long valorCentavos)
    {
        ValidarValor(valorCentavos);

        if (SaldoCentavos < valorCentavos)
            throw new SaldoInsuficienteException(Id, SaldoCentavos, valorCentavos);

        SaldoCentavos -= valorCentavos;
    }

    public void Creditar(long valorCentavos)
    {
        ValidarValor(valorCentavos);

        SaldoCentavos += valorCentavos;
    }

    private static void ValidarValor(long valorCentavos)
    {
        if (valorCentavos <= 0)
            throw new RegraNegocioException("O valor da operação deve ser maior que zero.");
    }
}
