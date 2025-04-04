using Application.Repositories;
using Common.Requests;
using Common.Wrapper;
using Domain;
using Mapster;
using MediatR;

namespace Application.Features.Accounts.Commands
{
    public class CreateAccountCommand : IRequest<ResponseWrapper<int>>
    {
        public CreateAccountRequest CreateAccount { get; set; }
    }

    public class CreateAccountCommandHandler : IRequestHandler<CreateAccountCommand, ResponseWrapper<int>>
    {
        private IUnitOfWork<int> _unitOfWork;

        public CreateAccountCommandHandler(IUnitOfWork<int> unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ResponseWrapper<int>> Handle(CreateAccountCommand request, CancellationToken cancellationToken)
        {
            var account = request.CreateAccount.Adapt<Account>();
            account.AccountNumber = AccountNumberGenerator.GenerateAccountNumber();
            account.IsActive = true;
            await _unitOfWork.WriteRepositoryFor<Account>().AddAsync(account);
            await _unitOfWork.CommitAsync(cancellationToken);

            return new ResponseWrapper<int>().Success(data: account.Id, "Account created successfully.");
        }
    }
}
