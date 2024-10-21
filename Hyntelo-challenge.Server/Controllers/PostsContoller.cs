using Hyntelo_challenge.Server.Models;
using Hyntelo_challenge.Server.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Hyntelo_challenge.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PostsController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public PostsController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<ActionResult<PaginatedResult<Post>>> GetPosts(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            
            return Ok(await _unitOfWork.PostRepository.GetAllAsync(page, pageSize));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Post>> GetPost(int id)
        {
            var post = await _unitOfWork.PostRepository.GetByIdAsync(id);
            if (post == null)
                return NotFound();
            return Ok(post);
        }

        [HttpPost]
        public async Task<ActionResult<Post>> CreatePost(Post post)
        {
            var created = await _unitOfWork.PostRepository.AddAsync(post);
            return CreatedAtAction(nameof(GetPost), new { id = created.Id }, created);
        }

        [HttpGet("{postId}/comments")]
        public async Task<ActionResult<PaginatedResult<Comment>>> GetComments(
       int postId,
       [FromQuery] int page = 1,
       [FromQuery] int pageSize = 10)
        {
            // First verify that the post exists
            var post = await _unitOfWork.PostRepository.GetByIdAsync(postId);
            if (post == null)
            {
                return NotFound($"Post with ID {postId} not found.");
            }

            // Get comments for the specific post with pagination
            var paginatedComments = await _unitOfWork.CommentRepository.GetAllAsync(
                c => c.PostId == postId,
                page,
                pageSize);

            return Ok(paginatedComments);
        }

        [HttpGet("{postId}/comments/{commentId}")]
        public async Task<ActionResult<Comment>> GetComment(int postId, int commentId)
        {
            var comment = await _unitOfWork.CommentRepository.GetByIdAsync(commentId);

            if (comment == null)
            {
                return NotFound($"Comment with ID {commentId} not found.");
            }

            if (comment.PostId != postId)
            {
                return BadRequest("Comment does not belong to the specified post.");
            }

            return Ok(comment);
        }

        [HttpPost("{postId}/comments")]
        public async Task<ActionResult<Comment>> AddComment(int postId, Comment comment)
        {
            comment.PostId = postId;
            var created = await _unitOfWork.CommentRepository.AddAsync(comment);
            return CreatedAtAction(nameof(GetPost), new { id = postId }, created);
        }
    }
}
