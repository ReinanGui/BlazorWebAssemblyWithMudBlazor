﻿using Application.Repositories;
using Common.Responses;
using Common.Wrapper;
using Domain;
using Mapster;
using MediatR;

namespace Application.Features.AccountHolders.Queries
{
    public class GetAccountHoldersQuery : IRequest<ResponseWrapper<List<AccountHolderResponse>>>
    {
    }

    public class GetAccountHoldersQueryHandler : IRequestHandler<GetAccountHoldersQuery, ResponseWrapper<List<AccountHolderResponse>>>
    {
        private readonly IUnitOfWork<int> _unitOfWork;

        public GetAccountHoldersQueryHandler(IUnitOfWork<int> unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ResponseWrapper<List<AccountHolderResponse>>> Handle(GetAccountHoldersQuery request, CancellationToken cancellationToken)
        {
            var accountHoldersInDb = await _unitOfWork.ReadRepositoryFor<AccountHolder>().GetAllAsync();

            if (accountHoldersInDb.Count > 0)
            {
                return new ResponseWrapper<List<AccountHolderResponse>>()
                    .Success(accountHoldersInDb.Adapt<List<AccountHolderResponse>>());
            }
            return new ResponseWrapper<List<AccountHolderResponse>>()
                .Failed("No Account Holders Were Found.");
        }
    }
}
