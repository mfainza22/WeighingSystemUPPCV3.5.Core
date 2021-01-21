
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using WeighingSystemUPPCV3_5_Repository.IRepositories;
using WeighingSystemUPPCV3_5_Repository.Models;
using WeighingSystemUPPCV3_5_Repository.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using SysUtility.Extensions;

namespace WeighingSystemUPPCV3_5_Repository.Repositories
{
    public class TransactionTypeRepository : ITransactionTypeRepository
    {
        private readonly DatabaseContext dbContext;
        public TransactionTypeRepository(DatabaseContext dbContext)
        {
            this.dbContext = dbContext;
        }


        public bool BulkDelete(string[] id)
        {
            throw new NotImplementedException();
        }

        public TransactionType Create(TransactionType model)
        {
            throw new NotImplementedException();
        }

        public string CreateSelecteQuery(TransactionType parameters)
        {
            throw new NotImplementedException();
        }

        public List<SqlParameter> CreateSQLParameters(TransactionType parameters)
        {
            throw new NotImplementedException();
        }

        public bool Delete(TransactionType model)
        {
            throw new NotImplementedException();
        }

        public IQueryable<TransactionType> Get(TransactionType model = null)
        {
            return dbContext.TransactionTypes.AsNoTracking();
        }

        public TransactionType GetById(long id)
        {
            throw new NotImplementedException();
        }

        public TransactionType GetById(string id)
        {
            throw new NotImplementedException();
        }

        public TransactionType GetByName(string name)
        {
            return Get().FirstOrDefault(a => a.Description == name.DefaultIfEmpty());
        }

        public SqlRawParameter GetSqlRawParameter(TransactionType parameters)
        {
            throw new NotImplementedException();
        }

        public TransactionType Update(TransactionType model)
        {
            throw new NotImplementedException();
        }

        public bool ValidateCode(TransactionType model)
        {
            throw new NotImplementedException();
        }

        public bool ValidateName(TransactionType model)
        {
            throw new NotImplementedException();
        }
    }
}
