using System;
using System.Collections.Generic;
using System.Text;

namespace Builder
{
    public interface IDbModel
    {
        string ConnectUrl { set; get; }
        TableModel GetByTable(string tableName);
    }
}
