﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinema.Service.Dtos.OrderDtos
{
    public class OrderStatisticsDto
    {
        public int TotalOrders { get; set; }
        public List<OrderStatsByDateDto> OrdersByDate { get; set; }
    }
}
