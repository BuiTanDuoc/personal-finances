﻿using System;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;

using PersonalFinances.Models;
using PersonalFinances.Repositories;
using PersonalFinances.Services.Exceptions;

namespace PersonalFinances.Services
{
    public class SubcategoryService
    {
        private SubcategoryRepository _repository = new SubcategoryRepository();

        /// <summary>
        /// Get all subcategories
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<Subcategory>> GetAll ()
        {
            return await _repository.GetSubcategories();
        }

        /// <summary>
        /// Get a subcategory by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<Subcategory> GetById (int id)
        {
            var subcategory = await _repository.GetSubcategoryById(id);

            if (subcategory != null)
                return subcategory;
            else
                throw new NotFoundException("This subcategory not exist");
        }

        /// <summary>
        /// Insert a new subcategory
        /// </summary>
        /// <param name="subcategory"></param>
        public async Task Add (Subcategory subcategory)
        {
            subcategory.Enabled = true;
            var nameExists = (await _repository.GetSubcategoryByName(subcategory.Name, subcategory.CategoryId)) != null;

            if (!nameExists)
                await _repository.Insert(subcategory);
            else
                throw new AlreadyExistsException($"Already exists a subcategory {subcategory.Name} in this category");
        }

        /// <summary>
        /// Update an existing subcategory
        /// </summary>
        /// <param name="subcategory"></param>
        public async Task Update (Subcategory subcategory)
        {
            var currentSubcategory = GetById(subcategory.Id);
            var quantity = (await _repository.GetSubcategoriesByName(subcategory.Name, subcategory.CategoryId))
                .Count(s => !s.Id.Equals(currentSubcategory.Id));

            subcategory.Enabled = true;
            
            if (quantity.Equals(0))
                await _repository.Update(subcategory);
            else
                throw new AlreadyExistsException($"Already exists a subcategory {subcategory.Name} in this category");
        }

        /// <summary>
        /// Delete an subcategory
        /// </summary>
        /// <param name="id"></param>
        public async Task Delete (int id)
        {
            var subcategory = await GetById(id);
            subcategory.Enabled = false;

            await _repository.Update(subcategory);
        }

        /// <summary>
        /// Delete a collection of subcategory
        /// </summary>
        /// <param name="subcategories"></param>
        public async Task Delete (ICollection<Subcategory> subcategories)
        {
            Func<Subcategory, Subcategory> disableSubcategoryAction = (s) => { s.Enabled = false; return s; };
            subcategories = subcategories.Select(disableSubcategoryAction).ToList();

            await _repository.Update(subcategories);
        }
    }
}