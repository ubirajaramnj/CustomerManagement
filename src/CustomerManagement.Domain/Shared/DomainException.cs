using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomerManagement.Domain.Shared
{
    /// <summary>
    /// Exceção customizada para representar erros de domínio.
    /// </summary>
    public class DomainException : Exception
    {
        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="DomainException"/>.
        /// </summary>
        public DomainException() { }

        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="DomainException"/> com uma mensagem de erro especificada.
        /// </summary>
        /// <param name="message">A mensagem que descreve o erro.</param>
        public DomainException(string message) : base(message) { }

        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="DomainException"/> com uma mensagem de erro especificada
        /// e uma referência à exceção interna que é a causa desta exceção.
        /// </summary>
        /// <param name="message">A mensagem que descreve o erro.</param>
        /// <param name="innerException">A exceção que é a causa da exceção atual.</param>
        public DomainException(string message, Exception innerException) : base(message, innerException) { }
    }
}