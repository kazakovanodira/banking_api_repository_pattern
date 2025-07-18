using banking_api_repo.Models.Requests;
using banking_api_repo.Models.Responses;

namespace banking_api_repo.Interface;

public interface IAccountsService
{
    Task<ApiResponse<AccountDto>> CreateAccount(CreateAccountRequest request);
    Task<ApiResponse<AccountDto>> GetAccount(AccountRequest request);
    Task<ApiResponse<BalanceResponse>> MakeDeposit(TransactionRequest request);
    Task<ApiResponse<BalanceResponse>> MakeWithdraw(TransactionRequest request);
    Task<ApiResponse<BalanceResponse>> MakeTransfer(TransactionRequest request);
    Task<ApiResponse<ConvertedBalances>> GetConvertedBalanceAsync(
        AccountRequest accountRequest, CurrencyRequest currencyRequest);
}