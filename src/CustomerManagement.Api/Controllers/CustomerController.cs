using CustomerManagement.Api.Dtos;
using CustomerManagement.Domain.Customers;
using CustomerManagement.Domain.Repositories;
using CustomerManagement.Domain.Shared;
using Microsoft.AspNetCore.Mvc;

namespace CustomerManagement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CustomersController : ControllerBase
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly ILogger<CustomersController> _logger;

        public CustomersController(ICustomerRepository customerRepository, ILogger<CustomersController> logger)
        {
            _customerRepository = customerRepository;
            _logger = logger;
        }

        private CustomerResponse MapToCustomerResponse(Customer customer)
        {
            return new CustomerResponse
            {
                Id = customer.Id,
                Name = customer.Name,
                Emails = customer.Emails.Select(e => new EmailResponse { Value = e.Value, IsPrimary = e.IsPrimary }).ToList(),
                Phones = customer.Phones.Select(p => new PhoneResponse { CountryCode = p.AreaCode, Number = p.Number, IsPrimary = p.IsPrimary }).ToList(),
                Addresses = customer.Addresses.Select(a => new AddressResponse
                {
                    Street = a.Street,
                    Number = a.Number,
                    Complement = a.Complement,
                    City = a.City,
                    State = a.State,
                    ZipCode = a.ZipCode,
                    Country = a.Country,
                    IsPrimary = a.IsPrimary
                }).ToList(),
                Documents = customer.Documents.Select(d => new DocumentResponse { Type = d.Type.ToString(), Number = d.Number }).ToList()
            };
        }

        /// <summary>
        /// Cria um novo cliente.
        /// </summary>
        /// <param name="request">Dados para criação do cliente.</param>
        /// <returns>O cliente criado.</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateCustomer([FromBody] CreateCustomerRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                // Map DTO to Domain Value Objects
                var primaryEmail = Email.Create(request.PrimaryEmail.Value, request.PrimaryEmail.IsPrimary);
                var primaryPhone = Phone.Create(request.PrimaryPhone.CountryCode, request.PrimaryPhone.Number, request.PrimaryPhone.IsPrimary);
                var primaryAddress = Address.Create(request.PrimaryAddress.Street, request.PrimaryAddress.Number, request.PrimaryAddress.Complement,
                                                    request.PrimaryAddress.City, request.PrimaryAddress.State, request.PrimaryAddress.ZipCode,
                                                    request.PrimaryAddress.Country, request.PrimaryAddress.IsPrimary);

                var documents = request.Documents.Select(d => {
                    return Document.Create(d.Type, d.Number);
                }).ToList();

                var customer = Customer.Create(request.Name);
                customer.AddEmail(primaryEmail.Value, primaryEmail.IsPrimary);
                customer.AddPhone(primaryPhone);
                customer.AddAddress(primaryAddress);
                
                foreach (var item in documents)
                    customer.AddDocument(item);
                
                await _customerRepository.Add(customer);
                await _customerRepository.SaveChangesAsync();

                var response = MapToCustomerResponse(customer);
                return CreatedAtAction(nameof(GetCustomerById), new { id = response.Id }, response);
            }
            catch (DomainException ex)
            {
                _logger.LogWarning(ex, "Erro de domínio ao criar cliente: {Message}", ex.Message);
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro interno ao criar cliente.");
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Ocorreu um erro interno ao processar sua requisição." });
            }
        }

        /// <summary>
        /// Obtém um cliente pelo seu ID.
        /// </summary>
        /// <param name="id">ID do cliente.</param>
        /// <returns>O cliente encontrado.</returns>
        [HttpGet("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetCustomerById(Guid id)
        {
            try
            {
                var customer = await _customerRepository.GetById(id);
                if (customer == null)
                {
                    return NotFound(new { message = $"Cliente com ID {id} não encontrado." });
                }

                var response = MapToCustomerResponse(customer);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro interno ao buscar cliente com ID {CustomerId}.", id);
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Ocorreu um erro interno ao processar sua requisição." });
            }
        }

        /// <summary>
        /// Lista todos os clientes com paginação opcional.
        /// </summary>
        /// <param name="pageNumber">Número da página (padrão: 1).</param>
        /// <param name="pageSize">Tamanho da página (padrão: 10).</param>
        /// <returns>Uma lista de clientes.</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllCustomers([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var customers = await _customerRepository.GetAll();
                var response = customers.Select(MapToCustomerResponse).ToList();
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro interno ao listar clientes.");
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Ocorreu um erro interno ao processar sua requisição." });
            }
        }

        /// <summary>
        /// Atualiza os dados básicos de um cliente existente.
        /// </summary>
        /// <param name="id">ID do cliente a ser atualizado.</param>
        /// <param name="request">Novos dados do cliente.</param>
        /// <returns>O cliente atualizado.</returns>
        [HttpPut("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateCustomer(Guid id, [FromBody] UpdateCustomerRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var customer = await _customerRepository.GetById(id);
                if (customer == null)
                {
                    return NotFound(new { message = $"Cliente com ID {id} não encontrado." });
                }

                customer.UpdateName(request.Name);

                await _customerRepository.Update(customer);
                await _customerRepository.SaveChangesAsync();

                var response = MapToCustomerResponse(customer);
                return Ok(response);
            }
            catch (DomainException ex)
            {
                _logger.LogWarning(ex, "Erro de domínio ao atualizar cliente {CustomerId}: {Message}", id, ex.Message);
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro interno ao atualizar cliente {CustomerId}.", id);
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Ocorreu um erro interno ao processar sua requisição." });
            }
        }

        /// <summary>
        /// Deleta um cliente pelo seu ID.
        /// </summary>
        /// <param name="id">ID do cliente a ser deletado.</param>
        /// <returns>Status de sucesso sem conteúdo.</returns>
        [HttpDelete("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteCustomer(Guid id)
        {
            try
            {
                var customer = await _customerRepository.GetById(id);
                if (customer == null)
                {
                    return NotFound(new { message = $"Cliente com ID {id} não encontrado." });
                }

                await _customerRepository.Delete(customer.Id);
                await _customerRepository.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro interno ao deletar cliente {CustomerId}.", id);
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Ocorreu um erro interno ao processar sua requisição." });
            }
        }

        /// <summary>
        /// Adiciona um novo email a um cliente existente.
        /// </summary>
        /// <param name="id">ID do cliente.</param>
        /// <param name="request">Dados do email a ser adicionado.</param>
        /// <returns>O cliente atualizado.</returns>
        [HttpPost("{id:guid}/emails")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> AddEmail(Guid id, [FromBody] EmailRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var customer = await _customerRepository.GetById(id);
                if (customer == null)
                {
                    return NotFound(new { message = $"Cliente com ID {id} não encontrado." });
                }

                customer.AddEmail(request.Value, request.IsPrimary);

                await _customerRepository.Update(customer);
                await _customerRepository.SaveChangesAsync();

                var response = MapToCustomerResponse(customer);
                return Ok(response);
            }
            catch (DomainException ex)
            {
                _logger.LogWarning(ex, "Erro de domínio ao adicionar email ao cliente {CustomerId}: {Message}", id, ex.Message);
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro interno ao adicionar email ao cliente {CustomerId}.", id);
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Ocorreu um erro interno ao processar sua requisição." });
            }
        }

        /// <summary>
        /// Adiciona um novo telefone a um cliente existente.
        /// </summary>
        /// <param name="id">ID do cliente.</param>
        /// <param name="request">Dados do telefone a ser adicionado.</param>
        /// <returns>O cliente atualizado.</returns>
        [HttpPost("{id:guid}/phones")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> AddPhone(Guid id, [FromBody] PhoneRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var customer = await _customerRepository.GetById(id);
                if (customer == null)
                {
                    return NotFound(new { message = $"Cliente com ID {id} não encontrado." });
                }

                var phone = Phone.Create(request.CountryCode, request.Number, request.IsPrimary);
                customer.AddPhone(phone);

                await _customerRepository.Update(customer);
                await _customerRepository.SaveChangesAsync();

                var response = MapToCustomerResponse(customer);
                return Ok(response);
            }
            catch (DomainException ex)
            {
                _logger.LogWarning(ex, "Erro de domínio ao adicionar telefone ao cliente {CustomerId}: {Message}", id, ex.Message);
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro interno ao adicionar telefone ao cliente {CustomerId}.", id);
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Ocorreu um erro interno ao processar sua requisição." });
            }
        }

        /// <summary>
        /// Adiciona um novo endereço a um cliente existente.
        /// </summary>
        /// <param name="id">ID do cliente.</param>
        /// <param name="request">Dados do endereço a ser adicionado.</param>
        /// <returns>O cliente atualizado.</returns>
        [HttpPost("{id:guid}/addresses")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> AddAddress(Guid id, [FromBody] AddressRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var customer = await _customerRepository.GetById(id);
                if (customer == null)
                {
                    return NotFound(new { message = $"Cliente com ID {id} não encontrado." });
                }

                var address = Address.Create(request.Street, request.Number, request.Complement,
                                             request.City, request.State, request.ZipCode,
                                             request.Country, request.IsPrimary);
                customer.AddAddress(address);

                await _customerRepository.Update(customer);
                await _customerRepository.SaveChangesAsync();

                var response = MapToCustomerResponse(customer);
                return Ok(response);
            }
            catch (DomainException ex)
            {
                _logger.LogWarning(ex, "Erro de domínio ao adicionar endereço ao cliente {CustomerId}: {Message}", id, ex.Message);
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro interno ao adicionar endereço ao cliente {CustomerId}.", id);
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Ocorreu um erro interno ao processar sua requisição." });
            }
        }
    }
}