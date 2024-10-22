using Hyntelo_challenge.Server.Models;

namespace Hyntelo_challenge.Server.DTO
{
    public class PostDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public string AuthorName { get; set; } 
    }

    public class CommentDto
    {
        public int Id { get; set; }
        public int PostId { get; set; }
        public int UserId { get; set; }
        public required string Body { get; set; }
        public string AuthorName { get; set; }
        public virtual Post? Post { get; set; }
    }

    public class LoginDto
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
