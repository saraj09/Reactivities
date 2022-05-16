using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Activities;
using Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Photos
{
    public class SetMain
    {
        public class Command : IRequest<Result<Unit>>
        {
            public string Id { get; set; }
        }
        public class Handler : IRequestHandler<Command, Result<Unit>>
        {
            private readonly DataContext _context;
            private readonly IUserAccessor _userAccessor;
            public Handler(DataContext context, IUserAccessor userAccessor)
            {
                this._userAccessor = userAccessor;
                this._context = context;
            }

            public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
            {
                var user = await _context.Users.Include(p=>p.Photos)
                .FirstOrDefaultAsync(x=>x.UserName  == _userAccessor.GetUsername());
                if( user == null) return null;
                var photo = user.Photos.FirstOrDefault(x=>x.Id == request.Id);
                if(photo == null) return null;
                var currtMain = user.Photos.FirstOrDefault(x=>x.IsMain);
                if(currtMain !=null) currtMain.IsMain = false;
                photo.IsMain = true;
                var success = await _context.SaveChangesAsync()>0;
                if(success) return Result<Unit>.Sucess(Unit.Value);
                return Result<Unit>.Failure("Problems setting main photo");

                
            }
        }
    }
}