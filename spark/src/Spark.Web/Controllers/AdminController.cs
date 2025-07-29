/*
 * Copyright (c) 2019-2025, Incendi <info@incendi.no>
 *
 * SPDX-License-Identifier: BSD-3-Clause
 */

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Spark.Web.Controllers;

[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    [HttpGet]
    public IActionResult Index()
    {
        return View();
    }

    [HttpGet]
    public IActionResult Maintenance()
    {
        return View();
    }
}