using Domain.Interfaces.Repositories;
using Domain.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;

namespace Repository.Implementation
{
    public class SearchTermRepository : ISearchTermRepository
    {
        private readonly string _connectionString;

        public SearchTermRepository()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["ApplicationContext"].ConnectionString;
        }


        public async Task<int> AddNewSearchedTerm(SearchedTerm searchedTerm)
        {
            int id;
            var now = DateTime.UtcNow;

            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                sqlConnection.Open();

                string sqlQuery = @"INSERT INTO [dbo].[SearchedTerms]([Inserted],[Updated],[Term],[IsDeleted],[Count]) 
                                    VALUES (@Inserted,@Updated,@Term,@IsDeleted,@Count);
                                    SELECT CAST(SCOPE_IDENTITY() as int)";

                id = await sqlConnection.ExecuteScalarAsync<int>(sqlQuery,
                    new
                    {
                        Inserted = now,
                        Updated = now,
                        Term = searchedTerm.Term.Trim(),
                        IsDeleted = false,
                        Count = 1
                    });

                sqlConnection.Close();
            }

            return id;
        }

        public async Task<SearchedTerm> FindSearchedTerm(string term)
        {
            SearchedTerm existingSearchedTerm = null;

            if (string.IsNullOrEmpty(term))
                return existingSearchedTerm;

            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                sqlConnection.Open();
                var selectTagQuery = "select * from [dbo].[SearchedTerms] where Term = @Term";

                var existingSearchedTerms = await sqlConnection.QueryAsync<SearchedTerm>(selectTagQuery, new { Term = term.Trim() });
                existingSearchedTerm = existingSearchedTerms.FirstOrDefault();
            }

            return existingSearchedTerm;
        }

        public async Task UpdateSearchedTerm(SearchedTerm searchedTerm)
        {
            var now = DateTime.UtcNow;

            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                sqlConnection.Open();

                await sqlConnection.ExecuteAsync("update [dbo].[SearchedTerms] set Term = @term, Updated = @updated, Count = @count where Id = @id", new { searchedTerm.Term, updated = now, searchedTerm.Count, id = searchedTerm.Id });
            }
        }
    }
}
