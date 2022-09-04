using AutoMapper;
using CourseProject.Dal;
using CourseProject.Domain.Abstractions;
using CourseProject.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace CourseProject.Service;

public class TopicService : ITopicService
{
    private readonly ApplicationContext _context;
    private readonly IMapper _mapper;

    public TopicService(ApplicationContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<IEnumerable<TopicModel>> GetTopics()
    {
        return _mapper.Map<IEnumerable<TopicModel>>(await _context.Topics.ToListAsync());
    }

    public async Task<TopicModel> GetTopicByCollection(int id)
    {
        return _mapper.Map<TopicModel>(await _context.Collections.Where(c=>c.Id==id).Select(c=>c.Topic).FirstOrDefaultAsync());
    }
}