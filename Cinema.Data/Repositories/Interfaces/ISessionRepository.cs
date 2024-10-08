﻿using Cinema.Core.Entites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinema.Data.Repositories.Interfaces
{
    public interface ISessionRepository:IRepository<Session>
    {
        Session Get(Func<Session, bool> predicate, string includeProperties = "");
    }
}
