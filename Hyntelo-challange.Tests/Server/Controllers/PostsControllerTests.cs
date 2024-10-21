using Hyntelo_challenge.Server.Controllers;
using Hyntelo_challenge.Server.Models;
using Hyntelo_challenge.Server.Repositories;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Hyntelo_challange.Tests.Server.Controllers
{
    public class PostsControllerTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IRepository<Post>> _mockPostRepository;
        private readonly Mock<IRepository<Comment>> _mockCommentRepository;
        private readonly PostsController _controller;

        public PostsControllerTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockPostRepository = new Mock<IRepository<Post>>();
            _mockCommentRepository = new Mock<IRepository<Comment>>();

            // Setup UnitOfWork to return our mock repositories
            _mockUnitOfWork.Setup(uow => uow.PostRepository)
                .Returns(_mockPostRepository.Object);
            _mockUnitOfWork.Setup(uow => uow.CommentRepository)
                .Returns(_mockCommentRepository.Object);

            _controller = new PostsController(_mockUnitOfWork.Object);
        }

        [Fact]
        public async Task GetPosts_ReturnsOkResult_WithPaginatedPosts()
        {
            // Arrange
            var expectedPosts = new PaginatedResult<Post>
            {
                Items = new List<Post>
                {
                    new Post { Id = 1, Title = "Test Post", Body="Test Body" }
                },
                TotalCount = 1,
                PageNumber = 1,
                PageSize = 10
            };

            _mockPostRepository
                .Setup(repo => repo.GetAllAsync(It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(expectedPosts);

            // Act
            var result = await _controller.GetPosts();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedPosts = Assert.IsType<PaginatedResult<Post>>(okResult.Value);
            Assert.Equal(expectedPosts.Items.Count, returnedPosts.Items.Count);
            Assert.Equal(expectedPosts.TotalCount, returnedPosts.TotalCount);
        }

        [Fact]
        public async Task GetPost_WithValidId_ReturnsOkResult()
        {
            // Arrange
            var expectedPost = new Post { Id = 1, Title = "Test Post", Body = "Test Body" };
            _mockPostRepository
                .Setup(repo => repo.GetByIdAsync(1))
                .ReturnsAsync(expectedPost);

            // Act
            var result = await _controller.GetPost(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedPost = Assert.IsType<Post>(okResult.Value);
            Assert.Equal(expectedPost.Id, returnedPost.Id);
        }

        [Fact]
        public async Task GetPost_WithInvalidId_ReturnsNotFound()
        {
            // Arrange
            _mockPostRepository
                .Setup(repo => repo.GetByIdAsync(99))
                .ReturnsAsync((Post?)null);

            // Act
            var result = await _controller.GetPost(99);

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task CreatePost_ReturnsCreatedAtAction()
        {
            // Arrange
            var postToCreate = new Post { Title = "New Post", Body = "Test Body" };
            var createdPost = new Post { Id = 1, Title = "New Post", Body = "Test Body" };

            _mockPostRepository
                .Setup(repo => repo.AddAsync(It.IsAny<Post>()))
                .ReturnsAsync(createdPost);

            // Act
            var result = await _controller.CreatePost(postToCreate);

            // Assert
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            Assert.Equal(nameof(PostsController.GetPost), createdAtActionResult.ActionName);
            var returnedPost = Assert.IsType<Post>(createdAtActionResult.Value);
            Assert.Equal(createdPost.Id, returnedPost.Id);
        }

        [Fact]
        public async Task GetComments_WithValidPostId_ReturnsOkResult()
        {
            // Arrange
            var postId = 1;
            var expectedPost = new Post { Id = postId, Title = "New Post", Body = "Test Body" };
            var expectedComments = new PaginatedResult<Comment>
            {
                Items = new List<Comment>
                {
                    new Comment { Id = 1, PostId = postId,Post= expectedPost, Body = "Test Body" }
                },
                TotalCount = 1,
                PageNumber = 1,
                PageSize = 10
            };

            _mockPostRepository
                .Setup(repo => repo.GetByIdAsync(postId))
                .ReturnsAsync(expectedPost);

            _mockCommentRepository
                .Setup(repo => repo.GetAllAsync(
                    It.IsAny<Expression<Func<Comment, bool>>>(),
                    It.IsAny<int>(),
                    It.IsAny<int>()))
                .ReturnsAsync(expectedComments);

            // Act
            var result = await _controller.GetComments(postId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedComments = Assert.IsType<PaginatedResult<Comment>>(okResult.Value);
            Assert.Equal(expectedComments.Items.Count, returnedComments.Items.Count);
        }

        [Fact]
        public async Task GetComments_WithInvalidPostId_ReturnsNotFound()
        {
            Post? nullPost = null;
            _mockPostRepository
                .Setup(repo => repo.GetByIdAsync(99))
                .ReturnsAsync(nullPost);

            // Act
            var result = await _controller.GetComments(99);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result.Result);
        }

        [Fact]
        public async Task GetComment_WithValidIds_ReturnsOkResult()
        {
            // Arrange
            var postId = 1;
            var commentId = 1;
            var expectedComment = new Comment { Id = 1, PostId = postId, Body = "Test Body" };

            _mockCommentRepository
                .Setup(repo => repo.GetByIdAsync(commentId))
                .ReturnsAsync(expectedComment);

            // Act
            var result = await _controller.GetComment(postId, commentId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedComment = Assert.IsType<Comment>(okResult.Value);
            Assert.Equal(expectedComment.Id, returnedComment.Id);
        }

        [Fact]
        public async Task GetComment_WithInvalidCommentId_ReturnsNotFound()
        {
            // Arrange
            _mockCommentRepository
                .Setup(repo => repo.GetByIdAsync(99))
                .ReturnsAsync((Comment?)null);

            // Act
            var result = await _controller.GetComment(1, 99);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result.Result);
        }

        [Fact]
        public async Task GetComment_WithMismatchedPostId_ReturnsBadRequest()
        {
            // Arrange
            var comment = new Comment { Id = 1, PostId = 2, Body = "Test Body" };
            _mockCommentRepository
                .Setup(repo => repo.GetByIdAsync(1))
                .ReturnsAsync(comment);

            // Act
            var result = await _controller.GetComment(1, 1); // Different postId than the comment's PostId

            // Assert
            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        [Fact]
        public async Task AddComment_ReturnsCreatedAtAction()
        {
            // Arrange
            var postId = 1;
            var commentToCreate = new Comment { Body = "New Comment" };
            var createdComment = new Comment { Id = 1, PostId = postId, Body = "New Comment" };

            _mockCommentRepository
                .Setup(repo => repo.AddAsync(It.IsAny<Comment>()))
                .ReturnsAsync(createdComment);

            // Act
            var result = await _controller.AddComment(postId, commentToCreate);

            // Assert
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            Assert.Equal(nameof(PostsController.GetPost), createdAtActionResult.ActionName);
            var returnedComment = Assert.IsType<Comment>(createdAtActionResult.Value);
            Assert.Equal(createdComment.Id, returnedComment.Id);
            Assert.Equal(postId, returnedComment.PostId);
        }
    }
}
