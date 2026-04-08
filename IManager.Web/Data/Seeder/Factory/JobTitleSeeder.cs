using AutoMapper;
using IManager.Web.Data.Seeder.Interfaces;
using IManager.Web.Data.Seeder.SeedDatas;
using IManager.Web.Domain.Entities.Companies;
using IManager.Web.Domain.Interfaces.Repositories;

namespace IManager.Web.Data.Seeder.Factory;

public sealed class JobTitleSeeder : IEntitySeeder<JobTitleSeedData>
{
    private readonly IRepository<JobTitle> _repo;
    private readonly IMapper _mapper;

    public JobTitleSeeder(IRepository<JobTitle> repo, IMapper mapper)
    {
        _repo = repo;
        _mapper = mapper;
    }

    public async Task SeedAsync(IEnumerable<JobTitleSeedData> data)
    {
        foreach (var item in data)
            await _repo.AddAsync(_mapper.Map<JobTitle>(item));
    }
}
