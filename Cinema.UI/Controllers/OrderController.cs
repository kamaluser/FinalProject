﻿using Cinema.UI.Models.OrderModels;
using Cinema.UI.Exceptions;
using Cinema.UI.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Cinema.UI.Models;
using Cinema.UI.Models.BranchModels;

namespace Cinema.UI.Controllers
{
    public class OrderController : Controller
    {
        private readonly ICrudService _crudService;

        public OrderController(ICrudService crudService)
        {
            _crudService = crudService;
        }

        public async Task<IActionResult> Index(int page = 1)
        {
            try
            {
                var orders = await _crudService.GetAllPaginated<OrderListItemGetResponse>("orders/getallbypagination", page);

                return View(orders);
            }
            catch (HttpException e)
            {
                Console.WriteLine($"HttpException: {e.Message}");

                if (e.Status == System.Net.HttpStatusCode.Unauthorized)
                {
                    return RedirectToAction("Login", "Account");
                }

                return RedirectToAction("Error", "Home");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Exception: {e.Message}");
                return RedirectToAction("Error", "Home");
            }
        }

    }
}