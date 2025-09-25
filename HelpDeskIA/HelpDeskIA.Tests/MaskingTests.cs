using Xunit;
using HelpDeskIA.Api.Services;

namespace HelpDeskIA.Tests {
    public class MaskingTests {
        [Fact]
        public void MaskEmailAndCpf() {
            var input = "Contato: joao.silva@example.com, CPF 123.456.789-09, cartão 4111 1111 1111 1111, telefone +55 (11) 91234-5678";
            var outp = MaskingService.MaskPII(input);
            Assert.Contains("[EMAIL]", outp);
            Assert.Contains("[CPF]", outp);
            Assert.Contains("[CREDIT_CARD]", outp);
            Assert.Contains("[PHONE]", outp);
        }
    }
}
