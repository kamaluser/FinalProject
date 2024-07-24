﻿using Cinema.Core.Entites;
using Cinema.Data.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinema.Data.Repositories.Implementations
{
    public class LanguageRepository : Repository<Language>, ILanguageRepository
    {
        public LanguageRepository(AppDbContext context) : base(context)
        {

        }
    }
}