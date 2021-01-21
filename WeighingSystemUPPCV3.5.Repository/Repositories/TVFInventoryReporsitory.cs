using WeighingSystemUPPCV3_5_Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.EntityFrameworkCore;
using System.Data.SqlClient;

namespace WeighingSystemUPPCV3_5_Repository.Repositories
{
    public class InventoryReporsitory
    {
        private readonly DatabaseContext dbContext;

        public InventoryReporsitory(DatabaseContext dbContext)
        {
            this.dbContext = dbContext;
        }

        
  

    }
}
