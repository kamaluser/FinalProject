using AutoMapper;
using Cinema.Core.Entites;
using Cinema.Data.Repositories.Implementations;
using Cinema.Data.Repositories.Interfaces;
using Cinema.Service.Dtos;
using Cinema.Service.Dtos.HallDtos;
using Cinema.Service.Exceptions;
using Cinema.Service.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Cinema.Service.Implementations
{
    public class HallService:IHallService
    {
        private readonly IHallRepository _hallRepository;
        private readonly IBranchRepository _branchRepository;
        private readonly IWebHostEnvironment _environment;
        private readonly IMapper _mapper;

        public HallService(IHallRepository hallRepository, IBranchRepository branchRepository, IWebHostEnvironment environment, IMapper mapper)
        {
            _hallRepository = hallRepository;
            _branchRepository = branchRepository;
            _environment = environment;
            _mapper = mapper;
        }
        public int Create(AdminHallCreateDto dto)
        {

            if (_hallRepository.Exists(x => x.Name.ToLower() == dto.Name.ToLower() && x.BranchId == dto.BranchId && !x.IsDeleted))
            {
                throw new RestException(StatusCodes.Status400BadRequest, "Name", "A hall with the given name already exists in this branch.");
            }


            Branch branch = _branchRepository.Get(x => x.Id == dto.BranchId && !x.IsDeleted, "Halls");

            if (branch == null)
            {
                throw new RestException(StatusCodes.Status404NotFound, "BranchId", "Branch not found by given BranchId");
            }

            var hall = _mapper.Map<Hall>(dto);

            _hallRepository.Add(hall);
            _hallRepository.Save();

            return hall.Id;
        }

        public List<AdminHallGetDto> GetAll(string? search = null)
        {

            var students = _hallRepository.GetAll(x => (search == null || x.Name.Contains(search)) && !x.IsDeleted, "Branch").ToList();

            var studentDtos = students.Select(student =>
            {
                var dto = _mapper.Map<AdminHallGetDto>(student);
                return dto;
            }).ToList();


            return studentDtos;
        }

        public AdminHallGetDto GetById(int id)
        {
            var hall = _hallRepository.Get(s => s.Id == id && !s.IsDeleted, "Branch");
            if (hall == null)
                throw new RestException(StatusCodes.Status404NotFound, $"Hall with {id} ID not found.");


            AdminHallGetDto dto = _mapper.Map<AdminHallGetDto>(hall);
            return dto;
        }
        public void Edit(int id, AdminHallEditDto dto)
        {
            var hall = _hallRepository.Get(s => s.Id == id && !s.IsDeleted);
            if (hall == null)
            {
                throw new RestException(StatusCodes.Status404NotFound, $"Hall with {id} ID not found.");
            }

            if (_hallRepository.Exists(x => x.Name.ToLower() == dto.Name.ToLower() && x.BranchId == dto.BranchId && x.Id != id && !x.IsDeleted))
            {
                throw new RestException(StatusCodes.Status400BadRequest, "Name", "A hall with the given name already exists in this branch.");
            }

            Branch branch = _branchRepository.Get(g => g.Id == dto.BranchId && !g.IsDeleted, "Halls");

            if (branch == null)
            {
                throw new RestException(StatusCodes.Status404NotFound, $"Branch with ID {dto.BranchId} not found.");
            }

            _mapper.Map(dto, hall);
            _hallRepository.Save();
        }


        public void Delete(int id)
        {

            var student = _hallRepository.Get(s => s.Id == id && !s.IsDeleted);
            if (student == null)
                throw new RestException(StatusCodes.Status404NotFound, $"Hall with {id} ID not found.");

            student.IsDeleted = true;
            _hallRepository.Save();

        }

        public PaginatedList<AdminHallGetDto> GetAllByPage(string? search = null, int page = 1, int size = 10)
        {
            var query = _hallRepository.GetAll(x => (search == null || x.Name.Contains(search)) && !x.IsDeleted, "Branch");
            var paginated = PaginatedList<Hall>.Create(query, page, size);
            return new PaginatedList<AdminHallGetDto>(_mapper.Map<List<AdminHallGetDto>>(paginated.Items), paginated.TotalPages, page, size);
        }
    }
}
