using DPTS.Domain.Entities;
using System;
using System.Collections.Generic;

namespace DPTS.Domain.Core.GenericAttributes
{
    public partial interface IGenericAttributeService
    {
        void Delete(GenericAttribute genericAttribute);

        GenericAttribute GetAttributeById(int attrId);

        void Insert(GenericAttribute genericAttribute);

        void Update(GenericAttribute genericAttribute);

        IList<GenericAttribute> GetAllSpecialities();

        IList<GenericAttribute> GetAllLocation();

        IPagedList<GenericAttribute> GetAllGenericAttributes(int pageIndex = 0,
            int pageSize = Int32.MaxValue,string locator = null);
    }
}
