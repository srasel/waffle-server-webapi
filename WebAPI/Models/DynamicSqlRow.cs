using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Web;

namespace WebAPI.Models
{
    public class DynamicSqlRow : DynamicObject
    {
        System.Data.IDataReader reader;

        public DynamicSqlRow(System.Data.IDataReader reader)
        {
            this.reader = reader;
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            var row = reader[binder.Name];

            result = row is DBNull ? null : row;

            return true;
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            return true;
        }

    }
}