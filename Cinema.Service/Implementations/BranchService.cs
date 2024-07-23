using AutoMapper;
using Cinema.Core.Entites;
using Cinema.Data.Repositories.Interfaces;
using Cinema.Service.Dtos;
using Cinema.Service.Dtos.BranchDtos;
using Cinema.Service.Exceptions;
using Cinema.Service.Interfaces;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinema.Service.Implementations
{
    public class BranchService:IBranchService
    {
        private readonly IBranchRepository _repository;
        private readonly IMapper _mapper;
        public BranchService(IBranchRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public int Create(AdminBranchCreateDto dto)
        {
            if (_repository.Exists(x => x.Name.ToLower() == dto.Name.ToLower()))
                throw new RestException(StatusCodes.Status400BadRequest, "Name", "Branch already exists.");

            Branch branch = _mapper.Map<Branch>(dto);

            _repository.Add(branch);
            _repository.Save();

            return branch.Id;
        }

        public List<AdminBranchGetDto> GetAll(string? search = null)
        {
            var branches = _repository.GetAll(x => !x.IsDeleted && (search == null || x.Name.Contains(search))).ToList();
            return _mapper.Map<List<AdminBranchGetDto>>(branches);
        }

        public PaginatedList<AdminBranchGetDto> GetAllByPage(string? search = null, int page = 1, int size = 10)
        {
            if (page < 1) page = 1;
            if (size < 1) size = 10;

            var query = _repository.GetAll(x => !x.IsDeleted && (search == null || x.Name.Contains(search)), "Halls");
            var paginated = PaginatedList<Branch>.Create(query, page, size);

            if (page > paginated.TotalPages)
            {
                page = paginated.TotalPages;
                paginated = PaginatedList<Branch>.Create(query, page, size);
            }

            return new PaginatedList<AdminBranchGetDto>(_mapper.Map<List<AdminBranchGetDto>>(paginated.Items), paginated.TotalPages, page, size);
        }

        public AdminBranchGetDto GetById(int id)
        {
            Branch entity = _repository.Get(x => x.Id == id && !x.IsDeleted);

            if (entity == null) throw new RestException(StatusCodes.Status404NotFound, "Branch not found");

            return _mapper.Map<AdminBranchGetDto>(entity);
        }

        public void Delete(int id)
        {
            Branch entity = _repository.Get(x => x.Id == id);

            if (entity == null) throw new RestException(StatusCodes.Status404NotFound, "Branch not found");

            //_repository.Delete(entity);
            entity.IsDeleted = true;
            entity.ModifiedAt = DateTime.Now;
            _repository.Save();
        }

        public void Edit(int id, AdminBranchEditDto updateDto)
        {
            Branch entity = _repository.Get(x => x.Id == id && !x.IsDeleted);

            if (entity == null)
                throw new RestException(StatusCodes.Status404NotFound, "Branch not found");


            if (entity.Name != updateDto.Name && _repository.Exists(x => x.Name == updateDto.Name))
                throw new RestException(StatusCodes.Status400BadRequest, "Name", "Branch by this name already exists.");

            entity.Name = updateDto.Name;
            entity.Address = updateDto.Address;
            entity.ModifiedAt = DateTime.Now;
            _repository.Save();
        }
    }
}
