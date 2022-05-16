using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Activities;
using Application.Interfaces;
using AutoMapper;
using Domain;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Comments
{
    public class Create
    {
        public class Command : IRequest<Result<CommentDto>>
        {
            public string Body { get; set; }
            public Guid ActivityId { get; set; }
        }
        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleFor(x => x.Body).NotEmpty();
            }
        }
        public class Handler : IRequestHandler<Command, Result<CommentDto>>
        {
            private readonly IMapper _mapper;
            private readonly IUserAccessor _userAccessor;
            private readonly DataContext _context;
            public Handler(DataContext context, IMapper mapper, IUserAccessor userAccessor)
            {
                this._context = context;
                this._userAccessor = userAccessor;
                this._mapper = mapper;
            }

            public async Task<Result<CommentDto>> Handle(Command request, CancellationToken cancellationToken)
            {
                var activity = await _context.Activities.FindAsync(request.ActivityId);
                if(activity==null) return null;
                var user = await _context.Users
                .Include(p=> p.Photos)
                .SingleOrDefaultAsync(x=>x.UserName== _userAccessor.GetUsername());
                var comment = new Comment
                {
                    Author = user,
                    Activity = activity,
                    Body = request.Body
                };
                activity.Comments.Add(comment);
                var sucess = await _context.SaveChangesAsync()> 0;
                if(sucess) return Result<CommentDto>.Sucess(_mapper.Map<CommentDto>(comment));
                return Result<CommentDto>.Failure("Failed to add comment");
            }
        }
    }
}