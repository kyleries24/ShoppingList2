﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ShoppingListApp.Models;
using ShoppingListApp.Services;
using Microsoft.AspNet.Identity;

namespace ShoppingList.Web.Controllers
{
    [Authorize]
    public class ShoppingListAppController : Controller
    {
        private readonly Lazy<ShoppingListAppService> _svc;

        public ShoppingListAppController()
        {
            _svc =
                new Lazy<ShoppingListAppService>(
                    () =>
                    {
                        var userId = Guid.Parse(User.Identity.GetUserId());
                        return new ShoppingListAppService(userId);
                    });
        }

        public ActionResult Index()
        {
            var ShoppingLists = _svc.Value.GetList();

            return View(ShoppingLists);
        }

        public ActionResult Create()
        {
            var vm = new ShoppingListCreateModel();

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ShoppingListCreateModel vm)
        {
            if (!ModelState.IsValid) return View(vm);

            if (!_svc.Value.CreateList(vm))
            {
                ModelState.AddModelError("", "Unable to create note");
                return View(vm);
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        [ActionName("Delete")]
        public ActionResult DeleteGet(int id)
        {
            var detail = _svc.Value.GetListById(id);

            return View(detail);
        }

        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeletePost(int id)
        {
            _svc.Value.DeleteList(id);

            return RedirectToAction("Index");
        }

        public ActionResult DeleteAllLists()
        {
            _svc.Value.DeleteAllLists();

            return RedirectToAction("Index");
        }
    }
}
