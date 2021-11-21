﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ERPSystem.Data;
using ERPSystem.Models;

namespace ERPSystem.Pages.Positions
{
    public class DeleteModel : PageModel
    {
        private readonly ERPSystem.Data.ApplicationDbContext _context;
        public int? PageIndex { get; set; }
        public string CurrentFilter { get; set; }
        public string CurrentSort { get; set; }
        public IEnumerable<Assignment> AssignmentsList { get; set; }


        public DeleteModel(ERPSystem.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Position Position { get; set; }

        public async Task<IActionResult> OnGetAsync(string sortOrder,
            string currentFilter, int? pageIndex, int? id)
        {
            PageIndex = pageIndex;
            CurrentSort = sortOrder;
            CurrentFilter = currentFilter;
            
            if (id == null)
            {
                return NotFound();
            }

            AssignmentsList = await _context.Assignments
                .Where(e => e.PositionId == id)
                .OrderBy(e => e.Name)
                .AsNoTracking()
                .ToListAsync();

            Position = await _context.Positions
                .Include(p => p.Project)
                .Include(p => p.Assignments)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == id);

            if (Position == null)
            {
                return NotFound();
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string sortOrder,
            string currentFilter, int? pageIndex, int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Position = await _context.Positions
                .Include(p => p.Project)
                .Include(p => p.Assignments)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (Position != null)
            {
                _context.Positions.Remove(Position);
                await _context.SaveChangesAsync();
            }
            await Utility.UpdateStateAsync(_context);
            return RedirectToPage("./Index", new
            {
                pageIndex = $"{pageIndex}",
                sortOrder = $"{sortOrder}",
                currentFilter = $"{currentFilter}"
            });
        }
    }
}
