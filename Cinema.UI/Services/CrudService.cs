using Cinema.UI.Exceptions;
using Cinema.UI.Models;
using Microsoft.Net.Http.Headers;
using System.Text.Json;
using System.Text;
using Cinema.UI.Models.UserModels;
using System.Net.Http;
using Cinema.Service.Dtos.OrderDtos;
using Cinema.UI.Models.SessionModels;
using Cinema.UI.Models.OrderModels;

namespace Cinema.UI.Services
{
    public class CrudService : ICrudService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly HttpClient _client;
        private const string baseUrl = "https://localhost:44324/api/admin/";

        public CrudService(IHttpContextAccessor httpContextAccessor)
        {
            _client = new HttpClient();
            _httpContextAccessor = httpContextAccessor;
        }

        private void AddAuthorizationHeader()
        {
            var token = _httpContextAccessor.HttpContext.Request.Cookies["token"];
            if (token != null)
            {
                if (_client.DefaultRequestHeaders.Contains(HeaderNames.Authorization))
                {
                    _client.DefaultRequestHeaders.Remove(HeaderNames.Authorization);
                }
                _client.DefaultRequestHeaders.Add(HeaderNames.Authorization, token);
            }
        }

        public async Task<List<SessionLanguageResponse>> GetSessionLanguagesAsync()
        {
            AddAuthorizationHeader();
            var response = await _client.GetAsync(baseUrl + "sessions/count/languages/monthly");
            var content = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                return JsonSerializer.Deserialize<List<SessionLanguageResponse>>(content, options);
            }
            else
            {
                Console.WriteLine($"Error: {content}");
                throw new HttpException(response.StatusCode);
            }
        }

        public async Task<List<TResponse>> GetAll<TResponse>(string path)
        {
            AddAuthorizationHeader();
            var response = await _client.GetAsync(baseUrl + path);
            var content = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                return JsonSerializer.Deserialize<List<TResponse>>(content, options);
            }
            else
            {
                Console.WriteLine($"Error: {content}");
                throw new HttpException(response.StatusCode);
            }
        }

      
        public async Task<int> GetTotalOrderedSeatsCountAsync()
        {
            var response = await _client.GetAsync(baseUrl+"orders/total-ordered-seats-count");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<int>();
        }

        public async Task<int> GetOrderCountLastMonthAsync()
        {
            AddAuthorizationHeader();
            var response = await _client.GetAsync(baseUrl + "orders/order-count-last-month");
            var content = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var countDto = JsonSerializer.Deserialize<OrderCountResponse>(content, options);
                return countDto.Count;
            }
            else
            {
                Console.WriteLine($"Error: {content}");
                throw new HttpException(response.StatusCode);
            }
        }


        public async Task<int> GetSessionCountLastMonthAsync()
        {
            AddAuthorizationHeader();
            var response = await _client.GetAsync(baseUrl + "sessions/count/monthly");
            var content = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var countDto = JsonSerializer.Deserialize<SessionCountResponse>(content, options);
                return countDto.Count;
            }
            else
            {
                Console.WriteLine($"Error: {content}");
                throw new HttpException(response.StatusCode);
            }
        }

        public async Task<int> GetMembersCountAsync()
        {
            AddAuthorizationHeader();
            var response = await _client.GetAsync(baseUrl + "members/count");
            var content = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var countDto = JsonSerializer.Deserialize<SessionCountResponse>(content, options);
                return countDto.Count;
            }
            else
            {
                Console.WriteLine($"Error: {content}");
                throw new HttpException(response.StatusCode);
            }
        }

        public async Task<int> GetOrderCountLastYearAsync()
        {
            AddAuthorizationHeader();
            var response = await _client.GetAsync(baseUrl + "orders/order-count-last-year");
            var content = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var countDto = JsonSerializer.Deserialize<OrderCountResponse>(content, options);
                return countDto.Count;
            }
            else
            {
                Console.WriteLine($"Error: {content}");
                throw new HttpException(response.StatusCode);
            }
        }

        public async Task<CreateResponse> Create<TRequest>(TRequest request, string path)
        {
            AddAuthorizationHeader();
            var content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");
            var response = await _client.PostAsync(baseUrl + path, content);
            var responseContent = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

            if (response.IsSuccessStatusCode)
            {
                return JsonSerializer.Deserialize<CreateResponse>(responseContent, options);
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                var errorResponse = JsonSerializer.Deserialize<ErrorResponse>(responseContent, options);
                throw new ModelException(System.Net.HttpStatusCode.BadRequest, errorResponse);
            }
            else
            {
                var errorResponse = JsonSerializer.Deserialize<ErrorResponse>(responseContent, options);
                throw new HttpException(response.StatusCode, errorResponse.Message);
            }
        }

        public async Task<TResponse> Get<TResponse>(string path)
        {
            AddAuthorizationHeader();
            var response = await _client.GetAsync(baseUrl + path);
            var content = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                return JsonSerializer.Deserialize<TResponse>(content, options);
            }
            else
            {
                Console.WriteLine($"Error: {content}");
                throw new HttpException(response.StatusCode);
            }
        }

        public async Task<PaginatedResponse<TResponse>> GetAllPaginated<TResponse>(string path, int page)
        {
            AddAuthorizationHeader();
            var response = await _client.GetAsync(baseUrl + path + "?page=" + page);
            var content = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                return JsonSerializer.Deserialize<PaginatedResponse<TResponse>>(content, options);
            }
            else
            {
                Console.WriteLine($"Error: {content}");
                throw new HttpException(response.StatusCode);
            }
        }

        public async Task Update<TRequest>(TRequest request, string path)
        {
            AddAuthorizationHeader();
            var content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");
            var response = await _client.PutAsync(baseUrl + path, content);
            var responseContent = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

            if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                var errorResponse = JsonSerializer.Deserialize<ErrorResponse>(responseContent, options);
                throw new ModelException(System.Net.HttpStatusCode.BadRequest, errorResponse);
            }
            else if (!response.IsSuccessStatusCode)
            {
                var errorResponse = JsonSerializer.Deserialize<ErrorResponse>(responseContent, options);
                throw new HttpException(response.StatusCode, errorResponse.Message);
            }
        }

        public async Task Delete(string path)
        {
            AddAuthorizationHeader();
            var response = await _client.DeleteAsync(baseUrl + path);

            if (!response.IsSuccessStatusCode)
            {
                throw new HttpException(response.StatusCode);
            }
        }

        public async Task<CreateResponse> CreateFromForm<TRequest>(TRequest request, string path)
        {
            AddAuthorizationHeader();
            var multipartContent = CreateMultipartFormDataContent(request);
            var response = await _client.PostAsync(baseUrl + path, multipartContent);
            var responseContent = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

            if (response.IsSuccessStatusCode)
            {
                return JsonSerializer.Deserialize<CreateResponse>(responseContent, options);
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                Console.WriteLine($"Bad Request Response: {responseContent}");
                var errorResponse = JsonSerializer.Deserialize<ErrorResponse>(responseContent, options);
                throw new ModelException(System.Net.HttpStatusCode.BadRequest, errorResponse);
            }
            else
            {
                var errorResponse = JsonSerializer.Deserialize<ErrorResponse>(responseContent, options);
                throw new HttpException(response.StatusCode, errorResponse.Message);
            }
        }

        public async Task UpdateFromForm<TRequest>(TRequest request, string path)
        {
            AddAuthorizationHeader();
            var multipartContent = CreateMultipartFormDataContent(request);
            var response = await _client.PutAsync(baseUrl + path, multipartContent);
            var responseContent = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

            if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                var errorResponse = JsonSerializer.Deserialize<ErrorResponse>(responseContent, options);
                throw new ModelException(System.Net.HttpStatusCode.BadRequest, errorResponse);
            }
            else if (!response.IsSuccessStatusCode)
            {
                var errorResponse = JsonSerializer.Deserialize<ErrorResponse>(responseContent, options);
                throw new HttpException(response.StatusCode, errorResponse.Message);
            }
        }

        async Task<AdminCreateResponse> ICrudService.CreateAdmin<TRequest>(TRequest request, string path)
        {
            _client.DefaultRequestHeaders.Remove(HeaderNames.Authorization);
            _client.DefaultRequestHeaders.Add(HeaderNames.Authorization, _httpContextAccessor.HttpContext.Request.Cookies["token"]);

            var content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");
            using (HttpResponseMessage response = await _client.PostAsync(baseUrl + "Auth/" + path, content))
            {
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

                if (response.IsSuccessStatusCode)
                {
                    return JsonSerializer.Deserialize<AdminCreateResponse>(await response.Content.ReadAsStringAsync(), options);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                {
                    ErrorResponse errorResponse = JsonSerializer.Deserialize<ErrorResponse>(await response.Content.ReadAsStringAsync(), options);
                    throw new ModelException(System.Net.HttpStatusCode.BadRequest, errorResponse);
                }
                else
                {
                    ErrorResponse errorResponse = JsonSerializer.Deserialize<ErrorResponse>(await response.Content.ReadAsStringAsync(), options);
                    throw new HttpException(response.StatusCode, errorResponse.Message);
                }
            }
        }

       

        public async Task<YearlyOrderResponse> GetMonthlyOrderCountForCurrentYearAsync()
        {
            AddAuthorizationHeader();
            var response = await _client.GetAsync(baseUrl + "orders/monthly-count-current-year");
            var content = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                return JsonSerializer.Deserialize<YearlyOrderResponse>(content, options);
            }
            else
            {
                Console.WriteLine($"Error: {content}");
                throw new HttpException(response.StatusCode, content);
            }
        }

        public async Task<YearlyRevenueResponse> GetMonthlyRevenueForCurrentYearAsync()
        {
            AddAuthorizationHeader();

            var response = await _client.GetAsync(baseUrl + "orders/monthly-revenue-current-year");
            var content = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var monthlyRevenue = JsonSerializer.Deserialize<YearlyRevenueResponse>(content, options);

                if (monthlyRevenue?.Months == null || monthlyRevenue.Revenue == null)
                {
                    return null;
                }

                return monthlyRevenue;
            }
            else
            {
                Console.WriteLine($"Error: {content}");
                throw new HttpException(response.StatusCode, content);
            }
        }

        public async Task<byte[]> SessionExcelExportAsync()
        {
            AddAuthorizationHeader();

            using (HttpResponseMessage response = await _client.GetAsync(baseUrl + "excel/SessionDownloadExcel"))
            {
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsByteArrayAsync();
                }
                else
                {
                    var errorMessage = await response.Content.ReadAsStringAsync();
                    await Console.Out.WriteLineAsync("ExcelExport Error: "+errorMessage);
                    throw new HttpException(response.StatusCode, errorMessage);
                }
            }
        }


        public async Task<byte[]> OrderExcelExportAsync()
        {
            AddAuthorizationHeader();

            using (HttpResponseMessage response = await _client.GetAsync(baseUrl + "excel/OrderDownloadExcel"))
            {
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsByteArrayAsync();
                }
                else
                {
                    var errorMessage = await response.Content.ReadAsStringAsync();
                    await Console.Out.WriteLineAsync("ExcelExport Error: " + errorMessage);
                    throw new HttpException(response.StatusCode, errorMessage);
                }
            }
        }

        private MultipartFormDataContent CreateMultipartFormDataContent<TRequest>(TRequest request)
        {
            var content = new MultipartFormDataContent();

            foreach (var prop in request.GetType().GetProperties())
            {
                var value = prop.GetValue(request);

                if (value is IFormFile file)
                {
                    var fileContent = new StreamContent(file.OpenReadStream());
                    fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(file.ContentType);
                    content.Add(fileContent, prop.Name, file.FileName);
                }
                else if (value is List<IFormFile> files)
                {
                    foreach (var photo in files)
                    {
                        var fileContent = new StreamContent(photo.OpenReadStream());
                        fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(photo.ContentType);
                        content.Add(fileContent, prop.Name, photo.FileName);
                    }
                }
                else if (value is List<int> numbers)
                {
                    foreach (var number in numbers)
                    {
                        content.Add(new StringContent(number.ToString()), prop.Name);
                    }
                }
                else if (value is DateTime dateTime)
                {
                    content.Add(new StringContent(dateTime.ToLongDateString()), prop.Name);
                }
                else if (value != null)
                {
                    content.Add(new StringContent(value.ToString()), prop.Name);
                }
            }

            return content;
        }

        public async Task<decimal> GetDailyTotalPriceAsync()
        {
            AddAuthorizationHeader();
            var response = await _client.GetAsync(baseUrl + "orders/price/daily");
            var content = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var dto = JsonSerializer.Deserialize<OrderRevenueResponse>(content, options);

                if (dto != null)
                {
                    return dto.TotalPrice;
                }
                else
                {
                    throw new HttpException(response.StatusCode);
                }
            }
            else
            {
                throw new HttpException(response.StatusCode);
            }
        }

    }
}