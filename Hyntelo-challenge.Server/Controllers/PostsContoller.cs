using Hyntelo_challenge.Server.DTO;
using Hyntelo_challenge.Server.Models;
using Hyntelo_challenge.Server.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Hyntelo_challenge.Server.Controllers
{
    /// <summary>
    /// Controller for managing blog posts and their associated comments.
    /// Provides endpoints for CRUD operations on posts and comments.
    /// All endpoints require authentication.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class PostsController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        /// <summary>
        /// Initializes a new instance of the PostsController.
        /// </summary>
        /// <param name="unitOfWork">Repository pattern implementation for database operations.</param>
        public PostsController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Retrieves a paginated list of posts.
        /// </summary>
        /// <param name="page">The page number to retrieve (default: 1).</param>
        /// <param name="pageSize">The number of items per page (default: 10).</param>
        /// <returns>A paginated list of posts.</returns>
        /// <response code="200">Returns the paginated list of posts</response>
        /// <response code="401">If the user is not authenticated</response>
        [HttpGet(Name = "GetPosts")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<PaginatedResult<PostDto>>> GetPosts(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            return Ok(await _unitOfWork.PostRepository.GetAllAsync(page, pageSize));
        }

        /// <summary>
        /// Retrieves a specific post by its ID.
        /// </summary>
        /// <param name="id">The ID of the post to retrieve.</param>
        /// <returns>The requested post if found.</returns>
        /// <response code="200">Returns the requested post</response>
        /// <response code="404">If the post is not found</response>
        /// <response code="401">If the user is not authenticated</response>
        [HttpGet("{id}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<PostDto>> GetPost(int id)
        {
            var post = await _unitOfWork.PostRepository.GetByIdAsync(id);
            if (post == null)
                return NotFound();
            return Ok(post);
        }

        /// <summary>
        /// Creates a new post.
        /// </summary>
        /// <param name="post">The post data to create.</param>
        /// <returns>The created post with its assigned ID.</returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /api/posts
        ///     {
        ///        "title": "Sample Post",
        ///        "content": "This is the content of the post",
        ///        "authorId": 1
        ///     }
        ///
        /// </remarks>
        /// <response code="201">Returns the newly created post</response>
        /// <response code="401">If the user is not authenticated</response>
        [HttpPost]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<Post>> CreatePost(Post post)
        {
            var created = await _unitOfWork.PostRepository.AddAsync(post);
            return CreatedAtAction(nameof(GetPost), new { id = created.Id }, created);
        }

        /// <summary>
        /// Retrieves a paginated list of comments for a specific post.
        /// </summary>
        /// <param name="postId">The ID of the post to get comments for.</param>
        /// <param name="page">The page number to retrieve (default: 1).</param>
        /// <param name="pageSize">The number of items per page (default: 10).</param>
        /// <returns>A paginated list of comments for the specified post.</returns>
        /// <response code="200">Returns the list of comments</response>
        /// <response code="404">If the post is not found</response>
        /// <response code="401">If the user is not authenticated</response>
        [HttpGet("{postId}/comments")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<PaginatedResult<Comment>>> GetComments(
            int postId,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            var post = await _unitOfWork.PostRepository.GetByIdAsync(postId);
            if (post == null)
            {
                return NotFound($"Post with ID {postId} not found.");
            }

            var paginatedComments = await _unitOfWork.CommentRepository.GetAllAsync(
                postId,
                page,
                pageSize);
            return Ok(paginatedComments);
        }

        /// <summary>
        /// Retrieves a specific comment from a post.
        /// </summary>
        /// <param name="postId">The ID of the post containing the comment.</param>
        /// <param name="commentId">The ID of the comment to retrieve.</param>
        /// <returns>The requested comment if found and belongs to the specified post.</returns>
        /// <response code="200">Returns the requested comment</response>
        /// <response code="404">If the comment is not found</response>
        /// <response code="400">If the comment doesn't belong to the specified post</response>
        /// <response code="401">If the user is not authenticated</response>
        [HttpGet("{postId}/comments/{commentId}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
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

        /// <summary>
        /// Adds a new comment to a specific post.
        /// </summary>
        /// <param name="postId">The ID of the post to add the comment to.</param>
        /// <param name="comment">The comment data to create.</param>
        /// <returns>The created comment.</returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /api/posts/{postId}/comments
        ///     {
        ///        "content": "This is a comment",
        ///        "authorId": 1
        ///     }
        ///
        /// </remarks>
        /// <response code="201">Returns the newly created comment</response>
        /// <response code="401">If the user is not authenticated</response>
        [HttpPost("{postId}/comments")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<Comment>> AddComment(int postId, Comment comment)
        {
            var created = await _unitOfWork.CommentRepository.AddAsync(comment);
            return CreatedAtAction(nameof(GetPost), new { id = postId }, created);
        }
    }
}