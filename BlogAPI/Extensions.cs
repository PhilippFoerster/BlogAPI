using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using BlogAPI.Interfaces;
using BlogAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace BlogAPI
{
    public static class Extensions
    {
        public static string GetUser(this HttpRequest request) 
            => !request.Headers.ContainsKey("Authorization") ? null : Encoding.UTF8.GetString(Convert.FromBase64String(request.Headers["Authorization"].ToString()?[6..])).Split(":")[0];

        //from https://stackoverflow.com/a/53476825/14742712
        public static IQueryable<T> If<T>(this IQueryable<T> source, bool condition, Func<IQueryable<T>, IQueryable<T>> transform) => condition ? transform(source) : source;

        public static IQueryable<Comment> IncludeLikes(this IQueryable<Comment> source) 
            => source.Select(x => new Comment { Id = x.Id, ArticleId = x.ArticleId, CreatedById = x.CreatedById, CreatedAt = x.CreatedAt, Likes = x.LikedBy.Count });

        public static T UpdateFrom<T, U>(this T source, U update) where T : IUpdateable where U : IUpdater
        {
            update.GetType().GetProperties().ToList().ForEach(p =>
                source.GetType().GetProperty(p.Name)?.SetValue(source, p.GetValue(update)));
            return source;
        }

        public static bool HasNullProperty(this object o) => o.GetType().GetProperties().Any(x => x.GetValue(o) is null);

        public static string GetUserID(this ClaimsPrincipal user) => user.Claims.First(x => x.Type == "id").Value;
    }
}
