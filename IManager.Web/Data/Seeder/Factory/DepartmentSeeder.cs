using AutoMapper;
using IManager.Web.Data.Seeder.Interfaces;
using IManager.Web.Data.Seeder.SeedDatas;
using IManager.Web.Domain.Entities.Companies;
using IManager.Web.Domain.Interfaces.Repositories;

namespace IManager.Web.Data.Seeder.Factory;

public sealed class DepartmentSeeder : IEntitySeeder<DepartmentSeedData>
{
    private readonly IRepository<Department> _repo;
    private readonly IMapper _mapper;

    public DepartmentSeeder(IRepository<Department> repo, IMapper mapper)
    {
        _repo = repo;
        _mapper = mapper;
    }

    public async Task SeedAsync(IEnumerable<DepartmentSeedData> data)
    {
        foreach (var item in data)
            await _repo.AddAsync(_mapper.Map<Department>(item));
    }
}
