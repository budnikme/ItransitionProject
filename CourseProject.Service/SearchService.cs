using Azure.Search.Documents;
using Azure.Search.Documents.Models;
using CourseProject.Domain.Abstractions;
using CourseProject.Domain.Models;
using CourseProject.Domain.Models.AddModels;

namespace CourseProject.Service;

public class SearchService : ISearchService
{
    private readonly IndexDocumentsOptions _options = new() {ThrowOnAnyError = true};
    private readonly SearchClient _searchClient;

    public SearchService(SearchClient searchClient)
    {
        _searchClient = searchClient;
    }

    public async Task<List<int>> SearchAsync(string searchText)
    {
        SearchResults<SearchDocument> response = await _searchClient.SearchAsync<SearchDocument>(searchText);
        return response.GetResults().Select(result => result.Document).Select(doc => int.Parse((string) doc["id"]))
            .ToList();
    }

    public async Task AddToIndexAsync(ItemSearchModel item)
    {
        var batch = IndexDocumentsBatch.Create(IndexDocumentsAction.MergeOrUpload(item));
        await _searchClient.IndexDocumentsAsync(batch, _options);
    }

    public async Task DeleteFromIndexAsync(int itemId)
    {
        var batch = IndexDocumentsBatch.Create(
            IndexDocumentsAction.Delete(new ItemSearchModel {Id = itemId.ToString()}));
        await _searchClient.IndexDocumentsAsync(batch, _options);
    }

    public async Task AddCommentToIndex(AddCommentModel comment)
    {
        var item = (await _searchClient.GetDocumentAsync<ItemSearchModel>(comment.ItemId.ToString())).Value;
        item.Id = comment.ItemId.ToString();
        item.CommentTexts.Add(comment.Text);
        await AddToIndexAsync(item);
    }
}