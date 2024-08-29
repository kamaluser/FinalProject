using Cinema.Core.Entites;
using Cinema.Service.Dtos;
using Cinema.Service.Dtos.LanguageDtos;
using Cinema.Service.Dtos.SeatDtos;
using Cinema.Service.Dtos.SessionDtos;
using System;
using System.Collections.Generic;

namespace Cinema.Service.Interfaces
{
    public interface ISessionService
    {
        int Create(AdminSessionCreateDto dto);
        void Edit(int id, AdminSessionEditDto dto);
        void Delete(int id);
        AdminSessionGetDto GetById(int id);
        PaginatedList<AdminSessionGetDto> GetAllByPage(string? search = null, int page = 1, int size = 10);
        List<AdminSessionGetDto> GetAll();
        Task<List<AdminSessionGetDto>> GetSessionsByHall(int hallId);
        Task<int> GetSessionCountLastMonthAsync();
        List<UserSessionDetailsDto> GetSessionsByMovieAndDateAsync(int movieId, DateTime date, int? branchId = null, int? languageId = null);
        List<UserSeatGetDto> GetSeatsForSession(int sessionId);
        List<Session> GetSessionsForReminder(DateTime currentDateTime, TimeSpan reminderWindow);
        Task<List<AdminSessionLanguageDto>> GetSessionCountByLanguageThisMonthAsync();
        List<UserSessionDetailsDto> GetSessionsByFiltersAsync(DateTime? date = null, int? branchId = null, int? languageId = null);
    }
}