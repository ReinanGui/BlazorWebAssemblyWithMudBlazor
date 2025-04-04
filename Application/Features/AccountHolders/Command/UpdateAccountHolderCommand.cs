using Application.Repositories;
using Common.Requests;
using Common.Wrapper;
using Domain;
using MediatR;

namespace Application.Features.AccountHolders.Command
{
    public class UpdateAccountHolderCommand : IRequest<ResponseWrapper<int>>
    {
        public UpdateAccountHolder UpdateAccountHolder { get; set; }
    }

    public class UpdateAccountHolderCommandHandler : IRequestHandler<UpdateAccountHolderCommand, ResponseWrapper<int>>
    {
        private readonly IUnitOfWork<int> _unitOfWork;

        public UpdateAccountHolderCommandHandler(IUnitOfWork<int> unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ResponseWrapper<int>> Handle(UpdateAccountHolderCommand request, CancellationToken cancellationToken)
        {
            var accountHolderInDb = await _unitOfWork.ReadRepositoryFor<AccountHolder>().GetByIdAsync(request.UpdateAccountHolder.Id);

            if (accountHolderInDb is not null)
            {
                var updatedAccountHolder = accountHolderInDb.Update(request.UpdateAccountHolder.FirstName, request.UpdateAccountHolder.LastName,
                    request.UpdateAccountHolder.ContactNumber, request.UpdateAccountHolder.Email);

                await _unitOfWork.WriteRepositoryFor<AccountHolder>().UpdateAsync(updatedAccountHolder);
                await _unitOfWork.CommitAsync(cancellationToken);

                return new ResponseWrapper<int>().Success(updatedAccountHolder.Id, "Account Holder Updated Successfully.");
            }
            return new ResponseWrapper<int>().Failed("Account Holder Does not Exists.");
        }
    }
}
