using AutoMapper;
using IManager.Web.Data.Seeder.SeedDatas;
using IManager.Web.Domain.Entities.Companies;
using IManager.Web.Domain.Interfaces.Repositories;

namespace IManager.Web.Data.Seeder.Factory;

public sealed class CompanySeeder
{
    private readonly IRepository<Company> _repo;
    private readonly IMapper _mapper;

    public CompanySeeder(IRepository<Company> repo, IMapper mapper)
    {
        _repo = repo;
        _mapper = mapper;
    }

    public Task SeedAsync(CompanySeedData data)
        => _repo.AddAsync(_mapper.Map<Company>(data));
}
