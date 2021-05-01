using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using BlogAPI.Models;
using BlogAPI.Models.Respond;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace BlogAPI
{
    public static class Extensions
    {
        //from https://stackoverflow.com/a/53476825/14742712
        public static IQueryable<T> If<T>(this IQueryable<T> source, bool condition, Func<IQueryable<T>, IQueryable<T>> transform) => condition ? transform(source) : source;
        public static IQueryable<T> Apply<T>(this IQueryable<T> source, Func<IQueryable<T>, IQueryable<T>> transform) =>transform(source);

        public static IQueryable<Comment> IncludeLikes(this IQueryable<Comment> source)
            => source.Select(x => new Comment
            {
                Id = x.Id,
                ArticleId = x.ArticleId,
                CreatedById = x.CreatedById,
                CreatedAt = x.CreatedAt, 
                Likes = x.LikedBy.Count,
                Text = x.Text,
            });
        public static IQueryable<UserResponse> SelectResponse(this IQueryable<User> source)
            => source.Select(x => new UserResponse
            {
                Email = x.Email, 
                Id = x.Id, 
                Username = x.UserName
            });

        public static IQueryable<CommentResponse> SelectResponse(this IQueryable<Comment> source, string userId)
            => source.Select(x => new CommentResponse
            {
                ArticleId = x.ArticleId,
                CreatedAt = x.CreatedAt,
                CreatedBy = x.CreatedBy.GetUserResponse(),
                Likes = x.LikedBy.Count, 
                Id = x.Id, 
                Text = x.Text,
                Liked = x.LikedBy.Contains(new User{Id = userId})
            });

        public static IQueryable<ArticleResponse> SelectResponse(this IQueryable<Article> source)
            => source.Select(x => new ArticleResponse{
                Topics = x.Topics.Select(x => x.Name),
                Image = x.Image, 
                Caption = x.Caption,
                CreatedAt = x.CreatedAt,
                CreatedBy = x.CreatedBy.GetUserResponse(),
                Id = x.Id,
                Text = x.Text
            });
        
        public static bool HasNullProperty(this object o) => o.GetType().GetProperties().Any(x => x.GetValue(o) is null);

        public static string GetUserID(this ClaimsPrincipal user) => user.Claims.FirstOrDefault(x => x.Type == "id")?.Value ?? "";

        public static List<string> GetErrors(this ModelStateDictionary modelState)
            => modelState.Values.SelectMany(x => x.Errors.Select(x => x.ErrorMessage)).ToList();

        public static string GetDescription(this Enum value)
        {
            Type type = value.GetType();
            string name = Enum.GetName(type, value);
            if (name != null)
            {
                FieldInfo field = type.GetField(name);
                if (field != null)
                {
                    if (Attribute.GetCustomAttribute(field,
                        typeof(DescriptionAttribute)) is DescriptionAttribute attr)
                    {
                        return attr.Description;
                    }
                }
            }
            return null;
        }
    }
}
