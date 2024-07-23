using AutoMapper;
using Cinema.Core.Entites;
using Cinema.Data.Repositories.Interfaces;
using Cinema.Service.Dtos.LanguageDtos;
using Cinema.Service.Dtos;
using Cinema.Service.Exceptions;
using Cinema.Service.Interfaces;
using FluentValidation.Resources;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinema.Service.Implementations
{
    public class LanguageService:ILanguageService
    {
        private readonly ILanguageRepository _languageRepository;
        private readonly IWebHostEnvironment _environment;
        private readonly IMapper _mapper;

        public LanguageService(ILanguageRepository languageRepository, IWebHostEnvironment environment, IMapper mapper)
        {
            _languageRepository = languageRepository;
            _environment = environment;
            _mapper = mapper;
        }

        public int Create(AdminLanguageCreateDto createDto)
        {
            if (_languageRepository.Exists(x => x.Name.ToLower() == createDto.Name.ToLower()))
                throw new RestException(StatusCodes.Status400BadRequest, "Name", "Language by given Name already exists.");

            var language = _mapper.Map<Language>(createDto);

            if (createDto.FlagPhoto != null && createDto.FlagPhoto.Length > 0)
            {
                language.FlagPhoto = UploadImage(createDto.FlagPhoto);
            }

            _languageRepository.Add(language);
            _languageRepository.Save();

            return language.Id;
        }

        public void Edit(int id, AdminLanguageEditDto editDto)
        {
            var language = _languageRepository.Get(x => x.Id == id);

            if (language == null)
            {
                throw new RestException(StatusCodes.Status404NotFound, "Language not found");
            }

            if (editDto.FlagPhoto != null && editDto.FlagPhoto.Length > 0)
            {
                DeletePhotoFile(language.FlagPhoto);
                language.FlagPhoto = UploadImage(editDto.FlagPhoto);
            }

            _mapper.Map(editDto, language);
            _languageRepository.Save();
        }

        public void Delete(int id)
        {
            var language = _languageRepository.Get(x => x.Id == id);

            if (language == null)
            {
                throw new RestException(StatusCodes.Status404NotFound, "Language not found");
            }

            _languageRepository.Delete(language);
            _languageRepository.Save();
        }

        public AdminLanguageGetDto GetById(int id)
        {
            var language = _languageRepository.Get(x => x.Id == id);

            if (language == null)
            {
                throw new RestException(StatusCodes.Status404NotFound, "Language not found");
            }

            return _mapper.Map<AdminLanguageGetDto>(language);
        }

        public List<AdminLanguageGetDto> GetAll(string? search = null)
        {
            var languages = _languageRepository.GetAll(x => search == null || x.Name.Contains(search)).ToList();

            return _mapper.Map<List<AdminLanguageGetDto>>(languages);
        }

        public PaginatedList<AdminLanguageGetDto> GetAllByPage(string? search = null, int page = 1, int size = 10)
        {
            var query = _languageRepository.GetAll(x => search == null || x.Name.Contains(search));
            var paginated = PaginatedList<Language>.Create(query, page, size);

            return new PaginatedList<AdminLanguageGetDto>(_mapper.Map<List<AdminLanguageGetDto>>(paginated.Items), paginated.TotalPages, page, size);
        }

        private void DeletePhotoFile(string photo)
        {
            string filePath = Path.Combine(_environment.WebRootPath, "uploads\\languages", photo);

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }

        private string UploadImage(IFormFile imageFile)
        {
            if (imageFile == null || imageFile.Length == 0)
            {
                throw new RestException(StatusCodes.Status400BadRequest, "Image file is required");
            }

            string uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads", "flags");
            string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(imageFile.FileName);
            string filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                imageFile.CopyTo(fileStream);
            }

            return uniqueFileName;
        }
    }
}
