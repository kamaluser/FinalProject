using AutoMapper;
using Cinema.Core.Entites;
using Cinema.Data.Repositories.Implementations;
using Cinema.Data.Repositories.Interfaces;
using Cinema.Service.Dtos;
using Cinema.Service.Dtos.NewsDtos;
using Cinema.Service.Dtos.SliderDtos;
using Cinema.Service.Exceptions;
using Cinema.Service.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Cinema.Service.Implementations
{
    public class NewsService : INewsService
    {
        private readonly INewsRepository _newsRepository;
        private readonly IWebHostEnvironment _environment;
        private readonly IMapper _mapper;

        public NewsService(INewsRepository newsRepository, IWebHostEnvironment environment, IMapper mapper)
        {
            _newsRepository = newsRepository;
            _environment = environment;
            _mapper = mapper;
        }

        public int Create(AdminNewsCreateDto createDto)
        {

            if (_newsRepository.Exists(x => x.Title.ToLower() == createDto.Title.ToLower()))
                throw new RestException(StatusCodes.Status400BadRequest, "Name", "Branch already exists.");

            var news = _mapper.Map<News>(createDto);

            if (createDto.Image != null)
            {
                string imagePath = SaveImage(createDto.Image);
                news.Image = imagePath;
            }

            _newsRepository.Add(news);
            _newsRepository.Save();

            return news.Id;
        }

        public PaginatedList<AdminNewsGetDto> GetAllByPage(string? search = null, int page = 1, int size = 10)
        {
            var query = _newsRepository.GetAll(x => (search == null || x.Title.Contains(search)) && !x.IsDeleted);
            var paginated = PaginatedList<News>.Create(query, page, size);
            return new PaginatedList<AdminNewsGetDto>(_mapper.Map<List<AdminNewsGetDto>>(paginated.Items), paginated.TotalPages, page, size);
        }

        public void Edit(int id, AdminNewsEditDto editDto)
        {
            var news = _newsRepository.Get(x => x.Id == id && !x.IsDeleted);
            if (news == null)
                throw new RestException(StatusCodes.Status404NotFound, "News", "News not found.");

            _mapper.Map(editDto, news);

            if (editDto.Image != null)
            {
                string imagePath = SaveImage(editDto.Image);
                news.Image = imagePath;
            }
            news.ModifiedAt = DateTime.Now;

            _newsRepository.Save();
        }

        public void Delete(int id)
        {
            var news = _newsRepository.Get(x => x.Id == id && !x.IsDeleted);
            if (news == null)
                throw new RestException(StatusCodes.Status404NotFound, "News", "News not found.");

            news.IsDeleted = true;
            news.ModifiedAt = DateTime.Now;
            _newsRepository.Save();
        }

        public AdminNewsGetDto GetById(int id)
        {
            var news = _newsRepository.Get(x => x.Id == id && !x.IsDeleted);
            if (news == null)
                throw new RestException(StatusCodes.Status404NotFound, "News", "News not found.");

            return _mapper.Map<AdminNewsGetDto>(news);
        }

        public PaginatedList<UserNewsGetDto> GetAllByPageUser(int page = 1, int size = 10)
        {
            var query = _newsRepository.GetAll(x => !x.IsDeleted && x.CreatedAt>DateTime.Now.AddDays(-7)).OrderByDescending(x => x.CreatedAt);
            var paginated = PaginatedList<News>.Create(query, page, size);
            return new PaginatedList<UserNewsGetDto>(_mapper.Map<List<UserNewsGetDto>>(paginated.Items), paginated.TotalPages, page, size);
        }

        private string SaveImage(IFormFile image)
        {
            string uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads/news");
            Directory.CreateDirectory(uploadsFolder);

            string uniqueFileName = Guid.NewGuid().ToString() + "_" + image.FileName;
            string filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                image.CopyTo(fileStream);
            }

            return uniqueFileName;
        }
    }
}