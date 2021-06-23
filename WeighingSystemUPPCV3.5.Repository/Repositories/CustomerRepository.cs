using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using WeighingSystemUPPCV3_5_Repository.IRepositories;
using WeighingSystemUPPCV3_5_Repository.Models;
using WeighingSystemUPPCV3_5_Repository.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SysUtility.Extensions;

namespace WeighingSystemUPPCV3_5_Repository.Repositories
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly DatabaseContext dbContext;

        public CustomerRepository(DatabaseContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public Customer Create(Customer model)
        {
            model.CustomerCode = (model.CustomerCode ?? "").ToUpper();
            model.CustomerName = (model.CustomerName ?? "").ToUpper();
            model.Location = (model.Location ?? "").ToUpper();
            model.ContactNum = (model.Location ?? "").ToUpper();
            model.ContactPerson = model.ContactPerson;
            dbContext.Customers.Add(model);
            dbContext.SaveChanges();
            return model;
        }

        public bool Delete(Customer model)
        {
                dbContext.Customers.Remove(model);
                dbContext.SaveChanges();
                return true;
        }

        public bool BulkDelete(string[] id)
        {
                if (id == null) return false;
                if (id.Length == 0) return false;

                var entitiesToDelete = dbContext.Customers.Where(a => id.Contains(a.CustomerId.ToString()));

                dbContext.Customers.RemoveRange(entitiesToDelete);
                dbContext.SaveChanges();
                return true;
        }

        public IQueryable<Customer> Get(Customer parameters = null)
        {
            var sqlRawParams = GetSqlRawParameter(parameters);
            if (sqlRawParams.SqlParameters.Count == 0) return dbContext.Customers.AsNoTracking();
            return dbContext.Customers.FromSqlRaw(sqlRawParams.SqlQuery, sqlRawParams.SqlParameters.ToArray()).AsNoTracking();
        }

        public Customer GetById(long id)
        {
            return dbContext.Customers.Find(id);
        }

        public Customer GetById(string id)
        {
            return dbContext.Customers.Find(id);
        }

        public Customer Update(Customer model)
        {
            var entity = dbContext.Customers.Find(model.CustomerId);
            if (entity == null)
            {
                throw new Exception("Selected Record does not exists.");
            }

            entity.CustomerCode = model.CustomerCode;
            entity.CustomerName = model.CustomerName;
            entity.ContactNum = model.ContactNum;
            entity.ContactPerson = model.ContactPerson;
            entity.Location = model.Location;
            entity.IsActive = model.IsActive;

            dbContext.Customers.Update(entity);
            dbContext.SaveChanges();
            return entity;
        }

        public bool ValidateCode(Customer model)
        {
            var existing = Get().FirstOrDefault(a => a.CustomerCode == model.CustomerCode);
            if (existing == null) return true;
            return existing.CustomerId == model.CustomerId;
        }

        public bool ValidateName(Customer model)
        {
            var existing = Get().FirstOrDefault(a => a.CustomerName == model.CustomerName);
            if (existing == null) return true;
            return existing.CustomerId == model.CustomerId;
        }

        public SqlRawParameter GetSqlRawParameter(Customer parameters)
        {
            if (parameters == null) return new SqlRawParameter();
            var sqlQry = new StringBuilder();
            sqlQry.AppendLine("SELECT * FROM Customers");
            var whereClauses = new List<string>();
            var sqlParams = new List<SqlParameter>();

            if (!parameters.CustomerCode.IsNull())
            {
                sqlParams.Add(new SqlParameter(nameof(parameters.CustomerCode).Parametarize(), parameters.CustomerCode));
                whereClauses.Add($"{nameof(parameters.CustomerCode)} = {nameof(parameters.CustomerCode).Parametarize()}");
            }
            if (!parameters.CustomerName.IsNull())
            {
                sqlParams.Add(new SqlParameter(nameof(parameters.CustomerName).Parametarize(), parameters.CustomerName));
                whereClauses.Add($"{nameof(parameters.CustomerName)} = {nameof(parameters.CustomerName).Parametarize()}");
            }
            if (!parameters.IsActive.IsNull())
            {
                sqlParams.Add(new SqlParameter(nameof(parameters.IsActive).Parametarize(), parameters.IsActive));
                whereClauses.Add($"{nameof(parameters.IsActive)} = {nameof(parameters.IsActive).Parametarize()}");
            }
            if (whereClauses.Count > 0)
            {
                sqlQry.AppendLine(" WHERE ");
                sqlQry.AppendLine(String.Join(" AND ", whereClauses.ToArray()));
            }

            return new SqlRawParameter() { SqlParameters = sqlParams, SqlQuery = sqlQry.ToString() };
        }

        public Customer GetByName(string name)
        {
            return Get().FirstOrDefault(a => a.CustomerName == name.DefaultIfEmpty());
        }

        public void MigrateOldDb()
        {

            var oldCustomers = dbContext.CustomersOld.AsNoTracking().ToList();
            foreach (var oldCustomer in oldCustomers)
            {

                var customer = new Customer()
                {
                    IsActive = oldCustomer.Status,
                    ContactNum = oldCustomer.ContactNo,
                    ContactPerson = oldCustomer.ContactPerson,
                    CustomerCode = oldCustomer.CustomerCode,
                    CustomerIdOld = oldCustomer.CustomerID,
                    CustomerName = oldCustomer.CustomerName,
                    Location = oldCustomer.CustomerName,
                };

                dbContext.Customers.Add(customer);
                dbContext.SaveChanges();
            };
        }
    }
}
