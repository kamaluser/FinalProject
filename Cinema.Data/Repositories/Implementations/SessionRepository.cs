using Cinema.Core.Entites;
using Cinema.Data.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinema.Data.Repositories.Implementations
{
    public class SessionRepository : Repository<Session>, ISessionRepository
    {
        public SessionRepository(AppDbContext context) : base(context)
        {
        }

        public Session Get(Func<Session, bool> predicate, string includeProperties = "")
        {
            IQueryable<Session> query = _context.Set<Session>();

            if (!string.IsNullOrEmpty(includeProperties))
            {
                foreach (var includeProperty in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProperty.Trim());
                }
            }

            return query.FirstOrDefault(predicate);
        }
    }

}
