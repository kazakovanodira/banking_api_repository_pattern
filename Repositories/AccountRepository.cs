using banking_api_repo.Data;
using banking_api_repo.Interface;
using banking_api_repo.Models;
using banking_api_repo.Models.Responses;
using Microsoft.EntityFrameworkCore;

namespace banking_api_repo.Repositories;

public class AccountRepository : IAccountRepository
{
    private readonly AccountsContext _context;

    public AccountRepository(AccountsContext context)
    {
        _context = context;
    }
    
    public async Task<Account> AddAccount(AccountDto accountDto)
    {
        var account = new Account
        {
            AccountNumber = accountDto.AccountId,
            Balance = accountDto.Balance,
            Name = accountDto.Name
        };
        
        _context.Accounts.Add(account);
        await _context.SaveChangesAsync();

        return account;
    }

    public async Task<Account?> UpdateAccount(Account account, decimal amount)
    {
        account.Balance += amount;
        
        await _context.SaveChangesAsync();
        
        return account;
    }

    public async Task<Account?> GetAccountById(Guid accountId) =>
        await _context.Accounts.FirstOrDefaultAsync(account => account.AccountNumber == accountId);
}