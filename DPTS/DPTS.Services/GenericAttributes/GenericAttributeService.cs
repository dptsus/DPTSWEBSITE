using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DPTS.Domain.Entities;
using DPTS.Domain.Core.GenericAttributes;
using DPTS.Domain.Core;

namespace DPTS.Services.GenericAttributes
{
    public class GenericAttributeService : IGenericAttributeService
    {
        #region Fields
        private readonly IRepository<GenericAttribute> _attributeRepository;
        #endregion

        #region Ctor
        public GenericAttributeService(IRepository<GenericAttribute> attributeRepository)
        {
            _attributeRepository = attributeRepository;
        }
        #endregion

        public void Delete(GenericAttribute genericAttribute)
        {
            if (genericAttribute == null)
                throw new ArgumentNullException("genericAttribute");

            _attributeRepository.Delete(genericAttribute);
        }

        public IList<GenericAttribute> GetAllAttribute()
        {
            var query = _attributeRepository.Table;
            var attributes = query.ToList();
            return attributes;
        }
        public IPagedList<GenericAttribute> GetAllGenericAttributes(int pageIndex = 0,
            int pageSize = Int32.MaxValue,string locator=null)
        {
            var query = _attributeRepository.Table;
            if (!string.IsNullOrWhiteSpace(locator))
                query = query.Where(c => c.EntityKey == locator);

            query = query.OrderBy(c => c.Id);
            return new PagedList<GenericAttribute>(query, pageIndex, pageSize);
        }
        public IList<GenericAttribute> GetAllLocation()
        {
            var query = _attributeRepository.Table;
            query = query.Where(c => c.EntityKey == "location");
            var attributes = query.ToList();
            return attributes;
        }
        public IList<GenericAttribute> GetAllSpecialities()
        {
            var query = _attributeRepository.Table;
            query = query.Where(c => c.EntityKey == "speciality");
            var attributes = query.ToList();
            return attributes;
        }

        public GenericAttribute GetAttributeById(int attrId)
        {
            if (attrId == 0)
                return null;

            return _attributeRepository.GetById(attrId);
        }

        public void Insert(GenericAttribute genericAttribute)
        {
            if (genericAttribute == null)
                throw new ArgumentNullException("genericAttribute");

            _attributeRepository.Insert(genericAttribute);
        }

        public void Update(GenericAttribute genericAttribute)
        {
            if (genericAttribute == null)
                throw new ArgumentNullException("genericAttribute");

            _attributeRepository.Update(genericAttribute);
        }
    }
}
