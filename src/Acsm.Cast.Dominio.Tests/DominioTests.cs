using Acsm.Cast.Dominio.Entidades;
using Acsm.Cast.Dominio.Excecoes;
using Xunit;

namespace Acsm.Cast.Dominio.Tests
{
    public class DominioTests
    {
        [Fact]
        public void Transferencia_Sucesso_DeveAtualizarSaldosECriarRegistro()
        {
            // Arrange: Cenário inicial do teste
            var contaA = new Conta("A", 100000);
            var contaB = new Conta("B", 50000);
            const long valorTransferencia = 3000;

            // Act: Executa operação
            contaA.Debitar(valorTransferencia);
            contaB.Creditar(valorTransferencia);
            var transferencia = new Transferencia(contaA.Id, contaB.Id, valorTransferencia);

            // Assert: Valida saldos e entidade gerada
            Assert.Equal(97000, contaA.SaldoCentavos);
            Assert.Equal(53000, contaB.SaldoCentavos);
            Assert.NotEqual(Guid.Empty, transferencia.Id);
            Assert.Equal("A", transferencia.OrigemId);
            Assert.Equal("B", transferencia.DestinoId);
            Assert.Equal(3000, transferencia.ValorCentavos);
        }

        [Fact]
        public void Debito_SaldoInsuficiente_DeveLancarExcecaoEManterSaldoIntacto()
        {
            // Arrange
            var contaA = new Conta("A", 100000);
            const long valorTentado = 200000;

            // Act & Assert
            var ex = Assert.Throws<SaldoInsuficienteException>(() => contaA.Debitar(valorTentado));

            Assert.Contains("saldo insuficiente", ex.Message, StringComparison.OrdinalIgnoreCase);
            Assert.Equal(100000, contaA.SaldoCentavos); // Saldo inalterado
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-500)]
        [InlineData(-1)]
        public void DebitoECredito_ComValorZeroOuNegativo_DeveLancarRegraNegocioException(long valorInvalido)
        {
            // Arrange
            var conta = new Conta("A", 100000);

            // Act & Assert
            Assert.Throws<RegraNegocioException>(() => conta.Debitar(valorInvalido));
            Assert.Throws<RegraNegocioException>(() => conta.Creditar(valorInvalido));
            Assert.Equal(100000, conta.SaldoCentavos);
        }

        [Fact]
        public void CriarTransferencia_ComOrigemIgualAoDestino_DeveLancarRegraNegocioException()
        {
            // Arrange & Act & Assert
            var ex = Assert.Throws<RegraNegocioException>(() => new Transferencia("A", "A", 3000));
            Assert.Contains("não pode ser igual", ex.Message, StringComparison.OrdinalIgnoreCase);
        }

        [Theory]
        [InlineData("", "B")]
        [InlineData("A", "")]
        [InlineData(null, "B")]
        public void CriarTransferencia_ComContasVaziasOuNulas_DeveLancarRegraNegocioException(string? origem, string? destino)
        {
            // Arrange & Act & Assert
            Assert.Throws<RegraNegocioException>(() => new Transferencia(origem!, destino!, 3000));
        }
    }
}
