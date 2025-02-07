using Newtonsoft.Json;
using PostApi.Models;
using PostApi.Utils;

namespace PostApi.Services;

public class PostService : IPostService
{
    private readonly List<Post> _posts = [];

    public readonly LoggerProvider _logger;
    public readonly string _nameService = "PostService";

    public PostService(LoggerProvider logger)
    {
        _logger = logger;
    }

    public async Task<IEnumerable<Post>> GetAllPostsAsync()
    {
        if (_posts.Count == 0)
            await PopulateDatabase();

        return _posts;
    }

    public async Task<Post> GetPostByIdAsync(int id)
    {
        try
        {
            if (_posts.Count == 0)
                await PopulateDatabase();

            _logger.Log(LogLevel.Trace, 200, _nameService, $"Pesquisa de post por ID({id}) realizada com sucesso!");
            return _posts.FirstOrDefault(p => p.Id == id)!;
        }
        catch (Exception ex) 
        {
            _logger.Log(LogLevel.Debug, 5000, _nameService, $"Ocorreu um erro ao tentar localizar post pelo ID({id}): {ex.Message}");
            return await Task.FromResult(new Post());
        }
    }

    public Task<Post> CreatePostAsync(Post newPost)
    {
        try
        {
            if (newPost.Id == 0 || newPost.UserId == 0 || string.IsNullOrEmpty(newPost.Title) || string.IsNullOrEmpty(newPost.Body))
                return Task.FromResult<Post>(null!);

            _posts.Add(newPost);
            _logger.Log(LogLevel.Trace, 200, _nameService, $"Post {newPost} adicionado com sucesso!");
            return Task.FromResult(newPost);
        }
        catch (Exception ex) 
        {
            _logger.Log(LogLevel.Debug, 5000, _nameService, $"Ocorreu um erro ao adicionar o post: {ex.Message}");
            return Task.FromResult(new Post());
        }
    }

    public Task<Post> UpdatePostAsync(int id, Post updatedPost)
    {
        try
        {
            var existingPost = _posts.FirstOrDefault(p => p.Id == id);

            if (existingPost == null) return Task.FromResult<Post?>(null)!;

            existingPost.Title = updatedPost.Title;
            existingPost.Body = updatedPost.Body;

            _logger.Log(LogLevel.Trace, 200, _nameService, $"Post {id} ({existingPost}) atualizado com sucesso!");
            return Task.FromResult(existingPost);
        }
        catch (Exception ex) 
        {
            _logger.Log(LogLevel.Debug, 5000, _nameService, $"Ocorreu um erro ao tentar atualizar o post {id}: {ex.Message}");
            return Task.FromResult(new Post());
        }
    }

    public Task<bool> DeletePostAsync(int id)
    {
        try
        {
            var post = _posts.FirstOrDefault(p => p.Id == id);

            if (post == null) return Task.FromResult(false);

            _posts.Remove(post);

            _logger.Log(LogLevel.Trace, 200, _nameService, "Post removido com sucesso!");
            return Task.FromResult(true);
        }
        catch (Exception ex)
        {
            _logger.Log(LogLevel.Debug, 5000, _nameService, $"Ocorreu algum erro ao tentar remover o post: {ex.Message}");
            return Task.FromResult(false);
        }
    }

    private async Task PopulateDatabase()
    {
        try
        {
            var httpClient = new HttpClient();
            var response = await httpClient.GetAsync("https://jsonplaceholder.typicode.com/posts");
            var content = await response.Content.ReadAsStringAsync();
            var retrievedPosts = JsonConvert.DeserializeObject<List<Post>>(content);
            _posts.AddRange(retrievedPosts!);

            _logger.Log(LogLevel.Trace, 200, _nameService, "Base populada com sucesso!");
        }
        catch (Exception ex)
        {
            _logger.Log(LogLevel.Debug, 5000,_nameService, $"Ocorreu algum erro ao tentar popular a base: {ex.Message}");
        }
    }
}