using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using DistributedPizza.Core.Data.Entities;
using DistributedPizza.Core.Data.Models;

namespace DistributedPizza.Core
{
    public static class Objects
    {
    }

    public class MapperConfigurationHelper
    {
        public static void Build(IMapperConfigurationExpression config)
        {
            config.CreateMap<Order, OrderDTO>().ReverseMap();
            //config.CreateMap<OrderDTO, Order>();
        }
    }
}
