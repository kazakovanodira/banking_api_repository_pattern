using banking_api_repo.Interface;
using banking_api_repo.Models.Requests;
using banking_api_repo.Models.Responses;
using Microsoft.AspNetCore.Mvc;

namespace banking_api_repo.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AccountController : ControllerBase
{
    private readonly IAccountsService _service;

    public AccountController(IAccountsService service)
    {
        _service = service;
    }
    
    /// <summary>
    /// Creates a new bank account with the specified account holder name.
    /// </summary>
    /// <param name="request">Contains the account holder's name.</param>
    /// <returns>The created account details.</returns>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<AccountDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<AccountDto>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateNewAccount([FromBody] CreateAccountRequest request)
    {
        var account = await _service.CreateAccount(request);
        
        if (!account.IsSuccess)
        {
            return StatusCode(account.HttpStatusCode, account);
        }
        
        return CreatedAtAction(nameof(GetAccount), new { accountNumber = account.Result.AccountId }, account);
    }
    
    /// <summary>
    /// Retrieves account details by account number.
    /// </summary>
    /// <param name="accountNumber">The unique identifier of the account.</param>
    /// <returns>The account details.</returns>
    [HttpGet("{accountNumber}")]
    [ProducesResponseType(typeof(ApiResponse<AccountDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<AccountDto>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetAccount(Guid accountNumber)
    {
        var account = await _service.GetAccount(new AccountRequest(accountNumber));
        if (!account.IsSuccess)
        {
            return StatusCode(account.HttpStatusCode, account);
        }
        return Ok(account);
    }
    
    /// <summary>
    /// Makes a deposit into the specified account.
    /// </summary>
    /// <param name="accountNumber">The account number to deposit into.</param>
    /// <param name="request">The deposit request containing the deposit amount.</param>
    /// <returns>The updated balance.</returns>
    [HttpPost("{accountNumber}/deposits")]
    [ProducesResponseType(typeof(ApiResponse<BalanceResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<BalanceResponse>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> MakeDeposit(Guid accountNumber,[FromBody] TransactionRequest request)
    {
        var account = await _service.MakeDeposit(new TransactionRequest()
        {
            SenderAccId = accountNumber,
            Amount = request.Amount,
        });
        
        if (!account.IsSuccess)
        {
            return StatusCode(account.HttpStatusCode, account);
        }
        return Ok(account);
    }
    
    /// <summary>
    /// Makes a withdrawal from the specified account.
    /// </summary>
    /// <param name="accountNumber">The account number to withdraw from.</param>
    /// <param name="request">The withdrawal request containing the withdrawal amount.</param>
    /// <returns>The updated balance.</returns>
    [HttpPost("{accountNumber}/withdrawals")]
    [ProducesResponseType(typeof(ApiResponse<BalanceResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<BalanceResponse>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<BalanceResponse>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> MakeWithdrawal(Guid accountNumber,[FromBody] TransactionRequest request)
    {
        var account = await _service.MakeWithdraw(new TransactionRequest()
        {
            SenderAccId = accountNumber,
            Amount = request.Amount,
        });
        
        if (!account.IsSuccess)
        {
            return StatusCode(account.HttpStatusCode, account);
        }
        return Ok(account);
    }
    
    /// <summary>
    /// Transfers funds between two accounts.
    /// </summary>
    /// <param name="request">The transfer request containing sender, receiver, and transfer amount.</param>
    /// <returns>The updated sender balance.</returns>
    [HttpPost("transfers")]
    [ProducesResponseType(typeof(ApiResponse<BalanceResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<BalanceResponse>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<BalanceResponse>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> MakeTransfer([FromBody] TransactionRequest request)
    {
        var account = await _service.MakeTransfer(new TransactionRequest()
        {
            SenderAccId = request.SenderAccId,
            ReceiverAccId = request.ReceiverAccId,
            Amount = request.Amount,
        });
        
        if (!account.IsSuccess)
        {
            return StatusCode(account.HttpStatusCode, account);
        }
        return Ok(account);
    }
    
    /// <summary>
    /// Converts balance of the specified account to the requested currencies.
    /// </summary>
    /// <param name="accountNumber">The account number to check balance from</param>
    /// <param name="currency">The currencies to convert the balance to.</param>
    /// <returns>The list of the converted balance.</returns>
    [HttpGet("{accountNumber}/balances")]
    [ProducesResponseType(typeof(ApiResponse<BalanceResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<BalanceResponse>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CheckConvertedBalance(Guid accountNumber, [FromQuery] string currency)
    {
        var response = await _service.GetConvertedBalanceAsync(
            new AccountRequest(accountNumber),
            new CurrencyRequest { Currency = currency });

        if (!response.IsSuccess)
        {
            return StatusCode(response.HttpStatusCode, response);
        }
        
        return Ok(response.Result);
    }
}