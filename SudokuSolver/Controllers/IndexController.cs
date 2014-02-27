using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SudokuSolver.Models;

namespace SudokuSolver.Controllers
{
    public class IndexController : Controller
    {
        //
        // GET: /Index/

        public ActionResult Index()
        {
            Puzzle tempPuzzle = new Puzzle();
            return View(tempPuzzle);
        }

        public PartialViewResult Solve(string start)
        {
            Puzzle puzzle = new Puzzle(start);
            puzzle.Start();
            return PartialView(puzzle);
        }

    }
}
