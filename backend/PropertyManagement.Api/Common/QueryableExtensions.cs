using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using PropertyManagement.Api.Models;
using System.Linq.Dynamic.Core;
using System.Text.RegularExpressions;

namespace PropertyManagement.Api.Common
{
    public static class QueryableExtensions
    {
        // Pagination
        public static IQueryable<T> ApplyPagination<T>(this IQueryable<T> query, int pageNumber, int pageSize)
        {
            return query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize);
        }

        // Sorting
        public static IQueryable<T> ApplySort<T>(this IQueryable<T> query, string? sortBy) where T : class
        {
            if (string.IsNullOrWhiteSpace(sortBy))
                return query;

            var allowedProperties = typeof(T)
                .GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance)
                .Select(p => p.Name)
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            var sortExpressions = new List<string>();

            foreach (var part in sortBy.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
            {
                var tokens = part.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                if (tokens.Length == 0 || !allowedProperties.Contains(tokens[0]))
                    continue;

                var direction = tokens.Length > 1 && tokens[1].Equals("desc", StringComparison.OrdinalIgnoreCase)
                    ? "descending"
                    : "ascending";

                sortExpressions.Add($"{tokens[0]} {direction}");
            }

            return sortExpressions.Count > 0
                ? query.OrderBy(string.Join(", ", sortExpressions))
                : query;
        }

        // Helper to normalize phone number
        public static string NormalizePhoneNumber(string? phoneNumber)
        {
            if (string.IsNullOrWhiteSpace(phoneNumber))
            {
                return string.Empty;
            }

            return Regex.Replace(phoneNumber, "[^0-9]", "");
        }

        // User Search
        public static IQueryable<AppUser> ApplySearch(this IQueryable<AppUser> query, string? search)
        {
            if (string.IsNullOrWhiteSpace(search))
                return query;

            search = search.Trim();
            var like = $"%{search}%";

            query = query.Where(u =>
                EF.Functions.Like(u.FirstName, like) ||
                EF.Functions.Like(u.LastName, like) ||
                EF.Functions.Like(u.Email, like)
            );

            return query;
        }


        // AllowedEmail search
        public static IQueryable<AllowedEmail> ApplySearch(this IQueryable<AllowedEmail> query, string? search)
        {
            if (string.IsNullOrWhiteSpace(search))
                return query;

            search = search.Trim();
            var like = $"%{search}%";

            query = query
                .Where(e => EF.Functions.Like(e.Email, like));

            return query;
        }

        // Guest search
        public static IQueryable<Guest> ApplySearch(this IQueryable<Guest> query, string? search)
        {
            if (string.IsNullOrWhiteSpace(search))
                return query;

            search = search.Trim();
            var like = $"%{search}%";
            var normalizedSearch = NormalizePhoneNumber(search);
            var normalizedLike = $"%{normalizedSearch}%";

            query = query.Where(g =>
                EF.Functions.Like(g.FirstName, like) ||
                EF.Functions.Like(g.LastName, like) ||
                EF.Functions.Like(g.Address, like) ||
                EF.Functions.Like(g.City, like) ||
                EF.Functions.Like(g.State, like) ||
                EF.Functions.Like(g.ZipCode, like) ||
                EF.Functions.Like(g.Email, like) ||
                EF.Functions.Like(g.Notes, like) || // remove for performance??
                EF.Functions.Like(g.Company, like) ||
                (!string.IsNullOrEmpty(normalizedSearch) &&
                EF.Functions.Like(g.NormalizedPhoneNumber ?? "", normalizedLike))
            );

            return query;
        }
    }
}
