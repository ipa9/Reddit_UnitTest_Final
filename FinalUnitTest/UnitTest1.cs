using Microsoft.EntityFrameworkCore;
using Reddit.Models;
using Reddit.Repositories;

namespace Reddit.UnitTest;

public class UnitTest1
{

    private IPostsRepository GetPostsRepostory()
    {

        var dbName = Guid.NewGuid().ToString();
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: dbName)
            .Options;

        var dbContext = new ApplicationDbContext(options);

        dbContext.Posts.Add(new Post { Title = "What is Python?", Content = "It's a snake :DDD", Upvotes = 3546, Downvotes = 345 });
        dbContext.Posts.Add(new Post { Title = "Ice Cream", Content = "Vanile ice cream is the best one!", Upvotes = 98434793, Downvotes = 4656 });
        dbContext.Posts.Add(new Post { Title = "One Like Post", Content = "this post will get only one like", Upvotes = 1, Downvotes = 998 });
        dbContext.Posts.Add(new Post { Title = "I want millions of dollars", Content = "Gotta find the way to become a millionaire", Upvotes = 8765445, Downvotes = 432 });
        dbContext.Posts.Add(new Post { Title = ".Net Subject", Content = "It great experience to go through the subject", Upvotes = 99999999, Downvotes = 0 });
        dbContext.Posts.Add(new Post { Title = "Thesis Presentation", Content = "Will be presenting the project soon", Upvotes = 999999, Downvotes = 0 });
        dbContext.Posts.Add(new Post { Title = "The sport", Content = "I do judo, and can easily brake anyone's bones", Upvotes = 12345678, Downvotes = 9876543 });
        dbContext.Posts.Add(new Post { Title = "I'm the coach for my professor", Content = "I became a coach for my .Net professor, he's doing great tho", Upvotes = 99911999, Downvotes = 1 });
        dbContext.SaveChanges();

        return new PostsRepository(dbContext);
    }

    [Fact]
    public async Task GetPosts_ReturnsCorrectPagination()
    {
        var postsRepository = GetPostsRepostory();
        var posts = await postsRepository.GetPosts(2, 4, null, null, null);

        Assert.Equal(4, posts.Items.Count);
        Assert.Equal(8, posts.TotalCount);
        Assert.False(posts.HasNextPage);
        Assert.True(posts.HasPreviousPage);
    }


    [Fact]
    public async Task GetPosts_SortPopularCorrect()
    {
        var postsRepository = GetPostsRepostory();
        var posts = await postsRepository.GetPosts(1, 4, null, "popular", false);

        Assert.Equal(4, posts.Items.Count);
        Assert.Equal(8, posts.TotalCount);
        Assert.True(posts.HasNextPage);
        Assert.False(posts.HasPreviousPage);
        Assert.Equal(".Net Subject", posts.Items.First().Title);
    }

    [Fact]
    public async Task GetPosts_SortPositiveCorrect()
    {
        var postsRepository = GetPostsRepostory();
        var posts = await postsRepository.GetPosts(2, 2, null, "positivity", false);

        Assert.True(posts.HasNextPage);
        Assert.True(posts.HasPreviousPage);
        Assert.Equal("What is Python?", posts.Items.First().Title);
    }

    [Fact]
    public async Task GetPosts_SearchTermCorrect()
    {
        var postsRepository = GetPostsRepostory();
        var posts = await postsRepository.GetPosts(page: 1, pageSize: 8, searchTerm: "professor", SortTerm: null);
        Assert.Single(posts.Items);
        Assert.Equal("I'm the coach for my professor", posts.Items.First().Title);
    }

    [Fact]
    public async Task GetPosts_InvalidPageSize_ThrowsArgumentOutOfRangeException()
    {
        var repository = GetPostsRepostory();

        var exception = await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => repository.GetPosts(page: 1, pageSize: 0, searchTerm: null, SortTerm: null));
        Assert.Equal("pageSize", exception.ParamName);
    }

    [Fact]
    public async Task GetPosts_InvalidPage_ThrowsArgumentException()
    {
        var repository = GetPostsRepostory();

        var exception = await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => repository.GetPosts(page: 0, pageSize: 10, searchTerm: null, SortTerm: null));
        Assert.Equal("page", exception.ParamName);
    }


}