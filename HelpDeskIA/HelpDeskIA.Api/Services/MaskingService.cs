using System.Text.RegularExpressions;

namespace HelpDeskIA.Api.Services {
    public static class MaskingService {
        // Simple masking for common PII: emails, cpf (Brazil), credit cards (basic), phone numbers
        private static Regex EmailRegex = new Regex(@"[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}", RegexOptions.Compiled);
        private static Regex CpfRegex = new Regex(@"\d{3}\.\d{3}\.\d{3}-\d{2}\b|\b\d{11}\b", RegexOptions.Compiled);
        private static Regex CreditCardRegex = new Regex("\\b(?:\d[ -]*?){13,16}\\b", RegexOptions.Compiled);
        private static Regex PhoneRegex = new Regex("\\b\+?\d{2,3}[ \-]?\(?\d{2,3}\)?[ \-]?\d{4,5}[ \-]?\d{4}\b", RegexOptions.Compiled);

        public static string MaskPII(string input) {
            if (string.IsNullOrEmpty(input)) return input;

            var s = EmailRegex.Replace(input, "[EMAIL]");
            s = CpfRegex.Replace(s, "[CPF]");
            s = CreditCardRegex.Replace(s, "[CREDIT_CARD]");
            s = PhoneRegex.Replace(s, "[PHONE]");
            return s;
        }
    }
}
