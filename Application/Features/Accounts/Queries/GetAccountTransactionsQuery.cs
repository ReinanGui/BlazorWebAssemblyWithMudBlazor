using Application.Repositories;
using Common.Responses;
using Common.Wrapper;
using Domain;
using Mapster;
using MediatR;

namespace Application.Features.Accounts.Queries
{
    public class GetAccountTransactionsQuery : IRequest<ResponseWrapper<List<TransactionResponse>>>
    {
        public int AccountId { get; set; }
    }

    public class GetAccountTransactionsQueryHandler : IRequestHandler<GetAccountTransactionsQuery, ResponseWrapper<List<TransactionResponse>>>
    {
        private readonly IUnitOfWork<int> _unitOfWork;

        public GetAccountTransactionsQueryHandler(IUnitOfWork<int> unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ResponseWrapper<List<TransactionResponse>>> Handle(GetAccountTransactionsQuery request, CancellationToken cancellationToken)
        {
            var transactionsInDb = _unitOfWork.ReadRepositoryFor<Transaction>()
                .Entities
                .Where(transaction => transaction.AccountId == request.AccountId)
                .ToList();

            if (transactionsInDb.Count > 0)
            {
                return await Task.FromResult(new ResponseWrapper<List<TransactionResponse>>().Success(data: transactionsInDb.Adapt<List<TransactionResponse>>()));
            }
            return await Task.FromResult(new ResponseWrapper<List<TransactionResponse>>().Failed(message: "No Transactions on specified account were found."));
        }
    }
}
