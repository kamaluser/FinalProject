using AutoMapper;
using Cinema.Core.Entites;
using Cinema.Data.Repositories.Interfaces;
using Cinema.Service.Dtos;
using Cinema.Service.Dtos.SliderDtos;
using Cinema.Service.Exceptions;
using Cinema.Service.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinema.Service.Implementations
{
    public class SliderService : ISliderService
    {
        private readonly ISliderRepository _repository;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _environment;

        public SliderService(ISliderRepository repository, IMapper mapper, IWebHostEnvironment environment)
        {
            _repository = repository;
            _mapper = mapper;
            _environment = environment;
        }

        public int Create(AdminSliderCreateDto createDto)
        {
            var slider = _mapper.Map<Slider>(createDto);

            if (createDto.Image != null && createDto.Image.Length > 0)
            {
                slider.Image = UploadImage(createDto.Image);
            }

            _repository.Add(slider);
            _repository.Save();

            return slider.Id;
        }

        public void Edit(int id, AdminSliderEditDto editDto)
        {
            var slider = _repository.Get(x => x.Id == id && !x.IsDeleted);

            if (slider == null)
            {
                throw new RestException(StatusCodes.Status404NotFound, "Slider not found");
            }

            if (editDto.Image != null && editDto.Image.Length > 0)
            {
                var allowedContentTypes = new[] { "image/jpeg", "image/png" };
                if (!allowedContentTypes.Contains(editDto.Image.ContentType))
                {
                    throw new RestException(StatusCodes.Status400BadRequest, "Invalid photo format. Only .jpg, .jpeg and .png are allowed.");
                }
            }

            if (editDto.Order != null && editDto.Order > 0)
            {
                slider.Order = editDto.Order;
            }
            if (editDto.Image != null && editDto.Image.Length > 0)
            {
                DeletePhotoFile(slider.Image);
                slider.Image = UploadImage(editDto.Image);
            }
            slider.ModifiedAt = DateTime.Now;


            _repository.Save();
        }

        public void Delete(int id)
        {
            var slider = _repository.Get(x => x.Id == id);
            if (slider == null)
            {
                throw new RestException(StatusCodes.Status404NotFound, "Slider not found");
            }

            if (slider.IsDeleted)
            {
                throw new RestException(StatusCodes.Status400BadRequest, "Slider already deleted");
            }


            slider.ModifiedAt = DateTime.Now;
            slider.IsDeleted = true;
            //_repository.Delete(slider);
            _repository.Save();
        }

        public PaginatedList<AdminSliderGetDto> GetAllByPage(int page = 1, int size = 10)
        {
            var query = _repository.GetAll(x => !x.IsDeleted);
            var paginated = PaginatedList<Slider>.Create(query, page, size);
            if (page > paginated.TotalPages)
            {
                page = paginated.TotalPages;
                paginated = PaginatedList<Slider>.Create(query, page, size);
            }
            return new PaginatedList<AdminSliderGetDto>(_mapper.Map<List<AdminSliderGetDto>>(paginated.Items), paginated.TotalPages, page, size);
        }

        public AdminSliderGetDto GetById(int id)
        {
            var slider = _repository.Get(x => x.Id == id && !x.IsDeleted);

            if (slider == null)
            {
                throw new RestException(StatusCodes.Status404NotFound, "Slider not found");
            }

            return _mapper.Map<AdminSliderGetDto>(slider);
        }

        private void DeletePhotoFile(string photo)
        {
            string filePath = Path.Combine(_environment.WebRootPath, "uploads\\sliders", photo);

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

            string uploadsFolder = Path.Combine(_environment.ContentRootPath, "wwwroot", "uploads", "sliders");
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
