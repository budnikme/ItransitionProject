using System.Dynamic;
using System.Text;
using AutoMapper;
using CourseProject.Common.Interfaces;
using CourseProject.Dal;
using CourseProject.Dal.Entities;
using CourseProject.Domain;
using CourseProject.Domain.Abstractions;
using CourseProject.Domain.Models;
using CourseProject.Domain.Models.AddModels;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using ServiceStack.Text;

namespace CourseProject.Service;

public class CollectionService : ICollectionService
{
    private readonly ApplicationContext _context;
    private readonly IAzureStorageService _azureStorageService;
    private readonly IMapper _mapper;
    private readonly ICurrentUserService _currentUserService;

    public CollectionService(ApplicationContext context, IAzureStorageService azureStorageService, IMapper mapper,
        ICurrentUserService currentUserService)
    {
        _context = context;
        _azureStorageService = azureStorageService;
        _mapper = mapper;
        _currentUserService = currentUserService;
    }

    public async Task<byte[]> GenerateCsv(int collectionId)
    {
        var collectionItems = await _context.Items.Include(i=>i.Tags).Where(i => i.Collection.Id == collectionId).ToListAsync();
        var items = GenerateCsvObject(collectionItems);
        var csv = CsvSerializer.SerializeToCsv(items);
        return new UTF8Encoding().GetBytes(csv);
    }

    private dynamic GenerateCsvObject(List<Item> collectionItems)
    {
        var items = new dynamic[collectionItems.Count];
        for (var i = 0; i < collectionItems.Count; i++)
        {
            items[i] = new ExpandoObject();
            MapItemFields(items[i], collectionItems.ElementAt(i));
        }

        return items;
    }

    private void MapItemFields(dynamic item, Item itemEntity)
    {
        item.Name = itemEntity.Name;
        item.Tags = string.Join(", ", itemEntity.Tags.Select(t => new string(t.Name)).ToArray());
        item.Likes = itemEntity.UsersLikes.Count;
        item.CreatedDate = itemEntity.CreatedDate.ToString("dd.MM.yyyy HH:mm");
        item.ModifiedDate = itemEntity.ModifiedDate.ToString("dd.MM.yyyy HH:mm");
        MapItemCustomFields(item, itemEntity.CustomFields);
    }

    private void MapItemCustomFields(dynamic item, Dictionary<string, CustomField> fields)
    {
        foreach (var field in fields)
        {
            var value = field.Value.Value;
            if (field.Value.Type == "checkbox")
            {
                value = value == "on" ? "Yes" : "No";
            }

            AddExpandoProperty(item, field.Key, value);
        }
    }

    private void AddExpandoProperty(ExpandoObject expando, string propertyName, object propertyValue)
    {
        var expandoDict = expando as IDictionary<string, object>;
        if (expandoDict.ContainsKey(propertyName))
            expandoDict[propertyName] = propertyValue;
        else
            expandoDict.Add(propertyName, propertyValue);
    }

    public async Task<int> GetCollectionsCount()
    {
        return await _context.Collections.CountAsync();
    }

    public async Task<IEnumerable<CollectionModel>> GetCollections(int startRow = 0, int count = 9)
    {
        return _mapper.Map<IEnumerable<CollectionModel>>
        (await _context.Collections.OrderByDescending(c => c.CreatedDate).Include(c => c.Topic).Include(c => c.User)
            .Skip(startRow).Take(count)
            .ToListAsync()).ToList();
    }

    public async Task<FullCollectionModel> GetCollection(int collectionId)
    {
        return _mapper.Map<FullCollectionModel>(await _context.Collections.Include(c => c.Topic)
            .Include(c => c.User)
            .Where(c => c.Id == collectionId).FirstOrDefaultAsync()) ?? throw new KeyNotFoundException("Collection not found");
    }

    public async Task<int> AddCollection(AddCollectionModel collection)
    {
        var coll = await CreateCollection(collection);
        _context.Collections.Add(coll);
        await _context.SaveChangesAsync();
        return coll.Id;
    }

    private async Task<Collections> CreateCollection(AddCollectionModel collection)
    {
        var coll = _mapper.Map<Collections>(collection);
        coll.User = await _currentUserService.GetCurrentUserAsync();
        coll.ImageUrl = await _azureStorageService.UploadImageAsync(collection.Image!);
        await SetCollectionTopic(coll, collection.TopicId);
        return coll;
    }

    private async Task SetCollectionTopic(Collections coll, int topicId)
    {
        coll.Topic = await _context.Topics.FirstOrDefaultAsync(t => t.Id == topicId) ??
                     throw new KeyNotFoundException("Topic not found");
    }

    public async Task EditCollection(int collectionId, AddCollectionModel collection)
    {
        var coll = await _context.Collections.Include(c => c.Items).FirstOrDefaultAsync(c => c.Id == collectionId);
        CheckCollection(coll);
        ChangeCustomFields(collection, coll);
        await ChangeCollection(coll, collection);
        await ChangeCollectionImage(coll, collection.Image);
        await _context.SaveChangesAsync();
    }

    private void ChangeCustomFields(AddCollectionModel addCollection, Collections collection)
    {
        var oldFields = collection.CustomFieldsModel;
        var newFields = addCollection.CustomFieldsModel;
        var fieldsToDelete = GetFieldsToDelete(oldFields, newFields);
        var fieldsToAdd = GetFieldsToAdd(oldFields, newFields);
        ChangeEditedFields(fieldsToDelete, fieldsToAdd, collection.Id);
    }

    private void ChangeEditedFields(List<string> fieldsToDelete, Dictionary<string, CustomField> fieldsToAdd,
        int collectionId)
    {
        if (fieldsToAdd.Count > 0)
            AddFieldsToItems(fieldsToAdd, collectionId);
        if (fieldsToDelete.Count > 0)
            DeleteFieldsFromItems(fieldsToDelete, collectionId);
    }

    private Dictionary<string, CustomField> GetFieldsToAdd(Dictionary<string, string>? oldFields,
        Dictionary<string, string> newFields)
    {
        return newFields.Where(f => !oldFields.ContainsKey(f.Key))
            .ToDictionary(f => f.Key, f => new CustomField {Type = f.Value, Value = ""});
    }

    private List<string> GetFieldsToDelete(Dictionary<string, string>? oldFields, Dictionary<string, string> newFields)
    {
        return oldFields
            .Where(f => !newFields.ContainsKey(f.Key) || (newFields.ContainsKey(f.Key) && f.Value != newFields[f.Key]))
            .Select(f => f.Key).ToList();
    }

    private List<Item> GetItemsByCollectionId(int collectionId)
    {
        return _context.Items.Where(i => i.Collection.Id == collectionId).ToList();
    }

    private void DeleteFieldsFromItems(List<string> fieldsToDelete, int collectionId)
    {
        GetItemsByCollectionId(collectionId).ForEach(i =>
        {
            fieldsToDelete.ForEach(f =>
            {
                i.CustomFields = i.CustomFields.Where(cf => cf.Key != f)
                    .ToDictionary(k => k.Key, v => v.Value);
            });
        });
    }

    private void AddFieldsToItems(Dictionary<string, CustomField> fieldsToAdd, int collectionId)
    {
        GetItemsByCollectionId(collectionId).ForEach(i =>
        {
            i.CustomFields = i.CustomFields.Union(fieldsToAdd)
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        });
    }

    private void CheckCollection(Collections coll)
    {
        if (coll == null) throw new KeyNotFoundException("Collection not found");
        if (!_currentUserService.IsCurrentUser(coll.User.Id) && !_currentUserService.IsAdmin())
            throw new UnauthorizedAccessException("You are not the owner of this collection");
    }

    private async Task ChangeCollection(Collections coll, AddCollectionModel collection)
    {
        coll.Name = collection.Name;
        coll.Description = collection.Description;
        await SetCollectionTopic(coll, collection.TopicId);
        coll.CustomFieldsModel = collection.CustomFieldsModel;
        coll.ModifiedDate = DateTime.Now;
    }

    private async Task ChangeCollectionImage(Collections coll, IFormFile? image)
    {
        if (image != null)
        {
            coll.ImageUrl = await _azureStorageService.UploadImageAsync(image);
        }
    }

    public async Task DeleteCollection(int collectionId)
    {
        var coll = await _context.Collections.FirstOrDefaultAsync(c => c.Id == collectionId);
        CheckCollection(coll);
        _context.Collections.Remove(coll);
        await _context.SaveChangesAsync();
        await _azureStorageService.DeleteImageAsync(coll.ImageUrl);
    }

    public async Task<Dictionary<string, string>?> GetCustomFieldsModel(int collectionId)
    {
        return await _context.Collections.Where(c => c.Id == collectionId).Select(c => c.CustomFieldsModel)
            .FirstOrDefaultAsync();
    }
}