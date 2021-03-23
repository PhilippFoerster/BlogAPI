﻿using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using BlogAPI.Interfaces;
using BlogAPI.Models;
using BlogAPI.Models.Respond;
using Microsoft.EntityFrameworkCore;

namespace BlogAPI
{
    public static class Extensions
    {
        //from https://stackoverflow.com/a/53476825/14742712
        public static IQueryable<T> If<T>(this IQueryable<T> source, bool condition, Func<IQueryable<T>, IQueryable<T>> transform) => condition ? transform(source) : source;

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
                Interests = (List<string>)x.Interests.Select(x => x.Name), 
                Username = x.UserName
            });

        public static IQueryable<CommentResponse> SelectResponse(this IQueryable<Comment> source)
            => source.Select(x => new CommentResponse
            {
                ArticleId = x.ArticleId,
                CreatedAt = x.CreatedAt,
                CreatedBy = x.CreatedBy.GetUserResponse(),
                Likes = x.LikedBy.Count, 
                Id = x.Id, 
                Text = x.Text
            });

        public static IQueryable<ArticleResponse> SelectResponse(this IQueryable<Article> source)
            => source.Select(x => new ArticleResponse{
                Topics = x.Topics.Select(x => x.Name),
                Comments = new List<CommentResponse>(),
                Image = x.Image, 
                Caption = x.Caption,
                CreatedAt = x.CreatedAt,
                CreatedBy = x.CreatedBy.GetUserResponse(),
                Id = x.Id,
                Text = x.Text
            });

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
