using Application.Repositories;
using Common.Requests;
using Common.Wrapper;
using Domain;
using MediatR;

namespace Application.Features.Accounts.Commands
{
    public class CreateTransactionCommand : IRequest<ResponseWrapper<int>>
    {
        public TransactionRequest Transaction { get; set; }
    }

    public class CreateTransactionCommandHandler : IRequestHandler<CreateTransactionCommand, ResponseWrapper<int>>
    {
        private readonly IUnitOfWork<int> _unitOfWork;

        public CreateTransactionCommandHandler(IUnitOfWork<int> unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ResponseWrapper<int>> Handle(CreateTransactionCommand request, CancellationToken cancellationToken)
        {
            var accountInDb = await _unitOfWork.ReadRepositoryFor<Account>().GetByIdAsync(request.Transaction.AccountId);
            if (accountInDb is not null)
            {
                // Know the transaction type
                if (request.Transaction.Type == Common.Enums.TransactionType.Withdrawal)
                {
                    // Validate if ness
                    if (request.Transaction.Amount > accountInDb.Balance)
                    {
                        return new ResponseWrapper<int>().Failed(message: "Withdrawal amount is higher than account balance.");
                    }
                    // Create a transaction
                    var transaction = new Transaction() { AccountId = accountInDb.Id, Amount = request.Transaction.Amount,
                        Type = Common.Enums.TransactionType.Withdrawal, Date = DateTime.Now };
                    // Update account balance
                    accountInDb.Balance -= request.Transaction.Amount;
                    await _unitOfWork.WriteRepositoryFor<Transaction>().AddAsync(transaction);                
                    await _unitOfWork.WriteRepositoryFor<Account>().UpdateAsync(accountInDb);
                    await _unitOfWork.CommitAsync(cancellationToken);
                    return new ResponseWrapper<int>().Success(data: transaction.Id, message: "Withdrawal was successfully.");
                }
                else if(request.Transaction.Type == Common.Enums.TransactionType.Deposit)
                {
                    // Create a transaction
                    var transaction = new Transaction() { AccountId = accountInDb.Id, Amount = request.Transaction.Amount,
                        Type = Common.Enums.TransactionType.Deposit, Date = DateTime.Now };
                    // Update account balance
                    accountInDb.Balance += request.Transaction.Amount;

                    await _unitOfWork.WriteRepositoryFor<Transaction>().AddAsync(transaction);
                    await _unitOfWork.WriteRepositoryFor<Account>().UpdateAsync(accountInDb);
                    await _unitOfWork.CommitAsync(cancellationToken);
                    return new ResponseWrapper<int>().Success(data: transaction.Id ,message: "Deposit was successfully.");
                }
            }
            return new ResponseWrapper<int>().Failed(message: "Account Does Exists.");
        }
    }
}
