using Acsm.Cast.Questao2.Tests.Modelos;

namespace Acsm.Cast.Questao2.Tests;

public class FalhaECorrecaoTests
{
    [Fact]
    public void SimuladorOriginal_QuandoDestinoBloqueado_DemonstraFalhaComPerdaDeSaldo()
    {
        // Arrange: Cenário exato do enunciado (A=10000, B=5000, B bloqueada, valor 3000)
        var servico = new SimuladorOriginal();
        var contaA = new ContaMemoria { Id = "A", Saldo = 10000, Bloqueada = false };
        var contaB = new ContaMemoria { Id = "B", Saldo = 5000, Bloqueada = true };

        // Act & Assert
        var ex = Assert.Throws<InvalidOperationException>(() => servico.Transferir(contaA, contaB, 3000));
        Assert.Equal("destino bloqueado", ex.Message);

        // EVIDÊNCIA DO BUG: A perdeu 3000 indevidamente e ficou com 7000, B continuou com 5000
        Assert.Equal(7000, contaA.Saldo);
        Assert.Equal(5000, contaB.Saldo);
        Assert.Empty(servico.Movimentacoes);
    }

    [Fact]
    public void ServicoCorrigido_QuandoDestinoBloqueado_MantemSaldosIntactosESemMovimentacao()
    {
        // Arrange: Mesmo cenário com o serviço corrigido
        var servico = new ServicoCorrigido();
        var contaA = new ContaMemoria { Id = "A", Saldo = 10000, Bloqueada = false };
        var contaB = new ContaMemoria { Id = "B", Saldo = 5000, Bloqueada = true };

        // Act & Assert
        var ex = Assert.Throws<InvalidOperationException>(() => servico.Transferir(contaA, contaB, 3000));
        Assert.Equal("destino bloqueado", ex.Message);

        // CORREÇÃO COMPROVADA: Nenhum saldo foi alterado (A=10000, B=5000)
        Assert.Equal(10000, contaA.Saldo);
        Assert.Equal(5000, contaB.Saldo);
        Assert.Empty(servico.Movimentacoes);
    }

    [Fact]
    public void ServicoCorrigido_QuandoDestinoDesbloqueado_RealizaTransferenciaComSucesso()
    {
        // Arrange
        var servico = new ServicoCorrigido();
        var contaA = new ContaMemoria { Id = "A", Saldo = 10000, Bloqueada = false };
        var contaB = new ContaMemoria { Id = "B", Saldo = 5000, Bloqueada = false };

        // Act
        servico.Transferir(contaA, contaB, 3000);

        // Assert
        Assert.Equal(7000, contaA.Saldo);
        Assert.Equal(8000, contaB.Saldo);
        Assert.Single(servico.Movimentacoes);

        var mov = servico.Movimentacoes[0];
        Assert.Equal("A", mov.Origem);
        Assert.Equal("B", mov.Destino);
        Assert.Equal(3000, mov.Valor);
    }

    [Fact]
    public void ServicoCorrigido_QuandoSaldoInsuficiente_LancaExcecaoEMantemSaldosIntactos()
    {
        // Arrange
        var servico = new ServicoCorrigido();
        var contaA = new ContaMemoria { Id = "A", Saldo = 2000, Bloqueada = false };
        var contaB = new ContaMemoria { Id = "B", Saldo = 5000, Bloqueada = false };

        // Act & Assert
        var ex = Assert.Throws<InvalidOperationException>(() => servico.Transferir(contaA, contaB, 3000));
        Assert.Equal("saldo insuficiente", ex.Message);

        Assert.Equal(2000, contaA.Saldo);
        Assert.Equal(5000, contaB.Saldo);
        Assert.Empty(servico.Movimentacoes);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-100)]
    public void ServicoCorrigido_QuandoValorInvalido_LancaArgumentException(long valorInvalido)
    {
        // Arrange
        var servico = new ServicoCorrigido();
        var contaA = new ContaMemoria { Id = "A", Saldo = 10000, Bloqueada = false };
        var contaB = new ContaMemoria { Id = "B", Saldo = 5000, Bloqueada = false };

        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() => servico.Transferir(contaA, contaB, valorInvalido));
        Assert.Equal("valor inválido", ex.Message);

        Assert.Equal(10000, contaA.Saldo);
        Assert.Equal(5000, contaB.Saldo);
    }
}