using Microsoft.EntityFrameworkCore;
using PropertyManagement.Api.Models;
using System.Linq.Dynamic.Core;
using System.Reflection;
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
        //public static IQueryable<T> ApplySort<T>(this IQueryable<T> query, string? sortBy) where T : class
        //{
        //    if (string.IsNullOrWhiteSpace(sortBy))
        //        return query;

        //    var allowedProperties = typeof(T)
        //        .GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance)
        //        .Select(p => p.Name)
        //        .ToHashSet(StringComparer.OrdinalIgnoreCase);

        //    var sortExpressions = new List<string>();

        //    foreach (var part in sortBy.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
        //    {
        //        var tokens = part.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        //        if (tokens.Length == 0 || !allowedProperties.Contains(tokens[0]))
        //            continue;

        //        var direction = tokens.Length > 1 && tokens[1].Equals("desc", StringComparison.OrdinalIgnoreCase)
        //            ? "descending"
        //            : "ascending";

        //        sortExpressions.Add($"{tokens[0]} {direction}");
        //    }

        //    return sortExpressions.Count > 0
        //        ? query.OrderBy(string.Join(", ", sortExpressions))
        //        : query;
        //}

        public static IQueryable<T> ApplySort<T>(this IQueryable<T> query, string? sortBy) where T : class
        {
            if (string.IsNullOrWhiteSpace(sortBy))
                return query;

            var sortExpressions = new List<string>();

            foreach (var part in sortBy
                .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
            {
                var tokens = part.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                if (tokens.Length == 0) continue;

                var sortPath = tokens[0];

                if (!IsValidPropertyPath(typeof(T), sortPath))
                    continue;

                var direction = tokens.Length > 1 && tokens[1].Equals("desc", StringComparison.OrdinalIgnoreCase)
                    ? "descending"
                    : "ascending";

                sortExpressions.Add($"{sortPath} {direction}");
            }

            return sortExpressions.Count > 0
                ? query.OrderBy(string.Join(", ", sortExpressions)) // System.Linq.Dynamic.Core
                : query;
        }

        private static bool IsValidPropertyPath(Type rootType, string propertyPath)
        {
            var current = rootType;

            foreach (var segment in propertyPath.Split('.', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
            {
                var prop = current.GetProperty(segment,
                    BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);

                if (prop is null)
                    return false;

                current = prop.PropertyType;
            }

            return true;
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

        // Booking Search
        public static IQueryable<Booking> ApplySearch(this IQueryable<Booking> query, string? search)
        {
            if (string.IsNullOrWhiteSpace(search))
                return query;

            search = search.Trim();
            var like = $"%{search}%";

            query = query.Where(b =>
                EF.Functions.Like(b.Guest.FirstName, like) ||
                EF.Functions.Like(b.Guest.LastName, like) ||
                EF.Functions.Like(b.Unit.UnitNumber, like) ||
                EF.Functions.Like(b.CreatedByUser != null
                        ? (b.CreatedByUser.FirstName == null || b.CreatedByUser.FirstName == ""
                           || b.CreatedByUser.LastName == null || b.CreatedByUser.LastName == ""
                              ? b.CreatedByUser.Email
                              : b.CreatedByUser.FirstName + " " + b.CreatedByUser.LastName)
                        : null, like) ||
                EF.Functions.Like(b.Status, like) ||
                EF.Functions.Like(b.Notes, like) ||
                EF.Functions.Like(b.Guest.Address, like) ||
                EF.Functions.Like(b.Guest.City, like) ||
                EF.Functions.Like(b.Guest.State, like) ||
                EF.Functions.Like(b.Guest.ZipCode, like) ||
                EF.Functions.Like(b.CardLastFour.ToString(), like));

            return query;
        }
    }
}
